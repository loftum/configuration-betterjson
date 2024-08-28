using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Configuration.Better.Json;

public class BetterJsonConfigurationFileParser
{
    public IDictionary<string, string> Data { get; } = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private readonly Stack<string> _context = new();
    private string _currentPath;

    public BetterJsonConfigurationFileParser(Stream input)
    {
        using var jsonDocument = JsonDocument.Parse(input, new JsonDocumentOptions
        {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        });
        VisitJsonElement(jsonDocument.RootElement);
    }

    private void VisitJsonElement(JsonElement jObject)
    {
        switch (jObject.ValueKind)
        {
            case JsonValueKind.Object:
                VisitJsonObject(jObject);
                break;
            case JsonValueKind.Array:
                VisitJsonArray(jObject);
                break;
            default:
                VisitJsonValue(jObject);
                break;
        }
    }

    private bool VisitJsonValue(JsonElement jValue)
    {
        var currentPath = _currentPath;
        if (Data.ContainsKey(currentPath))
        {
            throw new FormatException($"Setting {currentPath} is duplicated");
        }

        switch (jValue.ValueKind)
        {
            case JsonValueKind.Null:
                Data[currentPath] = null;
                MarkCurrentPathAsOverridden();
                return true;
            default:
                Data[currentPath] = jValue.ToString();
                return false;
        }
    }

    private void VisitJsonArray(JsonElement jArray)
    {
        var length = jArray.GetArrayLength();
        switch (length)
        {
            case 0:
                Data[_currentPath] = "";
                break;
            default:
                var counter = 0;
                foreach (var arrayElement in jArray.EnumerateArray())
                {
                    counter++;
                    EnterContext(counter.ToString());
                    VisitJsonElement(arrayElement);
                    ExitContext();
                }
                break;
        }
        MarkCurrentPathAsOverridden();
    }

    private void VisitJsonObject(JsonElement jObject)
    {
        foreach (var property in jObject.EnumerateObject())
        {
            VisitJsonProperty(property);
        }
    }

    private void VisitJsonProperty(JsonProperty property)
    {
        EnterContext(property.Name);
        VisitJsonElement(property.Value);
        ExitContext();
    }


    private void MarkCurrentPathAsOverridden()
    {
        Data[$"{_currentPath}~"] = null;
    }

    private void EnterContext(string context)
    {
        _context.Push(context);
        _currentPath = ConfigurationPath.Combine(_context.Reverse());
    }

    private void ExitContext()
    {
        _context.Pop();
        _currentPath = ConfigurationPath.Combine(_context.Reverse());
    }
}