using Avalonia;
using Avalonia.Controls;

namespace ViewModelSerializationDemo.Models.Properties;

/// <summary>
/// Узел для direct-свойств.
/// </summary>
public class DirectPropertyNode : PropertyNode
{
    public static DirectPropertyNode? From(AvaloniaProperty property, Control control)
    {
        if (property.IsReadOnly)
            return null;

        var value = control.GetValue(property);
        if (value == null)
            return null;

        return new DirectPropertyNode
        {
            Name = property.Name,
            Value = value.ToString()!
        };
    }
}