using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;

namespace ViewModelSerializationDemo.Models.Properties;

public static class KnownContentHelper
{
    private static readonly Dictionary<Type, string> FallbackContentProperties = new()
    {
        // Контейнеры
        [typeof(ContentControl)] = "Content",
        [typeof(HeaderedContentControl)] = "Header",
        [typeof(ItemsControl)] = "Items",
        [typeof(Decorator)] = "Child",
        [typeof(TabItem)] = "Content",
        [typeof(Expander)] = "Content",
        [typeof(HeaderedSelectingItemsControl)] = "Header",
        [typeof(ContentPresenter)] = "Content",
    };

    /// <summary>
    /// Получает имя контентного свойства для данного типа.
    /// </summary>
    public static bool TryGetContentPropertyName(Type type, out string? contentPropertyName)
    {
        // 1. Поиск атрибута [Content]
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
        {
            if (prop.GetCustomAttribute<ContentAttribute>() != null)
            {
                contentPropertyName = prop.Name;
                return true;
            }
        }

        // 2. Поиск в fallback-словаре (учитывает базовые типы)
        while (type != typeof(object))
        {
            if (FallbackContentProperties.TryGetValue(type, out contentPropertyName))
                return true;

            type = type.BaseType!;
        }

        contentPropertyName = null;
        return false;
    }

    /// <summary>
    /// Является ли указанное имя свойства контентным для данного типа.
    /// </summary>
    public static bool IsKnownContentProperty(Type type, string propertyName)
    {
        return TryGetContentPropertyName(type, out var knownName) &&
               string.Equals(knownName, propertyName, StringComparison.OrdinalIgnoreCase);
    }
}