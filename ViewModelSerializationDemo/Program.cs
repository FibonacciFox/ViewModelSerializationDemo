namespace ViewModelSerializationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            
            // Формируем логическое дерево из StackPanel.
            LogicalNode logicalTree = LogicalTreeBuilder.BuildLogicalTree(new UserControl1());

            // Выводим логическое дерево в консоль для проверки.
            PrintLogicalTree(logicalTree, 0);
        }

        // Рекурсивный метод для вывода структуры логического дерева в консоль.
        static void PrintLogicalTree(LogicalNode node, int indent)
        {
            string indentStr = new string(' ', indent);
            Console.WriteLine($"{indentStr}Control: {node.ElementType}");
            foreach (var attr in node.Attributes)
            {
                Console.WriteLine($"{indentStr}  Field:{attr.Key} = {attr.Value}");
            }
            if (node is TextNode textNode)
            {
                Console.WriteLine($"{indentStr}  Text: {textNode.Text}");
            }
            foreach (var child in node.Children)
            {
                PrintLogicalTree(child, indent + 2);
            }
        }
    }
}