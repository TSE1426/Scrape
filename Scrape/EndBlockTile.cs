using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Scrape.Backend;

namespace Scrape;

public class EndBlockTile : Tile
{
    private readonly string endLabel;
    private readonly Node nodeEnded;
    private readonly OutFlowPin completedPin;

    public EndBlockTile(string endLabel, Node nodeEnded, OutFlowPin completedPin)
    {
        this.endLabel = endLabel;
        this.nodeEnded = nodeEnded;
        this.completedPin = completedPin;
        b.Content = endLabel;
        b.Background = Brushes.Gainsboro;
    }

    public override BlockInstance CreateBlockInstance(NodeGraph graph)
    {
        var node = new NoOpNode(endLabel);
        graph.AddNode(node);

        var block = new Border
        {
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(1),
            Background = Brushes.Gainsboro,
            CornerRadius = new CornerRadius(5),
            Padding = new Thickness(8),
            Child = new TextBlock
            {
                Text = endLabel,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            },
        };

        return new BlockInstance
        {
            Border = block,
            Node = node,
            InFlow = node.InFlowPin,
            OutFlow = completedPin,
            PseudocodeLabel = endLabel,
            AllNodes = { node },
        };
    }
}
