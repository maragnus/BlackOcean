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
    
    public static void Add<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key, TValue value) 
        where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out var list))
            list.Add(value);
        else
            dictionary.Add(key, [value]);
    }
    
    public static Dictionary<TKey, List<TValue>> ToListDictionary<T, TKey, TValue>(this IEnumerable<T> values, Func<T, TKey> keySelector, Func<T, TValue> elementSelector)
        where TKey : notnull
    {
        var dictionary = new Dictionary<TKey, List<TValue>>();

        foreach (var val in values)
            dictionary.Add(keySelector(val), elementSelector(val));

        return dictionary;
    }

    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory) 
        where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out var value))
            return value;

        value = factory(key);
        dictionary.Add(key, value);
        return value;
    }
}