namespace ViewModelSerializationDemo.Models.Properties;

/// <summary>
/// Базовый класс для сериализованного свойства.
/// </summary>
public abstract class PropertyNode
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    protected static bool IsSimpleValue(object value)
    {
        var type = value.GetType();
        return type.IsPrimitive || type.IsEnum || value is string || value is double || value is float || value is decimal;
    }
}