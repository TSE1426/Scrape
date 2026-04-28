using Scrape.Backend;

namespace Scrape.Sandbox;

internal class Program
{
    private static void Main(string[] args)
    {
        var graph = new NodeGraph();

        var loopNode = graph.AddNode(new LoopNode());
        var printCurrentNode = graph.AddNode(new DebugValueNode("Current"));
        var printCompletedNode = graph.AddNode(new DebugStringNode("Loop finished"));

        var startValueNode = graph.AddNode(new ConstantNode<double>(new(1)));
        var endValueNode = graph.AddNode(new ConstantNode<double>(new(10)));
        var stepValueNode = graph.AddNode(new ConstantNode<double>(new(2)));

        graph.StartNode.StartPin.Connect(loopNode.InFlowPin);

        startValueNode.ValuePin.Connect(loopNode.StartPin);
        endValueNode.ValuePin.Connect(loopNode.EndPin);
        stepValueNode.ValuePin.Connect(loopNode.StepPin);
        loopNode.BodyPin.Connect(printCurrentNode.InFlowPin);
        loopNode.CurrentPin.Connect(printCurrentNode.ValuePin);
        loopNode.CompletedPin.Connect(printCompletedNode.InFlowPin);

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
