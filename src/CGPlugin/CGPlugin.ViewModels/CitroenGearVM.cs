﻿namespace CGPlugin.ViewModels;

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

using CGPlugin.Models;

/// <summary>
///   ViewModel для работы с данными из MainWindow
/// </summary>
public class CitroenGearVM : ValidationBase
{
    private readonly CitroenGearModel _gear;
    private readonly Dictionary<string, ICollection<string>> _validationErrors;

    public CitroenGearVM()
    {
        _gear = new CitroenGearModel();
        _validationErrors = new Dictionary<string, ICollection<string>>();
    }

    public uint Diameter
    {
        get => _gear.Diameter;
        set
        {
            _gear.Diameter = value;
            Module = GetModule;
            ValidateModelProperty(value);
            ValidateModelProperty(TeethCount, nameof(TeethCount));
            ValidateModelProperty(Module, nameof(Module));
            OnPropertyChanged();
        }
    }

    public override bool HasErrors => _validationErrors.Count > 0;

    public uint Module
    {
        get => _gear.Module;
        set
        {
            _gear.Module = value;
            ValidateModelProperty(value);
            OnPropertyChanged();
        }
    }

    public int TeethAngle
    {
        get => _gear.TeethAngle;
        set
        {
            _gear.TeethAngle = value;
            ValidateModelProperty(value);
            OnPropertyChanged();
        }
    }

    public uint TeethCount
    {
        get => _gear.TeethCount;
        set
        {
            _gear.TeethCount = value;
            Module = GetModule;
            ValidateModelProperty(value);
            ValidateModelProperty(Module, nameof(Module));
            OnPropertyChanged();
        }
    }

    public uint Width
    {
        get => _gear.Width;
        set
        {
            _gear.Width = value;
            ValidateModelProperty(value);
            OnPropertyChanged();
        }
    }

    public override IEnumerable GetErrors(string? propertyName = "")
    {
        if (string.IsNullOrEmpty(propertyName))
            return _validationErrors;

        if (!_validationErrors.ContainsKey(propertyName))
            return Enumerable.Empty<string>();

        return _validationErrors[propertyName];
    }

    private uint GetModule => (Diameter) / (TeethCount + 2);

    protected void ValidateModelProperty(object value, [CallerMemberName] string propertyName = "")
    {
        if (_validationErrors.ContainsKey(propertyName))
            _validationErrors.Remove(propertyName);

        ICollection<ValidationResult> validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(_gear, null, null)
        {
            MemberName = propertyName
        };

        if (!Validator.TryValidateProperty(value, validationContext, validationResults))
        {
            _validationErrors.Add(propertyName, new List<string>());
            foreach (var validationResult in validationResults.Where(validationResult =>
                         validationResult.ErrorMessage != null))
            {
                _validationErrors[propertyName].Add(validationResult.ErrorMessage);
            }
        }

        OnErrorsChanged(propertyName);
    }
}