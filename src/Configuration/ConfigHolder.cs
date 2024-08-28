using Microsoft.Extensions.Configuration;

namespace Configuration;

public class ConfigHolder<T>
{
    public event EventHandler<T> OnReloaded;
        
    public IConfiguration Raw { get; }
    public T Settings { get; private set; }

    public ConfigHolder(IConfiguration config)
    {
        Raw = config;
        Settings = config.GetAndValidate<T>();
        ReloadOnNextChange();
    }

    private void ReloadOnNextChange()
    {
        Raw.GetReloadToken().RegisterChangeCallback(o => Reload(), null);
    }

    private void Reload()
    {
        try
        {
            Settings = Raw.GetAndValidate<T>();
            ReloadOnNextChange();
            OnReloaded?.Invoke(this, Settings);
        }
        catch
        {
            DoNothing();
        }
    }

    private static void DoNothing()
    {
    }
}

public static class ConfigurationExtensions
{
    public static ConfigHolder<T> GetConfigHolder<T>(this IConfiguration config)
    {
        return new ConfigHolder<T>(config);
    }
}