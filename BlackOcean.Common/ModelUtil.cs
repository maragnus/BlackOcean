using System.Diagnostics.CodeAnalysis;

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

    public static Dictionary<string, object> Diff<TModel>(TModel newModel, TModel oldModel) where TModel : class, new()
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
}