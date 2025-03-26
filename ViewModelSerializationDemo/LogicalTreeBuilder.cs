using System.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace ViewModelSerializationDemo;

public static class LogicalTreeBuilder
{
    public static LogicalNode BuildLogicalTree(Control control)
    {
        // Создаем базовый узел и получаем атрибуты через PropertySerializer.
        var node = new ElementNode
        {
            ElementType = control.GetType().Name,
            Attributes = PropertySerializer.SerializeProperties(control)
        };

        // Обрабатываем содержимое для ContentControl.
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
}