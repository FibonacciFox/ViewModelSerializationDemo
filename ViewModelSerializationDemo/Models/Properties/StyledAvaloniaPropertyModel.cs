using Avalonia;
using Avalonia.Controls;
using ViewModelSerializationDemo.Helpers;

namespace ViewModelSerializationDemo.Models.Properties;

/// <summary>
/// Узел для styled-свойств.
/// </summary>
public class StyledAvaloniaPropertyModel : AvaloniaPropertyModel
{
    public static StyledAvaloniaPropertyModel? From(AvaloniaProperty property, Control control)
    {
        if ((control is ContentControl && property.Name == "Content") ||
            property.IsReadOnly ||
            !control.IsSet(property))
            return null;

        var value = control.GetValue(property);
        if (value == null)
            return null;

        return new StyledAvaloniaPropertyModel
        {
            Name = property.Name,
            Value = PropertySerializationHelper.SerializeValue(value),
            ValueKind = PropertySerializationHelper.ResolveValueKind(value),
            SerializedValue = PropertySerializationHelper.TryBuildSerializedValue(value)
        };
    }
}