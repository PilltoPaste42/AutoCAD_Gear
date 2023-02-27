namespace CGPlugin.UnitTests.Models;

using System.ComponentModel.DataAnnotations;

using CGPlugin.Models;

using NUnit.Framework;

/// <summary>
///     unit-тестирование HelicalGearModel
/// </summary>
[TestFixture]
public class HelicalGearModelTests
{
    [SetUp]
    public void Setup()
    {
        _model = new HelicalGearModel();
        _validCon = new ValidationContext(_model, null, null);
    }

    private HelicalGearModel _model;
    private ValidationContext _validCon;

    [TestCase((uint)1000)]
    [TestCase((uint)5)]
    [TestCase((uint)5000)]
    public void Test_Valid_Diameter_ReturnTrue(uint value)
    {
        _validCon.MemberName = "Diameter";
        var result = Validator.TryValidateProperty(value, _validCon, null);

        Assert.IsTrue(result);
    }

    [TestCase((uint)0)]
    [TestCase((uint)4)]
    [TestCase((uint)5001)]
    public void Test_Invalid_Diameter_ReturnFalse(uint value)
    {
        _validCon.MemberName = "Diameter";
        var result = Validator.TryValidateProperty(value, _validCon, null);

        Assert.IsFalse(result);
    }

    [TestCase((uint)1000)]
    [TestCase((uint)5)]
    [TestCase((uint)5000)]
    public void Test_Valid_Width_ReturnTrue(uint value)
    {
        _validCon.MemberName = "Width";
        var result = Validator.TryValidateProperty(value, _validCon, null);

        Assert.IsTrue(result);
        Assert.DoesNotThrow(() => { _model.Width = value; });
    }

    [TestCase((uint)0)]
    [TestCase((uint)4)]
    [TestCase((uint)5001)]
    public void Test_Invalid_Width_ReturnFalse(uint value)
    {
        _validCon.MemberName = "Width";
        var result = Validator.TryValidateProperty(value, _validCon, null);

        Assert.IsFalse(result);
        Assert.DoesNotThrow(() => { _model.Width = value; });
    }

    [TestCase(30)]
    [TestCase(-30)]
    [TestCase(25)]
    [TestCase(-25)]
    [TestCase(45)]
    [TestCase(-45)]
    public void Test_Valid_TeethAngle_ReturnTrue(int value)
    {
        _validCon.MemberName = "TeethAngle";

        var result = Validator.TryValidateProperty(value, _validCon, null);

        Assert.IsTrue(result);
        Assert.DoesNotThrow(() => { _model.TeethAngle = value; });
    }

    [TestCase(0)]
    [TestCase(-100)]
    [TestCase(100)]
    [TestCase(-15)]
    [TestCase(15)]
    [TestCase(46)]
    public void Test_Invalid_TeethAngle_ReturnFalse(int value)
    {
        _validCon.MemberName = "TeethAngle";
        var result = Validator.TryValidateProperty(value, _validCon, null);

        Assert.IsFalse(result);
        Assert.DoesNotThrow(() => { _model.TeethAngle = value; });
    }

    [TestCase((uint)10, (uint)100)]
    [TestCase((uint)1, (uint)2)]
    public void Test_Valid_TeethCount_ReturnTrue(uint teethCount, uint diameter)
    {
        _validCon.MemberName = "TeethCount";
        _model.Diameter = diameter;
        var result = Validator.TryValidateProperty(teethCount, _validCon, null);

        Assert.IsTrue(result);
    }

    [TestCase((uint)0, (uint)100)]
    [TestCase((uint)1000, (uint)1)]
    public void Test_Invalid_TeethCount_ReturnFalse(uint teethCount, uint diameter)
    {
        _validCon.MemberName = "TeethCount";
        _model.Diameter = diameter;
        var result = Validator.TryValidateProperty(teethCount, _validCon, null);

        Assert.IsFalse(result);
    }

    [TestCase((uint)0, (uint)100, (uint)100)]
    [TestCase((uint)1, (uint)0, (uint)0)]
    public void Test_Invalid_Module_ReturnFalse(uint module, uint teethCount, uint diameter)
    {
        _validCon.MemberName = "Module";
        _model.Diameter = diameter;
        _model.TeethCount = teethCount;
        var result = Validator.TryValidateProperty(teethCount, _validCon, null);

        Assert.IsFalse(result);
    }
}