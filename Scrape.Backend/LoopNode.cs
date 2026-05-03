namespace Scrape.Backend;

public sealed class LoopNode : Node
{
    public InPin<double> StartPin;
    public InPin<double> EndPin;
    public InPin<double> StepPin;
    public InFlowPin InFlowPin;
    public OutFlowPin BodyPin;
    public OutFlowPin CompletedPin;
    public OutPin<double> CurrentPin;

    // Used for CurrentPin
    private bool _isLooping;
    private int _index;

    public LoopNode() : base("Loop")
    {
        StartPin = AddInPin<InPin<double>>("Start");
        EndPin = AddInPin<InPin<double>>("End");
        StepPin = AddInPin<InPin<double>>("Step");
        InFlowPin = AddInPin<InFlowPin>("In");

        BodyPin = AddOutPin<OutFlowPin>("Body");
        CompletedPin = AddOutPin<OutFlowPin>("Completed");
        CurrentPin = AddOutPin<OutPin<double>>("Current");
    }

    public override void Evaluate(EvaluationContext ctx)
    {
        if (_isLooping)
        {
            // We're inside the loop, this must be the the 'current' pin
            ctx.Push(_index);
            return;
        }

        // Evaluate Start, To, and Step
        base.Evaluate(ctx);

        // Pop values in reverse order to pins
        var step = (int)ctx.PopNumber();
        var end = (int)ctx.PopNumber();
        var start = (int)ctx.PopNumber();

        if (step == 0)
        {
            ctx.ReportError(this, "Loop step cannot be 0.");
            PinHelper.ContinueFlow(ctx, CompletedPin);
            return;
        }

        bool shouldContinue()
        {
            if (step > 0)
                return _index < end;
            else
                return _index > end;
        }

        _index = start;
        _isLooping = true;

        for (; shouldContinue(); _index += step)
        {
            // Run thru the loop N times
            PinHelper.ContinueFlow(ctx, BodyPin);
        }

        _isLooping = false;

        // Continue on afterwards
        PinHelper.ContinueFlow(ctx, CompletedPin);
    }
}
