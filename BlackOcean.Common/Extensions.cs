using System.Reflection;
using System.Runtime.CompilerServices;

namespace BlackOcean.Common;

public static class Extensions
{
    public static string ToCamelCase(this string str) => 
        char.ToLowerInvariant(str[0]) + str[1..];
    
    public static bool IsNullable(this Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)
        || type.GetCustomAttribute<NullableAttribute>() != null;
    
    public static bool IsNumber(this Type type) =>
        type.IsPrimitive && (
            type == typeof(decimal)
            || type == typeof(decimal?)
            || type == typeof(byte) || type == typeof(byte?)
            || type == typeof(short) || type == typeof(short?)
            || type == typeof(int) || type == typeof(int?)
            || type == typeof(long) || type == typeof(long?)
            || type == typeof(float) || type == typeof(float?)
            || type == typeof(double) || type == typeof(double?));
}