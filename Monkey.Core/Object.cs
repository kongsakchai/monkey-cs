namespace Monkey.Core;

public enum ObjectType
{
    Number,
    String,
    Boolean,
    Null,
    Return,
    Error
}

public interface IObject
{
    public ObjectType Type { get; }
    public string String { get; }
}
public record NumberObject(double Value) : IObject
{
    public ObjectType Type => ObjectType.Number;
    public string String => Value.ToString();
}
public record StringObject(string Value) : IObject
{
    public ObjectType Type => ObjectType.String;
    public string String => Value;
}

public record ReturnObject(IObject Value):IObject
{
    public ObjectType Type => ObjectType.Return;
    public string String => Value.String;
}

public record BooleanObject(bool Value) : IObject
{
    public ObjectType Type => ObjectType.Boolean;
    public string String => Value.ToString();
}
public record NullObject() : IObject
{
    public ObjectType Type => ObjectType.Null;
    public string String => "Null";
}
public record ErrorObject(string Message) : IObject
{
    public ObjectType Type => ObjectType.Error;
    public string String => Message;
}