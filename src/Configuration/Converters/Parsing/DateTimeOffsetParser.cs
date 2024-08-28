using System.Globalization;

namespace Configuration.Converters.Parsing;

public static class DateTimeOffsetParser
{
    public static DateTimeOffset? TryParseDateToDateTimeOffset(this string input)
    {
        return TryParseDateToDateTimeOffset(input, "yyyy-MM-dd")
               ?? TryParseDateToDateTimeOffset(input, "dd.MM.yyyy");
    }




    private static DateTimeOffset? TryParseDateToDateTimeOffset(this string input, string format)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        var yearIndex = 2;
        var monthIndex = 1;
        var dayIndex = 0;
        if (format.StartsWith("y"))
        {
            yearIndex = 0;
            dayIndex = 2;
        }

        var parts = input.Split('-', '.');
        if (parts.Length != 3) return null;

        var year = parts[yearIndex].TryParseToInt() ?? 0;
        if (parts[yearIndex].Length == 2 && year > 0 && format.Contains("yyyy"))
        {
            year += 2000;
        }
        var toParse = "";
        toParse += year
                   + "-"
                   + (parts[monthIndex].TryParseToInt() ?? 0).ToString("D2")
                   + "-"
                   + (parts[dayIndex].TryParseToInt() ?? 0).ToString("D2");

        if (DateTimeOffset.TryParseExact(toParse, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out var date))
        {
            return date;
        }

        return null;
    }
    public static DateTimeOffset? TryParseToDateTimeOffset(this string input, string format)
    {
           
        if (DateTimeOffset.TryParseExact(input, format, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out var date))
        {
            return date;
        }

        return null;
    }
    public static Guid? TryParseToGuid(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        if (Guid.TryParse(input, out var guid))
        {
            return guid;
        }

        return null;
    }
    public static int? TryParseToInt(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        if (int.TryParse(input, out var number))
        {
            return number;
        }

        return null;
    }
    public static long? TryParseToLong(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        if (long.TryParse(input, out var number))
        {
            return number;
        }

        return null;
    }
    public static bool? TryParseToBool(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        if ("true".Equals(input, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if ("false".Equals(input, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        return null;
    }
}