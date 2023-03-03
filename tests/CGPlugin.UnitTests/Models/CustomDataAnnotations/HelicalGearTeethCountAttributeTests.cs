namespace CGPlugin.UnitTests.Models.CustomDataAnnotations;

using System.ComponentModel.DataAnnotations;

using CGPlugin.Models;
using CGPlugin.Models.CustomDataAnnotations;

using NUnit.Framework;

/// <summary>
///     Модульное тестирование пользовательского атрибута HelicalGearTeethCountAttribute
/// </summary>
[TestFixture]
public class HelicalGearTeethCountAttributeTests
{
    [SetUp]
    public void Setup()
    {
        _attribute = new HelicalGearTeethCountAttribute();
        _model = new HelicalGearModel();
        _context = new ValidationContext(_model);
    }

    private HelicalGearTeethCountAttribute _attribute;
    private HelicalGearModel _model;
    private ValidationContext _context;

    [TestCase(null, (uint)10)]
    [TestCase((uint)0, (uint)10)]
    [TestCase((uint)100, (uint)1)]
    public void Test_IsValid_ReturnFalse(uint? teethCount, uint diameter)
    {
        _model.Diameter = diameter;

        var expected = ValidationResult.Success;
        var actual = _attribute.GetValidationResult(teethCount, _context);

        Assert.AreNotEqual(expected, actual);
    }

    [TestCase((uint)20, (uint)100)]
    public void Test_IsValid_ReturnTrue(uint teethCount, uint diameter)
    {
        _model.Diameter = diameter;

        var expected = ValidationResult.Success;
        var actual = _attribute.GetValidationResult(teethCount, _context);

        Assert.AreEqual(expected, actual);
    }
}