using System.Collections.Generic;

namespace Scrape.Backend;

public sealed class NodeGraph
{
    public StartNode StartNode;
    public readonly List<Node> Nodes;

    public NodeGraph()
    {
        StartNode = new();
        Nodes = [StartNode];
    }

    public TNode AddNode<TNode>(TNode node) where TNode : Node
    {
        Nodes.Add(node);
        return node;
    }

    public void Evaluate(EvaluationContext ctx)
    {
        StartNode.Evaluate(ctx);
    }
}
