using ViewModelSerializationDemo.Models.Properties;

namespace ViewModelSerializationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Формируем логическое дерево из UserControl1.
            LogicalNode logicalTree = LogicalTreeBuilder.BuildLogicalTree(new UserControl1());

            // Выводим логическое дерево в консоль.
            PrintLogicalTree(logicalTree);
        }

        static void PrintLogicalTree(LogicalNode node, string indent = "", bool isLast = true)
        {
            string prefix = isLast ? "└─" : "├─";
            string childIndent = indent + (isLast ? "   " : "│  ");

            // 🎨 Цвета для ValueKind
            ConsoleColor GetKindColor(AvaloniaPropertyValueKind kind) => kind switch
            {
                AvaloniaPropertyValueKind.Control => ConsoleColor.Cyan,
                AvaloniaPropertyValueKind.Logical => ConsoleColor.DarkCyan,
                AvaloniaPropertyValueKind.StyledClasses => ConsoleColor.Magenta,
                AvaloniaPropertyValueKind.Complex => ConsoleColor.DarkYellow,
                AvaloniaPropertyValueKind.Brush => ConsoleColor.Green,
                AvaloniaPropertyValueKind.Template => ConsoleColor.DarkGray,
                AvaloniaPropertyValueKind.Binding => ConsoleColor.Blue,
                AvaloniaPropertyValueKind.Resource => ConsoleColor.DarkGreen,
                _ => ConsoleColor.Gray
            };

            // 🔷 Заголовок узла
            Console.ForegroundColor = ConsoleColor.Cyan;
            var name = node is ElementNode el && !string.IsNullOrWhiteSpace(el.Name)
                ? $"{el.DisplayName}"
                : node.ElementType ?? "Unknown";
            Console.Write($"{indent}{prefix} Element: {name}");

            Console.ForegroundColor = GetKindColor(node.ValueKind);
            Console.Write($" (Kind: {node.ValueKind})");

            if (node.IsContainsControl)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" [HasChildControls]");
            }

            Console.ResetColor();
            Console.WriteLine();

            // Свойства
            void PrintProperty(AvaloniaPropertyModel prop, string category)
            {
                ConsoleColor catColor = category switch
                {
                    "StyledProperty" => ConsoleColor.Yellow,
                    "AttachedProperty" => ConsoleColor.Magenta,
                    "DirectProperty" => ConsoleColor.Green,
                    "ClrProperty" => ConsoleColor.Blue,
                    _ => ConsoleColor.White
                };

                Console.ForegroundColor = catColor;
                Console.Write($"{childIndent}• {category}: ");
                Console.ResetColor();

                Console.Write($"{prop.Name} = {prop.Value} ");

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("(Kind: ");
                Console.ForegroundColor = GetKindColor(prop.ValueKind);
                Console.Write($"{prop.ValueKind}");
                Console.ResetColor();

                Console.Write(", Xaml: ");
                Console.ForegroundColor = prop.CanBeSerializedToXaml ? ConsoleColor.Green : ConsoleColor.DarkRed;
                Console.Write(prop.CanBeSerializedToXaml ? "yes" : "no");

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

                // 🔽 Вложенные значения
                if (prop.SerializedValue != null && prop.ValueKind != AvaloniaPropertyValueKind.Simple)
                    PrintLogicalTree(prop.SerializedValue, childIndent, true);
            }

            foreach (var prop in node.StyledProperties)
                PrintProperty(prop, "StyledProperty");

            foreach (var prop in node.AttachedProperties)
                PrintProperty(prop, "AttachedProperty");

            foreach (var prop in node.DirectProperties)
                PrintProperty(prop, "DirectProperty");

            foreach (var prop in node.ClrProperties)
                PrintProperty(prop, "ClrProperty");

            // 👶 Дочерние элементы
            for (int i = 0; i < node.Children.Count; i++)
            {
                bool last = i == node.Children.Count - 1;
                PrintLogicalTree(node.Children[i], childIndent, last);
            }
        }
    }
}
