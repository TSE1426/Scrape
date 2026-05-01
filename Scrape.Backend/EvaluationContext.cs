using System.Collections.Generic;

namespace Scrape.Backend;

public sealed class EvaluationContext
{
    public readonly Stack<Value> Stack = [];
    public readonly Dictionary<string, Value> Variables = [];
    public readonly List<(Node Node, string Message)> Errors = [];

    public void Push(double value) => Stack.Push(new(value));
    public void Push(bool value) => Stack.Push(new(value));
    public void Push(string value) => Stack.Push(new(value));
    public void Push(Value value) => Stack.Push(value);

    public Value Pop() => Stack.Pop();

    public double PopNumber() => Stack.Pop().AsNumber();
    public bool PopBoolean() => Stack.Pop().AsBoolean();
    public string PopString() => Stack.Pop().AsString();

    public void Set(string identifier, Value value) => Variables[identifier] = value;
    public Value Get(string identifier) => Variables[identifier];

    public void ReportError(Node node, string msg) => Errors.Add((node, msg));
}
