namespace CGPlugin.Models.CustomDataAnnotations;

using System.ComponentModel.DataAnnotations;

/// <summary>
///     Настройка валидации значения модуля шестерни
/// </summary>
public class ModuleAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return new ValidationResult(ErrorMessage);

        var gearModel = (HelicalGearModel)validationContext.ObjectInstance;

        var uintVal = (uint)value;

        uint module = 0;
        if (gearModel.TeethCount != 0) module = gearModel.Diameter / gearModel.TeethCount;

        if (uintVal == 0)
            return new ValidationResult(
                $"The field {validationContext.DisplayName} must not be zero.");

        if (uintVal != module)
            return new ValidationResult(
                $"The field {validationContext.DisplayName} must be {module}.");

        return ValidationResult.Success;
    }
}