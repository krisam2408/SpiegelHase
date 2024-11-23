using System.ComponentModel.DataAnnotations;

namespace SpiegelHase.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class HoneyTrapAttribute : ValidationAttribute
{
    private const string HoneyErrorMessage = "Trap sprung";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        string? honey = value?.ToString();

        if (string.IsNullOrWhiteSpace(honey))
            return ValidationResult.Success;

        return new(HoneyErrorMessage);
    }
}
