namespace Monkey.Core
{

    public interface INode
    {
        public string Literal { get; }
        public string String { get; }
    }

    #region Expression

    public class Expression : INode
    {
        private Token Token;
        public string Literal => Token.Literal;
        public virtual string String => Token.Literal;
        public Expression(Token token) => this.Token = token;
    }

    public class Identifier : Expression
    {
        public string Value { get; }
        public Identifier(Token token, string value) : base(token) => this.Value = value;
    }

    public class NumberLiteral : Expression
    {
        public float Value { get; }
        public NumberLiteral(Token token, float value) : base(token) => this.Value = value;
    }

    public class BooleanLiteral : Expression
    {
        public bool Value { get; }
        public override string String => $"\"{Literal.ToLower()}\"";
        public BooleanLiteral(Token token, bool value) : base(token) => this.Value = value;
    }
    public class StringLiteral : Expression
    {
        public string Value => Literal;
        public override string String => $"\"{Literal}\"";
        public StringLiteral(Token token) : base(token) { }
    }

    public class InfixExpression : Expression
    {
        public Expression Left { get; }
        public string Operator => Literal;
        public Expression Right { get; }
        public override string String => $"{Left.String} {Operator} {Right.String}";
        public InfixExpression(Token token, Expression left, Expression right) : base(token)
        {
            this.Left = left;
            this.Right = right;
        }
    }

    public class PrefixExpression : Expression
    {
        public string Operator => Literal;
        public Expression Right { get; }
        public override string String => $"{Operator} {Right.String}";
        public PrefixExpression(Token token, Expression right) : base(token) => this.Right = right;
    }

    #endregion

    #region Statement

    public class Statement : INode
    {
        private Token Token;
        public string Literal => this.Token.Literal;
        public virtual string String => this.Token.Literal;
        public Statement(Token token) => this.Token = token;
    }

    #endregion

}