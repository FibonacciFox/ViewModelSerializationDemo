using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using ViewModelSerializationDemo.Helpers;

namespace ViewModelSerializationDemo.Models.Properties
{
    /// <summary>
    /// Узел для attached-свойств.
    /// </summary>
    public class AttachedAvaloniaPropertyModel : AvaloniaPropertyModel
    {
        public static AttachedAvaloniaPropertyModel? From(AvaloniaProperty property, Control control)
        {
            if (property.Name == "NameScope" || property.IsReadOnly || !control.IsSet(property))
                return null;

            var value = control.GetValue(property);
            if (value == null)
                return null;

            var node = new AttachedAvaloniaPropertyModel
            {
                Name = $"{property.OwnerType.Name}.{property.Name}",
                Value = PropertySerializationHelper.SerializeValue(value),
                ValueKind = PropertySerializationHelper.ResolveValueKind(value)
            };

            if (value is Control childControl)
            {
                node.SerializedValue = LogicalTreeBuilder.BuildLogicalTree(childControl);
            }
            else if (value is ILogical logical && logical.LogicalChildren.Any())
            {
                // Можно добавить сериализацию логических элементов, если нужно
            }

            return node;
        }

    }
}
