using Scrape.Backend;

namespace Scrape.Sandbox;

internal class Program
{
    private static void Main(string[] args)
    {
        var graph = new NodeGraph();

        // Nodes for:
        // i = 0
        // while (i < 10):
        //      i += 1
        //      print(i)

        // Nodes

        // Make nodes for value of 0 and setting i
        var zero = graph.AddNode(new ConstantNode<double>(new Value(0.0)));
        var setI = graph.AddNode(new SetVariableNode("i"));

        // Node for while loop
        var whileLoop = graph.AddNode(new WhileLoopNode());

        // Make the condition nodes: need nodes for getting i, value of 10, comparing 
        var getI_cond = graph.AddNode(new GetVariableNode("i"));
        var ten = graph.AddNode(new ConstantNode<double>(new Value(10.0)));
        var compare = graph.AddNode(new CompareNode() { Operation = CompareNode.OperationType.LessThan });

        // Make the body nodes: need to get i, value of 1 (for increment), adding, then setting i again
        var getI_body = graph.AddNode(new GetVariableNode("i"));
        var one = graph.AddNode(new ConstantNode<double>(new Value(1.0)));
        var add = graph.AddNode(new OperatorNode() { Operation = OperatorNode.OperationType.Add });
        var setI2 = graph.AddNode(new SetVariableNode("i"));

        // Nodes for printing i: need to get i, printing i
        var getI_debug = graph.AddNode(new GetVariableNode("i"));
        var debug = graph.AddNode(new DebugValueNode("i"));

        // Links

        // Connect start node to first set of i,
        // Connect the value of zero to the value of setI
        // Connect setI to the while loop
        graph.StartNode.StartPin.Connect(setI.InFlowPin);
        zero.ValuePin.Connect(setI.ValuePin);
        setI.OutFlowPin.Connect(whileLoop.InFlowPin);

        // Get the i for the condition and connect its value to compare node of the left side
        // Set 10 to the right side of the compare node
        // Connect the compare node to the while loop
        getI_cond.ValuePin.Connect(compare.LhsPin);
        ten.ValuePin.Connect(compare.RhsPin);
        compare.ResultPin.Connect(whileLoop.CondPin);

        // Connect body of while loop to setting i
        // Connect value of i in the body to left side of add
        // Connect value of 1 to right side of add
        // Connect the result of add to setI2 
        // Keep flowing to debug
        // Connect value of i to the value at debug
        whileLoop.BodyPin.Connect(setI2.InFlowPin);
        getI_body.ValuePin.Connect(add.LhsPin);
        one.ValuePin.Connect(add.RhsPin);
        add.ResultPin.Connect(setI2.ValuePin);
        setI2.OutFlowPin.Connect(debug.InFlowPin);
        getI_debug.ValuePin.Connect(debug.ValuePin);

        var ctx = new EvaluationContext();
        graph.Evaluate(ctx);

        foreach (var (_, message) in ctx.Errors)
            Console.Error.WriteLine(message);
    }
}


internal sealed class DebugValueNode : Node
{
    public readonly InFlowPin InFlowPin;
    public readonly InPin ValuePin;
    public readonly OutFlowPin OutFlowPin;
    private readonly string _label;

    public DebugValueNode(string label) : base("Debug Value")
    {
        InFlowPin = AddInPin<InFlowPin>("In");
        ValuePin = AddInPin<InPin>("Value");
        OutFlowPin = AddOutPin<OutFlowPin>("Out");
        _label = label;
    }

    public override void Evaluate(EvaluationContext ctx)
    {
        base.Evaluate(ctx);

        var value = ctx.Pop();
        Console.Error.WriteLine($"{_label} = {FormatValue(value)}");

        PinHelper.ContinueFlow(ctx, OutFlowPin);
    }

    private static string FormatValue(Value value)
    {
        return value.Type switch
        {
            Value.ValueType.Number => value.AsNumber().ToString(),
            Value.ValueType.Boolean => value.AsBoolean().ToString(),
            Value.ValueType.String => value.AsString(),
            _ => "<unknown>",
        };
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
