using System.Text;
using ViewModelSerializationDemo.Models.Properties;

namespace ViewModelSerializationDemo;

public static class AxamlGenerator
{
    public static string GenerateAxaml(LogicalNode root)
    {
        var sb = new StringBuilder();

        // Начало документа и пространство имён
        sb.AppendLine(@"<UserControl xmlns=""https://github.com/avaloniaui""");
        sb.AppendLine(@"             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""");
        sb.AppendLine(@"             x:Class=""ViewModelSerializationDemo.UserControl1"">");

        foreach (var child in root.Children)
        {
            GenerateElement(child, sb, "    ");
        }

        sb.AppendLine("</UserControl>");
        return sb.ToString();
    }

    private static void GenerateElement(LogicalNode node, StringBuilder sb, string indent)
    {
        var tag = node.ElementType ?? "Unknown";

        var attributes = new List<string>();
        foreach (var prop in node.StyledProperties.Cast<AvaloniaPropertyModel>()
                     .Concat(node.DirectProperties)
                     .Concat(node.AttachedProperties)
                     .Concat(node.ClrProperties))
        {
            if (!prop.CanBeSerializedToXaml || prop.Value == null)
                continue;

            // Attached свойства
            if (prop is AttachedAvaloniaPropertyModel)
                attributes.Add($"{prop.Name}=\"{EscapeXml(prop.Value)}\"");
            else
                attributes.Add($"{prop.Name}=\"{EscapeXml(prop.Value)}\"");
        }

        var attrs = string.Join(" ", attributes);
        var hasChildren = node.Children.Count > 0;

        if (hasChildren)
        {
            sb.AppendLine($"{indent}<{tag} {attrs}>");
            foreach (var child in node.Children)
            {
                GenerateElement(child, sb, indent + "    ");
            }
            sb.AppendLine($"{indent}</{tag}>");
        }
        else
        {
            sb.AppendLine($"{indent}<{tag} {attrs} />");
        }
    }

    private static string EscapeXml(string input)
    {
        return input.Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;")
                    .Replace("\"", "&quot;");
    }
}
