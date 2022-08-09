namespace Monkey.Core;

public enum TokenType
{
    Illegal, // Unknown token/character
    Eof, //End of files
    Eol, //End of line

    Ident, // ok, a, b, c
    Number, // 123
    String, // "Ok"

    //Operator
    Assign, // =
    Mod, // %
    Add, // Addition +
    Sub, // Subtraction -
    Multiply, //Multiplicative *
    Divide, //Division /
    Sign,// -n

    //Keywords
    True,
    False
}

public class Token
{
    public TokenType Type { get; }

    public string Literal { get; }

    public Token(TokenType type, string literal)
    {
        Type = type;
        Literal = literal;
    }

    public override string ToString() => $"({Type}) {Literal}";
}