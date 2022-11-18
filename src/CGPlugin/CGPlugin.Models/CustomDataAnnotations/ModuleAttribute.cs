namespace CGPlugin.Models.CustomDataAnnotations;

using System.ComponentModel.DataAnnotations;

/// <summary>
///   Настройка валидации значения модуля шестерни
/// </summary>
public class ModuleAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return new ValidationResult(ErrorMessage);

        var gearModel = (CitroenGearModel) validationContext.ObjectInstance;

        var uintVal = (uint) value;
        var module = gearModel.Diameter / (gearModel.TeethCount + 2);

        if (uintVal == 0)
        {
            return new ValidationResult($"The field {validationContext.DisplayName} must not be zero.");
        }

        if (uintVal != module)
            return new ValidationResult($"The field {validationContext.DisplayName} must be {module}.");

        return ValidationResult.Success;
    }
}