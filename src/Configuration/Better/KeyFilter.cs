using Microsoft.Extensions.Configuration;

namespace Configuration.Better;

public class KeyFilter : IKeyFilter
{
    private readonly ISet<string> _whitelist = new HashSet<string>();
    private readonly ISet<string> _blacklist = new HashSet<string>();
    private readonly IConfigurationRoot _root;

    public KeyFilter(IConfigurationRoot root)
    {
        _root = root;
        Reset();
    }

    public IEnumerable<IConfigurationSection> GetChildren(string path)
    {
        var childKeys = _root.Providers
            .SelectMany(p => p.GetChildKeys(Enumerable.Empty<string>(), path))
            .Distinct()
            .Select(k => path == null ? k : ConfigurationPath.Combine(path, k));
        var filteredKeys = childKeys.Where(Allow);
            
        var children = filteredKeys
            .Select(_root.GetSection);
        return children.ToList();
    }

    public void Reset()
    {
        _whitelist.Clear();
        _blacklist.Clear();
        foreach (var provider in _root.Providers.Reverse())
        {
            Filter(provider);
        }
    }

    public bool Allow(string key)
    {
        return _whitelist.Contains(key);
    }

    private bool ShouldAdd(string key)
    {
        return !(_whitelist.Contains(key) || _blacklist.Any(key.StartsWith));
    }

    private void Filter(IConfigurationProvider provider)
    {
        var localBlacklist = new HashSet<string>();
        var keys = provider.TraverseChildKeys();
        var filteredKeys = keys.Where(ShouldAdd);
        foreach (var childKey in filteredKeys)
        {
            if (_blacklist.Any(childKey.StartsWith))
            {
                continue;
            }
            if (childKey.EndsWith("~"))
            {
                var blackListed = childKey.Substring(0, childKey.Length - 1);
                localBlacklist.Add(blackListed);
                continue;
            }

            _whitelist.Add(childKey);
        }
        _blacklist.AddRange(localBlacklist);
    }
}