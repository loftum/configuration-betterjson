using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Configuration.Better;

public class BetterConfigurationSection: IConfigurationSection
{
    private readonly IConfigurationRoot _root;
    private readonly IKeyFilter _filter;

    public BetterConfigurationSection(IConfigurationRoot root, IKeyFilter keyFilter, string path)
    {
        _filter = keyFilter ?? throw new ArgumentNullException(nameof(keyFilter));
        _root = root ?? throw new ArgumentNullException(nameof(root));
        Path = path ?? throw new ArgumentNullException(nameof(path));
        Key = ConfigurationPath.GetSectionKey(path);
    }

    public string Path { get; }
    public string Key { get; }
        
    public string Value
    {
        get => _root[Path];
        set => _root[Path] = value;
    }

    public string this[string key]
    {
        get => _root[ConfigurationPath.Combine(Path, key)];
        set => _root[ConfigurationPath.Combine(Path, key)] = value;
    }

    public IConfigurationSection GetSection(string key)
    {
        return _root.GetSection(ConfigurationPath.Combine(Path, key));
    }

    public IEnumerable<IConfigurationSection> GetChildren()
    {
        var children = _filter.GetChildren(Path);
        return children;
    }

    public IChangeToken GetReloadToken()
    {
        return _root.GetReloadToken();
    }

    public override string ToString()
    {
        return $"{Key} ({Path})";
    }
}