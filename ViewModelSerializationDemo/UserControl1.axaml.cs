using Avalonia.Controls;

namespace ViewModelSerializationDemo;

public partial class UserControl1 : UserControl
{
    public UserControl1()
    {
        InitializeComponent();
        Console.WriteLine(string.Join(", " , TextBlockH1.Classes  ));
        
    }
}