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

    private AvaloniaPropertyValueKind _valueKind = AvaloniaPropertyValueKind.Unknown;

    /// <summary>
    /// Категория значения свойства (простое, Control, логическое и т.п.).
    /// Устанавливая это значение, автоматически обновляется CanBeSerializedToXaml.
    /// </summary>
    public AvaloniaPropertyValueKind ValueKind
    {
        get => _valueKind;
        set
        {
            _valueKind = value;
            CanBeSerializedToXaml = IsXamlCompatible(value);
        }
    }

    /// <summary>
    /// Возвращает true, если значение представляет собой Control или ILogical-объект с деревом.
    /// </summary>
    public bool IsContainsControl => SerializedValue != null;

    /// <summary>
    /// Если значение свойства является логическим деревом (например, Control),
    /// сохраняется его сериализованное представление.
    /// </summary>
    public LogicalNode? SerializedValue { get; set; }

    /// <summary>
    /// Можно ли сериализовать это свойство в axaml.
    /// </summary>
    public bool CanBeSerializedToXaml { get; private set; } = true;

    /// <summary>
    /// Определяет, совместим ли тип значения с axaml (можно ли представить его как строку).
    /// </summary>
    private static bool IsXamlCompatible(AvaloniaPropertyValueKind kind)
    {
        return kind switch
        {
            AvaloniaPropertyValueKind.Binding => false,
            AvaloniaPropertyValueKind.Template => false,
            AvaloniaPropertyValueKind.Resource => false,
            _ => true
        };
    }

    /// <summary>
    /// Проверяет, является ли значение простым типом (string, number и т.п.).
    /// </summary>
    protected static bool IsSimpleValue(object value)
    {
        var type = value.GetType();
        return type.IsPrimitive || type.IsEnum || value is string || value is double || value is float || value is decimal;
    }
}
