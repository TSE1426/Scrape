namespace Scrape.Backend;

public sealed class ConstantNode<TValue> : Node
{
    public Value Value;
    public readonly OutPin<TValue> ValuePin;

    public ConstantNode(Value value) : base("Constant")
    {
        Value = value;
        ValuePin = AddOutPin<OutPin<TValue>>("Out");
    }

    public override void Evaluate(EvaluationContext ctx)
    {
        ctx.Push(Value);
    }
}
