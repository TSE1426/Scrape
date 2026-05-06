using System;
using System.Threading;

namespace Scrape.Backend;

public sealed class WhileLoopNode : Node
{
    public InFlowPin InFlowPin;
    public InPin<bool> CondPin;
    public OutFlowPin BodyPin;
    public OutFlowPin CompletedPin;

    public WhileLoopNode() : base("While Loop")
    {
        InFlowPin = AddInPin<InFlowPin>("In");
        CondPin = AddInPin<InPin<bool>>("Condition");
        BodyPin = AddOutPin<OutFlowPin>("Body");
        CompletedPin = AddOutPin<OutFlowPin>("Completed");
    }

    public override void Evaluate(EvaluationContext ctx)
    {
        while (true)
        {
            base.Evaluate(ctx);
            var cond = ctx.PopBoolean();

            if(cond)
            { 
                // added a rate limiter so while loop doesn't freeze program
                var startTime = DateTime.Now;
                PinHelper.ContinueFlow(ctx, BodyPin);
                var endTime = DateTime.Now;
                var delta = endTime - startTime;
                if (delta.TotalMilliseconds < 16)
                    Thread.Sleep((int)(16 - delta.TotalMilliseconds));
            }
            else
            {
                break;
            }
        }

        PinHelper.ContinueFlow(ctx, CompletedPin);
    }

}