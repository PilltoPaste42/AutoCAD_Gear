namespace CGPlugin.UnitTests.ViewModels;

using System.Collections.Generic;
using System.Linq;

using CGPlugin.ViewModels;

using NUnit.Framework;

/// <summary>
///     unit-тестирование MainViewModel
/// </summary>
[TestFixture]
public class MainViewModelTests
{
    [SetUp]
    public void Setup()
    {
        _vm = new MainViewModel();
    }

    private MainViewModel _vm;

    [TestCase]
    public void Test_HasErrors_ReturnTrue()
    {
        Assert.DoesNotThrow(() =>
        {
            var hasErrors = _vm.HasErrors;
        });
    }

    [TestCase]
    public void Test_GetErrors_ReturnTrue()
    {
        var expected = new Dictionary<string, ICollection<string>>();
        var actual = _vm.GetErrors();

        Assert.AreEqual(expected, actual);
    }

    [TestCase]
    public void Test_GetErrorsOfNonexistentProperty_ReturnTrue()
    {
        var expected = Enumerable.Empty<string>();
        var actual = _vm.GetErrors("NoneProperty");

        Assert.AreEqual(expected, actual);
    }

    [TestCase]
    public void Test_GetErrorsOfExistentProperty_ReturnTrue()
    {
        _vm.Module = uint.MaxValue;
        var expected = Enumerable.Empty<string>();
        var actual = _vm.GetErrors("Module");

        Assert.AreNotEqual(expected, actual);
    }
}