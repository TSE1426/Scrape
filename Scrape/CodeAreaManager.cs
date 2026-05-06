using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Scrape.Backend;

namespace Scrape
{
    public class CodeAreaManager
    {
        public List<BlockInstance> Instances => instances;

        private readonly Canvas codeArea;
        private readonly BlockDragManager dragManager;
        private readonly List<BlockInstance> instances = new();
        private readonly BlockInstance programStart;
        private bool running = false;

        public NodeGraph Graph { get; }


        public void ClearProgram()
        {
            for (int i = instances.Count - 1; i >= 0; i--)
            {
                if (!instances[i].IsLocked)
                {
                    codeArea.Children.Remove(instances[i].Border);
                    instances.RemoveAt(i);
                }
            }

            programStart.Below = null;
        }


        public CodeAreaManager(Canvas canvas)
        {
            codeArea = canvas;
            Graph = new NodeGraph();
            dragManager = new BlockDragManager(codeArea, OnBlockDropped, OnBlockDragged);
            programStart = CreateProgramStartBlock();
            instances.Add(programStart);
            codeArea.Children.Add(programStart.Border);
        }

        public BlockInstance AddTileBlock(Tile tile)
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

                if (tile is ForLoopTile)
                {
                    var endFor = new EndBlockTile("end for", inst.Node, ((LoopNode)inst.Node).CompletedPin).CreateBlockInstance(Graph);
                    AddBlockInstance(endFor);
                    Connect(inst, endFor);
                }
                else if (tile is IfTile)
                {
                    var endIf = new EndBlockTile("end if", inst.Node, ((BranchNode)inst.Node).CompletedPin).CreateBlockInstance(Graph);
                    AddBlockInstance(endIf);
                    Connect(inst, endIf);
                }
                else if (tile is WhileLoopTile)
                {
                    var endWhile = new EndBlockTile("end while", inst.Node, ((WhileLoopNode)inst.Node).CompletedPin).CreateBlockInstance(Graph);
                    AddBlockInstance(endWhile);
                    Connect(inst, endWhile);
                }
                return inst;
            }
            else
            {
                // tile has no backend node, just put a plain block on the canvas
                Border block = tile.CreateBlock();
                Canvas.SetLeft(block, 20);
                Canvas.SetTop(block, 20 + (codeArea.Children.Count * 50));
                codeArea.Children.Add(block);
                dragManager.Attach(block);
                return null;
            }
             
        }


        private void AddBlockInstance(BlockInstance inst)
        {
            instances.Add(inst);
            Canvas.SetLeft(inst.Border, 20);
            Canvas.SetTop(inst.Border, 20 + (codeArea.Children.Count * 50));
            inst.Border.Tag = inst;

            var menu = new ContextMenu();
            var deleteItem = new MenuItem { Header = "Delete" };
            deleteItem.Click += (s, e) => Delete(inst);
            menu.Items.Add(deleteItem);
            inst.Border.ContextMenu = menu;

            codeArea.Children.Add(inst.Border);
            if (!inst.IsLocked)
            {
                dragManager.Attach(inst.Border);
            }
        }

        private BlockInstance CreateProgramStartBlock()
        {
            var border = new Border
            {
                BorderBrush = System.Windows.Media.Brushes.Black,
                BorderThickness = new Thickness(1),
                Background = System.Windows.Media.Brushes.LightGreen,
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(8),
                Child = new TextBlock { Text = "Program Start" },
            };

            Canvas.SetLeft(border, 20);
            Canvas.SetTop(border, 20);

            return new BlockInstance
            {
                Border = border,
                PseudocodeLabel = "Program Start",
                IsLocked = true,
                OutFlow = Graph.StartNode.StartPin,
            };
        }

        // Remove a block from the canvas and the graph. Reconnects the chain
        // around it: if it had Above and Below, they become snapped together.
        public void Delete(BlockInstance inst)
        {
            if (inst.IsLocked) return;
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
            // visual: snap exactly under the upper block and move lower chain with it
            MoveChain(lower, Canvas.GetLeft(upper.Border), Canvas.GetTop(upper.Border) + upper.Border.ActualHeight);

            // backend: connect flow pins
            upper.OutFlow.Connect(lower.InFlow);

            upper.Below = lower;
            lower.Above = upper;
        }

        private void OnBlockDragged(UIElement element)
        {
            if (element is not Border border) return;
            if (border.Tag is not BlockInstance inst) return;
            if (inst.Below == null) return;

            double nextLeft = Canvas.GetLeft(inst.Border);
            double nextTop = Canvas.GetTop(inst.Border) + inst.Border.ActualHeight;
            MoveChain(inst.Below, nextLeft, nextTop);
        }

        private void MoveChain(BlockInstance root, double left, double top)
        {
            BlockInstance current = root;
            double currentTop = top;

            while (current != null)
            {
                Canvas.SetLeft(current.Border, left);
                Canvas.SetTop(current.Border, currentTop);
                currentTop += current.Border.ActualHeight;
                current = current.Below;
            }
        }

        private void Disconnect(BlockInstance upper, BlockInstance lower)
        {
            upper.OutFlow.Disconnect(lower.InFlow);
            upper.Below = null;
            lower.Above = null;
        }

        public void Stop()
        {
            running = false;
        }

        // Called by the Run button. Wires up slot values to backend pins, hooks the
        // start node to the top blocks, evaluates, then cleans up the temporary nodes.
        public void Run(EvaluationContext ctx)
        {
            // create helper nodes for each filled slot
            var helperNodes = new List<Node>();
            var helperConnections = new List<(InPin In, OutPin Out)>();
            lock (programStart)
            {
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
                        else if (slot.Value is bool b)
                        {
                            var c = new ConstantNode<bool>(new Value(b));
                            Graph.AddNode(c);
                            helperNodes.Add(c);
                            helperOut = c.ValuePin;
                        }
                        else if (slot.Value is string s)
                        {
                            var c = new ConstantNode<string>(new Value(s));
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
            }

            running = true;

            // run!
            new Thread(() =>
            {
                lock (programStart)
                {
                    while (running)
                    {
                        var startTime = DateTime.Now;
                        Graph.Evaluate(ctx);
                        if (ctx.Errors.Count > 0)
                            break;
                        var endTime = DateTime.Now;
                        var delta = endTime - startTime;
                        if (delta.TotalMilliseconds < 16)
                            Thread.Sleep((int)(16 - delta.TotalMilliseconds));
                    }

                    // clean up so the next run starts fresh
                    foreach (var (inPin, outPin) in helperConnections)
                    {
                        inPin.Disconnect(outPin);
                    }
                    foreach (var n in helperNodes)
                    {
                        Graph.Nodes.Remove(n);
                    }

                    running = false;
                }

            }).Start();
        }

        public string BuildProgramText()
        {
            var lines = new List<string>();
            int chainIndex = 1;

            lines.Add("Program Start");

            foreach (var inst in instances)
            {
                if (inst.IsLocked) continue;
                if (inst.Above != null) continue;

                lines.Add($"Chain {chainIndex}:");
                chainIndex++;

                int step = 1;
                int indentLevel = 1;
                var blockStack = new Stack<string>();

                BlockInstance current = inst;
                while (current != null)
                {
                    string label = current.PseudocodeLabel ?? (current.Border.Child as TextBlock)?.Text ?? current.Node?.Label ?? "Block";
                    string indent = new string(' ', indentLevel * 2);
                    lines.Add($"{indent}{step}. {label}");

                    if (current.Node is LoopNode)
                    {
                        blockStack.Push("end for");
                        indentLevel++;
                    }
                    else if (current.Node is BranchNode)
                    {
                        blockStack.Push("end if");
                        indentLevel++;
                    }

                    step++;
                    current = current.Below;
                }

                while (blockStack.Count > 0)
                {
                    indentLevel = Math.Max(1, indentLevel - 1);
                    string indent = new string(' ', indentLevel * 2);
                    lines.Add($"{indent}{step}. {blockStack.Pop()}");
                    step++;
                }
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}
