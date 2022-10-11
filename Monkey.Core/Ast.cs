namespace Monkey.Core;

public record Node(Token Token)
{
    public virtual string String => this.Token.Literal;
}

public record Program(List<Statement> Statements) : Node(new Token(TokenType.Illegal, ""))
{
    public override string String => string.Join(" ", Statements.Select(s => s.String));
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

public record Statement(Token Token) : Node(Token);

public record ExpressionStatement(Expression Expression) : Statement(new Token(TokenType.Null, "Statement"))
{
    public override string String => $"{Expression.String}";
}

public record LetStatement(Identifier Name, Expression Value) : Statement(new Token(TokenType.Let, "let"))
{
    public override string String => $"{Name.Value} = {Value.String}";
}

public record BlockStatement(List<Statement> Statements) : Statement(new Token(TokenType.LBrace, "{"))
{
    public override string String => string.Join(" ", Statements.Select(s => s.String));
}

public record IfStatement(Expression Condition, Statement Consequence, Statement? Alternative) : Statement(new Token(TokenType.If, "If"))
{
    public override string String => $"if {Condition.String} {{ {Consequence.String} }} else {{ {Alternative?.String} }}";
}

#endregion