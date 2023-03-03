namespace CGPlugin.UnitTests.Models.CustomDataAnnotations;

using System.ComponentModel.DataAnnotations;

using CGPlugin.Models;
using CGPlugin.Models.CustomDataAnnotations;

using NUnit.Framework;

/// <summary>
///     Модульное тестирование пользовательского атрибута HelicalGearModule
/// </summary>
[TestFixture]
public class HelicalGearModuleAttributeTests
{
    [SetUp]
    public void Setup()
    {
        _attribute = new HelicalGearModuleAttribute();
        _model = new HelicalGearModel();
        _context = new ValidationContext(_model);
    }

    private HelicalGearModuleAttribute _attribute;
    private HelicalGearModel _model;
    private ValidationContext _context;

    [TestCase(null, (uint)10, (uint)10)]
    [TestCase((uint)0, (uint)10, (uint)10)]
    [TestCase((uint)1, (uint)4, (uint)2)]
    public void Test_IsValid_ReturnFalse(uint? module, uint diameter, uint teethCount)
    {
        _model.Diameter = diameter;
        _model.TeethCount = teethCount;

        var expected = ValidationResult.Success;
        var actual = _attribute.GetValidationResult(module, _context);

        Assert.AreNotEqual(expected, actual);
    }

    [TestCase((uint)2, (uint)4, (uint)2)]
    public void Test_IsValid_ReturnTrue(uint? module, uint diameter, uint teethCount)
    {
        _model.Diameter = diameter;
        _model.TeethCount = teethCount;

        var expected = ValidationResult.Success;
        var actual = _attribute.GetValidationResult(module, _context);

        Assert.AreEqual(expected, actual);
    }
}