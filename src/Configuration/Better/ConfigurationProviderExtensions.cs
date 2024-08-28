using Microsoft.Extensions.Configuration;

namespace Configuration.Better;

public static class ConfigurationProviderExtensions
{
    public static IEnumerable<string> TraverseChildKeys(this IConfigurationProvider provider)
    {
        return provider.TraverseChildKeys(null);
    }

    private static IEnumerable<string> TraverseChildKeys(this IConfigurationProvider provider, string parentPath)
    {
        var childKeys = provider.GetChildKeys(Enumerable.Empty<string>(), parentPath)
            .Select(k => parentPath == null ? k : ConfigurationPath.Combine(parentPath, k))
            .ToHashSet();
        if (childKeys.Any())
        {
            var descendants = childKeys.SelectMany(provider.TraverseChildKeys).ToHashSet();
            childKeys.AddRange(descendants);
        }
        return childKeys;
    }
}