using System.ComponentModel;
using System.Globalization;
using Configuration.Reflection;

namespace Configuration.Converters.Parsing;

public class StringHashSetConverterCustom : CollectionConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        return destinationType.In(typeof(HashSet<string>), typeof(ISet<string>));
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string strValue)
        {
            return strValue.SplitAndEnumerate(';').ToHashSet();
        }
        return base.ConvertFrom(context, culture, value);
    }
}