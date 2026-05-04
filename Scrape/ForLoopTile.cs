using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Scrape.Backend;

namespace Scrape
{
    public class ForLoopTile : Tile
    {
        private readonly RoutedEventHandler slotClickHandler;
        private readonly MouseButtonEventHandler slotDoubleClickHandler;

        public ForLoopTile(RoutedEventHandler slotClickHandler, MouseButtonEventHandler slotDoubleClickHandler)
        {
            this.slotClickHandler = slotClickHandler;
            this.slotDoubleClickHandler = slotDoubleClickHandler;

            b.Content = "For loop";
            b.Background = Brushes.LightSteelBlue;
        }

        public override Border CreateBlock()
        {
            return CreateBlockInstance(new NodeGraph()).Border;
        }

        public override BlockInstance CreateBlockInstance(NodeGraph graph)
        {
            // Backend: LoopNode does the iteration. We add a hidden SetVariableNode
            // so the counter variable is updated to the current loop index on every
            // iteration. Blocks snapped below this for-loop become its body.
            var loop = new LoopNode();
            var counterSet = new SetVariableNode("");
            var stepConst = new ConstantNode<double>(new Value(1.0));

            graph.AddNode(loop);
            graph.AddNode(counterSet);
            graph.AddNode(stepConst);

            // step is always 1
            loop.StepPin.Connect(stepConst.ValuePin);
            // each iteration: BodyPin -> SetVariable(counter), ValuePin <- CurrentPin
            loop.BodyPin.Connect(counterSet.InFlowPin);
            counterSet.ValuePin.Connect(loop.CurrentPin);

            Border block;

            StackPanel row = new StackPanel { Orientation = Orientation.Horizontal };

            Slot counterSlot = new Slot(SlotType.VariableOnly, "counter", slotClickHandler, slotDoubleClickHandler);
            Slot startSlot = new Slot(SlotType.NumberOrVariable, "start", slotClickHandler, slotDoubleClickHandler);
            Slot endSlot = new Slot(SlotType.NumberOrVariable, "end", slotClickHandler, slotDoubleClickHandler);

            // counter variable name -> SetVariableNode identifier
            counterSlot.OnValueSet = s =>
            {
                if (s.Value is Variable v) counterSet.Identifier = v.Name;
            };
            startSlot.TargetPin = loop.StartPin;
            endSlot.TargetPin = loop.EndPin;

            TextBlock forLabel = new TextBlock
            {
                Text = "for",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 4, 0)
            };
            TextBlock equalsText = new TextBlock
            {
                Text = "=",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(4, 0, 4, 0)
            };
            TextBlock toLabel = new TextBlock
            {
                Text = "to",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(4, 0, 4, 0)
            };

            row.Children.Add(forLabel);
            row.Children.Add(counterSlot.Button);
            row.Children.Add(equalsText);
            row.Children.Add(startSlot.Button);
            row.Children.Add(toLabel);
            row.Children.Add(endSlot.Button);

            var layout = new StackPanel { Orientation = Orientation.Vertical };
            layout.Children.Add(row);
            layout.Children.Add(new TextBlock
            {
                Text = "↳ loop body (drop blocks under this)",
                Margin = new Thickness(16, 4, 0, 4),
                FontSize = 11,
                Opacity = 0.8
            });
            layout.Children.Add(new TextBlock
            {
                Text = "end for",
                Margin = new Thickness(0, 2, 0, 0),
                FontWeight = FontWeights.SemiBold
            });

            block = CreateStyledBlock(Brushes.LightSteelBlue, layout);

            BlockInstance inst = new BlockInstance
            {
                Border = block,
                Node = loop,
                InFlow = loop.InFlowPin,
                // OutFlow is the SetVariable's out: anything snapped below ends up
                // INSIDE the loop body, with the counter already set for this iteration.
                OutFlow = counterSet.OutFlowPin
            };
            inst.Slots.Add(counterSlot);
            inst.Slots.Add(startSlot);
            inst.Slots.Add(endSlot);
            inst.AllNodes.Add(loop);
            inst.AllNodes.Add(counterSet);
            inst.AllNodes.Add(stepConst);
            return inst;
        }
    }
}
