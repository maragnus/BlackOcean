namespace BlackOcean.Common;

public partial class ModelDefinition
{
    public Dictionary<string, object> Diff(object? newModel, object? oldModel, bool apply, double epsilon = 0.0001)
    {
        var changes = new Dictionary<string, object>();
        if (newModel is null && oldModel is null)
            return changes;

        // If either is null and not both
        if (newModel is null || oldModel is null)
        {
            // Entire object considered "changed"
            changes[string.Empty] = newModel!;
            return changes;
        }

        CollectDifferences(this, newModel, oldModel, string.Empty, changes, apply);
        return changes;
    }

    private static void CollectDifferences(ModelDefinition definition, object newModel, object oldModel, string currentPath,
        Dictionary<string, object> changes, bool apply)
    {
        foreach (var property in definition._properties)
        {
            var name = property.Name;
            var newVal = property.Get(newModel);
            var oldVal = property.Get(oldModel);
            var fullPath = string.IsNullOrEmpty(currentPath) ? name : $"{currentPath}.{name}";

            // Compare nulls
            if (newVal is null && oldVal is null)
                continue;
            
            if (newVal is null || oldVal is null)
            {
                changes[fullPath] = newVal!;
                if (apply) ApplyProperty(property, oldModel, newVal);
                continue;
            }

            var propertyType = property.Type;

            // For value types and strings, shallow compare
            if (propertyType.IsPrimitive || propertyType.IsValueType || propertyType == typeof(string))
            {
                if (Equals(newVal, oldVal)) continue;

                changes[fullPath] = newVal;
                if (apply) ApplyProperty(property, oldModel, newVal);
                continue;
            }

            // For arrays, shallow compare
            if (propertyType.IsArray)
            {
                if (ArraysEqual((Array)newVal, (Array)oldVal)) continue;

                changes[fullPath] = newVal;
                if (apply) ApplyProperty(property, oldModel, newVal);
                continue;
            }

            // Generic reference types are not supported
            if (propertyType.IsGenericType)
                throw new NotSupportedException("Generic reference types are not supported.");

            // For other complex reference types, recursively compare
            var nestedDef = GetDefinition(propertyType);
            CollectDifferences(nestedDef, newVal, oldVal, fullPath, changes, apply);
        }
    }

    private static bool ArraysEqual(Array newArr, Array oldArr)
    {
        if (newArr.Length != oldArr.Length)
            return false;
        
        for (var i = 0; i < newArr.Length; i++)
        {
            var nVal = newArr.GetValue(i);
            var oVal = oldArr.GetValue(i);
            if (!Equals(nVal, oVal))
                return false;
        }
        return true;
    }
}