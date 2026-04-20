using System.Collections.Generic;
using System.Diagnostics;

namespace Scrape.Backend;

public abstract class Node(string label)
{
    public string Label { get; } = label;
    public readonly List<InPin> InPins = []; // These two are mainly for the sake of the frontend
    public readonly List<OutPin> OutPins = [];
    public double X, Y;

    public TPin AddInPin<TPin>(string label) where TPin : InPin, IPinFactory<TPin> => AddInPin(TPin.Create(label, this));
    public TPin AddInPin<TPin>(TPin pin) where TPin : InPin
    {
        Debug.Assert(pin.IsInPin());
        InPins.Add(pin);
        return pin;
    }

    public TPin AddOutPin<TPin>(string label) where TPin : OutPin, IPinFactory<TPin> => AddOutPin(TPin.Create(label, this));
    public TPin AddOutPin<TPin>(TPin pin) where TPin : OutPin
    {
        Debug.Assert(pin.IsOutPin());
        OutPins.Add(pin);
        return pin;
    }

    public virtual void Evaluate(EvaluationContext ctx)
    {
        foreach (var pin in InPins)
        {
            Debug.Assert(pin.IsInPin());
            // We certainly don't need to enter an infinite loop :P
            if (pin is InFlowPin) continue;
            // This will recursively call backwards and we should end up with the required inputs on the stack
            pin.ConnectedPin?.Parent.Evaluate(ctx);
        }
    }
}
