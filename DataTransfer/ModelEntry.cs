using System.Collections.ObjectModel;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SpiegelHase.DataTransfer;

public sealed class ModelEntry
{
    private readonly string m_key;
    public string Key => m_key;

    private readonly int m_isValid;
    public bool IsValid => m_isValid >= 2;

    public Collection<ModelError> Errors { get; set; }

    public ModelEntry(string key, object entry)
    {
        m_key = key;

        Type type = entry.GetType();

        m_isValid = HandleValidation(entry, type);
        
        Errors = HandleErrors(entry, type);
    }

    private static int HandleValidation(object entry, Type type)
    {
        PropertyInfo? validation = type.GetProperty("ValidationState");

        if (validation is null)
            return 4;

        object? value = validation.GetValue(entry);

        if (value is null)
            return 5;

        return (int)value;
    }

    private static Collection<ModelError> HandleErrors(object entry, Type type)
    {
        PropertyInfo? errors = type.GetProperty("Errors");

        if (errors is null)
            return [];

        object? value = errors.GetValue(entry);

        if (value is null)
            return [];

        Type errorsType = value.GetType();
        PropertyInfo? countProperty = errorsType.GetProperty("Count");

        if (countProperty is null)
            return [];

        PropertyInfo? itemProperty = errorsType.GetProperty("Item");

        if (itemProperty is null)
            return [];

        int? count = (int?)countProperty.GetValue(value);

        if(count is null)
            return [];

        List<ModelError> list = [];

        Type? errorType = null;
        PropertyInfo? messageProperty = null;
        PropertyInfo? exceptionProperty = null;

        for(int i = 0; i < count; i++)
        {
            object? error = itemProperty.GetValue(value, [i]);

            if (error is null)
                continue;

            if (errorType is null)
            {
                errorType = error.GetType();

                if (errorsType is null)
                    continue;
            }

            if(messageProperty is null)
            {
                messageProperty = errorType.GetProperty("ErrorMessage");

                if(messageProperty is null)
                    continue;
            }

            if(exceptionProperty is null)
            {
                exceptionProperty = errorType.GetProperty("Exception");

                if(exceptionProperty is null)
                    continue;
            }

            string? message = (string?)messageProperty.GetValue(error);

            if (string.IsNullOrWhiteSpace(message))
                message = "";

            Exception? exception = (Exception?)exceptionProperty.GetValue(error);

            ModelError modelError = new(message, exception);
            list.Add(modelError);


        }

        Collection<ModelError> result = [];

        foreach (ModelError modelError in list)
            result.Add(modelError);

        return result;
    }

    public override bool Equals(object? obj)
    {
        if(obj is ModelEntry entry)
        {
            if (Key == entry.Key)
                return true;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }
}
