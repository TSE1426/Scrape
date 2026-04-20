namespace Scrape.Backend;

public sealed class GetVariableNode : Node
{
    public string Identifier;
    public readonly OutPin ValuePin;

    public GetVariableNode(string identifier) : base("Get Variable")
    {
        Identifier = identifier;
        ValuePin = AddOutPin<OutPin>("Value");
    }

    public override void Evaluate(EvaluationContext ctx)
    {
        // Push variable to top of stack
        ctx.Push(ctx.Get(Identifier));
    }
}
