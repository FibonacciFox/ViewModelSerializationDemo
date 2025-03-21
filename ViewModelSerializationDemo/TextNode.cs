namespace ViewModelSerializationDemo;

/// <summary>
/// Узел, представляющий текстовое содержимое.
/// </summary>
public class TextNode : LogicalNode
{
    /// <summary>
    /// Собственно текст.
    /// </summary>
    public string? Text { get; set; }
}