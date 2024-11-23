using System.ComponentModel.DataAnnotations;

namespace SpiegelHase.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class RutAttribute : ValidationAttribute
{
    private static string GetErrorMessage(string field) => $"The {field} field doesn't have a valid rut format.";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        string displayName = validationContext.DisplayName;
        string? rut = value?.ToString();

        if (string.IsNullOrWhiteSpace(rut))
            return new(GetErrorMessage(displayName));

        rut = rut.ToLower();
        const string validChars = "0123456789k-.";

        foreach (char c in rut)
            if (!validChars.Contains(c))
                return new(GetErrorMessage(displayName));

        rut = rut.Replace(".", "");
        string[] rutParts = rut.Split('-');

        if (rutParts.Length != 2)
            return new(GetErrorMessage(displayName));

        if (rutParts[1].Length != 1)
            return new(GetErrorMessage(displayName));

        int rlen = rutParts[0].Length;

        if (rlen < 7 || rlen > 9)
            return new(GetErrorMessage(displayName));

        int[] serie = [2, 3, 4, 5, 6, 7];
        char[] reverseRut = rutParts[0]
            .Reverse()
            .ToArray();

        int next(int i)
        {
            while (i > serie.Length - 1)
                i -= serie.Length;
            return i;
        }

        int sum = 0;
        int index = 0;
        foreach (char c in reverseRut)
        {
            if (int.TryParse(c.ToString(), out int num))
            {
                int fact = serie[next(index)];
                int mult = num * fact;
                sum += mult;
                index++;
                continue;
            }
            return new(GetErrorMessage(displayName));
        }

        int div = sum / 11;
        div *= 11;
        int rest = Math.Abs(sum - div);
        int dv = 11 - rest;

        bool result = dv switch
        {
            10 => rutParts[1] == "k",
            11 => rutParts[1] == "0",
            _ => rutParts[1] == dv.ToString(),
        };

        if (!result)
            return new(GetErrorMessage(displayName));

        return ValidationResult.Success;
    }
}
