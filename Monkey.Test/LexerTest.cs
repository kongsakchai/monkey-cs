using Monkey.Core;

namespace Monkey.Test;

[TestClass]
public class LexerTest
{
    private Lexer lexer = new Lexer();

    [TestMethod]
    public void TestBasicLexer()
    {
        var source = @"abc cba
        x
        y
        2.5
        3
        ""Hello""
        ""Go""""Go""
        let
        Let
        true
        false
        True
        ?
        ";

        lexer.Set(source);
        var testCase = new List<Token>()
        {
            new Token(TokenType.Ident,"abc"),
            new Token(TokenType.Ident,"cba"),
            new Token(TokenType.Ident,"x"),
            new Token(TokenType.Ident,"y"),
            new Token(TokenType.Number,"2.5"),
            new Token(TokenType.Number,"3"),

            new Token(TokenType.String,"Hello"),
            new Token(TokenType.String,"Go"),
            new Token(TokenType.String,"Go"),
            new Token(TokenType.Let,"let"),
            new Token(TokenType.Ident,"Let"),
            new Token(TokenType.True,"true"),
            new Token(TokenType.False,"false"),
            new Token(TokenType.Ident,"True"),
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
    
    [TestMethod]
    public void TestOperatorLexer()
    {
        var source = @"+-*/()%><>=<===!==?&&||&|";

        lexer.Set(source);
        var testCase = new List<Token>()
        {
            new Token(TokenType.Add,"+"),
            new Token(TokenType.Sub,"-"),
            new Token(TokenType.Multiply,"*"),
            new Token(TokenType.Divide,"/"),
            new Token(TokenType.LParen,"("),
            new Token(TokenType.RParen,")"),
            new Token(TokenType.Mod,"%"),
            new Token(TokenType.Greater,">"),
            new Token(TokenType.Less,"<"),
            new Token(TokenType.GreaterEq,">="),
            new Token(TokenType.LessEq,"<="),
            new Token(TokenType.Equal,"=="),
            new Token(TokenType.NotEq,"!="),
            new Token(TokenType.Assign,"="),
            new Token(TokenType.Illegal,"?"),
            new Token(TokenType.AND,"&&"),
            new Token(TokenType.OR,"||"),
            new Token(TokenType.Illegal,"&"),
            new Token(TokenType.Illegal,"|"),
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