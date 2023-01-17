namespace CGPlugin.Services;

using System;
using System.Collections.Generic;

using CGPlugin.Models;

using Inventor;

/// <summary>
///     Класс-строитель для создания модели шевронной шестерни в САПР Inventor
/// </summary>
public class CitroenGearInventorBuilder
{
    private readonly CitroenGearModel _gear;
    private Application _app;

    private CircularPatternFeature _circularFeature;

    private PartDocument _doc;

    private TransientGeometry _geom;

    private ObjectCollection _involuteCollection;

    private PartComponentDefinition _partDef;

    public CitroenGearInventorBuilder(CitroenGearModel gear)
    {
        _gear = gear;
    }

    public void Build()
    {
        CreateDocument();
        CreateTeethProfile();
        CreateGearBody();
        CreateTeeth();
        DuplicateGear();
        CreateExtra();
    }

    private void CreateDocument()
    {
        _app = InventorWrapper.Connect();
        _app.Visible = true;

        _doc = (PartDocument)_app.Documents.Add(DocumentTypeEnum.kPartDocumentObject,
            _app.FileManager.GetTemplateFile
            (DocumentTypeEnum.kPartDocumentObject,
                SystemOfMeasureEnum.kMetricSystemOfMeasure));

        _doc.UnitsOfMeasure.LengthUnits = UnitsTypeEnum.kMillimeterLengthUnits;

        _geom = _app.TransientGeometry;
        _partDef = _doc.ComponentDefinition;
    }

    private void CreateExtra()
    {
        var camera = _app.ActiveView.Camera;
        camera.ViewOrientationType = ViewOrientationTypeEnum.kIsoTopRightViewOrientation;

        camera.Fit();
        camera.Apply();

        _partDef.ClearAppearanceOverrides();

        _doc.Update();
        _doc.Update2();
    }

    private void CreateGearBody()
    {
        var dim = (double)(_gear.Module * (_gear.TeethCount + 2)) / 2 / 10;
        var width = (double)_gear.Width / 2 / 10;

        var sketch = new InventorSketchWrapper(CreateNewSketch(3));
        sketch.Center = sketch.AddByProjectingEntity(_partDef.WorkPoints[1]) as SketchPoint;
        sketch.AddCircle(sketch.Center, dim);

        var profile = sketch.Profiles.AddForSolid();

        var extrudeDefinition = _partDef.Features.ExtrudeFeatures
            .CreateExtrudeDefinition(profile, PartFeatureOperationEnum.kJoinOperation);
        extrudeDefinition.SetDistanceExtent(width,
            PartFeatureExtentDirectionEnum.kPositiveExtentDirection);

        _partDef.Features.ExtrudeFeatures.Add(extrudeDefinition);
    }

    /// <summary>
    ///     Создает новый эскиз на рабочей плоскости.
    /// </summary>
    /// <param name="n">1 - ZY; 2 - ZX; 3 - XY.</param>
    /// <param name="offset">Расстояние от поверхности.</param>
    /// <returns>Новый эскиз.</returns>
    private PlanarSketch CreateNewSketch(int n, double offset = 0)
    {
        var mainPlane = _partDef.WorkPlanes[n];
        var offsetPlane = _partDef.WorkPlanes.AddByPlaneAndOffset(
            mainPlane, offset);

        offsetPlane.Visible = false;

        var sketch = _partDef.Sketches.Add(offsetPlane);

        return sketch;
    }

    private Point2d CreatePoint(double x = 0, double y = 0)
    {
        return _geom.CreatePoint2d(x, y);
    }

    private void CreateTeeth()
    {
        var loftDefinition = _partDef.Features.LoftFeatures
            .CreateLoftDefinition(_involuteCollection, PartFeatureOperationEnum.kCutOperation);
        var loftFeature = _partDef.Features.LoftFeatures.Add(loftDefinition);

        var loftFeatureCollection = _app.TransientObjects.CreateObjectCollection();
        loftFeatureCollection.Add(loftFeature);

        var circularDefinition = _partDef.Features.CircularPatternFeatures
            .CreateDefinition(loftFeatureCollection, _partDef.WorkAxes[3], true, _gear.TeethCount,
                2 * Math.PI);
        _circularFeature =
            _partDef.Features.CircularPatternFeatures.AddByDefinition(circularDefinition);
    }

    private void DuplicateGear()
    {
        var surfaceCollection = _app.TransientObjects.CreateObjectCollection();
        surfaceCollection.Add(_circularFeature.SurfaceBodies[1]);
        var mirrorDefinition =
            _partDef.Features.MirrorFeatures.CreateDefinition(surfaceCollection,
                _partDef.WorkPlanes[3]);
        _partDef.Features.MirrorFeatures.AddByDefinition(mirrorDefinition);
    }

    private void CreateTeethProfile()
    {
        var bottomSketch = CreateNewSketch(3);
        DrawInvoluteInSketch(bottomSketch);
        var topSketch = CreateNewSketch(3, (double)_gear.Width / 2 / 10);
        DrawInvoluteInSketch(topSketch, _gear.TeethAngle);

        _involuteCollection = _app.TransientObjects.CreateObjectCollection();
        _involuteCollection.Add(bottomSketch.Profiles.AddForSolid());
        _involuteCollection.Add(topSketch.Profiles.AddForSolid());
    }

    private void DrawInvoluteInSketch(PlanarSketch sketch, double offsetAngle = 0)
    {
        const double engagementAngle = 20.0;
        var radii = new Dictionary<string, double>
        {
            { "pitch", (double)_gear.Diameter / 2 / 10 },
            { "outside", (double)(_gear.Module * (_gear.TeethCount + 2)) / 2 / 10 },
            { "root", _gear.Module * (_gear.TeethCount - 2.5) / 2 / 10 }
        };

        var sk = new InventorSketchWrapper(sketch);

        sk.Center = sk.AddByProjectingEntity(_partDef.WorkPoints[1]) as SketchPoint;

        var circles = new Dictionary<string, SketchCircle>();
        foreach (var radius in radii)
        {
            var circle = sk.AddCircle(sk.Center, radius.Value);
            circle.CenterSketchPoint.Merge(sk.Center);
            sk.AddDimCon(circle);

            circles.Add(radius.Key, circle);
        }

        circles["pitch"].Construction = true;
        circles["root"].Construction = true;

        var construct = sk.AddLine(sk.Center, CreatePoint(0, radii["outside"]));
        construct.Construction = true;
        var vertical = sk.AddLine(sk.Center, CreatePoint(0, 1));
        vertical.Construction = true;
        sk.AddVerticalCon(vertical);
        sk.AddCoincidentCon(construct.EndSketchPoint, circles["outside"]);
        sk.AddCoincidentCon(vertical.EndSketchPoint, circles["outside"]);
        var offset = sk.AddTwoLineAngleCon(construct, vertical, offsetAngle);

        sk.Solve();

        // Построение baseCircle

        var tLines = new List<SketchLine>
        {
            sk.AddLine(CreatePoint(0, radii["pitch"]), CreatePoint(1, radii["pitch"]))
        };
        tLines.Add(
            sk.AddLine(tLines[0].StartSketchPoint, CreatePoint(1, radii["root"]))
        );

        sk.AddCoincidentCon(tLines[0].StartSketchPoint, circles["pitch"]);
        sk.AddCoincidentCon(tLines[0].StartSketchPoint, construct);

        sk.AddPerpendicularCon(tLines[0], construct);
        sk.AddTwoLineAngleCon(tLines[0], tLines[1], 180 - engagementAngle);

        var baseCircle = sk.AddCircle(sk.Center, 1);
        baseCircle.CenterSketchPoint.Merge(sk.Center);
        sk.AddTangentCon(baseCircle, tLines[1]);
        baseCircle.Construction = true;

        sk.DeleteFromSketch(tLines);
        tLines.Clear();

        sk.AddDimCon(baseCircle);
        circles.Add("base", baseCircle);
        radii.Add("base", baseCircle.Radius);

        // Построение эвольвенты

        tLines.Add(
            sk.AddLine(
                CreatePoint(0, radii["root"]),
                CreatePoint(0, radii["pitch"])));
        sk.AddCoincidentCon(tLines[0].StartSketchPoint, circles["root"]);
        sk.AddCoincidentCon(tLines[0].StartSketchPoint, construct);
        sk.AddCoincidentCon(tLines[0].EndSketchPoint, circles["pitch"]);
        sk.AddParallelCon(tLines[0], construct);

        tLines.Add(
            sk.AddLine(
                tLines[0].EndSketchPoint,
                CreatePoint(-1, radii["pitch"])));
        var s = Math.PI * _gear.Module / 2 / 10;
        sk.AddCoincidentCon(tLines[1].EndSketchPoint, circles["pitch"]);
        sk.AddTwoPointDistCon(tLines[1].StartSketchPoint, tLines[1].EndSketchPoint, s);

        tLines.Add(
            sk.AddLine(
                tLines[1].EndSketchPoint,
                CreatePoint(1, radii["base"])));
        var r = radii["pitch"] / 3;
        sk.AddCoincidentCon(tLines[2].EndSketchPoint, circles["base"]);
        sk.AddTwoPointDistCon(tLines[2].StartSketchPoint, tLines[2].EndSketchPoint, r);

        tLines.Add(
            sk.AddLine(
                tLines[2].EndSketchPoint,
                CreatePoint(-1, radii["base"])));
        sk.AddCoincidentCon(tLines[3].EndSketchPoint, circles["base"]);
        sk.AddTwoPointDistCon(tLines[3].StartSketchPoint, tLines[3].EndSketchPoint, r);

        tLines.Add(
            sk.AddLine(
                tLines[0].EndSketchPoint,
                CreatePoint(-2, radii["pitch"])));
        var midr = 0.75 * _gear.Module * Math.PI / 1 / 10;
        sk.AddCoincidentCon(tLines[4].EndSketchPoint, circles["pitch"]);
        sk.AddTwoPointDistCon(tLines[4].StartSketchPoint, tLines[4].EndSketchPoint, midr);

        tLines.Add(
            sk.AddLine(
                tLines[4].EndSketchPoint,
                CreatePoint(-2, radii["root"])));
        sk.AddCoincidentCon(tLines[5].EndSketchPoint, circles["root"]);
        sk.AddPerpendicularCon(tLines[5], circles["root"]);

        var startAngle1 = 180;
        var sweepAngle1 = 20;
        var involuteArk1 =
            sk.AddArc(tLines[2].EndSketchPoint, r, startAngle1, sweepAngle1);

        sk.AddRadCon(involuteArk1);
        sk.AddCoincidentCon(involuteArk1.CenterSketchPoint, circles["base"]);
        sk.AddCoincidentCon(involuteArk1.CenterSketchPoint, tLines[2]);
        sk.AddCoincidentCon(involuteArk1.EndSketchPoint, tLines[3]);
        sk.AddCoincidentCon(involuteArk1.StartSketchPoint, circles["outside"]);

        var startAngle2 = 270;
        var sweepAngle2 = 90;
        var involuteArk2Rad = 0.2 * _gear.Module / 10;
        var involuteArk2 = sk.AddArc(
            CreatePoint(0, radii["outside"] + 1),
            involuteArk2Rad,
            startAngle2,
            sweepAngle2);
        sk.AddRadCon(involuteArk2);
        sk.AddArcLenCon(involuteArk2);

        var involuteLine1 = sk.AddLine(involuteArk1.EndSketchPoint, involuteArk2.EndSketchPoint);

        sk.AddCoincidentCon(involuteArk2.StartSketchPoint, circles["root"]);
        sk.AddPerpendicularCon(involuteLine1, circles["root"]);

        var involuteArk3 =
            sk.AddArc(
                sk.Center,
                involuteArk2.StartSketchPoint,
                tLines[5].EndSketchPoint
            );

        sk.AddConcentricCon(involuteArk3, circles["root"]);
        sk.AddTangentCon(involuteArk2, involuteArk3);

        sk.DeleteFromSketch(tLines);

        sk.AddCoincidentCon(involuteLine1.StartSketchPoint, circles["base"]);
        sk.AddArcLenCon(involuteArk3);
        sk.AddCoincidentCon(involuteArk3.EndSketchPoint, construct);

        var involuteArk4 = sk.AddArc(sk.Center, involuteArk3.EndSketchPoint,
            CreatePoint(-1, radii["root"]));
        sk.AddSymmetryCon(involuteArk4, involuteArk3, construct);

        var involuteArk5 = sk.AddArc(
            CreatePoint(-1, radii["base"]),
            involuteArk4.EndSketchPoint,
            CreatePoint(-1, radii["pitch"]),
            false);
        sk.AddSymmetryCon(involuteArk5, involuteArk2, construct);

        var involuteLine2 = sk.AddLine(
            involuteArk5.StartSketchPoint,
            CreatePoint(-1, radii["base"])
        );
        sk.AddSymmetryCon(involuteLine2, involuteLine1, construct);
        sk.AddCoincidentCon(involuteLine2.EndSketchPoint, circles["base"]);

        var involuteArk6 = sk.AddArc(
            CreatePoint(-1, radii["pitch"]),
            involuteLine2.EndSketchPoint,
            CreatePoint(0, radii["outside"])
        );
        sk.AddCoincidentCon(involuteArk6.EndSketchPoint, circles["outside"]);
        sk.AddSymmetryCon(involuteArk6, involuteArk1, construct);

        var outArk = sk.AddArc(
            construct.EndSketchPoint,
            involuteArk6.EndSketchPoint,
            involuteArk1.StartSketchPoint, false);

        // Удаление вспомогательной геометрии

        construct.Delete();
        vertical.Delete();
        foreach (var sketchCircle in circles) sketchCircle.Value.Delete();


        sk.Solve();
    }
}