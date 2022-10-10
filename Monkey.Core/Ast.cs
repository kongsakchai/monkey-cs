namespace Monkey.Core;

public record Node(Token Token)
{
    public virtual string String => this.Token.Literal;
}

public record Program(List<Statement> Statements) : Node(new Token(TokenType.Illegal, ""))
{
    public override string String => string.Join("\n", Statements.Select(s => s.String));
}

#region Expression

public record Expression(Token Token) : Node(Token);

public record Identifier(Token Token) : Expression(Token)
{
    public string Value => Token.Literal;
}

public record NumberLiteral(Token Token, double Value) : Expression(Token);

public record BooleanLiteral(Token Token, bool Value) : Expression(Token);

public record StringLiteral(Token Token) : Expression(Token)
{
    public string Value => Token.Literal;
    public override string String => $"\"{Token.Literal}\"";
}

public record InfixExpression(Token Token, Expression Left, Expression Right) : Expression(Token)
{
    public string Operator => Token.Literal;
    public override string String => $"{Left.String} {Operator} {Right.String}";
}

public record PrefixExpression(Token Token, Expression Right) : Expression(Token)
{
    public string Operator => Token.Literal;
    public override string String => $"{Operator} {Right.String}";
}

public record AssignExpression(Identifier Name, Expression Value) : Expression(new Token(TokenType.Illegal, ""))
{
    public override string String => $"{Name.Value} = {Value.String}";
}

#endregion

#region Statement

public record Statement() : Node(new Token(TokenType.Illegal, ""));

public record ExpressionStatement(Expression Expression) : Statement()
{
    public override string String => $"{Expression.String}";
}

public record LetStatement(Identifier Name, Expression Value) : Statement()
{
    public override string String => $"{Name.Value} = {Value.String}";
}

#endregion