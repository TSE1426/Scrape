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
                PinHelper.ContinueFlow(ctx, BodyPin);
            }
            else
            {
                break;
            }
        }

        PinHelper.ContinueFlow(ctx, CompletedPin);
    }

}