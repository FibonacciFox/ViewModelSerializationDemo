using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;

namespace ViewModelSerializationDemo.Models.Properties
{
    /// <summary>
    /// Узел для обычных CLR-свойств.
    /// </summary>
    public class ClrPropertyNode : PropertyNode
    {
        public static ClrPropertyNode? From(PropertyInfo prop, Control control)
        {
            if (!IsXamlAssignableProperty(prop, control))
                return null;

            try
            {
                var value = prop.GetValue(control);
                if (value == null)
                    return null;

                return new ClrPropertyNode
                {
                    Name = prop.Name,
                    Value = value.ToString()!
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

            // Исключаем примитивы с Parse (bool, int, double и т.д.)
            if (type.IsPrimitive || type.IsEnum || type == typeof(string))
                return false;

            var parseMethod = type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static,
                null, new[] { typeof(string) }, null);

            return parseMethod != null;
        }

    }
}
