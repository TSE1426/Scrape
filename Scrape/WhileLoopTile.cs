using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Scrape.Backend;

namespace Scrape
{
    public class WhileLoopTile : Tile
    {
        private readonly RoutedEventHandler slotClickHandler;
        private readonly MouseButtonEventHandler slotDoubleClickHandler;

        public WhileLoopTile(
            RoutedEventHandler slotClickHandler,
            MouseButtonEventHandler slotDoubleClickHandler)
        {
            this.slotClickHandler = slotClickHandler;
            this.slotDoubleClickHandler = slotDoubleClickHandler;

            b.Content = "While loop";
            b.Background = Brushes.HotPink;
        }

        public override Border CreateBlock()
        {
            return CreateBlockInstance(new NodeGraph()).Border;
        }

        public override BlockInstance CreateBlockInstance(NodeGraph graph)
        {
            // Backend: CompareNode feeds its result into WhileLoopNode's CondPin
            var whileNode = new WhileLoopNode();
            var compare = new CompareNode { Operation = CompareNode.OperationType.Equal };
            graph.AddNode(whileNode);
            graph.AddNode(compare);

            compare.ResultPin.Connect(whileNode.CondPin);

            Border block = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Background = Brushes.Plum,
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(8)
            };

            StackPanel row = new StackPanel { Orientation = Orientation.Horizontal };

            // Two operand slots wired to the CompareNode
            Slot lhsSlot = new Slot(
                SlotType.AnyPrimitive,
                "left",
                slotClickHandler,
                slotDoubleClickHandler
            )
            {
                TargetPin = compare.LhsPin
            };

            Slot rhsSlot = new Slot(
                SlotType.AnyPrimitive,
                "right",
                slotClickHandler,
                slotDoubleClickHandler
            )
            {
                TargetPin = compare.RhsPin,
                RelatedSlot = lhsSlot
            };

            lhsSlot.RelatedSlot = rhsSlot;

            // Operator dropdown
            ComboBox opSelector = new ComboBox
            {
                Width = 52,
                Margin = new Thickness(4, 0, 4, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            opSelector.Items.Add("=");
            opSelector.Items.Add("!=");
            opSelector.Items.Add(">");
            opSelector.Items.Add("<");
            opSelector.SelectedIndex = 0;
            opSelector.SelectionChanged += (s, e) =>
            {
                compare.Operation = opSelector.SelectedIndex switch
                {
                    0 => CompareNode.OperationType.Equal,
                    1 => CompareNode.OperationType.NotEqual,
                    2 => CompareNode.OperationType.GreaterThan,
                    3 => CompareNode.OperationType.LessThan,
                    _ => CompareNode.OperationType.Equal,
                };
            };

            TextBlock whileLabel = new TextBlock
            {
                Text = "while",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 4, 0)
            };

            row.Children.Add(whileLabel);
            row.Children.Add(lhsSlot.Button);
            row.Children.Add(opSelector);
            row.Children.Add(rhsSlot.Button);

            block.Child = row;

            BlockInstance inst = new BlockInstance
            {
                Border = block,
                Node = whileNode,
                InFlow = whileNode.InFlowPin,
                OutFlow = whileNode.BodyPin,
                SaveType = "WhileLoop",
                ExtraSaveControl = opSelector
            };

            inst.Slots.Add(lhsSlot);
            inst.Slots.Add(rhsSlot);
            inst.AllNodes.Add(whileNode);
            inst.AllNodes.Add(compare);

            return inst;
        }
    }
}