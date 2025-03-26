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
            // Требуем наличие публичного геттера и сеттера, отсутствие индексаторов.
            if (!prop.CanRead || prop.GetIndexParameters().Length > 0 ||
                prop.GetMethod?.IsPublic != true || prop.SetMethod?.IsPublic != true)
                return null;

            // Если для свойства существует статическое поле с именем {PropertyName}Property,
            // и его тип наследуется от AvaloniaProperty, то это не чистое CLR-свойство.
            var fieldName = prop.Name + "Property";
            var fieldInfo = control.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (fieldInfo != null && typeof(AvaloniaProperty).IsAssignableFrom(fieldInfo.FieldType))
                return null;

            try
            {
                var value = prop.GetValue(control);
                if (value == null)
                    return null;

                if (IsSimpleValue(value))
                {
                    return new ClrPropertyNode
                    {
                        Name = prop.Name,
                        Value = value.ToString()!
                    };
                }
                else
                {
                    // Если тип свойства не простой, но предоставляет статический метод Parse(string),
                    // предполагаем, что ToString() даст корректное строковое представление.
                    var type = value.GetType();
                    var parseMethod = type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
                    if (parseMethod != null)
                    {
                        return new ClrPropertyNode
                        {
                            Name = prop.Name,
                            Value = value.ToString()!
                        };
                    }
                }
            }
            catch
            {
                // При необходимости можно логировать ошибку.
            }
            
            return null;
        }
        
    }
}
