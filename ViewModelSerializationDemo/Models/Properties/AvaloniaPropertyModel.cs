namespace ViewModelSerializationDemo.Models.Properties;

/// <summary>
/// Базовый класс для сериализованного свойства.
/// </summary>
public abstract class AvaloniaPropertyModel
{
    /// <summary>
    /// Имя свойства.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Строковое представление значения свойства.
    /// Используется для простых значений.
    /// </summary>
    public string? Value { get; set; }
    
    protected static bool IsSimpleValue(object value)
    {
        var type = value.GetType();
        return type.IsPrimitive || type.IsEnum || value is string || value is double || value is float || value is decimal;
    }
}