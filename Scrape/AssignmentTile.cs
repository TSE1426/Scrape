using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using Scrape.Backend;

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
            // kept for backwards compatibility, but CreateBlockInstance is what's used
            return CreateBlockInstance(new NodeGraph()).Border;
        }

        public override BlockInstance CreateBlockInstance(NodeGraph graph)
        {
            // Backend: SetVariableNode. The variable name comes from the left slot,
            // the value comes from the right slot.
            var setVar = new SetVariableNode("");
            graph.AddNode(setVar);

            // Visual border
            Border block = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Background = Brushes.Khaki,
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(8)
            };

            StackPanel layout = new StackPanel { Orientation = Orientation.Horizontal };

            Slot leftSlot = new Slot(SlotType.VariableOnly, "variable", slotClickHandler, slotDoubleClickHandler) { };
            Slot rightSlot = new Slot(SlotType.AnyPrimitive, "value", slotClickHandler, slotDoubleClickHandler) { RelatedSlot = leftSlot };
            leftSlot.RelatedSlot = rightSlot;

            // left slot: when a variable is picked, update the node identifier
            leftSlot.OnValueSet = s =>
            {
                if (s.Value is Variable v) setVar.Identifier = v.Name;
            };
            // right slot: feeds into ValuePin at run time
            rightSlot.TargetPin = setVar.ValuePin;

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

            BlockInstance inst = new BlockInstance
            {
                Border = block,
                Node = setVar,
                InFlow = setVar.InFlowPin,
                OutFlow = setVar.OutFlowPin,
                SaveType = "Assignment",
            };
            inst.Slots.Add(leftSlot);
            inst.Slots.Add(rightSlot);
            inst.AllNodes.Add(setVar);
            return inst;
        }
    }
}
