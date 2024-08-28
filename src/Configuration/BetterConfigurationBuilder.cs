using Microsoft.Extensions.Configuration;

namespace Configuration;

public class BetterConfigurationBuilder : ConfigurationBuilder
{
    public new IConfigurationRoot Build()
    {
        return this.BuildBetter();
    }
}