using System.Text;

namespace Configuration.Better;

internal static class ConfigurationPathExtensions
{
    public static IEnumerable<string> GetParentKeys(this string key)
    {
        var builder = new StringBuilder();

        foreach (var c in key)
        {
            if (c == ':' && builder.Length > 0)
            {
                yield return builder.ToString();
            }

            builder.Append(c);
        }
    }
}