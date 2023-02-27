namespace CGPlugin.UnitTests.Models.CustomDataAnnotations;

using CGPlugin.Models.CustomDataAnnotations;

using NUnit.Framework;

/// <summary>
///     Unit-тестирование пользовательского атрибута AbsoluteRange
/// </summary>
[TestFixture]
public class AbsoluteRangeAttributeTests
{
    [SetUp]
    public void Setup()
    {
        _attribute = new AbsoluteRangeAttribute(0, 100);
    }

    private AbsoluteRangeAttribute _attribute;

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