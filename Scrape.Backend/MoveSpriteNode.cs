namespace Scrape.Backend;

// Moves a sprite by (dx, dy). The actual movement happens in the frontend via the
// OnMoveSprite callback on EvaluationContext.
public sealed class MoveSpriteNode : Node
{
    public string SpriteName;
    public readonly InPin DxPin;
    public readonly InPin DyPin;
    public readonly InFlowPin InFlowPin;
    public readonly OutFlowPin OutFlowPin;

    public MoveSpriteNode(string spriteName) : base("Move Sprite")
    {
        SpriteName = spriteName;
        DxPin = AddInPin<InPin>("Dx");
        DyPin = AddInPin<InPin>("Dy");
        InFlowPin = AddInPin<InFlowPin>("In");
        OutFlowPin = AddOutPin<OutFlowPin>("Out");
    }

    public override void Evaluate(EvaluationContext ctx)
    {
        // Evaluate Dx and Dy
        base.Evaluate(ctx);

        // Stack is reverse order
        var dy = ctx.PopNumber();
        var dx = ctx.PopNumber();

        ctx.OnMoveSprite?.Invoke(SpriteName, dx, dy);

        PinHelper.ContinueFlow(ctx, OutFlowPin);
    }
}
