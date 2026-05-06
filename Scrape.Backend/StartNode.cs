using System.Diagnostics;

namespace Scrape.Backend;

public sealed class StartNode : Node
{
    public readonly OutFlowPin StartPin;

    public StartNode() : base("Start")
    {
        StartPin = AddOutPin<OutFlowPin>("Start");
    }

    public override void Evaluate(EvaluationContext ctx)
    {
        PinHelper.ContinueFlow(ctx, StartPin);
    }
}
