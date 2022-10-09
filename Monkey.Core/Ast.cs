namespace Monkey.Core;

public record INode(Token Token);

public record Program(List<Statement> Statements) : INode(new Token(TokenType.Illegal, ""));

#region Expression

public record Expression(Token Token) : INode(Token)
{
    public string Literal => Token.Literal;
    public virtual string String => Token.Literal;
}

public record Identifier(Token Token) : Expression(Token)
{
    public string Value => Literal;
}

public record NumberLiteral(Token Token, float Value) : Expression(Token);

public record BooleanLiteral(Token Token, bool Value) : Expression(Token);

public record StringLiteral(Token Token) : Expression(Token)
{
    public string Value => Literal;
    public override string String => $"\"{Literal}\"";
}

public record InfixExpression(Token Token, Expression Left, Expression Right) : Expression(Token)
{
    public string Operator => Literal;
    public override string String => $"{Left.String} {Operator} {Right.String}";
}

public record PrefixExpression(Token Token, Expression Right) : Expression(Token)
{
    public string Operator => Literal;
    public override string String => $"{Operator} {Right.String}";
}

#endregion

#region Statement

public record Statement(Token Token) : INode(Token)
{
    public string Literal => this.Token.Literal;
    public virtual string String => this.Token.Literal;
}

public record ExpressionStatement(Token Token, Expression Expression) : Statement(Token)
{
    public override string String => $"{Expression.String}";
}

#endregion