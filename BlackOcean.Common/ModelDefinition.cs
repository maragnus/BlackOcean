using System.Reflection;

namespace BlackOcean.Common;

public sealed partial class ModelDefinition
{
    public Type ModelType { get; }
    private readonly PropertyDefinition[] _properties;
    private readonly Dictionary<string, PropertyDefinition> _propertyDictionary;
    private readonly Func<object> _constructor;

    private readonly record struct PropertyDefinition(
        string Name,
        Type Type,
        Func<object, object?> Get,
        Action<object, object?> Set,
        Action<object, object> Cloner,
        bool IsNullable);

    private static readonly Dictionary<Type, ModelDefinition> Definitions = new(64);

    public static ModelDefinition GetDefinition(Type type)
    {
        lock (Definitions)
            return Definitions.GetOrAdd(type, key => new ModelDefinition(key));
    }

    private ModelDefinition(Type modelType)
    {
        var context = new NullabilityInfoContext();
        
        ModelType = modelType;
        _constructor = () => modelType.GetConstructor([])!.Invoke(null);
        _properties = modelType.GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Select(fi =>
            {
                var getter = CreateGetter(fi);
                var setter = CreateSetter(fi);
                var nullabilityInfo = context.Create(fi);
                var isNullable = nullabilityInfo.WriteState == NullabilityState.Nullable ||
                                 nullabilityInfo.ReadState == NullabilityState.Nullable;
                return new PropertyDefinition(fi.Name, fi.FieldType, getter, setter, BuildCloner(fi.FieldType, getter, setter), isNullable);
            })
            .Concat(
                modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(pi =>
                    {
                        var getter = CreateGetter(pi);
                        var setter = CreateSetter(pi);
                        var nullabilityInfo = context.Create(pi);
                        var isNullable = nullabilityInfo.WriteState == NullabilityState.Nullable ||
                                         nullabilityInfo.ReadState == NullabilityState.Nullable;
                        return new PropertyDefinition(pi.Name, pi.PropertyType, getter, setter, BuildCloner(pi.PropertyType, getter, setter), isNullable);
                    }))
            .ToArray();
        _propertyDictionary = _properties.ToDictionary(p => p.Name);
    }

    private static Func<object, object?> CreateGetter(FieldInfo property) => property.GetValue;

    private static Func<object, object?> CreateGetter(PropertyInfo property) => property.GetValue;

    private static Action<object, object?> CreateSetter(FieldInfo property) => property.SetValue;

    private static Action<object, object?> CreateSetter(PropertyInfo property) => property.SetValue;

    // Potential optimizations:
    // var getMethod = property.GetGetMethod(false)!;
    // return (Func<object, object?>)Delegate.CreateDelegate(typeof(Func<object, object?>), null, getMethod);
    // var setMethod = property.GetSetMethod(false)!;
    // return (Action<object, object?>)Delegate.CreateDelegate(typeof(Action<object, object?>), null, setMethod);
    
}