using ViewModelSerializationDemo.Models.Properties;

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
        /// Коллекция узлов-свойств для styled свойств.
        /// </summary>
        public List<StyledAvaloniaPropertyModel> StyledProperties { get; set; } = new List<StyledAvaloniaPropertyModel>();

        /// <summary>
        /// Коллекция узлов-свойств для attached свойств.
        /// </summary>
        public List<AttachedAvaloniaPropertyModel> AttachedProperties { get; set; } = new List<AttachedAvaloniaPropertyModel>();

        /// <summary>
        /// Коллекция узлов-свойств для direct свойств.
        /// </summary>
        public List<DirectAvaloniaPropertyModel> DirectProperties { get; set; } = new List<DirectAvaloniaPropertyModel>();

        /// <summary>
        /// Коллекция узлов-свойств для CLR свойств.
        /// </summary>
        public List<ClrAvaloniaPropertyModel> ClrProperties { get; set; } = new List<ClrAvaloniaPropertyModel>();

        /// <summary>
        /// Дочерние узлы, если элемент является контейнером.
        /// </summary>
        public List<LogicalNode> Children { get; set; } = new List<LogicalNode>();
    }
}