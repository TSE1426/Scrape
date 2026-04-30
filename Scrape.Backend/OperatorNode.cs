namespace Scrape.Backend;

// Takes two numbers and operates on them
public sealed class OperatorNode : Node
{
    public enum OperationType
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    public InPin LhsPin;
    public InPin RhsPin;
    public OutPin ResultPin;
    public OperationType Operation;

    public OperatorNode() : base("Operator")
    {
        LhsPin = AddInPin<InPin>("Left");
        RhsPin = AddInPin<InPin>("Right");
        ResultPin = AddOutPin<OutPin>("Result");
    }

    public override void Evaluate(EvaluationContext ctx)
    {
        // Evaluate Lhs & Rhs
        base.Evaluate(ctx);

        // NOTE: Stack is a stack, values are in reverse order to pins
        var rhs = ctx.Pop();
        var lhs = ctx.Pop();

        // This node only to be used with numbers, could add it for strings as well but idk if we need that
        if (lhs.Type != Value.ValueType.Number || rhs.Type != Value.ValueType.Number)
        {
            // NOTE: Ideally the frontend would prevent this, so the error handling is somewhat minimal (for now)
            ctx.ReportError(this, "Number type required");
            ctx.Push(false);
            return;
        }

        var lNum = lhs.AsNumber();
        var rNum = rhs.AsNumber();
        double result = 0;

        switch (Operation)
        {
            case OperationType.Add:
                result = lNum + rNum;
                break;

            case OperationType.Subtract:
                result = lNum - rNum;
                break;

            case OperationType.Multiply:
                result = lNum * rNum;
                break;

            case OperationType.Divide:
                result = lNum / rNum;
                break;
        }

        ctx.Push(result);
    }
}
