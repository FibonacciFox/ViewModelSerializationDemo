using Avalonia;
using Avalonia.Controls;

namespace ViewModelSerializationDemo.Models.Properties;

/// <summary>
/// Узел для attached-свойств.
/// </summary>
public class AttachedPropertyNode : PropertyNode
{
    public static AttachedPropertyNode? From(AvaloniaProperty property, Control control)
    {
        // Пропускаем "Content" для ContentControl и только для чтения свойства.
        if (property.Name == "NameScope")
            return null;
        if (property.IsReadOnly)
            return null;
        if (!control.IsSet(property))
            return null;

        var value = control.GetValue(property);
        if (value == null)
            return null;

        return new AttachedPropertyNode
        {
            Name = $"{property.OwnerType.Name}.{property.Name}",
            Value = value.ToString()!
        };
    }
}
