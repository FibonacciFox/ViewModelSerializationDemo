#nullable enable
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using System.Collections;

namespace ViewModelSerializationDemo
{
    public static class LogicalTreeBuilder
    {
        public static LogicalNode BuildLogicalTree(Control control)
        {
            // Создаем базовый узел для данного контроля.
            var node = new ElementNode
            {
                ElementType = control.GetType().Name
            };

            // Обрабатываем зарегистрированные styled-свойства для данного типа.
            var styledProperties = AvaloniaPropertyRegistry.Instance.GetRegistered(control.GetType());
            foreach (var prop in styledProperties)
            {
                // Пропускаем свойство "Content", если контрол является ContentControl,
                // чтобы избежать дублирования (оно будет обработано отдельно).
                if (control is ContentControl && prop.Name == "Content")
                    continue;

                if (control.IsSet(prop))
                {
                    var value = control.GetValue(prop);
                    if (value != null)
                    {
                        // Если значение можно адекватно преобразовать в строку, сериализуем его,
                        // иначе считаем, что оно установлено через Binding.
                        if (IsSimpleValue(value))
                        {
                            node.Attributes[prop.Name] = value.ToString()!;
                        }
                        else
                        {
                            // Здесь можно расширить логику для формирования binding-выражения.
                            //node.Attributes[prop.Name] = "{Binding}";
                        }
                    }
                }
            }

            // Обрабатываем зарегистрированные attached styled-свойства.
            var attachedProperties = AvaloniaPropertyRegistry.Instance.GetRegisteredAttached(control.GetType());
            foreach (var prop in attachedProperties)
            {
                if (control.IsSet(prop))
                {
                    var value = control.GetValue(prop);
                    if (value != null)
                    {
                        string attrName = $"{prop.OwnerType.Name}.{prop.Name}";
                        if (IsSimpleValue(value))
                        {
                            node.Attributes[attrName] = value.ToString()!;
                        }
                        else
                        {
                            //node.Attributes[attrName] = "{Binding}";
                        }
                    }
                }
            }

            // Если контрол унаследован от ContentControl, обрабатываем его свойство Content.
            if (control is ContentControl contentControl)
            {
                ProcessContent(contentControl.Content, node);
            }
            // Если контрол не является ContentControl, обходим его LogicalChildren.
            else if (control is ILogical logical)
            {
                foreach (var child in logical.LogicalChildren)
                {
                    if (child is Control childControl)
                    {
                        node.Children.Add(BuildLogicalTree(childControl));
                    }
                    else
                    {
                        // Если дочерний элемент не является Control, создаем текстовый узел.
                        node.Children.Add(new TextNode 
                        { 
                            ElementType = child.GetType().Name, 
                            Text = child.ToString() ?? "" 
                        });
                    }
                }
            }

            return node;
        }

        /// <summary>
        /// Универсальная обработка содержимого для свойства Content.
        /// Обрабатывает как отдельные объекты, так и коллекции.
        /// </summary>
        private static void ProcessContent(object? content, LogicalNode parentNode)
        {
            if (content == null)
                return;

            if (content is Control control)
            {
                parentNode.Children.Add(BuildLogicalTree(control));
            }
            else if (content is IEnumerable enumerable && !(content is string))
            {
                foreach (var item in enumerable)
                {
                    ProcessContent(item, parentNode);
                }
            }
            else
            {
                parentNode.Children.Add(new TextNode
                {
                    ElementType = content.GetType().Name,
                    Text = content.ToString() ?? ""
                });
            }
        }

        /// <summary>
        /// Проверяет, является ли значение простым для сериализации (числовые типы, строка, булево, перечисления, Thickness и т.п.).
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
}
