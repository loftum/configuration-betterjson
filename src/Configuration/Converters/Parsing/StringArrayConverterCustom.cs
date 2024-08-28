using System.ComponentModel;
using System.Globalization;

namespace Configuration.Converters.Parsing;

public class StringArrayConverterCustom : CollectionConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        return destinationType == typeof(string[]);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string strValue)
        {
            return strValue.SplitAndEnumerate(';').ToArray();
        }
        return base.ConvertFrom(context, culture, value);
    }
}

public static class StringExtensions
{
    public static IEnumerable<string> SplitAndEnumerate(this string me, char delimiter = ',')
    {
        return ("" + me).Split(delimiter).Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x));
    }
}