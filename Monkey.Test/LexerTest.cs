using Monkey.Core;

namespace Monkey.Test;

[TestClass]
public class LexerTest
{
    [TestMethod]
    public void TestExpression()
    {
        var source = @"ab=10*b+2.5+3
        +-*/%
        ""Hello""
        ""Hi""+""Bye""
        ""Go""""Go""
        true
        false
        True
        _a
        ?
        ";
        var lexer = new Lexer(source);

        var testCase = new List<Token>()
        {
            new Token(TokenType.Ident,"ab"),
            new Token(TokenType.Assign,"="),
            new Token(TokenType.Number,"10"),
            new Token(TokenType.Multiply,"*"),
            new Token(TokenType.Ident,"b"),
            new Token(TokenType.Add,"+"),
            new Token(TokenType.Number,"2.5"),
            new Token(TokenType.Add,"+"),
            new Token(TokenType.Number,"3"),
            ///////////////////////////////
            new Token(TokenType.Add,"+"),
            new Token(TokenType.Sub,"-"),
            new Token(TokenType.Multiply,"*"),
            new Token(TokenType.Divide,"/"),
            new Token(TokenType.Mod,"%"),
            ///////////////////////////////
            new Token(TokenType.String,"Hello"),
            new Token(TokenType.String,"Hi"),
            new Token(TokenType.Add,"+"),
            new Token(TokenType.String,"Bye"),
            new Token(TokenType.String,"Go"),
            new Token(TokenType.String,"Go"),
            ///////////////////////////////
            new Token(TokenType.True,"true"),
            new Token(TokenType.False,"false"),
            new Token(TokenType.Ident,"True"),
            new Token(TokenType.Ident,"_a"),
            new Token(TokenType.Illegal,"?"),
        };

        for (int i = 0; i < testCase.Count; i++)
        {
            var token = lexer.NextToken();
            if (token.Type == TokenType.Eol)
                i--;
            else
                Assert.AreEqual(testCase[i].ToString(), token.ToString());
        }
    }
}