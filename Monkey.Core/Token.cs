namespace Monkey.Core;

public enum TokenType
{
    Illegal, // Unknown token/character
    Eof, //End of files

    Ident, // ok, a, b, c
    Number, // 123
    String, // "Ok"

    //Operator
    Assign, // =
    MOD, // %
    PLUS, // +
    MINUS, // -
    MULTIPLY, // *
    DIVIDE, // /
    SIGN,// -n

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