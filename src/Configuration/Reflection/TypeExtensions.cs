using System.Collections;
using System.Reflection;

namespace Configuration.Reflection;

public static class TypeExtensions
{
    public static bool IsScalar(this Type type)
    {
        return type.IsEnum || type.In(
            typeof(string),
            typeof(Guid),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(bool),
            typeof(byte),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(float),
            typeof(double),
            typeof(decimal));
    }

    public static bool IsComplex(this Type type)
    {
        return type.IsClass && !type.IsCollection();
    }

    public static bool IsCollection(this Type type)
    {
        return typeof(ICollection).IsAssignableFrom(type) || 
               type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
    }

    public static bool Implements<TInterface>(this Type type) => type.GetInterfaces().Contains(typeof(TInterface));
    
    public static bool IsSubclassOf<T>(this Type type) => type.IsSubclassOf(typeof(T));

    public static bool IsEnumerable(this Type type)
    {
        return typeof(IEnumerable).IsAssignableFrom(type) ||
               type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    public static bool IsGeneric(this Type type, Type generic)
    {
        return type.IsGenericType &&
               type.GetGenericTypeDefinition() == generic;
    }

    private static readonly IDictionary<Type, string> ValueNames = new Dictionary<Type, string>
    {
        {typeof(int), "int"},
        {typeof(short), "short"},
        {typeof(byte), "byte"},
        {typeof(bool), "bool"},
        {typeof(long), "long"},
        {typeof(float), "float"},
        {typeof(double), "double"},
        {typeof(decimal), "decimal"},
        {typeof(string), "string"}
    };

    public static string GetFriendlyName(this Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        if (ValueNames.ContainsKey(type))
        {
            return ValueNames[type];
        }
        if (type.IsGenericType)
        {
            return $"{type.Name.Split('`')[0]}<{string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName))}>";
        }
        return type.Name;
    }

    public static IEnumerable<FieldInfo> GetConstants(this Type type)
    {
        return type.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral && !f.IsInitOnly);
    }

    public static IEnumerable<FieldInfo> GetConstants<T>(this Type type)
    {
        return type.GetConstants().Where(f => f.FieldType is T);
    }

    public static object NewUp(this Type type, params object[] ctorParameters)
    {
        var types = ctorParameters.Select(a => a.GetType()).ToArray();
        var ctor = type.GetConstructor(types);
        if (ctor == null)
        {
            var parameters = string.Join(", ", types.Select(GetFriendlyName));
            var ctorDisplay = $"{type.GetFriendlyName()}({parameters})";
            throw new InvalidOperationException($"There is no ctor {ctorDisplay}");
        }
        return ctor.Invoke(ctorParameters);
    }

    public static T NewUpAs<T>(this Type type, params Type[] ctorArguments)
    {
        var ctor = type.GetConstructor(ctorArguments);
        if (ctor == null)
        {
            throw new InvalidOperationException($"Cannot new up {type.Name}. (No default constructor)");
        }
        return (T)ctor.Invoke(new object[0]);
    }
}