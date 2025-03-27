using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace ViewModelSerializationDemo.Models.Properties
{
    /// <summary>
    /// Узел для attached-свойств.
    /// </summary>
    public class AttachedPropertyNode : PropertyNode
    {
        /// <summary>
        /// Если значение свойства является сложным объектом (например, Control),
        /// здесь сохраняется его логическое представление.
        /// </summary>
        public LogicalNode? SerializedValue { get; set; }

        public static AttachedPropertyNode? From(AvaloniaProperty property, Control control)
        {
            // Пропускаем нежелательные свойства.
            if (property.Name == "NameScope")
                return null;
            if (property.IsReadOnly)
                return null;
            if (!control.IsSet(property))
                return null;

            var value = control.GetValue(property);
            if (value == null)
                return null;

            var node = new AttachedPropertyNode
            {
                Name = $"{property.OwnerType.Name}.{property.Name}"
            };

            // Если значение является простым, используем ToString().
            if (IsSimpleValue(value))
            {
                node.Value = value.ToString()!;
            }
            else if (value is Control childControl)
            {
                // Если значение является Control, сериализуем его логическое дерево.
                node.SerializedValue = LogicalTreeBuilder.BuildLogicalTree(childControl);
                // Можно указать тип контрола или другое обозначение.
                node.Value = childControl.GetType().Name;
            }
            else if (value is ILogical logical)
            {
                // Если значение реализует ILogical, но не является Control, пробуем вызвать ToString().
                node.Value = value.ToString()!;
            }
            else
            {
                // Фолбэк для других типов.
                node.Value = value.ToString()!;
            }

            return node;
        }
        
    }
}
