namespace Scrape.Backend;

public sealed class NoOpNode : Node
{
    public readonly InFlowPin InFlowPin;
    public readonly OutFlowPin OutFlowPin;

    public NoOpNode(string label) : base(label)
    {
        InFlowPin = AddInPin<InFlowPin>("In");
        OutFlowPin = AddOutPin<OutFlowPin>("Out");
    }

    public override void Evaluate(EvaluationContext ctx) { }
}
