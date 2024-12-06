using System.Diagnostics.CodeAnalysis;

namespace BlackOcean.Common;

public partial class ModelDefinition
{
    private Action<object, object> BuildCloner(Type propertyType, Func<object, object?> getter,
        Action<object, object?> setter)
    {
        // Value types and strings can be assigned directly.
        if (propertyType.IsValueType || propertyType == typeof(string))
        {
            return (src, dest) => setter(dest, getter(src));
        }

        // Handle arrays
        if (propertyType.IsArray)
        {
            var elementType = propertyType.GetElementType()!;
            
            // Arrays of value types or strings can be cloned via ICloneable or just Array.Copy
            if (elementType.IsValueType || elementType == typeof(string))
                return (src, dest) =>
                {
                    if (getter(src) is Array srcArray)
                        setter(dest, srcArray.Clone());
                    else
                        setter(dest, null);
                };

            // Generic element types are not supported
            if (elementType.IsGenericType)
                throw new NotSupportedException("Generic properties cannot be cloned.");

            // Arrays of complex reference types
            var elementDefinition = GetDefinition(elementType);

            return (src, dest) =>
            {
                if (getter(src) is not Array srcArray)
                {
                    setter(dest, null);
                    return;
                }

                var length = srcArray.Length;
                var destArray = Array.CreateInstance(elementType, length);
                for (var i = 0; i < length; i++)
                {
                    var srcItem = srcArray.GetValue(i);
                    var clonedItem = elementDefinition.DeepClone(srcItem);
                    destArray.SetValue(clonedItem, i);
                }

                setter(dest, destArray);
            };
        }

        // Handle nullable primitives
        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return (src, dest) => setter(dest, getter(src));
        }

        // Other reference types
        if (propertyType.IsGenericType)
            throw new NotSupportedException("Generic properties cannot be cloned.");

        var definition = GetDefinition(propertyType);
        return (src, dest) =>
        {
            var srcValue = getter(src);
            setter(dest, definition.DeepClone(srcValue));
        };
    }

    private static object? ArrayClone(Array? srcArray, Type elementType)
    {
        if (srcArray == null) return null;
        
        // Arrays of value types or strings can be cloned via ICloneable or just Array.Copy
        if (elementType.IsValueType || elementType == typeof(string))
            return srcArray.Clone();

        // Generic element types are not supported
        if (elementType.IsGenericType)
            throw new NotSupportedException("Generic properties cannot be cloned.");

        // Arrays of complex reference types
        var elementDefinition = GetDefinition(elementType);

        var length = srcArray.Length;
        var destArray = Array.CreateInstance(elementType, length);
        for (var i = 0; i < length; i++)
        {
            var srcItem = srcArray.GetValue(i);
            var clonedItem = elementDefinition.DeepClone(srcItem);
            destArray.SetValue(clonedItem, i);
        }
        return destArray;
    }

    [return: NotNullIfNotNull(nameof(source))]
    public object? DeepClone(object? source)
    {
        if (source is null) return null;

        if (source.GetType() != ModelType)
            throw new InvalidOperationException("Source does not match ModelType");

        var dest = _constructor();
        foreach (var property in _properties)
            property.Cloner.Invoke(source, dest);
        return dest;
    }
    
    [return: NotNullIfNotNull(nameof(source))]
    private static object? DeepClone(Type type, object? source)
    {
        if (source is null) return null;
        return GetDefinition(type).DeepClone(source);
    }
}