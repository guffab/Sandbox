using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace EditorExample;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Editor.Text = """
        Disassemble[
            Parameters[
                Definition.Name ((PF_Number)|(Number)) 
            ].AsElementId.Name(^Ass)
        ].Name
        """;

        Editor.TextArea.TextView.LineTransformers.Add(new SyntaxHighlighting());
    }

    private void Cancel_Clicked(object? sender, RoutedEventArgs e)
    {
        Debug.WriteLine("Button Clicked!");
        this.Close();
        // Add your desired logic here
    }
}
