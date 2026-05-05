using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Scrape.Backend;

namespace Scrape
{
    public class KeyPressedTile : Tile
    {
        private readonly RoutedEventHandler slotClickHandler;
        private readonly MouseButtonEventHandler slotDoubleClickHandler;

        public static readonly Dictionary<string, Key> TranslatedKeys = new() {
            { "W", Key.W },
            { "A", Key.A },
            { "S", Key.S },
            { "D", Key.D },
            { "Up Arrow", Key.Left },
            { "Left Arrow", Key.Left },
            { "Down Arrow", Key.Down },
            { "Right Arrow", Key.Right },
        };

        public KeyPressedTile(RoutedEventHandler slotClickHandler, MouseButtonEventHandler slotDoubleClickHandler)
        {
            this.slotClickHandler = slotClickHandler;
            this.slotDoubleClickHandler = slotDoubleClickHandler;

            b.Content = "Key pressed";
            b.Background = Brushes.PaleGoldenrod;
        }

        public override Border CreateBlock()
        {
            return CreateBlockInstance(new NodeGraph()).Border;
        }

        public override BlockInstance CreateBlockInstance(NodeGraph graph)
        {
            var downNode = new IsKeyDownNode("W");
            var setVarNode = new SetVariableNode("");

            graph.AddNode(downNode);
            graph.AddNode(setVarNode);

            setVarNode.ValuePin.Connect(downNode.ValuePin);

            Border block = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Background = Brushes.PaleGoldenrod,
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(8)
            };

            StackPanel row = new StackPanel { Orientation = Orientation.Horizontal };

            Slot varSlot = new Slot(SlotType.VariableOnly, "variable", slotClickHandler, slotDoubleClickHandler);

            var keySelector = new ComboBox
            {
                Width = 52,
                Margin = new Thickness(4, 0, 4, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            foreach (var key in TranslatedKeys.Keys)
            {
                keySelector.Items.Add(key);
            }
            keySelector.SelectedIndex = 0;
            keySelector.SelectionChanged += (_, _) =>
            {
                downNode.Key = (string)keySelector.SelectedItem;
            };

            varSlot.OnValueSet = v =>
            {
                if (v.Value is Variable variable) setVarNode.Identifier = variable.Name;
            };

            var equalsLabel = new TextBlock
            {
                Text = "=",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 6, 0)
            };
            var isLabel = new TextBlock
            {
                Text = "is",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 4, 0)
            };
            var pressedLabel = new TextBlock
            {
                Text = "pressed?",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 4, 0)
            };

            row.Children.Add(varSlot.Button);
            row.Children.Add(equalsLabel);
            row.Children.Add(isLabel);
            row.Children.Add(keySelector);
            row.Children.Add(pressedLabel);

            block.Child = row;

            var inst = new BlockInstance
            {
                Border = block,
                Node = downNode,
                InFlow = setVarNode.InFlowPin,
                OutFlow = setVarNode.OutFlowPin,
                SaveType = "KeyPressed",
                ExtraSaveControl = keySelector,
                Slots = { varSlot },
                AllNodes = [downNode, setVarNode],
            };
            return inst;
        }
    }
}
