using Microsoft.Extensions.Configuration;

namespace Configuration;

public static class ConfigBuilderExtensions
{
    public static T GetAndValidate<T>(this IConfiguration builder)
    {
        var config = builder.Get<T>();
        AppSettingsValidator.Validate(config);
        return config;
    }
}