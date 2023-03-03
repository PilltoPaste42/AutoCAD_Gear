namespace CGPlugin.ViewModels;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

using CGPlugin.Models;
using CGPlugin.Services;
using CGPlugin.Services.Enums;
using CGPlugin.Services.Interfaces;

using CommunityToolkit.Mvvm.Input;

/// <summary>
///     ViewModel для работы с данными из MainWindow
/// </summary>
public class MainViewModel : ValidationBase
{
    private readonly HelicalGearModel _gear;
    private readonly IMessageService _message;
    private readonly Dictionary<string, ICollection<string>> _validationErrors;

    public MainViewModel()
    {
        _gear = new HelicalGearModel();
        _validationErrors = new Dictionary<string, ICollection<string>>();
        _message = new DisplayMessageService();

        BuildGearCommand = new RelayCommand(BuildCitroenGear, () => ModelIsValid);
        SetDefaultGearCommand = new RelayCommand(SetDefaultGear);
    }

    /// <summary>
    ///     Команда построения шестерни
    /// </summary>
    public RelayCommand BuildGearCommand { get; }

    /// <summary>
    ///     Флаг для построения обычной косозубой шестерни
    /// </summary>
    public bool IsCommonHelicalGear { get; set; }

    /// <summary>
    ///     Диаметр шестерни
    /// </summary>
    public uint Diameter
    {
        get => _gear.Diameter;
        set
        {
            _gear.Diameter = value;
            Module = GetModule;
            ValidateModelProperty(value);
            ValidateModelProperty(TeethCount, nameof(TeethCount));
            OnPropertyChanged();
        }
    }

    /// <summary>
    ///     Получение значения модуля шестерни методом прямого расчета
    /// </summary>
    private uint GetModule
    {
        get
        {
            if (TeethCount == 0)
            {
                return 0;
            }

            return Diameter / TeethCount;
        }
    }

    /// <inheritdoc chref="base" />
    public override bool HasErrors => !ModelIsValid;

    /// <summary>
    ///     Проверка валидности модели шестерни
    /// </summary>
    public bool ModelIsValid =>
        Validator.TryValidateObject(_gear, new ValidationContext(_gear, null, null), null, true);

    /// <summary>
    ///     Модуль шестерни
    /// </summary>
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

    /// <summary>
    ///     Команда установки стандартных значений шестерни
    /// </summary>
    public RelayCommand SetDefaultGearCommand { get; }

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

    /// <summary>
    ///     Количество зубьев шестерни
    /// </summary>
    public uint TeethCount
    {
        get => _gear.TeethCount;
        set
        {
            _gear.TeethCount = value;
            Module = GetModule;
            ValidateModelProperty(value);
            OnPropertyChanged();
        }
    }

    /// <summary>
    ///     Ширина шестерни
    /// </summary>
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

    /// <inheritdoc chref="base" />
    public override IEnumerable GetErrors(string? propertyName = "")
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            return _validationErrors;
        }

        if (!_validationErrors.ContainsKey(propertyName))
        {
            return Enumerable.Empty<string>();
        }

        return _validationErrors[propertyName];
    }

    /// <summary>
    ///     Валидация параметра модели
    /// </summary>
    /// <param name="value">Новое значение параметра</param>
    /// <param name="propertyName">Имя параметра</param>
    protected void ValidateModelProperty(object value, [CallerMemberName] string propertyName = "")
    {
        if (_validationErrors.ContainsKey(propertyName))
        {
            _validationErrors.Remove(propertyName);
        }

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
        BuildGearCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    ///     Построение шестерни в Inventor
    /// </summary>
    private void BuildCitroenGear()
    {
        if (HasErrors)
        {
            ShowErrorMessage("Gear parameters is not valid!");
            return;
        }

        try
        {
            var builder = new HelicalGearInventorBuilder();
            builder.FromModel(_gear);

            if (IsCommonHelicalGear)
            {
                builder.BuildHelicalGear();
            }
            else
            {
                builder.BuildCitroenGear();
            }
        }
        catch (Exception e)
        {
            ShowErrorMessage(e.Message);
        }
    }

    /// <summary>
    ///     Установка стандартных значений шестерни
    /// </summary>
    private void SetDefaultGear()
    {
        Diameter = 200;
        Module = 10;
        TeethAngle = 25;
        TeethCount = 20;
        Width = 50;
    }

    /// <summary>
    ///     Вызов окна сообщения ошибки
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    private void ShowErrorMessage(string message)
    {
        const string header = "Error";
        _message.Show(header, message, MessageType.Error);
    }
}