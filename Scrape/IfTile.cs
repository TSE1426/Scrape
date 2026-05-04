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
            graph.AddNode(branch);

            // Visual style for the block
            Border block = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Background = Brushes.LightSalmon,
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(8)
            };

            StackPanel row = new StackPanel { Orientation = Orientation.Horizontal };

            // Create slot for the boolean condition
            Slot condSlot = new Slot(
                SlotType.BooleanOrVariable,
                "condition",
                slotClickHandler,
                slotDoubleClickHandler
            )
            {
                TargetPin = branch.CondPin
            };

            // Simple label
            TextBlock ifLabel = new TextBlock
            {
                Text = "if",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 4, 0)
            };

            row.Children.Add(ifLabel);
            row.Children.Add(condSlot.Button);

            block.Child = row;

            BlockInstance inst = new BlockInstance
            {
                Border = block,
                Node = branch,
                InFlow = branch.FlowPin,
                OutFlow = branch.CompletedPin
            };

            inst.Slots.Add(condSlot);
            inst.AllNodes.Add(branch);
            return inst;
        }
    }
}

