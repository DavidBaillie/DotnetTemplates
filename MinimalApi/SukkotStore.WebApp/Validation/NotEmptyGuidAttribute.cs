using System.ComponentModel.DataAnnotations;

namespace SukkotStore.Domain.Validation;

/// <summary>
/// Validation attribute for GUIDs that check the GUID is not equal to <see cref="Guid.Empty"/> 
/// </summary>
public sealed class NotEmptyGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
        => IsValid(value, new ValidationContext(value ?? new object())) == ValidationResult.Success;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null || value is not Guid uuid || uuid != Guid.Empty)
            return ValidationResult.Success;

        return new ValidationResult("UUID cannot be zero/default");
    }
}
