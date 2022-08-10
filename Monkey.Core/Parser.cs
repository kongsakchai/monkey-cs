namespace Monkey.Core;

public class Parser
{
    private enum Precedence
    {
        Lowest,
        Assignment, // =
        OR, // ||
        AND, // &&
        Equality, // == !=
        Relational, // > < >= <=
        Additive, // + -
        Multiplicative, // * / %
        Prefix, // -x !x
    }
    private Lexer _lexer;
    private Token _curToken;
    private Token _peekToken;
    private List<TokenType> _infixFunctions = new List<TokenType>();
    private Dictionary<TokenType, Func<Expression>> _prefixFunctions = new Dictionary<TokenType, Func<Expression>>();
    private Dictionary<TokenType, Precedence> _precedences = new Dictionary<TokenType, Precedence>()
    {
        {TokenType.Multiply,Precedence.Multiplicative},
        {TokenType.Divide,Precedence.Multiplicative},
        {TokenType.Mod,Precedence.Multiplicative},
        {TokenType.Add,Precedence.Additive},
        {TokenType.Sub,Precedence.Additive},
        /////////////////////////////////////////
        {TokenType.Greater,Precedence.Relational},
        {TokenType.GreaterEq,Precedence.Relational},
        {TokenType.Less,Precedence.Relational},
        {TokenType.LessEq,Precedence.Relational},
        /////////////////////////////////////////
        {TokenType.AND,Precedence.AND},
        {TokenType.OR,Precedence.OR},
        {TokenType.Equal,Precedence.Equality},
        {TokenType.Not,Precedence.Equality},
        {TokenType.Assign,Precedence.Assignment},
    };

    private void RegisInfix(TokenType type) => _infixFunctions.Add(type);
    private void RegisPrefix(TokenType type, Func<Expression?> func) => _prefixFunctions.Add(type, func!);

    public Parser(Lexer lexer)
    {
        _lexer = lexer;
        _curToken = _lexer.NextToken();
        _peekToken = _lexer.NextToken();

        RegisInfix(TokenType.Add);
        RegisInfix(TokenType.Sub);
        RegisInfix(TokenType.Multiply);
        RegisInfix(TokenType.Divide);
        RegisInfix(TokenType.Mod);
        RegisInfix(TokenType.Greater);
        RegisInfix(TokenType.GreaterEq);
        RegisInfix(TokenType.Less);
        RegisInfix(TokenType.LessEq);
        RegisInfix(TokenType.Equal);
        RegisInfix(TokenType.NotEq);
        RegisInfix(TokenType.AND);
        RegisInfix(TokenType.OR);
        RegisInfix(TokenType.Assign);

        RegisPrefix(TokenType.Ident, ParseIdentifier);
        RegisPrefix(TokenType.Number, ParseNumberLiteral);
        RegisPrefix(TokenType.String, ParseStringLiteral);
        RegisPrefix(TokenType.True, ParseBooleanLiteral);
        RegisPrefix(TokenType.False, ParseBooleanLiteral);
        RegisPrefix(TokenType.Sub, ParsePrefixExpression);
        RegisPrefix(TokenType.Not, ParsePrefixExpression);
    }

    public Program ParseProgram()
    {
        var statements = new List<Statement>();
        while (!CurTokenIs(TokenType.Eof))
        {
            var s = ParseStatement();
            if (s != null)
                statements.Add(s);
            NextToken();
        }
        return new Program(statements);
    }

    private void NextToken()
    {
        _curToken = _peekToken;
        _peekToken = _lexer.NextToken();
    }

    private Statement? ParseStatement()
    {
        switch (_curToken.Type)
        {
            case TokenType.Eol:
                while (CurTokenIs(TokenType.Eol))
                    NextToken();
                return ParseStatement();
            default:
                return ParseExpressionStatement();
        }
    }

    private ExpressionStatement? ParseExpressionStatement()
    {
        var token = _curToken;
        var expression = ParseExpression(Precedence.Lowest);
        if (expression == null)
            return null;

        if (PeekTokenIs(TokenType.Eol))
            NextToken();

        return new ExpressionStatement(token, expression);
    }

    #region Expression

    private Expression? ParseExpression(Precedence p)
    {
        var token = _curToken;
        var ok = _prefixFunctions.TryGetValue(token.Type, out var prefix);
        if (!ok)
        {
            return null;
        }
        var expression = prefix!();

        while (!PeekTokenIs(TokenType.Eol) && p < GetPrecedence(_peekToken.Type))
        {
            ok = _infixFunctions.Contains(_peekToken.Type);
            if (!ok)
                return expression;
            NextToken();
            expression = ParseInfixExpression(expression);
            if (expression == null)
                return null;
        }

        return expression;
    }

    private InfixExpression? ParseInfixExpression(Expression left)
    {
        var token = _curToken;
        var p = GetPrecedence(_curToken.Type);
        NextToken();
        var right = ParseExpression(p);
        if (right == null)
            return null;

        return new InfixExpression(token, left, right);
    }

    private PrefixExpression? ParsePrefixExpression()
    {
        var token = _curToken;
        NextToken();
        var right = ParseExpression(Precedence.Prefix);
        if (right == null)
            return null;

        return new PrefixExpression(token, right);
    }

    private Identifier ParseIdentifier() => new Identifier(_curToken);
    private NumberLiteral ParseNumberLiteral() => new NumberLiteral(_curToken, float.Parse(_curToken.Literal));
    private BooleanLiteral ParseBooleanLiteral() => new BooleanLiteral(_curToken, CurTokenIs(TokenType.True));
    private StringLiteral ParseStringLiteral() => new StringLiteral(_curToken);

    #endregion

    private Precedence GetPrecedence(TokenType t)
    {
        var ok = _precedences.TryGetValue(t, out var p);
        return ok ? p : Precedence.Lowest;
    }
    private bool CurTokenIs(TokenType t) => _curToken.Type == t;
    private bool PeekTokenIs(TokenType t) => _peekToken.Type == t;
}