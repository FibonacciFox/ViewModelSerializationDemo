using ViewModelSerializationDemo.Models.Properties;

namespace ViewModelSerializationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Формируем логическое дерево из UserControl1.
            LogicalNode logicalTree = LogicalTreeBuilder.BuildLogicalTree(new UserControl1());

            // Выводим логическое дерево в консоль для проверки.
              PrintLogicalTree(logicalTree, 0);
        }

        // Рекурсивный метод для вывода структуры логического дерева в консоль.
        static void PrintLogicalTree(LogicalNode node, int indent)
{
    string indentStr = new string(' ', indent);

    // 🔷 Контрол
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"{indentStr}Control: {node.ElementType}");
    Console.ResetColor();

    // Метод для цветного вывода свойства
    void PrintProperty(AvaloniaPropertyModel prop, string category)
    {
        // Выбор цвета по типу свойства
        ConsoleColor color = category switch
        {
            "StyledProperty" => ConsoleColor.Yellow,
            "AttachedProperty" => ConsoleColor.Magenta,
            "DirectProperty" => ConsoleColor.Green,
            "ClrProperty" => ConsoleColor.Blue,
            _ => ConsoleColor.White
        };

        Console.ForegroundColor = color;
        Console.Write($"{indentStr}  {category}: ");
        Console.ResetColor();

        Console.Write($"{prop.Name} = {prop.Value} ");

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write($"(Kind: {prop.ValueKind}");

        if (prop.IsContainsControl)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(", ContainsControl: true");
        }

        Console.ResetColor();
        Console.WriteLine(")");
    }

    // Styled
    foreach (var prop in node.StyledProperties)
        PrintProperty(prop, "StyledProperty");

    // Attached
    foreach (var prop in node.AttachedProperties)
        PrintProperty(prop, "AttachedProperty");

    // Direct
    foreach (var prop in node.DirectProperties)
        PrintProperty(prop, "DirectProperty");

    // CLR
    foreach (var prop in node.ClrProperties)
        PrintProperty(prop, "ClrProperty");

    // Text node
    if (node is TextNode textNode)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"{indentStr}  Text: {textNode.Text}");
        Console.ResetColor();
    }

    // Дочерние узлы
    foreach (var child in node.Children)
        PrintLogicalTree(child, indent + 2);
}


    }
}