using System.Globalization;
using System.Reflection;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Styling;
using ViewModelSerializationDemo.Models.Properties;

namespace ViewModelSerializationDemo.Helpers;

public static class PropertySerializationHelper
{
    private static readonly HashSet<string> RuntimeOnlyProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "ActualWidth", "ActualHeight", "Bounds", "LayoutSlot",
        "IsFocused", "IsKeyboardFocusWithin", "IsPointerOver",
        "IsEffectivelyVisible", "IsInitialized", "TemplatedParent", "Parent"
    };

    /// <summary>
    /// Проверяет, можно ли сериализовать CLR-свойство в axaml.
    /// </summary>
    public static bool IsXamlSerializableClrProperty(PropertyInfo prop, Control control)
    {
        if (RuntimeOnlyProperties.Contains(prop.Name))
            return false;

        if (prop.GetIndexParameters().Length > 0)
            return false;

        // Пропускаем зарегистрированные AvaloniaProperty
        var fieldName = prop.Name + "Property";
        var fieldInfo = control.GetType().GetField(fieldName,
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        if (fieldInfo != null && typeof(AvaloniaProperty).IsAssignableFrom(fieldInfo.FieldType))
            return false;

        var type = prop.PropertyType;

        if (prop.SetMethod?.IsPublic == true)
            return true;

        if (typeof(AvaloniaList<string>).IsAssignableFrom(type))
            return true;

        return false;
    }

    /// <summary>
    /// Преобразует значение свойства в сериализуемую строку.
    /// </summary>
    public static string SerializeValue(object value)
    {
        switch (value)
        {
            case string s:
                return s;

            case bool b:
                return b.ToString().ToLowerInvariant();

            case double d:
                return d.ToString("G", CultureInfo.InvariantCulture);

            case float f:
                return f.ToString("G", CultureInfo.InvariantCulture);

            case decimal m:
                return m.ToString("G", CultureInfo.InvariantCulture);

            case Enum e:
                return e.ToString();

            case AvaloniaList<string> list:
                return string.Join(",", list);

            case IBrush brush:
                return brush.ToString() ?? "Unknown";

            case IBinding binding:
                return SerializeBinding(binding);

            case IResourceProvider:
                return "DynamicResource";
            
            default:
                var type = value.GetType();

                if (type.IsValueType)
                    return value.ToString() ?? string.Empty;

                return value.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    /// Преобразует привязку в строку (упрощённо).
    /// </summary>
    private static string SerializeBinding(IBinding binding)
    {
        return binding switch
        {
            Binding b => $"Binding Path={b.Path}, Mode={b.Mode}",
            _ => "Binding"
        };
    }

    /// <summary>
    /// Определяет, какого типа значение (для ValueKind).
    /// </summary>
    public static AvaloniaPropertyValueKind ResolveValueKind(object value)
    {
        switch (value)
        {
            case Control:
                return AvaloniaPropertyValueKind.Control;

            case ILogical logical:
                return logical.LogicalChildren.Any()
                    ? AvaloniaPropertyValueKind.Logical
                    : AvaloniaPropertyValueKind.Simple;

            case AvaloniaList<string>:
                return AvaloniaPropertyValueKind.Complex;

            case IBinding:
                return AvaloniaPropertyValueKind.Binding;

            case ITemplate:
                return AvaloniaPropertyValueKind.Template;

            case IResourceProvider:
                return AvaloniaPropertyValueKind.Resource;

            default:
                var type = value.GetType();
                if (type.IsPrimitive || type.IsEnum || value is string || type.IsValueType)
                    return AvaloniaPropertyValueKind.Simple;

                return AvaloniaPropertyValueKind.Unknown;
        }
    }
}
