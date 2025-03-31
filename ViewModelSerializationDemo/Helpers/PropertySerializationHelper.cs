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
    /// Проверяет, является ли свойство runtime-only (неподходящее для сериализации).
    /// </summary>
    public static bool IsRuntimeProperty(PropertyInfo prop)
    {
        return RuntimeOnlyProperties.Contains(prop.Name);
    }

    /// <summary>
    /// Проверяет, можно ли сериализовать CLR-свойство в XAML.
    /// </summary>
    public static bool IsXamlSerializableClrProperty(PropertyInfo prop, Control control)
    {
        if (RuntimeOnlyProperties.Contains(prop.Name))
            return false;

        if (prop.GetIndexParameters().Length > 0)
            return false;

        // Пропускаем свойства, зарегистрированные как AvaloniaProperty
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
    /// Преобразует значение свойства в строку для XAML или отладки.
    /// </summary>
    public static string SerializeValue(object value)
    {
        return value switch
        {
            string s => s,
            bool b => b.ToString().ToLowerInvariant(),
            double d => d.ToString("G", CultureInfo.InvariantCulture),
            float f => f.ToString("G", CultureInfo.InvariantCulture),
            decimal m => m.ToString("G", CultureInfo.InvariantCulture),
            Enum e => e.ToString(),
            AvaloniaList<string> list => string.Join(",", list),
            IBrush brush => brush.ToString() ?? "Unknown",
            IBinding binding => SerializeBinding(binding),
            IResourceProvider => "DynamicResource",
            ITemplate => "Template",
            ILogical logical => logical.GetType().Name,
            _ when value.GetType().IsValueType => value.ToString() ?? string.Empty,
            _ => value.ToString() ?? string.Empty
        };
    }

    /// <summary>
    /// Упрощённая сериализация привязки.
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
    /// Определяет тип значения свойства.
    /// </summary>
    public static AvaloniaPropertyValueKind ResolveValueKind(object value)
    {
        return value switch
        {
            Control control => control.GetLogicalChildren().Any()
                ? AvaloniaPropertyValueKind.Logical
                : AvaloniaPropertyValueKind.Control,

            ILogical logical => logical.GetLogicalChildren().Any()
                ? AvaloniaPropertyValueKind.Logical
                : AvaloniaPropertyValueKind.Simple,

            AvaloniaList<string> => AvaloniaPropertyValueKind.Complex,
            IBinding => AvaloniaPropertyValueKind.Binding,
            ITemplate => AvaloniaPropertyValueKind.Template,
            IResourceProvider => AvaloniaPropertyValueKind.Resource,
            IBrush => AvaloniaPropertyValueKind.Brush,

            _ => value.GetType() is { } type && (type.IsPrimitive || type.IsEnum || value is string || type.IsValueType)
                ? AvaloniaPropertyValueKind.Simple
                : AvaloniaPropertyValueKind.Unknown
        };
    }

    /// <summary>
    /// Пытается построить сериализованное логическое дерево из значения.
    /// </summary>
    public static LogicalNode? TryBuildSerializedValue(object value)
    {
        return value switch
        {
            Control or ILogical or System.Collections.IEnumerable => LogicalTreeBuilder.BuildLogicalTreeFromObject(value),
            _ => null
        };
    }
}
