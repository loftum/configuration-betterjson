using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Configuration.Better;

public class BetterConfigurationRoot : IConfigurationRoot
{
    private ConfigurationReloadToken _reloadToken = new ConfigurationReloadToken();
    private readonly IList<IConfigurationProvider> _providers;
    private readonly IKeyFilter _keyFilter;

    public IEnumerable<IConfigurationProvider> Providers => _providers;

    public BetterConfigurationRoot(IList<IConfigurationProvider> providers)
    {
        _providers = providers;
        foreach (var provider in providers)
        {
            var p = provider;
            p.Load();
            ChangeToken.OnChange(() => p.GetReloadToken(), RaiseReloaded);
        }
        _keyFilter = new KeyFilter(this);
    }

    public string this[string key]
    {
        get
        {
            foreach (var provider in Providers.Reverse())
            {
                if (provider.TryGet(key, out var v))
                {
                    return v;
                }

                foreach (var parent in key.GetParentKeys().Reverse())
                {
                    if (provider.TryGet(parent, out var p) && p == null)
                    {
                        return null;
                    }
                }
            }

            return null;
        }
        set
        {
            foreach (var provider in _providers)
            {
                provider.Set(key, value);
            }
        }
    }

    public IEnumerable<IConfigurationSection> GetChildren()
    {
        return _keyFilter.GetChildren(null);
    }
    public IConfigurationSection GetSection(string key) => new BetterConfigurationSection(this, _keyFilter, key);

    public IChangeToken GetReloadToken() => _reloadToken;
        

    public void Reload()
    {
        _keyFilter.Reset();
        foreach (var provider in _providers)
        {
            provider.Load();
        }

        RaiseReloaded();
    }

    private void RaiseReloaded()
    {
        Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken()).OnReload();
    }
}