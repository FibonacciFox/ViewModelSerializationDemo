using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using ViewModelSerializationDemo.Models.Properties;

namespace ViewModelSerializationDemo
{
    public static class PropertySerializer
    {
        public static void SerializeProperties(Control control, LogicalNode node)
        {
            // Множество для отслеживания имен свойств, которые уже сериализованы через AvaloniaPropertyRegistry.
            var addedPropertyNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Сериализация styled-свойств.
            var styledProperties = AvaloniaPropertyRegistry.Instance.GetRegistered(control.GetType());
            foreach (var prop in styledProperties)
            {
                var styledNode = StyledAvaloniaPropertyModel.From(prop, control);
                if (styledNode != null)
                {
                    node.StyledProperties.Add(styledNode);
                    addedPropertyNames.Add(prop.Name);
                }
            }

            // Сериализация attached-свойств.
            var attachedProperties = AvaloniaPropertyRegistry.Instance.GetRegisteredAttached(control.GetType());
            foreach (var prop in attachedProperties)
            {
                var attachedNode = AttachedAvaloniaPropertyModel.From(prop, control);
                if (attachedNode != null)
                {
                    node.AttachedProperties.Add(attachedNode);
                    // Обычно для attached-свойств имя CLR не совпадает,
                    // поэтому здесь не добавляем в addedPropertyNames.
                }
            }

            // Сериализация direct-свойств.
            var directProperties = AvaloniaPropertyRegistry.Instance.GetRegisteredDirect(control.GetType());
            foreach (var prop in directProperties)
            {
                var directNode = DirectAvaloniaPropertyModel.From(prop, control);
                if (directNode != null)
                {
                    node.DirectProperties.Add(directNode);
                    addedPropertyNames.Add(prop.Name);
                }
            }

            // Сериализация CLR-свойств.
            var clrProperties = control.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in clrProperties)
            {
                // Если имя свойства уже добавлено через styled или direct, пропускаем его.
                if (addedPropertyNames.Contains(prop.Name))
                    continue;

                var clrNode = ClrAvaloniaPropertyModel.From(prop, control);
                if (clrNode != null)
                    node.ClrProperties.Add(clrNode);
            }
        }
    }
}
