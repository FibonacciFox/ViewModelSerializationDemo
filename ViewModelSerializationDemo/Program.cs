namespace ViewModelSerializationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Формируем логическое дерево из UserControl1.
            LogicalNode logicalTree = LogicalTreeBuilder.BuildLogicalTree(new UserControl1());

            // Выводим логическое дерево в консоль для проверки.
            //  PrintLogicalTree(logicalTree, 0);
        }

        // Рекурсивный метод для вывода структуры логического дерева в консоль.
        static void PrintLogicalTree(LogicalNode node, int indent)
        {
            string indentStr = new string(' ', indent);
            Console.WriteLine($"{indentStr}Control: {node.ElementType}");

            // Вывод styled-свойств.
            foreach (var prop in node.StyledProperties)
            {
                Console.WriteLine($"{indentStr}  StyledProperty: {prop.Name} = {prop.Value}");
            }

            // Вывод attached-свойств.
            foreach (var prop in node.AttachedProperties)
            {
                Console.WriteLine($"{indentStr}  AttachedProperty: {prop.Name} = {prop.Value}");
            }

            // Вывод direct-свойств.
            foreach (var prop in node.DirectProperties)
            {
                Console.WriteLine($"{indentStr}  DirectProperty: {prop.Name} = {prop.Value}");
            }

            // Вывод CLR-свойств.
            foreach (var prop in node.ClrProperties)
            {
                Console.WriteLine($"{indentStr}  ClrProperty: {prop.Name} = {prop.Value}");
            }

            // Если узел является TextNode, выводим его текст.
            if (node is TextNode textNode)
            {
                Console.WriteLine($"{indentStr}  Text: {textNode.Text}");
            }

            // Рекурсивный вывод для дочерних узлов.
            foreach (var child in node.Children)
            {
                PrintLogicalTree(child, indent + 2);
            }
        }
    }
}