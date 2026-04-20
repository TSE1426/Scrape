using System.Diagnostics;

namespace Scrape.Backend;

public static class PinHelper
{
    public static void ContinueFlow(EvaluationContext ctx, OutFlowPin outFlowPin)
    {
        foreach (var pin in outFlowPin.ConnectedPins)
        {
            Debug.Assert(pin is InFlowPin);
            pin.Parent.Evaluate(ctx);
        }
    }
}
