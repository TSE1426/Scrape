using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Scrape.Backend;

namespace Scrape;

public class EndBlockTile : Tile
{
    private readonly string endLabel;

    public EndBlockTile(string endLabel)
    {
        this.endLabel = endLabel;
        b.Content = endLabel;
        b.Background = Brushes.Gainsboro;
    }

    public override BlockInstance CreateBlockInstance(NodeGraph graph)
    {
        var node = new WaitNode();
        var zero = new ConstantNode<double>(new Value(0.0));
        graph.AddNode(node);
        graph.AddNode(zero);
        node.DurationPin.Connect(zero.ValuePin);

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
            OutFlow = node.CompletedPin,
            PseudocodeLabel = endLabel,
            AllNodes = { node, zero },
        };
    }
}
