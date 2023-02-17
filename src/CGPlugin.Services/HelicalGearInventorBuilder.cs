namespace CGPlugin.Services;

using System;
using System.Collections.Generic;

using CGPlugin.Models;

using Inventor;

/// <summary>
///     Класс-строитель для создания моделей косозубых шестерней в САПР Inventor
/// </summary>
public class HelicalGearInventorBuilder
{
    private HelicalGearModel Gear { get; set; }
    private Application App { get; set; }

    private CircularPatternFeature CircularFeature { get; set; }

    private PartDocument Doc { get; set; }

    private TransientGeometry Geom { get; set; }

    private ObjectCollection InvoluteCollection { get; set; }

    private PartComponentDefinition PartDef { get; set; }

    private double GearWidth { get; set; }

    /// <summary>
    ///     Установка параметров модели
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public HelicalGearInventorBuilder FromModel(HelicalGearModel model)
    {
        Gear = model;

        return this;
    }

    /// <summary>
    ///     Построение косозубой шестерни в Inventor
    /// </summary>
    public void BuildHelicalGear()
    {
        GearWidth = Gear.Width;
        BuildBase();
        CreateExtra();
    }

    /// <summary>
    ///     Построение шевронной шестерни в Inventor
    /// </summary>
    public void BuildCitroenGear()
    {
        GearWidth = (double)Gear.Width / 2;
        BuildBase();
        DuplicateGear();
        CreateExtra();
    }

    /// <summary>
    ///     Запуск построения модели
    /// </summary>
    private void BuildBase()
    {
        CreateDocument();
        CreateTeethProfile();
        CreateGearBody();
        CreateTeeth();
    }

    /// <summary>
    ///     Создание документа для модели в Autodesk Inventor
    /// </summary>
    private void CreateDocument()
    {
        App = InventorWrapper.Connect();
        App.Visible = true;

        Doc = (PartDocument)App.Documents.Add(DocumentTypeEnum.kPartDocumentObject,
            App.FileManager.GetTemplateFile
            (DocumentTypeEnum.kPartDocumentObject,
                SystemOfMeasureEnum.kMetricSystemOfMeasure));

        Doc.UnitsOfMeasure.LengthUnits = UnitsTypeEnum.kMillimeterLengthUnits;

        Geom = App.TransientGeometry;
        PartDef = Doc.ComponentDefinition;
    }

    /// <summary>
    ///     Реализация дополнительных действий в среде Inventor
    /// </summary>
    private void CreateExtra()
    {
        var camera = App.ActiveView.Camera;
        camera.ViewOrientationType = ViewOrientationTypeEnum.kIsoTopRightViewOrientation;

        camera.Fit();
        camera.Apply();

        PartDef.ClearAppearanceOverrides();

        Doc.Update();
        Doc.Update2();
    }

    /// <summary>
    ///     Создание тела модели шестерни
    /// </summary>
    private void CreateGearBody()
    {
        var dim = (double)(Gear.Module * (Gear.TeethCount + 2)) / 2 / 10;
        var width = GearWidth / 10;

        var sketch = new InventorSketchWrapper(CreateNewSketch(3));
        sketch.Center = sketch.AddByProjectingEntity(PartDef.WorkPoints[1]) as SketchPoint;
        sketch.AddCircle(sketch.Center, dim);

        var profile = sketch.Profiles.AddForSolid();

        var extrudeDefinition = PartDef.Features.ExtrudeFeatures
            .CreateExtrudeDefinition(profile, PartFeatureOperationEnum.kJoinOperation);
        extrudeDefinition.SetDistanceExtent(width,
            PartFeatureExtentDirectionEnum.kPositiveExtentDirection);

        PartDef.Features.ExtrudeFeatures.Add(extrudeDefinition);
    }

    /// <summary>
    ///     Создает новый эскиз на рабочей плоскости.
    /// </summary>
    /// <param name="n">1 - ZY; 2 - ZX; 3 - XY.</param>
    /// <param name="offset">Расстояние от поверхности.</param>
    /// <returns>Новый эскиз.</returns>
    private PlanarSketch CreateNewSketch(int n, double offset = 0)
    {
        var mainPlane = PartDef.WorkPlanes[n];
        var offsetPlane = PartDef.WorkPlanes.AddByPlaneAndOffset(
            mainPlane, offset);

        offsetPlane.Visible = false;

        var sketch = PartDef.Sketches.Add(offsetPlane);

        return sketch;
    }

    private Point2d CreatePoint(double x = 0, double y = 0)
    {
        return Geom.CreatePoint2d(x, y);
    }

    /// <summary>
    ///     Создает зубья шестерни на теле модели
    /// </summary>
    private void CreateTeeth()
    {
        var loftDefinition = PartDef.Features.LoftFeatures
            .CreateLoftDefinition(InvoluteCollection, PartFeatureOperationEnum.kCutOperation);
        var loftFeature = PartDef.Features.LoftFeatures.Add(loftDefinition);

        var loftFeatureCollection = App.TransientObjects.CreateObjectCollection();
        loftFeatureCollection.Add(loftFeature);

        var circularDefinition = PartDef.Features.CircularPatternFeatures
            .CreateDefinition(loftFeatureCollection, PartDef.WorkAxes[3], true, Gear.TeethCount,
                2 * Math.PI);
        CircularFeature =
            PartDef.Features.CircularPatternFeatures.AddByDefinition(circularDefinition);
    }

    /// <summary>
    ///     Дублирует тело шестерни зеркально для получения Шевронной шестерни
    /// </summary>
    private void DuplicateGear()
    {
        var surfaceCollection = App.TransientObjects.CreateObjectCollection();
        surfaceCollection.Add(CircularFeature.SurfaceBodies[1]);
        var mirrorDefinition =
            PartDef.Features.MirrorFeatures.CreateDefinition(surfaceCollection,
                PartDef.WorkPlanes[3]);
        PartDef.Features.MirrorFeatures.AddByDefinition(mirrorDefinition);
    }

    /// <summary>
    ///     Создание эскизов описывающих профиль зуба
    /// </summary>
    private void CreateTeethProfile()
    {
        var bottomSketch = CreateNewSketch(3);
        DrawInvoluteInSketch(bottomSketch);
        var topSketch = CreateNewSketch(3, GearWidth / 10);
        var d = 0.0;
        if (Gear.TeethAngle < 0) d = 360;
        DrawInvoluteInSketch(topSketch, Gear.TeethAngle + d);

        InvoluteCollection = App.TransientObjects.CreateObjectCollection();
        InvoluteCollection.Add(bottomSketch.Profiles.AddForSolid());
        InvoluteCollection.Add(topSketch.Profiles.AddForSolid());
    }

    /// <summary>
    ///     Создание профиля зуба на эскизе
    /// </summary>
    /// <param name="sketch"> Экземпляр эскиза </param>
    /// <param name="offsetAngle"> Угол отклонения центра профиля зуба </param>
    private void DrawInvoluteInSketch(PlanarSketch sketch, double offsetAngle = 0)
    {
        const double engagementAngle = 20.0;
        var radii = new Dictionary<string, double>
        {
            { "pitch", (double)Gear.Diameter / 2 / 10 },
            { "outside", (double)(Gear.Module * (Gear.TeethCount + 2)) / 2 / 10 },
            { "root", Gear.Module * (Gear.TeethCount - 2.5) / 2 / 10 }
        };

        var sk = new InventorSketchWrapper(sketch);

        sk.Center = sk.AddByProjectingEntity(PartDef.WorkPoints[1]) as SketchPoint;

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
        sk.AddTwoLineAngleCon(construct, vertical, offsetAngle);

        sk.Solve();

        var constructX = construct.Geometry.EndPoint.X;

        // Построение baseCircle

        var tLines = new List<SketchLine>
        {
            sk.AddLine(CreatePoint(construct.Geometry.EndPoint.X, radii["pitch"]),
                CreatePoint(construct.Geometry.EndPoint.X + 1, radii["pitch"]))
        };
        tLines.Add(
            sk.AddLine(tLines[0].StartSketchPoint,
                CreatePoint(construct.Geometry.EndPoint.X + 1, radii["root"]))
        );

        sk.AddCoincidentCon(tLines[0].StartSketchPoint, circles["pitch"]);
        sk.AddCoincidentCon(tLines[0].StartSketchPoint, construct);
        sk.Solve();
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
                CreatePoint(constructX, radii["root"]),
                CreatePoint(constructX, radii["pitch"])));
        sk.AddCoincidentCon(tLines[0].StartSketchPoint, circles["root"]);
        sk.AddCoincidentCon(tLines[0].StartSketchPoint, construct);
        sk.AddCoincidentCon(tLines[0].EndSketchPoint, circles["pitch"]);
        sk.AddParallelCon(tLines[0], construct);

        tLines.Add(
            sk.AddLine(
                tLines[0].EndSketchPoint,
                CreatePoint(constructX - 3, radii["pitch"])));
        var s = Math.PI * Gear.Module / 2 / 10;
        sk.AddCoincidentCon(tLines[1].EndSketchPoint, circles["pitch"]);
        sk.AddTwoPointDistCon(tLines[1].StartSketchPoint, tLines[1].EndSketchPoint, s);

        tLines.Add(
            sk.AddLine(
                tLines[1].EndSketchPoint,
                CreatePoint(constructX + 3, radii["base"])));
        var r = radii["pitch"] / 3;
        sk.AddCoincidentCon(tLines[2].EndSketchPoint, circles["base"]);
        sk.AddTwoPointDistCon(tLines[2].StartSketchPoint, tLines[2].EndSketchPoint, r);

        tLines.Add(
            sk.AddLine(
                tLines[2].EndSketchPoint,
                CreatePoint(constructX - 3, radii["base"])));
        sk.AddCoincidentCon(tLines[3].EndSketchPoint, circles["base"]);
        sk.AddTwoPointDistCon(tLines[3].StartSketchPoint, tLines[3].EndSketchPoint, r);

        tLines.Add(
            sk.AddLine(
                tLines[0].EndSketchPoint,
                CreatePoint(constructX - 5, radii["pitch"])));
        var midr = 0.75 * Gear.Module * Math.PI / 1 / 10;
        sk.AddCoincidentCon(tLines[4].EndSketchPoint, circles["pitch"]);
        sk.AddTwoPointDistCon(tLines[4].StartSketchPoint, tLines[4].EndSketchPoint, midr);

        tLines.Add(
            sk.AddLine(
                tLines[4].EndSketchPoint,
                CreatePoint(constructX - 5, radii["root"])));
        sk.AddCoincidentCon(tLines[5].EndSketchPoint, circles["root"]);
        sk.AddPerpendicularCon(tLines[5], circles["root"]);

        var involuteArk1 =
            sk.AddArc(tLines[2].EndSketchPoint,
                CreatePoint(construct.Geometry.EndPoint.X, radii["outside"]),
                CreatePoint(construct.Geometry.EndPoint.X, radii["base"]));

        var ark1Rad = sk.AddRadCon(involuteArk1);
        ark1Rad.Parameter.Value = r;
        sk.AddCoincidentCon(involuteArk1.CenterSketchPoint, circles["base"]);
        sk.AddCoincidentCon(involuteArk1.CenterSketchPoint, tLines[2]);
        sk.AddCoincidentCon(involuteArk1.EndSketchPoint, tLines[3]);
        sk.AddCoincidentCon(involuteArk1.StartSketchPoint, circles["outside"]);

        var startAngle2 = 270;
        var sweepAngle2 = 90;
        var involuteArk2Rad = 0.2 * Gear.Module / 10;
        var involuteArk2 = sk.AddArc(
            CreatePoint(constructX - 1, radii["root"]),
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
            CreatePoint(constructX - 3, radii["root"]));
        sk.AddSymmetryCon(involuteArk4, involuteArk3, construct);

        var involuteArk5 = sk.AddArc(
            CreatePoint(constructX - 3, radii["base"]),
            involuteArk4.EndSketchPoint,
            CreatePoint(constructX - 3, radii["pitch"]),
            false);
        sk.AddSymmetryCon(involuteArk5, involuteArk2, construct);

        var involuteLine2 = sk.AddLine(
            involuteArk5.StartSketchPoint,
            CreatePoint(constructX - 3, radii["base"])
        );
        sk.AddSymmetryCon(involuteLine2, involuteLine1, construct);
        sk.AddCoincidentCon(involuteLine2.EndSketchPoint, circles["base"]);

        var involuteArk6 = sk.AddArc(
            CreatePoint(constructX - 5, radii["pitch"]),
            involuteLine2.EndSketchPoint,
            CreatePoint(constructX - 2, radii["outside"])
        );

        sk.AddCoincidentCon(involuteArk6.CenterSketchPoint, circles["base"]);
        sk.AddCoincidentCon(involuteArk6.EndSketchPoint, circles["outside"]);
        sk.AddSymmetryCon(involuteArk6, involuteArk1, construct);

        sk.AddArc(
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