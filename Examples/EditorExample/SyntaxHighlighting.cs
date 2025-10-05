using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using Example.SyntaxScanner;

namespace EditorExample;

class SyntaxHighlighting : DocumentColorizingTransformer
{
    private List<PathItem> pathItems = [];

    IBrush bracketBrush0 = Brush.Parse("#da70d6");
    IBrush bracketBrush1 = Brush.Parse("#179fff");
    IBrush bracketBrush2 = Brush.Parse("#ffd700");
    IBrush checkBrush = Brush.Parse("#ce9178");
    IBrush propertyBrush = Brush.Parse("#9cdcfe");

    protected override void Colorize(ITextRunConstructionContext context)
    {
        pathItems = ObjectPathParser.Parse(context.Document.Text)?.Flatten() ?? [];
        base.Colorize(context);
    }

    protected override void ColorizeLine(DocumentLine line)
    {
        //all items where at least parts are inside this line
        var currentItems = pathItems.Where(x => x.Start <= line.EndOffset && x.Start + x.Length >= line.Offset);
        foreach (var item in currentItems)
        {
            if (item is ListItem li)
            {
                int index = pathItems.Where(x => x is ListItem or CheckItem).Index().First(x => x.Item == li).Index;
                var brush = (index % 3) switch
                {
                    1 => bracketBrush1,
                    2 => bracketBrush2,
                    _ => bracketBrush0,
                };

                if (line.Offset <= li.OpeningBracket)
                    ChangeLinePart(li.OpeningBracket, li.OpeningBracket + 1, element => element.TextRunProperties.SetForegroundBrush(brush));

                if (line.EndOffset >= li.ClosingBracket)
                    ChangeLinePart(li.ClosingBracket, li.ClosingBracket + 1, element => element.TextRunProperties.SetForegroundBrush(brush));

                continue;
            }
            if (item is CheckItem ci)
            {
                int index = pathItems.Where(x => x is ListItem or CheckItem).Index().First(x => x.Item == ci).Index;
                var brush = (index % 3) switch
                {
                    1 => bracketBrush1,
                    2 => bracketBrush2,
                    _ => bracketBrush0,
                };

                if (line.Offset <= ci.OpeningBracket)
                    ChangeLinePart(ci.OpeningBracket, ci.OpeningBracket + 1, element => element.TextRunProperties.SetForegroundBrush(brush));

                if (line.EndOffset >= ci.ClosingBracket)
                    ChangeLinePart(ci.ClosingBracket, ci.ClosingBracket + 1, element => element.TextRunProperties.SetForegroundBrush(brush));
            }

            int start = Math.Max(item.Start, line.Offset);
            int end = Math.Min(item.Start + item.Length, line.EndOffset);

            Action<VisualLineElement> action = item switch
            {
                InvalidItem => element => element.TextRunProperties.SetForegroundBrush(Brushes.Red),
                CheckItem ci2 when !ci2.IsValid() => element => element.TextRunProperties.SetForegroundBrush(Brushes.Red), //should be underlined, but I can't figure out how to draw squiggly lines
                PropertyItem pi when !pi.IsNameValid() => element => element.TextRunProperties.SetForegroundBrush(Brushes.Red),
                CheckItem => element => element.TextRunProperties.SetForegroundBrush(checkBrush),
                PropertyItem => element => element.TextRunProperties.SetForegroundBrush(propertyBrush),
                _ => throw new NotImplementedException(),
            };

            ChangeLinePart(start, end, action);
        }
    }
}
