using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Scrape.Backend;

namespace Scrape
{
    public class CodeAreaManager
    {
        private readonly Canvas codeArea;
        private readonly BlockDragManager dragManager;
        private readonly List<BlockInstance> instances = new();

        public NodeGraph Graph { get; }

        public CodeAreaManager(Canvas canvas)
        {
            codeArea = canvas;
            Graph = new NodeGraph();
            dragManager = new BlockDragManager(codeArea, OnBlockDropped);
        }

        public void AddTileBlock(Tile tile)
        {
            BlockInstance inst = tile.CreateBlockInstance(Graph);

            if (inst != null)
            {
                instances.Add(inst);

                Canvas.SetLeft(inst.Border, 20);
                Canvas.SetTop(inst.Border, 20 + (codeArea.Children.Count * 50));
                inst.Border.Tag = inst;

                // right-click menu: Delete
                var menu = new ContextMenu();
                var deleteItem = new MenuItem { Header = "Delete" };
                deleteItem.Click += (s, e) => Delete(inst);
                menu.Items.Add(deleteItem);
                inst.Border.ContextMenu = menu;

                codeArea.Children.Add(inst.Border);
                dragManager.Attach(inst.Border);
            }
            else
            {
                // tile has no backend node, just put a plain block on the canvas
                Border block = tile.CreateBlock();
                Canvas.SetLeft(block, 20);
                Canvas.SetTop(block, 20 + (codeArea.Children.Count * 50));
                codeArea.Children.Add(block);
                dragManager.Attach(block);
            }
        }

        // Remove a block from the canvas and the graph. Reconnects the chain
        // around it: if it had Above and Below, they become snapped together.
        public void Delete(BlockInstance inst)
        {
            BlockInstance above = inst.Above;
            BlockInstance below = inst.Below;

            if (above != null)
            {
                Disconnect(above, inst);
            }
            if (below != null)
            {
                Disconnect(inst, below);
            }
            if (above != null && below != null)
            {
                Connect(above, below);
            }

            foreach (var node in inst.AllNodes)
            {
                Graph.Nodes.Remove(node);
            }

            codeArea.Children.Remove(inst.Border);
            instances.Remove(inst);
        }

        // Snapping: when a block is dropped close to the bottom of another, snap them
        // together and connect their flow pins in the backend graph.
        private const double SnapThreshold = 30;

        private void OnBlockDropped(UIElement element)
        {
            if (element is not Border border) return;
            if (border.Tag is not BlockInstance inst) return;
            if (inst.InFlow == null) return;

            // always drop any old "above" connection - we re-snap below
            if (inst.Above != null)
            {
                Disconnect(inst.Above, inst);
            }

            // look for a block to snap below
            BlockInstance best = null;
            double bestDistance = double.MaxValue;

            double dropTop = Canvas.GetTop(inst.Border);
            double dropLeft = Canvas.GetLeft(inst.Border);

            foreach (var other in instances)
            {
                if (other == inst) continue;
                if (other.OutFlow == null) continue;
                if (other.Below != null) continue;       // already has someone snapped below

                double otherBottom = Canvas.GetTop(other.Border) + other.Border.ActualHeight;
                double otherLeft = Canvas.GetLeft(other.Border);

                double dy = Math.Abs(dropTop - otherBottom);
                double dx = Math.Abs(dropLeft - otherLeft);

                if (dy < SnapThreshold && dx < SnapThreshold)
                {
                    double total = dy + dx;
                    if (total < bestDistance)
                    {
                        bestDistance = total;
                        best = other;
                    }
                }
            }

            if (best != null)
            {
                Connect(best, inst);
            }
        }

        private void Connect(BlockInstance upper, BlockInstance lower)
        {
            // visual: snap exactly under the upper block
            Canvas.SetLeft(lower.Border, Canvas.GetLeft(upper.Border));
            Canvas.SetTop(lower.Border, Canvas.GetTop(upper.Border) + upper.Border.ActualHeight);

            // backend: connect flow pins
            upper.OutFlow.Connect(lower.InFlow);

            upper.Below = lower;
            lower.Above = upper;
        }

        private void Disconnect(BlockInstance upper, BlockInstance lower)
        {
            upper.OutFlow.Disconnect(lower.InFlow);
            upper.Below = null;
            lower.Above = null;
        }

        // Called by the Run button. Wires up slot values to backend pins, hooks the
        // start node to the top blocks, evaluates, then cleans up the temporary nodes.
        public void Run(EvaluationContext ctx)
        {
            // create helper nodes for each filled slot
            var helperNodes = new List<Node>();
            var helperConnections = new List<(InPin In, OutPin Out)>();

            foreach (var inst in instances)
            {
                foreach (var slot in inst.Slots)
                {
                    if (slot.TargetPin == null || slot.Value == null) continue;

                    OutPin helperOut = null;

                    if (slot.Value is Variable v)
                    {
                        var get = new GetVariableNode(v.Name);
                        Graph.AddNode(get);
                        helperNodes.Add(get);
                        helperOut = get.ValuePin;
                    }
                    else if (slot.Value is double d)
                    {
                        var c = new ConstantNode<double>(new Value(d));
                        Graph.AddNode(c);
                        helperNodes.Add(c);
                        helperOut = c.ValuePin;
                    }

                    if (helperOut != null)
                    {
                        slot.TargetPin.Connect(helperOut);
                        helperConnections.Add((slot.TargetPin, helperOut));
                    }
                }
            }

            // connect the start node to the top of every chain
            var startConnections = new List<InPin>();
            foreach (var inst in instances)
            {
                if (inst.Above != null) continue;
                if (inst.InFlow == null) continue;
                Graph.StartNode.StartPin.Connect(inst.InFlow);
                startConnections.Add(inst.InFlow);
            }

            // run!
            Graph.Evaluate(ctx);

            // clean up so the next run starts fresh
            foreach (var (inPin, outPin) in helperConnections)
            {
                inPin.Disconnect(outPin);
            }
            foreach (var n in helperNodes)
            {
                Graph.Nodes.Remove(n);
            }
            foreach (var inPin in startConnections)
            {
                Graph.StartNode.StartPin.Disconnect(inPin);
            }
        }
    }
}
