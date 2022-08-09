namespace Monkey.Core;

public class Parser
{
    private Lexer _lexer;
    private Token _curToken;
    private Token _peekToken;
    private Dictionary<TokenType, Func<Expression>> infixFunction;
    private Dictionary<TokenType, Func<Expression>> prefixFunction;

    private void RegisInfix(TokenType type, Func<Expression> func) => infixFunction.Add(type, func);
    private void RegisPrefix(TokenType type, Func<Expression> func) => prefixFunction.Add(type, func);

    public Parser(Lexer lexer)
    {
        _lexer = lexer;
    }



}