using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace ViewModelSerializationDemo
{
    public static class LogicalTreeBuilder
    {
        public static LogicalNode BuildLogicalTree(Control control)
        {
            var node = new ElementNode
            {
                ElementType = control.GetType().Name,
            };

            // Сериализуем все типы свойств
            PropertySerializer.SerializeProperties(control, node);

            // Обрабатываем Content, если это ContentControl
            if (control is ContentControl contentControl)
            {
                ProcessContent(contentControl.Content, node);
            }
            // Обрабатываем логическое дерево
            else if (control is ILogical logical)
            {
                foreach (var child in logical.GetLogicalChildren())
                {
                    if (child is Control childControl)
                    {
                        node.Children.Add(BuildLogicalTree(childControl));
                    }
                }
            }

            return node;
        }

        public static LogicalNode BuildLogicalTreeFromILogical(ILogical logical)
        {
            var node = new ElementNode
            {
                ElementType = logical.GetType().Name
            };

            foreach (var child in logical.GetLogicalChildren())
            {
                switch (child)
                {
                    case Control control:
                        node.Children.Add(BuildLogicalTree(control));
                        break;

                    case { } sublogical:
                        node.Children.Add(BuildLogicalTreeFromILogical(sublogical));
                        break;

                    default:
                        node.Children.Add(new ElementNode
                        {
                            ElementType = child.GetType().Name
                        });
                        break;
                }
            }

            return node;
        }


        public static LogicalNode BuildLogicalTreeFromObject(object value)
        {
            return value switch
            {
                Control control => BuildLogicalTree(control),

                ILogical logical => BuildLogicalTreeFromILogical(logical),

                System.Collections.IEnumerable enumerable when value is not string =>
                    new ElementNode
                    {
                        ElementType = value.GetType().Name,
                        Children = enumerable
                            .Cast<object>()
                            .Select(BuildLogicalTreeFromObject)
                            .Where(_ => true)
                            .ToList()
                    },

                _ => new ElementNode
                {
                    ElementType = value.GetType().Name
                }
            };
        }

        private static void ProcessContent(object? content, LogicalNode parentNode)
        {
            if (content == null)
                return;

            switch (content)
            {
                case Control control:
                    parentNode.Children.Add(BuildLogicalTree(control));
                    break;

                case ILogical logical:
                    parentNode.Children.Add(BuildLogicalTreeFromILogical(logical));
                    break;

                case System.Collections.IEnumerable enumerable when content is not string:
                    foreach (var item in enumerable)
                    {
                        ProcessContent(item, parentNode);
                    }
                    break;

                default:
                    parentNode.Children.Add(new ElementNode
                    {
                        ElementType = content.GetType().Name
                    });
                    break;
            }
        }

    }
}
