using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using Example.SyntaxScanner;

namespace EditorExample;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Editor.TextArea.TextView.LineTransformers.Add(new SyntaxHighlighting());
    }

    private void Cancel_Clicked(object? sender, RoutedEventArgs e)
    {
        Debug.WriteLine("Button Clicked!");
        this.Close();
        // Add your desired logic here
    }
}

class SyntaxHighlighting : DocumentColorizingTransformer
{
    private PathItem? syntaxTree;

    protected override void Colorize(ITextRunConstructionContext context)
    {
        syntaxTree = ObjectPathParser.Parse(context.Document.Text);
        base.Colorize(context);
    }

    protected override void ColorizeLine(DocumentLine line)
    {
        if (syntaxTree is null)
            return;

        //all items where at least parts are inside this line
        var currentItems = syntaxTree.Flatten().Where(x => x.Start <= line.EndOffset && x.Start + x.Length > line.Offset);
        foreach (var item in currentItems)
        {
            int start = Math.Max(item.Start, line.Offset);
            int end = Math.Min(item.Start + item.Length, line.EndOffset);

            Action<VisualLineElement> action = item switch
            {
                InvalidItem => element => element.TextRunProperties.SetForegroundBrush(Brushes.Red),
                CheckItem ci when !ci.IsValid() => element => element.TextRunProperties.SetForegroundBrush(Brushes.Red),
                PropertyItem pi when !pi.IsNameValid() => element => element.TextRunProperties.SetForegroundBrush(Brushes.Red),
                CheckItem => element => element.TextRunProperties.SetForegroundBrush(Brush.Parse("#ce9178")),
                PropertyItem => element => element.TextRunProperties.SetForegroundBrush(Brush.Parse("#9cdcfe")),
                _ => throw new NotImplementedException(),
            };

            ChangeLinePart(start, end, action);
        }
    }
}
