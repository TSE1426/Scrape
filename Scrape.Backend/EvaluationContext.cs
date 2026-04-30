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

    // Needed to change how to add to dictionary, variables.Add crashes if you alreay have a value set
    // I want to increment i in a loop, which I think is important so this needs to be changed
    // This way can create and override the values of a variable by using identifier as a key
    public void Set(string identifier, Value value) => Variables[identifier] = value;
    public Value Get(string identifier) => Variables[identifier];

    public void ReportError(Node node, string msg) => Errors.Add((node, msg));
}
