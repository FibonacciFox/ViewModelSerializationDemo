using Avalonia;
using Avalonia.Controls;
using ViewModelSerializationDemo.Helpers;

namespace ViewModelSerializationDemo.Models.Properties;

/// <summary>
/// Узел для direct-свойств.
/// </summary>
public class DirectAvaloniaPropertyModel : AvaloniaPropertyModel
{
    /// <summary>
    /// Явно исключаемые свойства, даже если они валидные CLR.
    /// </summary>
    private static readonly HashSet<string> ExcludedDirectProperties = new()
    {
        "Inlines","SelectedItems","Selection"
    };
    
    public static DirectAvaloniaPropertyModel? From(AvaloniaProperty property, Control control)
    {
        if (ExcludedDirectProperties.Contains(property.Name))
            return null;
        
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