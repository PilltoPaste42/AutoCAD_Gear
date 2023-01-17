namespace CGPlugin.Services;

using System;
using System.Collections;

using Inventor;

/// <summary>
///     Класс-декоратор для упрощения работы с объектом Sketch из Inventor API
/// </summary>
public class InventorSketchWrapper
{
    public InventorSketchWrapper(PlanarSketch sketch)
    {
        Sketch = sketch;
        Profiles = sketch.Profiles;
    }

    public SketchPoint Center { get; set; }
    public Profiles Profiles { get; }
    public PlanarSketch Sketch { get; }

    public SketchArc AddArc(object CenterPoint, object StartPoint, object EndPoint,
        bool clockwise = true)
    {
        return Sketch.SketchArcs.AddByCenterStartEndPoint(CenterPoint, StartPoint, EndPoint,
            clockwise);
    }

    public SketchArc AddArc(object CenterPoint, double Radius, double StartAngle, double SweepAngle)
    {
        return Sketch.SketchArcs.AddByCenterStartSweepAngle(CenterPoint, Radius,
            Deg2Rad(StartAngle),
            Deg2Rad(SweepAngle));
    }

    public ArcLengthDimConstraint AddArcLenCon(object entity, bool visible = false)
    {
        var result =
            Sketch.DimensionConstraints.AddArcLength((SketchEntity)entity, Center.Geometry);
        result.Visible = visible;

        return result;
    }

    public SketchEntity AddByProjectingEntity(object projectingEntity)
    {
        return Sketch.AddByProjectingEntity(projectingEntity);
    }

    public SketchCircle AddCircle(SketchPoint center, double radius)
    {
        return Sketch.SketchCircles.AddByCenterRadius(center, radius);
    }

    public CoincidentConstraint AddCoincidentCon(object EntityOne, object EntityTwo)
    {
        return Sketch.GeometricConstraints.AddCoincident((SketchEntity)EntityOne,
            (SketchEntity)EntityTwo);
    }

    public ConcentricConstraint AddConcentricCon(object entityOne, object entityTwo)
    {
        return Sketch.GeometricConstraints.AddConcentric((SketchEntity)entityOne,
            (SketchEntity)entityTwo);
    }

    public DiameterDimConstraint AddDimCon(object entity, bool visible = false)
    {
        var result = Sketch.DimensionConstraints.AddDiameter((SketchEntity)entity, Center.Geometry);
        result.Visible = visible;

        return result;
    }

    public HorizontalConstraint AddHorizontalCon(object entity)
    {
        return Sketch.GeometricConstraints.AddHorizontal((SketchEntity)entity);
    }

    public SketchLine AddLine(object begin, object end)
    {
        return Sketch.SketchLines.AddByTwoPoints(begin, end);
    }

    public ParallelConstraint AddParallelCon(object entityOne, object entityTwo)
    {
        return Sketch.GeometricConstraints.AddParallel((SketchEntity)entityOne,
            (SketchEntity)entityTwo);
    }

    public PerpendicularConstraint AddPerpendicularCon(object entityOne, object entityTwo)
    {
        return Sketch.GeometricConstraints.AddPerpendicular((SketchEntity)entityOne,
            (SketchEntity)entityTwo);
    }

    public SketchPoint AddPoint(Point2d point)
    {
        return Sketch.SketchPoints.Add(point);
    }

    public RadiusDimConstraint AddRadCon(object entity, bool visible = false)
    {
        var result = Sketch.DimensionConstraints.AddRadius((SketchEntity)entity, Center.Geometry);
        result.Visible = visible;

        return result;
    }

    public SymmetryConstraint AddSymmetryCon(object entityOne, object entityTwo,
        SketchLine centerLine)
    {
        var result = Sketch.GeometricConstraints
            .AddSymmetry((SketchEntity)entityOne, (SketchEntity)entityTwo, centerLine);
        Solve();
        return result;
    }

    public TangentSketchConstraint AddTangentCon(object entityOne, object entityTwo)
    {
        return Sketch.GeometricConstraints.AddTangent((SketchEntity)entityOne,
            (SketchEntity)entityTwo);
    }

    public TwoLineAngleDimConstraint AddTwoLineAngleCon(SketchLine line1, SketchLine line2,
        double angle,
        bool visible = false)
    {
        var result = Sketch.DimensionConstraints.AddTwoLineAngle(line1, line2, Center.Geometry);
        result.Visible = visible;
        result.Parameter.Value = Deg2Rad(angle);

        return result;
    }

    public TwoPointDistanceDimConstraint AddTwoPointDistCon(SketchPoint pointOne,
        SketchPoint pointTwo, double dist,
        DimensionOrientationEnum orientation = DimensionOrientationEnum.kAlignedDim,
        bool visible = false)
    {
        var result = Sketch.DimensionConstraints
            .AddTwoPointDistance(pointOne, pointTwo, orientation, Center.Geometry);
        result.Visible = visible;
        result.Parameter.Value = dist;

        return result;
    }

    public VerticalConstraint AddVerticalCon(object entity)
    {
        return Sketch.GeometricConstraints.AddVertical((SketchEntity)entity);
    }

    public void DeleteFromSketch(IEnumerable objects)
    {
        foreach (SketchEntity o in objects) o.Delete();
    }

    public void Solve()
    {
        Sketch.Solve();
    }

    private double Deg2Rad(double deg)
    {
        return deg * Math.PI / 180;
    }
}