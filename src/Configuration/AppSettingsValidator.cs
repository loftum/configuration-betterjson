using System.Reflection;
using Configuration.Reflection;

namespace Configuration;

public static class AppSettingsValidator
{
    public static void Validate(object appSettings)
    {
        if (appSettings == null)
        {
            throw new ArgumentNullException(nameof(appSettings));
        }

        var sections = appSettings.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsComplex());

        var missing = (from section in sections
                let value = section.GetValue(appSettings)
                where value == null
                select $"'{section.Name}'")
            .ToList();

        if (!missing.Any())
        {
            return;
        }

        var m = missing.Count == 1 ? "Section" : "Sections";
        var message = $"{appSettings.GetType().Name} validation failed. {m} {Friendly.StringJoin(missing, "and")} not set.";
        throw new ApplicationException(message);
    }
}