using SpiegelHase.DataAnnotations;
using SpiegelHase.DataTransfer;
using SpiegelHase.Interfaces;
using System.Reflection;

namespace SpiegelHase;

public static class HaseExtension
{
    public static ModelDictionary RemoveIgnorable(this HaseViewModel model, object modelState)
    {
        PropertyInfo[] properties = model
            .GetType()
            .GetProperties();

        List<string> keys = [];

        foreach (PropertyInfo property in properties)
        {
            IgnorableAttribute? attr = property
                .GetCustomAttribute<IgnorableAttribute>();

            if (attr is not null)
                keys.Add(property.Name);
        }

        ModelDictionary result = ToModelDictionary(modelState);

        foreach (string name in keys)
            result.RemoveEntry(name);

        return result;
    }

    internal static ModelDictionary ToModelDictionary(object modelState)
    {
        ModelDictionary result = new();
        List<ModelEntry> list = [];

        Type modelType = modelState.GetType();
        PropertyInfo? keysProperty = modelType
            .GetProperty("Keys");

        if(keysProperty is null)
            return result;

        IEnumerable<string>? keys = (IEnumerable<string>?) keysProperty.GetValue(modelState);

        if(keys is null)
            return result;

        MethodInfo? value = modelType
            .GetMethod("TryGetValue");

        if(value is null) 
            return result;

        foreach(string key in keys)
        {
            object?[] parameters = [key, null];
            bool? exists = (bool?) value.Invoke(modelState, parameters);

            if(exists.HasValue && exists.Value)
            {
                object? entry = parameters[1];
                if(entry is not null)
                    list.Add(new ModelEntry(key, entry));
            }
        }

        foreach(ModelEntry entry in list)
            result.AddEntry(entry);

        return result;
    }

    public static string GetBackParameter(this IBack model)
    {
        List<string> parts =
        [
            model.BackController,
            model.BackAction
        ];

        if (!string.IsNullOrWhiteSpace(model.BackId))
            parts.Add(model.BackId);

        string result = "";
        foreach (string part in parts)
            result += $"/{part}";

        return result;
    }

    public static void SetBackModel(this IBack model, string controllerName, string actionName = "index", string backId = "")
    {
        model.BackController = controllerName;
        model.BackAction = actionName;
        model.BackId = backId;
        if (string.IsNullOrWhiteSpace(backId))
            model.BackId = null;
    }

    public static void SetBackModel(this IBack model, BackParameter parameter)
    {
        model.BackController = parameter.BackController;
        model.BackAction = parameter.BackAction;
        model.BackId = parameter.BackId;
    }

    public static void SetForwardModel(this IForward model, string forwardId, string controllerName, string actionName = "index", string backId = "")
    {
        model.ForwardId = forwardId;
        model.SetBackModel(controllerName, actionName, backId);
    }

    public static void SetForwardSidebarModel(this IForward model, ForwardParameter parameter)
    {
        model.ForwardId = parameter.ForwardId;
        model.SetBackModel(parameter);
    }

    public static BackParameter HandleBackParameter(this string? back, string fallbackController, string fallbackAction = "index", string fallbackBackId = "")
    {
        if (string.IsNullOrWhiteSpace(back))
            return new(fallbackController, fallbackAction, fallbackBackId);

        string[] backParts = back.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (backParts.Length == 0)
            return new(fallbackController, fallbackAction, fallbackBackId);

        BackParameter result;

        if (backParts.Length == 2)
        {
            result = new(backParts[0], backParts[1]);
            return result;
        }

        if (backParts.Length == 3)
        {
            result = new(backParts[0], backParts[1], backParts[2]);
            return result;
        }

        result = new(backParts[0]);
        return result;
    }

    public static BackParameter HandleBackParameter(this string back, BackParameter fallback)
    {
        string[] backParts = back.Split('/');

        int blen = backParts.Length;

        if (blen < 2 || blen > 3)
            return fallback;

        if (backParts.Length == 3)
            return new(backParts[0], backParts[1], backParts[2]);

        return new(backParts[0], backParts[1]);
    }

    public static void SetCustomSidebarModel(this ICustomPartial model, string sidebarUrl)
    {
        model.CustomPartial = $"~/Views/{sidebarUrl}.cshtml";
    }

    public static void SetPaginationModel<T>(this IPagination<T> model, int currentPage, int itemsPerPage)
    {
        model.TotalItems = model.List.Length;
        int start = (currentPage - 1) * itemsPerPage;
        int end = start + itemsPerPage;
        Range range = new(start, end);

        int totalItems = model.List.Length;
        int maxPages = (int)MathF.Ceiling((float)totalItems / itemsPerPage);

        T[] filter = model
            .List
            .Take(range)
            .ToArray();

        model.List = filter;
        model.CurrentPage = currentPage;
        model.MaxPages = maxPages;
    }

    public static async Task SetUserRolesModel<TType, TKey>(this IUserRoles<TType> model, TKey key, Func<TKey, Task<List<TType>>> predicate)
    {
        List<TType> roles = await predicate.Invoke(key);

        model.Roles = roles;
    }

    public static bool ContainsRole<T>(this IUserRoles<T> roles, params T[] roleNames)
    {
        foreach (T roleName in roleNames)
            if (roles.Roles.Contains(roleName))
                return true;
        return false;
    }
}
