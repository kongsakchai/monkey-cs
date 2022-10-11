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
    Divide, //Division
    Not, // !

    //Logic operator
    AND, // &&
    OR, // ||
    Equal, // ==
    NotEq, // !=
    Greater, // >
    GreaterEq, // >=
    Less, // <
    LessEq, // <=

    // Delimiters
    LParen, // (
    RParen, // )
    LBrace, // {
    RBrace, // }

    //Keywords
    True,
    False,
    Let,
    Null,
    If,
    Else
}

public record Token(TokenType Type,string Literal);