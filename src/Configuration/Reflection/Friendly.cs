using System.Text;

namespace Configuration.Reflection;

public static class Friendly
{
    public static string StringJoin<T>(IEnumerable<T> values, string lastDelimiter = "og")
    {
        if (values == null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        var items = values.Select(v => v.ToString()).ToList();
        switch (items.Count)
        {
            case 0:
                return string.Empty;
            case 1:
                return items.Single();
        }
        return new StringBuilder(string.Join(", ", items.Take(items.Count - 1)))
            .Append($" {lastDelimiter} ").Append(items.Last())
            .ToString();
    }

    public static string FriendlyJoin(this IEnumerable<string> values, string lastDelimiter = "og")
    {
        return StringJoin(values, lastDelimiter);
    }

    public static readonly string[] ByteUnits = {"B", "KiB", "MiB", "GiB", "TiB"};

    public static string FriendlyBytes(this double bytes)
    {
        var number = bytes;
        var index = 0;
        while (number > 1024 && index < ByteUnits.Length - 1)
        {
            number /= 1024;
            index++;
        }
        return $"{number:##0.#} {ByteUnits[index]}";
    }

    public static string FriendlyBytes(this long bytes)
    {
        return ((double)bytes).FriendlyBytes();
    }

    public static readonly string[] NumberUnits = { "", "K", "M" };

    public static string FriendlyNumber(this double number)
    {
        var ret = number;
        var index = 0;
        while (ret > 1000 && index < NumberUnits.Length - 1)
        {
            ret /= 1000;
            index++;
        }

        return $"{ret:##0.#}{NumberUnits[index]}";
    }
}