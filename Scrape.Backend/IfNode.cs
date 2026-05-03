using System;
using System.Collections.Generic;
using System.Text;

namespace Scrape.Backend;

public sealed class IfNode : Node
{
    public InFlowPin InFlowPin;
    public InPin<bool> CondPin;
    public OutFlowPin TruePin;
    public OutFlowPin FalsePin;
    public OutFlowPin CompletedPin;
   
    public IfNode() : base("If")
    {
        InFlowPin = AddInPin<InFlowPin>("In");
        CondPin = AddInPin<InPin<bool>>("Condition");
        TruePin = AddOutPin<OutFlowPin>("True");
        FalsePin = AddOutPin<OutFlowPin>("False");
        CompletedPin = AddOutPin<OutFlowPin>("Completed");
    }
    
    public override void Evaluate(EvaluationContext ctx)
    {
        base.Evaluate(ctx);

        var cond = ctx.PopBoolean();

        if (cond)
        {
            PinHelper.ContinueFlow(ctx, TruePin);
        }
        else
        {
            PinHelper.ContinueFlow(ctx, FalsePin);
        }

        PinHelper.ContinueFlow(ctx, CompletedPin);
    }
}

