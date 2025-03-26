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
        public List<StyledPropertyNode> StyledProperties { get; set; } = new List<StyledPropertyNode>();

        /// <summary>
        /// Коллекция узлов-свойств для attached свойств.
        /// </summary>
        public List<AttachedPropertyNode> AttachedProperties { get; set; } = new List<AttachedPropertyNode>();

        /// <summary>
        /// Коллекция узлов-свойств для direct свойств.
        /// </summary>
        public List<DirectPropertyNode> DirectProperties { get; set; } = new List<DirectPropertyNode>();

        /// <summary>
        /// Коллекция узлов-свойств для CLR свойств.
        /// </summary>
        public List<ClrPropertyNode> ClrProperties { get; set; } = new List<ClrPropertyNode>();

        /// <summary>
        /// Дочерние узлы, если элемент является контейнером.
        /// </summary>
        public List<LogicalNode> Children { get; set; } = new List<LogicalNode>();
    }
}