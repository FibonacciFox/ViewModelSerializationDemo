namespace ViewModelSerializationDemo
{
    /// <summary>
    /// Базовый класс для узлов логического дерева.
    /// </summary>
    public abstract class LogicalNode
    {
        /// <summary>
        /// Тип элемента (например, "StackPanel", "Button" и т.д.).
        /// </summary>
        public string? ElementType { get; set; }

        /// <summary>
        /// Атрибуты элемента (например, Margin, Width и т.п.).
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Дочерние узлы, если элемент является контейнером.
        /// </summary>
        public List<LogicalNode> Children { get; set; } = new List<LogicalNode>();
    }
}