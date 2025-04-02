using ViewModelSerializationDemo.Helpers;
using ViewModelSerializationDemo.Models.Properties;

namespace ViewModelSerializationDemo;

/// <summary>
/// Базовый класс для узлов логического дерева.
/// </summary>
public abstract class LogicalNode
{
    /// <summary>
    /// Тип элемента (например, "StackPanel", "Button").
    /// </summary>
    public string? ElementType { get; set; }

    /// <summary>
    /// Оригинальный экземпляр объекта (Control, ILogical и т.п.).
    /// </summary>
    public object? OriginalInstance { get; set; }

    /// <summary>
    /// Категория значения для самого узла (Control, Logical и т.п.).
    /// </summary>
    public AvaloniaValueKind ValueKind { get; set; } = AvaloniaValueKind.Unknown;

    /// <summary>
    /// Свойства типа Styled.
    /// </summary>
    public List<StyledAvaloniaPropertyModel> StyledProperties { get; set; } = new();

    /// <summary>
    /// Свойства типа Attached.
    /// </summary>
    public List<AttachedAvaloniaPropertyModel> AttachedProperties { get; set; } = new();

    /// <summary>
    /// Свойства типа Direct.
    /// </summary>
    public List<DirectAvaloniaPropertyModel> DirectProperties { get; set; } = new();

    /// <summary>
    /// Свойства типа CLR.
    /// </summary>
    public List<ClrAvaloniaPropertyModel> ClrProperties { get; set; } = new();

    /// <summary>
    /// Дочерние узлы (если элемент является контейнером).
    /// </summary>
    public List<LogicalNode> Children { get; set; } = new();
    
    /// <summary>
    /// Содержит ли хотя бы один дочерний Control (ElementNode).
    /// </summary>
    public bool IsContainsControl => Children.Any(c => c is ElementNode);
}