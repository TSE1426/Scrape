using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Scrape.Backend;

namespace Scrape
{
    public class OperatorTile : Tile
    {
        private readonly RoutedEventHandler slotClickHandler;
        private readonly MouseButtonEventHandler slotDoubleClickHandler;

        public OperatorTile(RoutedEventHandler slotClickHandler, MouseButtonEventHandler slotDoubleClickHandler)
        {
            this.slotClickHandler = slotClickHandler;
            this.slotDoubleClickHandler = slotDoubleClickHandler;

            b.Content = "Operator";
            b.Background = Brushes.LightCyan;
        }

        public override Border CreateBlock()
        {
            return CreateBlockInstance(new NodeGraph()).Border;
        }

        public override BlockInstance CreateBlockInstance(NodeGraph graph)
        {
            var op = new OperatorNode();
            var setVar = new SetVariableNode("");
            graph.AddNode(op);
            graph.AddNode(setVar);

            // feed the operator result into the variable
            setVar.ValuePin.Connect(op.ResultPin);

            Border block = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Background = Brushes.LightCyan,
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(8)
            };

            StackPanel row = new StackPanel { Orientation = Orientation.Horizontal };

            // result variable slot
            Slot varSlot = new Slot(SlotType.VariableOnly, "variable", slotClickHandler, slotDoubleClickHandler);
            varSlot.OnValueSet = s =>
            {
                if (s.Value is Variable v) setVar.Identifier = v.Name;
            };

            // left and right number slots
            Slot lhsSlot = new Slot(SlotType.NumberOrVariable, "left", slotClickHandler, slotDoubleClickHandler)
            {
                TargetPin = op.LhsPin
            };
            Slot rhsSlot = new Slot(SlotType.NumberOrVariable, "right", slotClickHandler, slotDoubleClickHandler)
            {
                TargetPin = op.RhsPin
            };

            // operator dropdown
            ComboBox opSelector = new ComboBox
            {
                Width = 42,
                Margin = new Thickness(4, 0, 4, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            opSelector.Items.Add("+");
            opSelector.Items.Add("-");
            opSelector.Items.Add("*");
            opSelector.Items.Add("/");
            opSelector.SelectedIndex = 0;
            opSelector.SelectionChanged += (s, e) =>
            {
                op.Operation = opSelector.SelectedIndex switch
                {
                    0 => OperatorNode.OperationType.Add,
                    1 => OperatorNode.OperationType.Subtract,
                    2 => OperatorNode.OperationType.Multiply,
                    3 => OperatorNode.OperationType.Divide,
                    _ => OperatorNode.OperationType.Add,
                };
            };

            TextBlock equalsLabel = new TextBlock
            {
                Text = "=",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(4, 0, 4, 0)
            };

            row.Children.Add(varSlot.Button);
            row.Children.Add(equalsLabel);
            row.Children.Add(lhsSlot.Button);
            row.Children.Add(opSelector);
            row.Children.Add(rhsSlot.Button);

            block.Child = row;

            BlockInstance inst = new BlockInstance
            {
                Border = block,
                Node = op,
                InFlow = setVar.InFlowPin,
                OutFlow = setVar.OutFlowPin,
                SaveType = "Operator",
                ExtraSaveControl = opSelector
            };
            inst.Slots.Add(varSlot);
            inst.Slots.Add(lhsSlot);
            inst.Slots.Add(rhsSlot);
            inst.AllNodes.Add(op);
            inst.AllNodes.Add(setVar);
            return inst;
        }
    }
}