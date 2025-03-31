using Avalonia.Controls;
using Avalonia.Input;

namespace ViewModelSerializationDemo;

public partial class UserControl1 : UserControl
{
    public UserControl1()
    {
        InitializeComponent();
        
        ListBox1.Items.Add(new ListBoxItem() { Content = "Item 1" });
    }

   
}