using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace Scrape
{
    public class AssignmentTile : Tile
    {
        private readonly RoutedEventHandler slotClickHandler;
        private readonly MouseButtonEventHandler slotDoubleClickHandler;

        public AssignmentTile(RoutedEventHandler slotClickHandler, MouseButtonEventHandler slotDoubleClickHandler)
        {
            this.slotClickHandler = slotClickHandler;
            this.slotDoubleClickHandler = slotDoubleClickHandler;

            b.Content = "Assignment";
            b.Background = Brushes.Khaki;
        }

        public override Border CreateBlock()
        {
            Border block = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Background = Brushes.Khaki,
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(8)
            };

            StackPanel layout = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            Slot leftSlot = new Slot(SlotType.VariableOnly, "variable", slotClickHandler, slotDoubleClickHandler);
            Slot rightSlot = new Slot(SlotType.NumberOrVariable, "value", slotClickHandler, slotDoubleClickHandler);

            TextBlock equalsText = new TextBlock
            {
                Text = "=",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(6, 0, 6, 0)
            };

            layout.Children.Add(leftSlot.Button);
            layout.Children.Add(equalsText);
            layout.Children.Add(rightSlot.Button);

            block.Child = layout;
            return block;
        }
    }
}