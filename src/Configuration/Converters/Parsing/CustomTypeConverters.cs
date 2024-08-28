using System.ComponentModel;

namespace Configuration.Converters.Parsing;

public static class CustomTypeConverters
{
    public static TypeDescriptionProvider Register<T, TC>() where TC : TypeConverter
    {
        return TypeDescriptor.AddAttributes(typeof(T), new TypeConverterAttribute(typeof(TC)));
    }
}