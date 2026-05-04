using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Scrape.Backend;

namespace Scrape
{
    public class MoveSpriteTile : Tile
    {
        private readonly RoutedEventHandler slotClickHandler;
        private readonly MouseButtonEventHandler slotDoubleClickHandler;

        public MoveSpriteTile(RoutedEventHandler slotClickHandler, MouseButtonEventHandler slotDoubleClickHandler)
        {
            this.slotClickHandler = slotClickHandler;
            this.slotDoubleClickHandler = slotDoubleClickHandler;

            b.Content = "Move sprite";
            b.Background = Brushes.PaleGreen;
        }

        public override Border CreateBlock()
        {
            return CreateBlockInstance(new NodeGraph()).Border;
        }

        public override BlockInstance CreateBlockInstance(NodeGraph graph)
        {
            var move = new MoveSpriteNode("");
            graph.AddNode(move);

            Border block = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Background = Brushes.PaleGreen,
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(8)
            };

            StackPanel row = new StackPanel { Orientation = Orientation.Horizontal };

            Slot spriteSlot = new Slot(SlotType.SpriteOnly, "sprite", slotClickHandler, slotDoubleClickHandler);
            Slot dxSlot = new Slot(SlotType.NumberOrVariable, "dx", slotClickHandler, slotDoubleClickHandler);
            Slot dySlot = new Slot(SlotType.NumberOrVariable, "dy", slotClickHandler, slotDoubleClickHandler);

            // sprite name -> node SpriteName field
            spriteSlot.OnValueSet = s =>
            {
                if (s.Value is Sprite sp) move.SpriteName = sp.Name;
            };
            dxSlot.TargetPin = move.DxPin;
            dySlot.TargetPin = move.DyPin;

            TextBlock moveLabel = new TextBlock
            {
                Text = "move",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 4, 0)
            };
            TextBlock byLabel = new TextBlock
            {
                Text = "by",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(4, 0, 4, 0)
            };
            TextBlock commaLabel = new TextBlock
            {
                Text = ",",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(2, 0, 2, 0)
            };

            row.Children.Add(moveLabel);
            row.Children.Add(spriteSlot.Button);
            row.Children.Add(byLabel);
            row.Children.Add(dxSlot.Button);
            row.Children.Add(commaLabel);
            row.Children.Add(dySlot.Button);

            block.Child = row;

            BlockInstance inst = new BlockInstance
            {
                Border = block,
                Node = move,
                InFlow = move.InFlowPin,
                OutFlow = move.OutFlowPin
            };
            inst.Slots.Add(spriteSlot);
            inst.Slots.Add(dxSlot);
            inst.Slots.Add(dySlot);
            inst.AllNodes.Add(move);
            return inst;
        }
    }
}
