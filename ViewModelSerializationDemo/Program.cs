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
            PrintLogicalTree(logicalTree);
        }

        // Рекурсивный метод для вывода структуры логического дерева в консоль.
        static void PrintLogicalTree(LogicalNode node, string indent = "", bool isLast = true)
        {
            // └─ или ├─ перед узлом
            string prefix = isLast ? "└─" : "├─";

            // 🔷 Заголовок: тип + имя (если есть)
            Console.ForegroundColor = ConsoleColor.Cyan;
            if (node is ElementNode element && !string.IsNullOrWhiteSpace(element.Name))
                Console.WriteLine($"{indent}{prefix} Element: {node.ElementType} (Name: {element.Name})");
            else
                Console.WriteLine($"{indent}{prefix} Element: {node.ElementType}");
            Console.ResetColor();

            // 📐 Для последующих отступов
            string childIndent = indent + (isLast ? "   " : "│  ");

            void PrintProperty(AvaloniaPropertyModel prop, string category)
            {
                ConsoleColor color = category switch
                {
                    "StyledProperty" => ConsoleColor.Yellow,
                    "AttachedProperty" => ConsoleColor.Magenta,
                    "DirectProperty" => ConsoleColor.Green,
                    "ClrProperty" => ConsoleColor.Blue,
                    _ => ConsoleColor.White
                };

                Console.ForegroundColor = color;
                Console.Write($"{childIndent}• {category}: ");
                Console.ResetColor();

                Console.Write($"{prop.Name} = {prop.Value} ");

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($"(Kind: {prop.ValueKind}, Xaml: ");

                Console.ForegroundColor = prop.CanBeSerializedToXaml ? ConsoleColor.Green : ConsoleColor.DarkRed;
                Console.Write(prop.CanBeSerializedToXaml ? "serializable" : "not serializable");

                if (prop.IsContainsControl)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(", ContainsControl: true");
                }

                if (prop.IsRuntimeOnly)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(", RuntimeOnly: true");
                }

                Console.ResetColor();
                Console.WriteLine(")");

                // 🔽 Вывод вложенного логического дерева, если это не простое значение
                if (prop.SerializedValue != null && prop.ValueKind != AvaloniaPropertyValueKind.Simple)
                {
                    PrintLogicalTree(prop.SerializedValue, childIndent, true);
                }
            }

            // ⏬ Вывод свойств
            foreach (var prop in node.StyledProperties)
                PrintProperty(prop, "StyledProperty");

            foreach (var prop in node.AttachedProperties)
                PrintProperty(prop, "AttachedProperty");

            foreach (var prop in node.DirectProperties)
                PrintProperty(prop, "DirectProperty");

            foreach (var prop in node.ClrProperties)
                PrintProperty(prop, "ClrProperty");

            
            // 👶 Дочерние узлы
            for (int i = 0; i < node.Children.Count; i++)
            {
                bool last = i == node.Children.Count - 1;
                PrintLogicalTree(node.Children[i], childIndent, last);
            }
        }
    }
}