using System.Text;

namespace BlackOcean.Common;

public partial class ModelDefinition
{
    private static Dictionary<Type, string> _typeIndex = new()
    {
        [typeof(bool?)] = "boolean | undefined",
        [typeof(byte?)] = "number | undefined",
        [typeof(short?)] = "number | undefined",
        [typeof(int?)] = "number | undefined",
        [typeof(long?)] = "number | undefined",
        [typeof(float?)] = "number | undefined",
        [typeof(double?)] = "number | undefined",
        [typeof(bool)] = "boolean",
        [typeof(byte)] = "number",
        [typeof(short)] = "number",
        [typeof(int)] = "number",
        [typeof(long)] = "number",
        [typeof(float)] = "number",
        [typeof(double)] = "number",
    };
    
    public string ToTypeScript()
    {
        var sb = new StringBuilder();
        var typesAdded = new HashSet<Type>();
        var typesRemaining = new Stack<Type>();
        
        typesRemaining.Push(ModelType);

        while (typesRemaining.Count > 0)
            AppendTypeScriptType(sb, typesAdded, typesRemaining, typesRemaining.Pop());
        
        return sb.ToString();
    }

    private static void AppendTypeScriptType(StringBuilder sb, HashSet<Type> typesAdded, Stack<Type> typesRemaining, Type type)
    {
        if (!typesAdded.Add(type)) return;

        if (type.IsEnum)
        {
            sb.AppendLine($"export enum {type.Name} {{");
            foreach (var value in Enum.GetValues(type))
                sb.AppendLine($"    {value.ToString()},");
        }
        else
        {
            sb.AppendLine($"export interface {type.Name} {{");
            sb.AppendLine("    [index: string]: unknown");
            var definition = GetDefinition(type);
            foreach (var property in definition._properties)
            {
                var typeName = GetTypeScriptType(property.Type, property.IsNullable, typesAdded, typesRemaining);
                sb.AppendLine($"    {property.Name.ToCamelCase()}: {typeName}");
            }
        }
        sb.AppendLine("}").AppendLine();
    }

    private static string GetTypeScriptType(Type type, bool isNullable, HashSet<Type> typesAdded, Stack<Type> typesRemaining)
    {
        if (_typeIndex.TryGetValue(type, out var typeName))
            return typeName;
        
        typeName = type == typeof(string) ? "string" : type.Name;
        
        if (type.IsArray)
        {
            var elementType = GetTypeScriptType(type.GetElementType()!, false, typesAdded, typesRemaining);
            typeName = elementType.Contains('|') ? $"({elementType})[]" : $"{elementType}[]";
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            return GetTypeScriptType(type.GetGenericArguments()[0], true, typesAdded, typesRemaining);
        else if (!type.IsPrimitive && type != typeof(string) && !typesAdded.Contains(type))
            typesRemaining.Push(type);
        
        return isNullable ? $"{typeName} | undefined" : typeName;
    }
}