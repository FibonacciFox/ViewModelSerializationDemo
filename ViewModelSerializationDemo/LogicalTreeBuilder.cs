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

            // Сериализуем все типы свойств (Styled, Attached, Direct, CLR)
            PropertySerializer.SerializeProperties(control, node);

            // Обработка содержимого для ContentControl.
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
                        // node.Children.Add(new TextNode 
                        // { 
                        //     ElementType = child.GetType().Name, 
                        //     Text = child.ToString() ?? "" 
                        // });
                    }
                }
            }

            return node;
        }

        private static void ProcessContent(object? content, LogicalNode parentNode)
        {
            if (content == null)
                return;

            if (content is Control control)
            {
                parentNode.Children.Add(BuildLogicalTree(control));
            }
            else if (content is System.Collections.IEnumerable enumerable && !(content is string))
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
    }
}
