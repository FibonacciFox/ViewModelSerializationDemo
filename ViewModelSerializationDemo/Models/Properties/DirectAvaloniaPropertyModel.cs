using Avalonia;
using Avalonia.Controls;
using ViewModelSerializationDemo.Helpers;

namespace ViewModelSerializationDemo.Models.Properties;

/// <summary>
/// Узел для direct-свойств.
/// </summary>
public class DirectAvaloniaPropertyModel : AvaloniaPropertyModel
{
    public static DirectAvaloniaPropertyModel? From(AvaloniaProperty property, Control control)
    {
        if (property.IsReadOnly)
            return null;

        var value = control.GetValue(property);
        if (value == null)
            return null;

        return new DirectAvaloniaPropertyModel
        {
            Name = property.Name,
            Value = PropertySerializationHelper.SerializeValue(value),
            ValueKind = PropertySerializationHelper.ResolveValueKind(value),
            SerializedValue = PropertySerializationHelper.TryBuildSerializedValue(value)
        };
    }
}