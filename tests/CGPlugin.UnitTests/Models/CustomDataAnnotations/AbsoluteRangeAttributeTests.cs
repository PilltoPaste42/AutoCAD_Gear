using CGPlugin.Models.CustomDataAnnotations;
using NUnit.Framework;

namespace CGPlugin.UnitTests.Models.CustomDataAnnotations;

/// <summary>
///     Unit-тестирование пользовательского атрибута AbsoluteRange
/// </summary>
[TestFixture]
public class AbsoluteRangeAttributeTests
{
    private AbsoluteRangeAttribute _attribute;

    [SetUp]
    public void Setup()
    {
        _attribute = new AbsoluteRangeAttribute(0, 100);
    }

    [TestCase(10)]
    [TestCase(-10)]
    [TestCase(100)]
    [TestCase(-100)]
    [TestCase(0)]
    public void Test_IsValid_ReturnTrue(int value)
    {
        var result = _attribute.IsValid(value);
        
        Assert.IsTrue(result);
    }

    [TestCase(null)]
    [TestCase(-1000)]
    [TestCase(1000)]
    public void Test_IsValid_ReturnFalse(int? value)
    {
        var result = _attribute.IsValid(value);

        Assert.IsFalse(result);
    }
}