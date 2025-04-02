using System.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using ViewModelSerializationDemo.Helpers;

namespace ViewModelSerializationDemo
{
    public static class LogicalTreeBuilder
    {
        public static LogicalNode BuildLogicalTree(Control control)
        {
            var node = new ElementNode
            {
                ElementType = control.GetType().Name,
                OriginalInstance = control,
                ValueKind = PropertySerializationHelper.ResolveValueKind(control)
            };

            PropertySerializer.SerializeProperties(control, node);

            if (control is ContentControl contentControl)
            {
                ProcessContent(contentControl.Content, node);
            }
            else if (control is ILogical logical)
            {
                foreach (var child in logical.GetLogicalChildren())
                {
                    if (child is Control childControl)
                        node.Children.Add(BuildLogicalTree(childControl));
                }
            }

            return node;
        }


        public static LogicalNode BuildLogicalTreeFromILogical(ILogical logical)
        {
            var node = new ElementNode
            {
                ElementType = logical.GetType().Name,
                OriginalInstance = logical,
                ValueKind = PropertySerializationHelper.ResolveValueKind(logical)
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
                            ElementType = child.GetType().Name ?? "null",
                            ValueKind = AvaloniaValueKind.Unknown,
                            OriginalInstance = child
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

                IEnumerable enumerable when value is not string =>
                    new ElementNode
                    {
                        ElementType = value.GetType().Name,
                        ValueKind = PropertySerializationHelper.ResolveValueKind(value),
                        OriginalInstance = value,
                        Children = enumerable
                            .Cast<object>()
                            .Select(BuildLogicalTreeFromObject)
                            .Where(_ => true)
                            .ToList()
                    },

                _ => new ElementNode
                {
                    ElementType = value.GetType().Name,
                    ValueKind = PropertySerializationHelper.ResolveValueKind(value),
                    OriginalInstance = value
                }
            };
        }


        private static void ProcessContent(object? content, LogicalNode parentNode)
        {
            switch (content)
            {
                case null:
                    return;
                case Control control:
                    parentNode.Children.Add(BuildLogicalTree(control));
                    break;
                case ILogical logical:
                    parentNode.Children.Add(BuildLogicalTreeFromILogical(logical));
                    break;
                case IEnumerable enumerable when content is not string:
                    foreach (var item in enumerable)
                        ProcessContent(item, parentNode);
                    break;
                default:
                    parentNode.Children.Add(new ElementNode
                    {
                        ElementType = content.GetType().Name,
                        ValueKind = PropertySerializationHelper.ResolveValueKind(content),
                        OriginalInstance = content
                    });
                    break;
            }
        }


    }
}
