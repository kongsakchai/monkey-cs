namespace Monkey.Core;

public class Parser
{
    private enum Precedence
    {
        // Sort By Precedence (Infix)
        Lowest,
        Assignment, // =
        OR, // ||
        AND, // &&
        Equals, // == !=
        Relation, // > < >= <=
        Sum, // + -
        Product, // * / %
        Call, // (
        Prefix, // -x !x +x
    }
    private Lexer _lexer;
    private Token _curToken = new Token(TokenType.Illegal, "");
    private Token _peekToken = new Token(TokenType.Illegal, "");
    private Dictionary<TokenType, Func<Expression, Expression?>> _infixFunctions;
    private Dictionary<TokenType, Func<Expression?>> _prefixFunctions;
    private readonly Dictionary<TokenType, Precedence> _precedences = new Dictionary<TokenType, Precedence>()
    {
        // Precedence for Infix
        {TokenType.Multiply,Precedence.Product},
        {TokenType.Divide,Precedence.Product},
        {TokenType.Mod,Precedence.Product},
        {TokenType.Add,Precedence.Sum},
        {TokenType.Sub,Precedence.Sum},

        {TokenType.Greater,Precedence.Relation},
        {TokenType.GreaterEq,Precedence.Relation},
        {TokenType.Less,Precedence.Relation},
        {TokenType.LessEq,Precedence.Relation},

        {TokenType.AND,Precedence.AND},
        {TokenType.OR,Precedence.OR},

        {TokenType.Equal,Precedence.Equals},
        {TokenType.NotEq,Precedence.Equals},
        {TokenType.Assign,Precedence.Assignment},

        { TokenType.LParen, Precedence.Call }
    };

    private void RegisInfix(TokenType type, Func<Expression, Expression?> func) => _infixFunctions.Add(type, func);
    private void RegisPrefix(TokenType type, Func<Expression?> func) => _prefixFunctions.Add(type, func);

    public Parser(Lexer lexer)
    {
        this._lexer = lexer;

        _infixFunctions = new Dictionary<TokenType, Func<Expression, Expression?>>();
        RegisInfix(TokenType.Add, ParseInfixExpression);
        RegisInfix(TokenType.Sub, ParseInfixExpression);
        RegisInfix(TokenType.Multiply, ParseInfixExpression);
        RegisInfix(TokenType.Divide, ParseInfixExpression);
        RegisInfix(TokenType.Mod, ParseInfixExpression);
        RegisInfix(TokenType.Greater, ParseInfixExpression);
        RegisInfix(TokenType.GreaterEq, ParseInfixExpression);
        RegisInfix(TokenType.Less, ParseInfixExpression);
        RegisInfix(TokenType.LessEq, ParseInfixExpression);
        RegisInfix(TokenType.Equal, ParseInfixExpression);
        RegisInfix(TokenType.NotEq, ParseInfixExpression);
        RegisInfix(TokenType.AND, ParseInfixExpression);
        RegisInfix(TokenType.OR, ParseInfixExpression);
        RegisInfix(TokenType.Assign, ParseAssignExpression);

        _prefixFunctions = new Dictionary<TokenType, Func<Expression?>>();
        RegisPrefix(TokenType.Ident, ParseIdentifier);
        RegisPrefix(TokenType.Number, ParseNumber);
        RegisPrefix(TokenType.String, ParseString);
        RegisPrefix(TokenType.True, ParseBoolean);
        RegisPrefix(TokenType.False, ParseBoolean);
        RegisPrefix(TokenType.Add, ParsePrefixExpression);
        RegisPrefix(TokenType.Sub, ParsePrefixExpression);
        RegisPrefix(TokenType.Not, ParsePrefixExpression);
        RegisPrefix(TokenType.LParen, ParseGroupExpression);
    }

    public Program ParseProgram()
    {
        _curToken = _lexer.NextToken();
        _peekToken = _lexer.NextToken();

        var statements = new List<Statement>();
        while (!CurTokenIs(TokenType.Eof))
        {
            var s = ParseStatement();
            if (s != null)
                statements.Add(s);
            else
                break;
            NextToken();
        }
        return new Program(statements);
    }

    public Program ParseProgram(Lexer lexer)
    {
        this._lexer = lexer;
        return ParseProgram();
    }

    private void NextToken()
    {
        _curToken = _peekToken;
        _peekToken = _lexer!.NextToken();
    }

    private Statement? ParseStatement()
    {
        switch (_curToken.Type)
        {
            case TokenType.Eol:
                while (CurTokenIs(TokenType.Eol)) NextToken();
                return ParseStatement();
            case TokenType.Let:
                return ParseLetStatement();
            case TokenType.Ident:
                return ParseIdentifierStatement();
            default:
                return ParseExpressionStatement();
        }
    }
    private LetStatement? ParseLetStatement()
    {
        if (!ExpectPeek(TokenType.Ident))
            return null;

        var name = ParseIdentifier();

        if (!ExpectPeek(TokenType.Assign))
            return null;

        NextToken();
        var expression = ParseExpression(Precedence.Lowest);
        if (expression == null)
            return null;

        return new LetStatement(name, expression);
    }

    private Statement? ParseIdentifierStatement()
    {
        var name = ParseIdentifier();
        if (!ExpectPeek(TokenType.Assign))
        {
            return ParseExpressionStatement();
        }
        NextToken();
        var expression = ParseExpression(Precedence.Lowest);
        if (expression == null)
            return null;

        return new LetStatement(name, expression);
    }

    private ExpressionStatement? ParseExpressionStatement()
    {
        var expression = ParseExpression(Precedence.Lowest);
        if (expression == null)
            return null;

        if (PeekTokenIs(TokenType.Eol))
            NextToken();

        return new ExpressionStatement(expression);
    }

    // === Expression ===
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
        if (expression == null)
            return null;


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

    private AssignExpression? ParseAssignExpression(Expression ident)
    {
        if (ident is Identifier name)
        {
            NextToken();
            var expression = ParseExpression(Precedence.Lowest);
            if (expression == null)
                return null;

            return new AssignExpression(name, expression);
        }
        return null;
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

    private Expression? ParseGroupExpression()
    {
        NextToken();
        var expression = ParseExpression(Precedence.Lowest);
        return !ExpectPeek(TokenType.RParen) ? null : expression;
    }

    private Identifier ParseIdentifier() => new Identifier(_curToken);
    private NumberLiteral ParseNumber() => new NumberLiteral(_curToken, double.Parse(_curToken.Literal));
    private BooleanLiteral ParseBoolean() => new BooleanLiteral(_curToken, CurTokenIs(TokenType.True));
    private StringLiteral ParseString() => new StringLiteral(_curToken);

    #endregion

    private bool PrefixFunction(TokenType type, out Func<Expression?>? prefix)
    {
        prefix = null;
        switch (type)
        {
            case TokenType.Ident:
                prefix = ParseIdentifier;
                return true;
            case TokenType.Number:
                prefix = ParseNumber;
                return true;
            case TokenType.String:
                prefix = ParseString;
                return true;
            case TokenType.True:
                prefix = ParseBoolean;
                return true;
            case TokenType.False:
                prefix = ParseBoolean;
                return true;
            case TokenType.Add:
            case TokenType.Sub:
            case TokenType.Not:
                prefix = ParsePrefixExpression;
                return true;
            default:
                return false;
        }
    }
    private Precedence GetPrecedence(TokenType t)
    {
        var ok = _precedences.TryGetValue(t, out var p);
        return ok ? p : Precedence.Lowest;
    }
    private bool CurTokenIs(TokenType t) => _curToken.Type == t;
    private bool PeekTokenIs(TokenType t) => _peekToken.Type == t;
    private bool ExpectPeek(TokenType t)
    {
        if (PeekTokenIs(t))
        {
            NextToken();
            return true;
        }
        return false;
    }
}