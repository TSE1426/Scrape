using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Scrape.Backend;

namespace Scrape
{
    public class IfTile : Tile
    {
        private readonly RoutedEventHandler slotClickHandler;
        private readonly MouseButtonEventHandler slotDoubleClickHandler;

        public IfTile(RoutedEventHandler slotClickHandler, MouseButtonEventHandler slotDoubleClickHandler)
        {
            this.slotClickHandler = slotClickHandler;
            this.slotDoubleClickHandler = slotDoubleClickHandler;

            b.Content = "If";
            b.Background = Brushes.LightSalmon;
        }

        public override Border CreateBlock()
        {
            return CreateBlockInstance(new NodeGraph()).Border;
        }

        public override BlockInstance CreateBlockInstance(NodeGraph graph)
        {
            var branch = new BranchNode();
            var compare = new CompareNode { Operation = CompareNode.OperationType.Equal };
            graph.AddNode(branch);
            graph.AddNode(compare);

            compare.ResultPin.Connect(branch.CondPin);

            Border block;

            StackPanel row = new StackPanel { Orientation = Orientation.Horizontal };

            // Create slots for left/right operands
            Slot lhsSlot = new Slot(
                SlotType.NumberOrVariable,
                "left",
                slotClickHandler,
                slotDoubleClickHandler
            )
            {
                TargetPin = compare.LhsPin
            };

            Slot rhsSlot = new Slot(
                SlotType.NumberOrVariable,
                "right",
                slotClickHandler,
                slotDoubleClickHandler
            )
            {
                TargetPin = compare.RhsPin
            };

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

            TextBlock ifLabel = new TextBlock
            {
                Text = "if",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 4, 0)
            };

            row.Children.Add(ifLabel);
            row.Children.Add(lhsSlot.Button);
            row.Children.Add(opSelector);
            row.Children.Add(rhsSlot.Button);

            var layout = new StackPanel { Orientation = Orientation.Vertical };
            layout.Children.Add(row);
            layout.Children.Add(new TextBlock
            {
                Text = "↳ if body (drop blocks under this)",
                Margin = new Thickness(16, 4, 0, 4),
                FontSize = 11,
                Opacity = 0.8
            });
            layout.Children.Add(new TextBlock
            {
                Text = "end if",
                Margin = new Thickness(0, 2, 0, 0),
                FontWeight = FontWeights.SemiBold
            });

            block = CreateStyledBlock(Brushes.LightSalmon, layout);

            BlockInstance inst = new BlockInstance
            {
                Border = block,
                Node = branch,
                InFlow = branch.FlowPin,
                OutFlow = branch.CompletedPin
            };

            inst.Slots.Add(lhsSlot);
            inst.Slots.Add(rhsSlot);
            inst.AllNodes.Add(branch);
            inst.AllNodes.Add(compare);
            return inst;
        }
    }
}
