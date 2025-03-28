using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;

namespace ViewModelSerializationDemo.Models.Properties
{
    /// <summary>
    /// Узел для обычных CLR-свойств.
    /// </summary>
    public class ClrAvaloniaPropertyModel : AvaloniaPropertyModel
    {
        /// <summary>
        /// Явно исключаемые свойства, даже если они валидные CLR.
        /// </summary>
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

            if (!IsXamlAssignableProperty(prop, control))
                return null;

            try
            {
                var value = prop.GetValue(control);
                if (value == null)
                    return null;

                var stringValue = SerializeValue(prop, value);
                if (string.IsNullOrWhiteSpace(stringValue))
                    return null;

                return new ClrAvaloniaPropertyModel
                {
                    Name = prop.Name,
                    Value = stringValue
                };
            }
            catch
            {
                return null;
            }
        }

        private static bool IsXamlAssignableProperty(PropertyInfo prop, Control control)
        {
            if (!prop.CanRead || prop.GetIndexParameters().Length > 0 || prop.GetMethod?.IsPublic != true)
                return false;

            var fieldName = prop.Name + "Property";
            var fieldInfo = control.GetType().GetField(fieldName,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (fieldInfo != null && typeof(AvaloniaProperty).IsAssignableFrom(fieldInfo.FieldType))
                return false;

            if (prop.SetMethod?.IsPublic == true)
                return true;

            var type = prop.PropertyType;

            if (type.IsPrimitive || type.IsEnum || type == typeof(string))
                return false;

            var parseMethod = type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static,
                null, new[] { typeof(string) }, null);

            return parseMethod != null;
        }

        /// <summary>
        /// Обрабатывает сериализацию значений особых типов (например, Classes).
        /// </summary>
        private static string SerializeValue(PropertyInfo prop, object value)
        {
            // Обработка всех AvaloniaList<string> (например: Classes, PseudoClasses и т.п.)
            if (value is AvaloniaList<string> stringList)
                return string.Join(",", stringList);

            return value.ToString() ?? string.Empty;
        }
    }
}
