using System.Globalization;
using System.Text.RegularExpressions;

namespace Configuration.Converters.Parsing;

public static class TimeSpanParser
{
    private static readonly IEnumerable<Parser> Parsers = new[]
    {
        new Parser
        {
            Pattern = @"^\d+\.[mM]illis?$",
            Parse = @int => TimeSpan.FromMilliseconds(@int)
        },
        new Parser
        {
            Pattern = @"^\d+\.[mM]illiseconds?$",
            Parse = @int => TimeSpan.FromMilliseconds(@int)
        },
        new Parser
        {
            Pattern = @"^\d+\.[sS]econds?$",
            Parse = @int => TimeSpan.FromSeconds(@int)
        },
        new Parser
        {
            Pattern = @"^\d+\.[mM]inutes?$",
            Parse = @int => TimeSpan.FromMinutes(@int)
        },
        new Parser
        {
            Pattern = @"^\d+\.[hH]ours?$",
            Parse = @int => TimeSpan.FromHours(@int)
        },
        new Parser
        {
            Pattern = @"^\d+\.[dD]ays?$",
            Parse = @int => TimeSpan.FromDays(@int)
        },
        new Parser
        {
            Pattern = @"^\d+\.[wW]eeks?$",
            Parse = @int => TimeSpan.FromDays(@int*7)
        }
    };

    public static TimeSpan ToTimeSpan(this string timeSpan)
    {
        var parser = Parsers.FirstOrDefault(p => Regex.IsMatch(timeSpan, p.Pattern));

        if (parser != default(Parser))
        {
            return parser.Parse(timeSpan.IntPart());
        }

        TimeSpan parsed;
        if (TimeSpan.TryParse(timeSpan, CultureInfo.InvariantCulture, out parsed))
        {
            return parsed;
        }

        throw new NotSupportedException($@"The timespan string '{timeSpan}' is not a supported format. 
Supported formats are default TimeSpan formats and the following custom formats: 
{Parsers.Select(p => p.Pattern).Aggregate((first, second) => $"{first}, {second}")}");
    }

    public static TimeSpan? TryParseToTimeSpan(this string timeSpan)
    {
        try
        {
            return timeSpan.ToTimeSpan();
        }
        catch (ArgumentNullException)
        {
            return null;
        }
        catch (NotSupportedException)
        {
            return null;
        }
    }

    public static TimeSpan TryParseToTimeSpan(this string timeSpan, TimeSpan defaultValue)
    {
        try
        {
            return timeSpan.ToTimeSpan();
        }
        catch (ArgumentNullException)
        {
            return defaultValue;
        }
        catch (NotSupportedException)
        {
            return defaultValue;
        }
    }

    private static int IntPart(this string timespan)
    {
        return int.Parse(timespan.Split('.').First());
    }

    private class Parser
    {
        public string Pattern { get; set; }
        public Func<int, TimeSpan> Parse { get; set; }
    }
}