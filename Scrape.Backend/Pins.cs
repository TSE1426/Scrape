using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Scrape.Backend;

public enum PinType
{
    In,
    Out,
}

public interface IPinFactory<TSelf> where TSelf : Pin
{
    static abstract TSelf Create(string label, Node parent);
}

public abstract class Pin
{
    public string Label { get; }
    public readonly Node Parent;

    protected Pin(string label, Node parent)
    {
        Label = label;
        Parent = parent;
    }

    public abstract PinType GetPinType();
    public bool IsInPin() => GetPinType() == PinType.In;
    public bool IsOutPin() => GetPinType() == PinType.Out;

    public abstract void Connect(Pin pin);
    public abstract void Disconnect(Pin pin);
}

public class InPin(string label, Node parent) : Pin(label, parent), IPinFactory<InPin>
{
    public OutPin? ConnectedPin;
    public override PinType GetPinType() => PinType.In;

    public void Connect(OutPin pin)
    {
        if (ConnectedPin is not null)
            Disconnect(ConnectedPin);
        ConnectedPin = pin;
        pin.ConnectedPins.Add(this);
    }
    public override void Connect(Pin pin)
    {
        Debug.Assert(pin is OutPin);
        Connect((OutPin)pin);
    }

    public void Disconnect(OutPin pin)
    {
        ConnectedPin = null;
        pin.ConnectedPins.Remove(this);
    }
    public override void Disconnect(Pin pin)
    {
        Debug.Assert(pin is OutPin);
        Disconnect((OutPin)pin);
    }

    public static InPin Create(string label, Node parent) => new(label, parent);

    public virtual Value DefaultValue => throw new InvalidOperationException();
}

public class InPin<TValue>(string label, Node parent) : InPin(label, parent), IPinFactory<InPin<TValue>>
{
    public static new InPin<TValue> Create(string label, Node parent) => new(label, parent);

    public override Value DefaultValue => Value.Default<TValue>();
}

public class OutPin(string label, Node parent) : Pin(label, parent), IPinFactory<OutPin>
{
    public readonly HashSet<InPin> ConnectedPins = [];
    public override PinType GetPinType() => PinType.Out;

    public void Connect(InPin pin) => pin.Connect(this);
    public override void Connect(Pin pin)
    {
        Debug.Assert(pin is OutPin);
        Connect((OutPin)pin);
    }

    public void Disconnect(InPin pin) => pin.Disconnect(this);
    public override void Disconnect(Pin pin)
    {
        Debug.Assert(pin is InPin);
        Disconnect((InPin)pin);
    }

    public static OutPin Create(string label, Node parent) => new(label, parent);
}

public class OutPin<TValue>(string label, Node parent) : OutPin(label, parent), IPinFactory<OutPin<TValue>>
{
    public static new OutPin<TValue> Create(string label, Node parent) => new(label, parent);
}

public struct FlowMarker { }

public sealed class InFlowPin(string label, Node parent) : InPin<FlowMarker>(label, parent), IPinFactory<InFlowPin>
{
    static InFlowPin IPinFactory<InFlowPin>.Create(string label, Node parent) => new(label, parent);
}

public sealed class OutFlowPin(string label, Node parent) : OutPin<FlowMarker>(label, parent), IPinFactory<OutFlowPin>
{
    static OutFlowPin IPinFactory<OutFlowPin>.Create(string label, Node parent) => new(label, parent);
}
