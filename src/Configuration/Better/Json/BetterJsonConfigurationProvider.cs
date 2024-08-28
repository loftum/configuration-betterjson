using Microsoft.Extensions.Configuration;

namespace Configuration.Better.Json;

public class BetterJsonConfigurationProvider : FileConfigurationProvider
{
        
    public BetterJsonConfigurationProvider(FileConfigurationSource source) : base(source)
    {
    }

    public override void Load(Stream stream)
    {
        var parser = new BetterJsonConfigurationFileParser(stream);
        Data = parser.Data;
    }
}