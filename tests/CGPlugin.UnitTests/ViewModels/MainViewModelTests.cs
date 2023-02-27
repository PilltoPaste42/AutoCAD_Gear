using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CGPlugin.UnitTests.ViewModels;

using NUnit.Framework;
using CGPlugin.ViewModels;
using System.Linq;
using System.Collections;

/// <summary>
///     unit-тестирование MainViewModel
/// </summary>
[TestFixture]
public class MainViewModelTests
{
    private MainViewModel _vm;

    [SetUp]
    public void Setup()
    {
        _vm = new MainViewModel();
    }

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
        _vm.Module = UInt32.MaxValue;
        var expected = Enumerable.Empty<string>();
        var actual = _vm.GetErrors("Module");

        Assert.AreNotEqual(expected, actual);
    }
}