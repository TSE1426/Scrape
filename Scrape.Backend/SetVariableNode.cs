namespace Scrape.Backend;

public sealed class SetVariableNode : Node
{
    public string Identifier;
    public readonly InFlowPin InFlowPin;
    public readonly OutFlowPin OutFlowPin;
    public readonly InPin ValuePin;

    public SetVariableNode(string identifier) : base("Set Variable")
    {
        Identifier = identifier;
        InFlowPin = AddInPin<InFlowPin>("In");
        OutFlowPin = AddOutPin<OutFlowPin>("Out");
        ValuePin = AddInPin<InPin>("Value");
    }

    public override void Evaluate(EvaluationContext ctx)
    {
        // Evaluate the 'Value' input pin
        base.Evaluate(ctx);
        // Set variable to the value at the top of the stack
        ctx.Set(Identifier, ctx.Pop());

        // Continue on our merry way
        PinHelper.ContinueFlow(ctx, OutFlowPin);
    }
}
