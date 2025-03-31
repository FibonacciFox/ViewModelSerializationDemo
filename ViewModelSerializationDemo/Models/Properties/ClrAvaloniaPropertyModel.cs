using System.Reflection;
using Avalonia.Controls;
using ViewModelSerializationDemo.Helpers;

namespace ViewModelSerializationDemo.Models.Properties;

/// <summary>
/// Узел для обычных CLR-свойств.
/// </summary>
public class ClrAvaloniaPropertyModel : AvaloniaPropertyModel
{
    private static readonly HashSet<string> ExcludedProperties = new()
    {
        "DefiningGeometry",
        "RenderedGeometry",
        "Resources"
    };

    public static ClrAvaloniaPropertyModel? From(PropertyInfo prop, Control control)
    {
        if (ExcludedProperties.Contains(prop.Name))
            return null;

        bool isRuntimeOnly = PropertySerializationHelper.IsRuntimeProperty(prop);

        if (!PropertySerializationHelper.IsXamlSerializableClrProperty(prop, control))
            return null;

        try
        {
            var value = prop.GetValue(control);
            if (value == null)
                return null;

            return new ClrAvaloniaPropertyModel
            {
                Name = prop.Name,
                Value = PropertySerializationHelper.SerializeValue(value),
                ValueKind = PropertySerializationHelper.ResolveValueKind(value),
                IsRuntimeOnly = isRuntimeOnly,
                SerializedValue = PropertySerializationHelper.TryBuildSerializedValue(value)
            };
        }
        catch
        {
            return null;
        }
    }
}