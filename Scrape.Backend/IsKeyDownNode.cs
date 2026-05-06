namespace Scrape.Backend;

public sealed class IsKeyDownNode : Node
{
    public string Key;
    public readonly InFlowPin InFlowPin;
    public readonly OutFlowPin OutFlowPin;
    public readonly OutPin<bool> ValuePin;

    public IsKeyDownNode(string key) : base("Is Key Down")
    {
        Key = key;
        InFlowPin = AddInPin<InFlowPin>("In");
        OutFlowPin = AddOutPin<OutFlowPin>("Out");
        ValuePin = AddOutPin<OutPin<bool>>("Value");
    }

    public override void Evaluate(EvaluationContext ctx)
    {
        ctx.Push(ctx.IsKeyPressed?.Invoke(Key) ?? false);

        PinHelper.ContinueFlow(ctx, OutFlowPin);
    }
}
