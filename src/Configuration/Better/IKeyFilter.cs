using Microsoft.Extensions.Configuration;

namespace Configuration.Better;

public interface IKeyFilter
{
    void Reset();
    bool Allow(string key);
    IEnumerable<IConfigurationSection> GetChildren(string path);
}