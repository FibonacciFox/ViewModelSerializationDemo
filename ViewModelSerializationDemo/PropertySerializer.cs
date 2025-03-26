using Avalonia;
using Avalonia.Controls;

namespace ViewModelSerializationDemo;

/// <summary>
/// Класс для сериализации свойств контрола в набор атрибутов,
/// включая styled, attached и direct свойства.
/// </summary>
public static class PropertySerializer
{
    /// <summary>
    /// Сериализует styled, attached и direct свойства контрола в словарь.
    /// </summary>
    public static Dictionary<string, string> SerializeProperties(Control control)
    {
        Dictionary<string, string> attributes = new Dictionary<string, string>();

        // Обработка styled-свойств (используем control.IsSet(prop))
        var styledProperties = AvaloniaPropertyRegistry.Instance.GetRegistered(control.GetType());
        foreach (var prop in styledProperties)
        {
            // Пропускаем свойство "Content" для ContentControl, чтобы избежать дублирования.
            if (control is ContentControl && prop.Name == "Content")
                continue;

            // Пропускаем только для чтения
            if (prop.IsReadOnly)
                continue;

            if (control.IsSet(prop))
            {
                var value = control.GetValue(prop);
                if (value != null)
                {
                    attributes[prop.Name] = value.ToString()!;
                }
            }
        }

        // Обработка attached-свойств
        var attachedProperties = AvaloniaPropertyRegistry.Instance.GetRegisteredAttached(control.GetType());
        foreach (var prop in attachedProperties)
        {
            // Пропускаем свойство "NameScope.NameScope".
            if (prop.Name == "NameScope")
                continue;
            
            if (prop.IsReadOnly)
                continue;

            if (control.IsSet(prop))
            {
                var value = control.GetValue(prop);
                if (value != null)
                {
                    string attrName = $"{prop.OwnerType.Name}.{prop.Name}";
                    attributes[attrName] = value.ToString()!;
                }
            }
        }

        // Обработка direct-свойств
        var directProperties = AvaloniaPropertyRegistry.Instance.GetRegisteredDirect(control.GetType());
        foreach (var prop in directProperties)
        {
            if (prop.IsReadOnly)
                continue;

            var value = control.GetValue(prop);
            if (value != null)
            {
                attributes[prop.Name] = value.ToString()!;
            }
        }
        
        // Обработка CLR-свойств через рефлексию
        var clrProperties = control.GetType()
            .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(prop => prop.CanRead &&
                           prop.GetIndexParameters().Length == 0 && // исключаем индексаторы
                           prop.GetMethod?.IsPublic == true &&       // обязательный публичный getter
                           prop.SetMethod?.IsPublic == true &&       // обязательный публичный setter
                           !attributes.ContainsKey(prop.Name));       // исключаем уже обработанные свойства

        foreach (var prop in clrProperties)
        {
            try
            {
                var value = prop.GetValue(control);
                if (value != null)
                {
                    // Если значение является простым (число, строка, bool и т.п.), просто сериализуем его.
                    if (IsSimpleValue(value))
                    {
                        attributes[prop.Name] = value.ToString()!;
                    }
                    else
                    {
                        // Если тип свойства не является простым, но содержит статический метод Parse(string),
                        // считаем, что его строковое представление достаточно для сериализации.
                        var type = value.GetType();
                        var parseMethod = type.GetMethod("Parse", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, new[] { typeof(string) }, null);
                        if (parseMethod != null)
                        {
                            attributes[prop.Name] = value.ToString()!;
                        }
                    }
                }
            }
            catch
            {
                // При необходимости можно логировать ошибки получения значения.
            }
        }



        return attributes;
    }



    /// <summary>
    /// Проверяет, является ли значение простым для сериализации (числовой тип, строка, булево, перечисление и т.п.).
    /// </summary>
    private static bool IsSimpleValue(object value)
    {
        var type = value.GetType();
        return type.IsPrimitive ||
               type.IsEnum ||
               value is string ||
               value is double ||
               value is float ||
               value is decimal;
    }
}