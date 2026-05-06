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

    public static Value Default(ValueType type)
    {
        switch (type)
        {
            case ValueType.Number:
                return new(0.0);
            case ValueType.Boolean:
                return new(false);
            case ValueType.String:
                return new("");
        }
        throw new InvalidOperationException();
    }

    public static Value Default<TValue>()
    {
        if (typeof(TValue) == typeof(double))
            return Default(ValueType.Number);
        if (typeof(TValue) == typeof(string))
            return Default(ValueType.String);
        if (typeof(TValue) == typeof(bool))
            return Default(ValueType.Boolean);
        throw new InvalidOperationException();
    }
}
