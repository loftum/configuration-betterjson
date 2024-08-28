using Configuration.Better;
using Configuration.Better.Json;
using Configuration.Converters.Parsing;
using Microsoft.Extensions.Configuration;

namespace Configuration;

public static class ConfigBuilder
{   
    static ConfigBuilder()
    {
        CustomTypeConverters.Register<TimeSpan, TimeSpanConverterCustom>(); //be able to read "30.Days" as TimeSpan
        CustomTypeConverters.Register<TimeSpan, NullableTimeSpanConverterCustom>(); //be able to read "30.Days" as TimeSpan?
        CustomTypeConverters.Register<List<string>, StringListConverterCustom>(); //be able to read "a;b;c" as new List<string>{"a","b","c"};
        CustomTypeConverters.Register<IList<string>, StringListConverterCustom>();
        CustomTypeConverters.Register<string[], StringArrayConverterCustom>();
        CustomTypeConverters.Register<HashSet<string>, StringHashSetConverterCustom>();
        CustomTypeConverters.Register<ISet<string>, StringHashSetConverterCustom>();
    }

    /// <summary>
    /// Returns ConfigurationBuilder with default settings
    /// </summary>
    public static IConfigurationBuilder Default
    {
        get
        {
            var path = FolderPath.Parse(AppDomain.CurrentDomain.BaseDirectory);
            var builder = new ConfigurationBuilder();
            builder.Sources.Clear();
            return new ConfigurationBuilder().SetBasePath(path);
        }
    }

    /// <summary>
    /// Adds json file with optional local override. E.g: appsettings.something.json + appsettings.something.local.json (optional)
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="path"></param>
    /// <param name="optional"></param>
    /// <returns></returns>
    public static IConfigurationBuilder AddBetterJsonWithLocalOverride(this IConfigurationBuilder builder, string path, bool optional = false)
    {
        var overrideName = path.Replace(".json", ".local.json");
        return builder
            .AddBetterJsonFile(path, optional, false)
            .AddBetterJsonFile(overrideName, true, false);
    }

    public static IConfigurationBuilder AddBetterJsonFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
    {
        return builder
            .Add<BetterJsonConfigurationSource>(s =>
            {
                s.Path = path;
                s.Optional = optional;
                s.ReloadOnChange = reloadOnChange;
                s.ResolveFileProvider();
            });
    }
 
    public static IConfigurationRoot BuildBetter(this IConfigurationBuilder builder)
    {
        return new BetterConfigurationRoot(builder.Sources.Select(s => s.Build(builder)).ToList());
    }
}