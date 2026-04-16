using System.Collections.Generic;
using System.Diagnostics;

namespace Scrape.Backend;

public sealed class EvaluationContext
{
    public readonly Stack<Value> Stack = [];
    public readonly Dictionary<string, Value> Variables = [];

    public void Push(double value) => Stack.Push(new(value));
    public void Push(bool value) => Stack.Push(new(value));
    public void Push(string value) => Stack.Push(new(value));
    public void Push(Value value) => Stack.Push(value);

    public Value Pop() => Stack.Pop();

    public double PopNumber() => Stack.Pop().AsNumber();
    public bool PopBoolean() => Stack.Pop().AsBoolean();
    public string PopString() => Stack.Pop().AsString();

    public void Set(string identifier, Value value) => Variables.Add(identifier, value);
    public Value Get(string identifier) => Variables[identifier];
}

public sealed class Value
{
    public enum Type
    {
        Number,
        Boolean,
        String,
    }

    private Type _type;
    private object _value;

    public Value(double value)
    {
        _type = Type.Number;
        _value = value;
    }

    public Value(bool value)
    {
        _type = Type.Boolean;
        _value = value;
    }

    public Value(string value)
    {
        _type = Type.String;
        _value = value;
    }

    public double AsNumber()
    {
        Debug.Assert(_type == Type.Number);
        return (double)_value;
    }

    public bool AsBoolean()
    {
        Debug.Assert(_type == Type.Boolean);
        return (bool)_value;
    }

    public string AsString()
    {
        Debug.Assert(_type == Type.String);
        return (string)_value;
    }
}