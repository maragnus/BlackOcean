using System.Reflection;

namespace BlackOcean.Common;

public sealed partial class ModelDefinition
{
    public Type ModelType { get; set; }
    private PropertyDefinition[] _properties;
    private Dictionary<string, PropertyDefinition> _propertyDictionary;
    private Func<object> _constructor;

    readonly record struct PropertyDefinition(
        string Name,
        Type Type,
        Func<object, object?> Get,
        Action<object, object?> Set,
        Action<object, object> Cloner,
        bool IsNullable);

    private static readonly Dictionary<Type, Common.ModelDefinition> Definitions = new(64);

    public static Common.ModelDefinition GetDefinition(Type type)
    {
        if (Definitions.TryGetValue(type, out var definition))
            return definition;
        return Definitions[type] = new Common.ModelDefinition(type);
    }
    
    public ModelDefinition(Type modelType)
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

    private static Func<object, object?> CreateGetter(FieldInfo property) => (obj) => property.GetValue(obj);

    private static Func<object, object?> CreateGetter(PropertyInfo property) => (obj) => property.GetValue(obj);

    private static Action<object, object?> CreateSetter(FieldInfo property) => (obj, value) => property.SetValue(obj, value);

    private static Action<object, object?> CreateSetter(PropertyInfo property) => (obj, value) => property.SetValue(obj, value);

    // Potential optimizations:
    // var getMethod = property.GetGetMethod(false)!;
    // return (Func<object, object?>)Delegate.CreateDelegate(typeof(Func<object, object?>), null, getMethod);
    // var setMethod = property.GetSetMethod(false)!;
    // return (Action<object, object?>)Delegate.CreateDelegate(typeof(Action<object, object?>), null, setMethod);
    
}