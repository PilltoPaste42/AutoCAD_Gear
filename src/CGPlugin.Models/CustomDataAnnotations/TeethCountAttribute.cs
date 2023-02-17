namespace CGPlugin.Models.CustomDataAnnotations;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>
///     Настройка валидации значения количества зубьев шестерни
/// </summary>
public class TeethCountAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return new ValidationResult(ErrorMessage);

        var gearModel = (HelicalGearModel)validationContext.ObjectInstance;

        var uintVal = (uint)value;
        var minimum = 0;
        var maximum = (uint)(0.5 * Math.PI * gearModel.Diameter);

        if (uintVal <= minimum || uintVal > maximum)
            return new ValidationResult(
                $"Field {validationContext.DisplayName} must be between {minimum} and {maximum}.");

        return ValidationResult.Success;
    }
}