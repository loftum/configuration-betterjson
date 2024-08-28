using Microsoft.Extensions.Configuration;

namespace Configuration.Better.Json;

public class BetterJsonConfigurationSource : FileConfigurationSource
{
    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        EnsureDefaults(builder);
        return new BetterJsonConfigurationProvider(this);
    }
}