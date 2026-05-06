namespace Scrape.Backend;

public sealed class CompareNode : Node
{
    public enum OperationType
    {
        Equal,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
    }

    public InPin LhsPin;
    public InPin RhsPin;
    public OutPin<bool> ResultPin;
    public OperationType Operation;

    public CompareNode() : base("Compare")
    {
        LhsPin = AddInPin<InPin>("Left");
        RhsPin = AddInPin<InPin>("Right");
        ResultPin = AddOutPin<OutPin<bool>>("Result");
    }

    public override void Evaluate(EvaluationContext ctx)
    {
        // Evaluate Lhs & Rhs
        base.Evaluate(ctx);

        // NOTE: Stack is a stack, values are in reverse order to pins
        var rhs = ctx.Pop();
        var lhs = ctx.Pop();

        if (lhs.Type != rhs.Type)
        {
            // NOTE: Ideally the frontend would prevent this, so the error handling is somewhat minimal (for now)
            ctx.ReportError(this, "Type mismatch");
            ctx.Push(false);
            return;
        }

        bool result = false;

        switch (lhs.Type)
        {
            case Value.ValueType.Number:
                var lhsNum = lhs.AsNumber();
                var rhsNum = rhs.AsNumber();
                result = Operation switch
                {
                    OperationType.Equal => lhsNum == rhsNum,
                    OperationType.NotEqual => lhsNum != rhsNum,
                    OperationType.LessThan => lhsNum < rhsNum,
                    OperationType.LessThanOrEqual => lhsNum <= rhsNum,
                    OperationType.GreaterThan => lhsNum > rhsNum,
                    OperationType.GreaterThanOrEqual => lhsNum >= rhsNum,
                    _ => false,
                };
                break;
            case Value.ValueType.Boolean:
                var lhsBool = lhs.AsBoolean();
                var rhsBool = rhs.AsBoolean();
                result = Operation switch
                {
                    OperationType.Equal => lhsBool == rhsBool,
                    OperationType.NotEqual => lhsBool != rhsBool,
                    OperationType.LessThan => lhsBool && !rhsBool,
                    OperationType.LessThanOrEqual => lhsBool == rhsBool || lhsBool && !rhsBool,
                    OperationType.GreaterThan => !lhsBool && rhsBool,
                    OperationType.GreaterThanOrEqual => lhsBool == rhsBool || !lhsBool && rhsBool,
                    _ => false,
                };
                break;
            case Value.ValueType.String:
                var lhsStr = lhs.AsString();
                var rhsStr = rhs.AsString();
                result = Operation switch
                {
                    OperationType.Equal => lhsStr == rhsStr,
                    OperationType.NotEqual => lhsStr != rhsStr,
                    OperationType.LessThan => lhsStr.CompareTo(rhsStr) < 0,
                    OperationType.LessThanOrEqual => lhsStr.CompareTo(rhsStr) <= 0,
                    OperationType.GreaterThan => lhsStr.CompareTo(rhsStr) > 0,
                    OperationType.GreaterThanOrEqual => lhsStr.CompareTo(rhsStr) >= 0,
                    _ => false,
                };
                break;
        }

        ctx.Push(result);
    }
}
