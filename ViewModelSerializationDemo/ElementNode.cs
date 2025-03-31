namespace ViewModelSerializationDemo;

/// <summary>
/// Узел, представляющий элемент управления.
/// </summary>
public class ElementNode : LogicalNode
{
    /// <summary>
    /// Имя элемента, если задано через свойство Name.
    /// </summary>
    public string? Name =>
        StyledProperties.FirstOrDefault(p => p.Name == "Name")?.Value
        ?? DirectProperties.FirstOrDefault(p => p.Name == "Name")?.Value;

    /// <summary>
    /// Отображаемое имя узла: "ElementType (Name: ...)".
    /// </summary>
    public string DisplayName =>
        string.IsNullOrWhiteSpace(Name)
            ? ElementType ?? "Unknown"
            : $"{ElementType} (Name: {Name})";
}