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
    public class DelayTile : Tile
    {
        private readonly RoutedEventHandler slotClickHandler;
        private readonly MouseButtonEventHandler slotDoubleClickHandler;

        public DelayTile(RoutedEventHandler slotClickHandler, MouseButtonEventHandler slotDoubleClickHandler)
        {
            this.slotClickHandler = slotClickHandler;
            this.slotDoubleClickHandler = slotDoubleClickHandler;

            b.Content = "Delay";
            b.Background = Brushes.MediumPurple;
        }

        public override Border CreateBlock()
        {
            return CreateBlockInstance(new NodeGraph()).Border;
        }

        public override BlockInstance CreateBlockInstance(NodeGraph graph)
        {
            // Backend: create the WaitNode
            var wait = new WaitNode();
            graph.AddNode(wait);

            // Visual styling for the tile
            Border block = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Background = Brushes.MediumPurple,
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(8)
            };

            StackPanel row = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            // Create a time slot so the user can set or connect a value
            Slot secondsSlot = new Slot(
                SlotType.NumberOrVariable,
                "seconds",
                slotClickHandler,
                slotDoubleClickHandler
            )
            {
                TargetPin = wait.DurationPin
            };

            TextBlock label = new TextBlock
            {
                Text = "Wait",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 4, 0)
            };

            TextBlock secondsLabel = new TextBlock
            {
                Text = "seconds",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(4, 0, 0, 0)
            };

            // Compose the row visually: “Wait [ N ] seconds”
            row.Children.Add(label);
            row.Children.Add(secondsSlot.Button);
            row.Children.Add(secondsLabel);

            block.Child = row;

            // Create the frontend block instance for the WaitNode
            BlockInstance inst = new BlockInstance
            {
                Border = block,
                Node = wait,
                InFlow = wait.InFlowPin,
                OutFlow = wait.CompletedPin,
                SaveType = "Delay"
            };

            inst.Slots.Add(secondsSlot);
            inst.AllNodes.Add(wait);

            return inst;
        }
    }
}

