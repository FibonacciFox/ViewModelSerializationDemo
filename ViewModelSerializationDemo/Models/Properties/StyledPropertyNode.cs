using Avalonia;
using Avalonia.Controls;

namespace ViewModelSerializationDemo.Models.Properties;

/// <summary>
/// Узел для styled-свойств.
/// </summary>
public class StyledPropertyNode : PropertyNode
{
    public static StyledPropertyNode? From(AvaloniaProperty property, Control control)
    {
        // Пропускаем "Content" для ContentControl и только для чтения свойства.
        if (control is ContentControl && property.Name == "Content")
            return null;
        if (property.IsReadOnly)
            return null;
        if (!control.IsSet(property))
            return null;

        var value = control.GetValue(property);
        if (value == null)
            return null;

        return new StyledPropertyNode
        {
            Name = property.Name,
            Value = value.ToString()!
        };
    }
}