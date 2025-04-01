using Avalonia.Controls;
using Avalonia.Controls.Documents;

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
    public bool IsContainsControl =>
        SerializedValue is ElementNode el &&
        el.ElementType is not (nameof(Classes) or nameof(InlineCollection) or nameof(RowDefinitions) or nameof(ColumnDefinitions)) &&
        el.Children.Count > 0;

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
    /// Является ли свойство только для времени выполнения (например, ActualWidth).
    /// Такие свойства не сериализуются и используются только для отображения.
    /// </summary>
    public bool IsRuntimeOnly { get; set; } = false;
    
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
}
