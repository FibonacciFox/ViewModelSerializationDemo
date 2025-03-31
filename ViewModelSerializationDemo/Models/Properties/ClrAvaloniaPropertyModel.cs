using System.Reflection;
using Avalonia.Controls;
using ViewModelSerializationDemo.Helpers;

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
                    ValueKind = PropertySerializationHelper.ResolveValueKind(value)
                };
            }
            catch
            {
                return null;
            }
        }

    }
}
