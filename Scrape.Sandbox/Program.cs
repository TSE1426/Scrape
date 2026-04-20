using Scrape.Backend;

namespace Scrape.Sandbox;

internal class Program
{
    private static void Main(string[] args)
    {
        var graph = new NodeGraph();

        // The code below produces a graph roughly equivalent to the following
        // ┌───────────┐     ┌────────────┐      ┌─────────────┐      ┌────────────┐ 
        // │   Start   │     │   SetVar   │      │   Branch    │      │   Print    │ 
        // │           │     │            │      │             │      │            │ 
        // │      Start├───►│Flow    Flow├────►│Flow   IfTrue│────►│Flow        │ 
        // └───────────┘     │     ┌───┐  │      │             │      │  ┌────────┐│ 
        //                   │Name │var│  │  ┌─►│Cond  IfFalse│──┐   │  │Was true││ 
        //                   │     └───┘  │  │   └─────────────┘  │   │  └────────┘│ 
        // ┌───────────┐  ┌►│Val         │  │                    │   └────────────┘ 
        // │   Const   │  │  └────────────┘  │                    │                  
        // │     ┌────┐│  │  ┌───────────┐   │                    │   ┌─────────────┐
        // │     │true│├──┘  │   GetVar  │   │                    │   │   Print     │
        // │     └────┘│     │     ┌───┐ │   │                    │   │             │
        // └───────────┘     │Name │var│ │   │                    └─►│Flow         │
        //                   │     └───┘ │   │                        │  ┌─────────┐│
        //                   │        Val├───┘                        │  │Was false││
        //                   └───────────┘                            │  └─────────┘│
        //                                                            └─────────────┘

        // Change to false and the output should change
        const bool BRANCH = true;

        var constNode = graph.AddNode(new ConstantNode<bool>(new(BRANCH)));
        var setVarNode = graph.AddNode(new SetVariableNode("var"));

        var getVarNode = graph.AddNode(new GetVariableNode("var"));
        var branchNode = graph.AddNode(new BranchNode());
        var trueBranch = graph.AddNode(new DebugStringNode("var was true"));
        var falseBranch = graph.AddNode(new DebugStringNode("var was false"));

        graph.StartNode.StartPin.Connect(setVarNode.InFlowPin);
        setVarNode.OutFlowPin.Connect(branchNode.FlowPin);
        setVarNode.ValuePin.Connect(constNode.ValuePin);

        branchNode.CondPin.Connect(getVarNode.ValuePin);
        branchNode.IfTruePin.Connect(trueBranch.InFlowPin);
        branchNode.IfFalsePin.Connect(falseBranch.InFlowPin);

        var ctx = new EvaluationContext();
        graph.Evaluate(ctx);
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
