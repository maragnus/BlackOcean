using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlackOcean.Common;

public static class ModelUtil
{
    [return: NotNullIfNotNull(nameof(model))]
    public static TModel? DeepClone<TModel>(TModel? model) where TModel : class, new()
    {
        if (model == null) return null;
        var definition = ModelDefinition.GetDefinition(typeof(TModel));
        return (TModel)definition.DeepClone(model);
    }

    public static Dictionary<string, object> Diff<TModel>(TModel? newModel, TModel? oldModel) where TModel : class, new()
    {
        var definition = ModelDefinition.GetDefinition(typeof(TModel));
        return definition.Diff(newModel, oldModel, false)!;
    }
    
    public static Dictionary<string, object> DiffApply<TModel>(TModel newModel, TModel oldModel) where TModel : class, new()
    {
        var definition = ModelDefinition.GetDefinition(typeof(TModel));
        return definition.Diff(newModel, oldModel, true)!;
    }

    public static void Apply<TModel>(TModel model, Dictionary<string, object> diff)
    {
        var definition = ModelDefinition.GetDefinition(typeof(TModel));
        definition.Apply(model, diff);
    }

    private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        IncludeFields = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        AllowTrailingCommas = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        PropertyNameCaseInsensitive = true
    };
    
    public static string Serialize(object model) => 
        JsonSerializer.Serialize(model, _jsonSerializerOptions);

    public static void Serialize(Stream stream, object model)
    {
        using var writer = new Utf8JsonWriter(stream);
        JsonSerializer.Serialize(writer, model, _jsonSerializerOptions);
    }

    public static string ToTypeScript<TModel>()
    {
        var definition = ModelDefinition.GetDefinition(typeof(TModel));
        return definition.ToTypeScript();
    }
}