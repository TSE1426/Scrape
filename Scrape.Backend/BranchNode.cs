using System.Collections.Generic;

namespace Scrape.Backend;

public sealed class BranchNode : Node
{
    public InFlowPin FlowPin;
    public InPin<bool> CondPin;
    public OutFlowPin CompletedPin;
    public OutFlowPin IfTruePin;
    public OutFlowPin IfFalsePin;

    public BranchNode() : base("Branch")
    {
        FlowPin = AddInPin<InFlowPin>("Flow");
        CondPin = AddInPin<InPin<bool>>("Condition");
        CompletedPin = AddOutPin<OutFlowPin>("Completed");
        IfTruePin = AddOutPin<OutFlowPin>("If True");
        IfFalsePin = AddOutPin<OutFlowPin>("If False");
    }

    public override void Evaluate(EvaluationContext ctx)
    {
        // Goes backwards and performs actions until a bool is at the top of the stack.
        base.Evaluate(ctx);

        // Determine the branch we need to take
        var cond = ctx.PopBoolean();
        OutFlowPin branchPin;
        if (cond)
            branchPin = IfTruePin;
        else
            branchPin = IfFalsePin;

        // Evaluate all connections from the chosen branch
        PinHelper.ContinueFlow(ctx, branchPin);

        // Continue on our merry way
        PinHelper.ContinueFlow(ctx, CompletedPin);
    }
}
