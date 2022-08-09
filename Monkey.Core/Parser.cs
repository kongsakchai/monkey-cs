namespace Monkey.Core;

public class Parser
{
    private enum Precedence
    {
        Lowest,
        Assignment,
        Additive, // + -
        Multiplicative // * / %
    }
    private Lexer _lexer;
    private Token _curToken;
    private Token _peekToken;
    private Dictionary<TokenType, Func<Expression, Expression>> _infixFunctions = new Dictionary<TokenType, Func<Expression, Expression>>();
    private Dictionary<TokenType, Func<Expression>> _prefixFunctions = new Dictionary<TokenType, Func<Expression>>();
    private Dictionary<TokenType, Precedence> _precedences = new Dictionary<TokenType, Precedence>()
    {
        {TokenType.Multiply,Precedence.Multiplicative},
        {TokenType.Divide,Precedence.Multiplicative},
        {TokenType.Mod,Precedence.Multiplicative},
        {TokenType.Add,Precedence.Additive},
        {TokenType.Sub,Precedence.Additive},
        {TokenType.Assign,Precedence.Assignment}
    };

    private void RegisInfix(TokenType type, Func<Expression, Expression?> func) => _infixFunctions.Add(type, func);
    private void RegisPrefix(TokenType type, Func<Expression> func) => _prefixFunctions.Add(type, func);

    public Parser(Lexer lexer)
    {
        _lexer = lexer;
        _curToken = _lexer.NextToken();
        _peekToken = _lexer.NextToken();

        RegisInfix(TokenType.Add, ParseInfixExpression);
        RegisInfix(TokenType.Sub, ParseInfixExpression);
        RegisInfix(TokenType.Multiply, ParseInfixExpression);
        RegisInfix(TokenType.Divide, ParseInfixExpression);
        RegisInfix(TokenType.Mod, ParseInfixExpression);
        RegisInfix(TokenType.Assign, ParseInfixExpression);

        RegisPrefix(TokenType.Ident, ParseIdentifier);
        RegisPrefix(TokenType.Number, ParseNumberLiteral);
        RegisPrefix(TokenType.String, ParseStringLiteral);
        RegisPrefix(TokenType.True, ParseBooleanLiteral);
        RegisPrefix(TokenType.False, ParseBooleanLiteral);
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
            ok = _infixFunctions.TryGetValue(_peekToken.Type, out var infix);
            if (!ok)
                return expression;
            NextToken();
            expression = infix!(expression);
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