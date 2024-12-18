namespace BlackOcean.Common;

public partial class ModelDefinition
{
    public void Apply<TModel>(TModel model, Dictionary<string, object?> diff)
    {
        foreach (var (path, value) in diff)
            SetValueByPath(model!, path, value);
    }

    private void SetValueByPath(object obj, ReadOnlySpan<char> path, object? value)
    {
        // If path contains a separator, follow into the property
        var index = path.IndexOf('.');
        if (index >= 0)
        {
            var propertyName = path[..index].ToString();
            if (!_propertyDictionary.TryGetValue(propertyName, out var nestedProperty))
                throw new InvalidOperationException($"Property '{propertyName}' does not exist.");
            
            var nestedObject = nestedProperty.Get(obj);
            if (nestedObject is null) return;
            
            var definition = GetDefinition(nestedProperty.Type);
            definition.SetValueByPath(nestedObject, path[(index+1)..], value);
            return;
        }
        
        if (!_propertyDictionary.TryGetValue(path.ToString(), out var property))
            throw new InvalidOperationException($"Property '{path}' does not exist.");

        // Otherwise, apply the property
        ApplyProperty(property, obj, value);
    }

    private static void ApplyProperty(in PropertyDefinition property, object obj, object? value)
    {
        var propertyType = property.Type;
        if (value is null)
            property.Set(obj, null);
        else if (propertyType == typeof(int) && value is double doubleValue)
            property.Set(obj, (int)doubleValue);
        else if (propertyType.IsValueType || propertyType == typeof(string))
            property.Set(obj, value);
        else if (propertyType.IsArray)
            property.Set(obj, ArrayClone((Array)value, value.GetType().GetElementType()!));
        else
            property.Set(obj, DeepClone(propertyType, value));
    }
}