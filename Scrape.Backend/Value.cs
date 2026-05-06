using System;
using System.Diagnostics;

namespace Scrape.Backend;

public readonly struct Value
{
    public enum ValueType
    {
        Number,
        Boolean,
        String,
    }

    public readonly ValueType Type;
    private readonly object _value;

    public Value(double value)
    {
        Type = ValueType.Number;
        _value = value;
    }

    public Value(bool value)
    {
        Type = ValueType.Boolean;
        _value = value;
    }

    public Value(string value)
    {
        Type = ValueType.String;
        _value = value;
    }

    public double AsNumber()
    {
        Debug.Assert(Type == ValueType.Number);
        return (double)_value;
    }

    public bool AsBoolean()
    {
        Debug.Assert(Type == ValueType.Boolean);
        return (bool)_value;
    }

    public string AsString()
    {
        Debug.Assert(Type == ValueType.String);
        return (string)_value;
    }

    public static Value Default<TValue>()
    {
        if (typeof(TValue) == typeof(double))
            return new(0.0);
        if (typeof(TValue) == typeof(string))
            return new("");
        if (typeof(TValue) == typeof(bool))
            return new(false);
        throw new InvalidOperationException();
    }
}
