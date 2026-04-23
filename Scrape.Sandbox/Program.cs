using Scrape.Backend;

namespace Scrape.Sandbox;

internal class Program
{
    private static void Main(string[] args)
    {
        var graph = new NodeGraph();

        var branchNode = graph.AddNode(new BranchNode());
        var trueBranch = graph.AddNode(new DebugStringNode("var was true"));
        var falseBranch = graph.AddNode(new DebugStringNode("var was false"));

        var lhsNode = graph.AddNode(new ConstantNode<double>(new(34)));
        var rhsNode = graph.AddNode(new ConstantNode<double>(new(35)));
        var compNode = graph.AddNode(new CompareNode());

        compNode.LhsPin.Connect(lhsNode.ValuePin);
        compNode.RhsPin.Connect(rhsNode.ValuePin);
        compNode.Operation = CompareNode.OperationType.LessThan;

        branchNode.IfFalsePin.Connect(falseBranch.InFlowPin);
        branchNode.IfTruePin.Connect(trueBranch.InFlowPin);
        branchNode.CondPin.Connect(compNode.ResultPin);

        graph.StartNode.StartPin.Connect(branchNode.FlowPin);

        var ctx = new EvaluationContext();
        graph.Evaluate(ctx);

        foreach (var (_, message) in ctx.Errors)
            Console.Error.WriteLine(message);
    }
}

internal sealed class DebugStringNode : Node
{
    public readonly InFlowPin InFlowPin;
    public readonly OutFlowPin OutFlowPin;
    private readonly string _text;

    public DebugStringNode(string text) : base("Debug")
    {
        InFlowPin = AddInPin<InFlowPin>("In");
        OutFlowPin = AddOutPin<OutFlowPin>("Out");
        _text = text;
    }

    public override void Evaluate(EvaluationContext ctx)
    {
        base.Evaluate(ctx);
        Console.Error.WriteLine(_text);
    }
}
