using ViewModelSerializationDemo.Helpers;
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
            
            
            Console.WriteLine("\n--- AXAML ---\n");
            Console.WriteLine(AxamlGenerator.GenerateAxaml(logicalTree));
        }

        static void PrintLogicalTree(LogicalNode node, string indent = "", bool isLast = true)
        {
            string prefix = isLast ? "\u2514" : "\u251c";
            string childIndent = indent + (isLast ? "   " : "│  ");

            // 🎨 Цвета для ValueKind
            ConsoleColor GetKindColor(AvaloniaValueKind kind) => kind switch
            {
                AvaloniaValueKind.Control => ConsoleColor.Cyan,
                AvaloniaValueKind.Logical => ConsoleColor.DarkCyan,
                AvaloniaValueKind.StyledClasses => ConsoleColor.Magenta,
                AvaloniaValueKind.Complex => ConsoleColor.DarkYellow,
                AvaloniaValueKind.Brush => ConsoleColor.Green,
                AvaloniaValueKind.Template => ConsoleColor.DarkGray,
                AvaloniaValueKind.Binding => ConsoleColor.Blue,
                AvaloniaValueKind.Resource => ConsoleColor.DarkGreen,
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
                Console.Write($"{childIndent} \u25a0 {category}: ");
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
                if (prop.SerializedValue != null && prop.ValueKind != AvaloniaValueKind.Simple)
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
