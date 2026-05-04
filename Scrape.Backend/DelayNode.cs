using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Scrape.Backend;

public sealed class WaitNode : Node
{
    public InFlowPin InFlowPin;
    public InPin<double> DurationPin;
    public OutFlowPin CompletedPin;

    public WaitNode() : base("Wait")
    {
        InFlowPin = AddInPin<InFlowPin>("In");
        DurationPin = AddInPin<InPin<double>>("Seconds");
        CompletedPin = AddOutPin<OutFlowPin>("Completed");
    }

    public override void Evaluate(EvaluationContext ctx)
    {
        base.Evaluate(ctx);

        // Pop the duration (in seconds) from the evaluation context stack
        var duration = ctx.PopNumber();

        // Convert seconds to milliseconds and delay
        int delayMs = (int)(duration * 1000);
        Thread.Sleep(delayMs); // blocks synchronously

        PinHelper.ContinueFlow(ctx, CompletedPin);
    }
}

