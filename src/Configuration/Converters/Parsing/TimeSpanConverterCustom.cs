using System.ComponentModel;
using System.Globalization;

namespace Configuration.Converters.Parsing;

public class TimeSpanConverterCustom : TimeSpanConverter
{
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        switch (value)
        {
            case string strValue:
                var ret = strValue.TryParseToTimeSpan();
                if (ret.HasValue) return ret.Value;
                break;
        }
        return base.ConvertFrom(context, culture, value);
    }
}

public class NullableTimeSpanConverterCustom : TimeSpanConverter
{
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string strValue)
        {
            var ret = strValue.TryParseToTimeSpan();
            if (ret.HasValue)
            {
                return ret.Value;
            }
        }

        try
        {
            return base.ConvertFrom(context, culture, value);
        }
        catch
        {
            return null;
        }
    }
}