using Monkey.Core;

namespace Monkey.Test;

[TestClass]
public class LexerTest
{
    [TestMethod]
    public void TestBasic()
    {
        var source = @"abc
        x
        y
        2.5
        3
        ""Hello""
        ""Go""""Go""
        true
        false
        True
        ?
        ";
        var lexer = new Lexer(source);

        var testCase = new List<Token>()
        {
            new Token(TokenType.Ident,"abc"),
            new Token(TokenType.Ident,"x"),
            new Token(TokenType.Ident,"y"),
            new Token(TokenType.Number,"2.5"),
            new Token(TokenType.Number,"3"),
            
            new Token(TokenType.String,"Hello"),
            new Token(TokenType.String,"Go"),
            new Token(TokenType.String,"Go"),
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
    public void TestOperator()
    {
        var source = @"+-*/%=!_#@$";
        var lexer = new Lexer(source);

        var testCase = new List<Token>()
        {
            new Token(TokenType.Add,"+"),
            new Token(TokenType.Sub,"-"),
            new Token(TokenType.Multiply,"*"),
            new Token(TokenType.Divide,"/"),
            new Token(TokenType.Mod,"%"),
            new Token(TokenType.Assign,"="),
            new Token(TokenType.Not,"!"),
            new Token(TokenType.Illegal,"_"),
            new Token(TokenType.Illegal,"#"),
            new Token(TokenType.Illegal,"@"),
            new Token(TokenType.Illegal,"$"),
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
    public void TestLogicOperator()
    {
        var source = @"> >= < <= == != && ||
        >&>
        <|<
        =!";
        var lexer = new Lexer(source);

        var testCase = new List<Token>()
        {
            new Token(TokenType.Greater,">"),
            new Token(TokenType.GreaterEq,">="),
            new Token(TokenType.Less,"<"),
            new Token(TokenType.LessEq,"<="),
            new Token(TokenType.Equal,"=="),
            new Token(TokenType.NotEq,"!="),
            new Token(TokenType.AND,"&&"),
            new Token(TokenType.OR,"||"),

            new Token(TokenType.Greater,">"),
            new Token(TokenType.Illegal,"&"),
            new Token(TokenType.Greater,">"),
            new Token(TokenType.Less,"<"),
            new Token(TokenType.Illegal,"|"),
            new Token(TokenType.Less,"<"),
            new Token(TokenType.Assign,"="),
            new Token(TokenType.Not,"!"),
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
    public void TestDelimitersLexer()
    {
        var source = @"(1+2+5)+((10)+5)-(((1)))";
        var lexer = new Lexer(source);

        var testCase = new List<Token>()
        {
            new Token(TokenType.LParen,"("),
            new Token(TokenType.Number,"1"),
            new Token(TokenType.Add,"+"),
            new Token(TokenType.Number,"2"),
            new Token(TokenType.Add,"+"),
            new Token(TokenType.Number,"5"),
            new Token(TokenType.RParen,")"),
            new Token(TokenType.Add,"+"),
            new Token(TokenType.LParen,"("),
            new Token(TokenType.LParen,"("),
            new Token(TokenType.Number,"10"),
            new Token(TokenType.RParen,")"),
            new Token(TokenType.Add,"+"),
            new Token(TokenType.Number,"5"),
            new Token(TokenType.RParen,")"),
            new Token(TokenType.Sub,"-"),
            new Token(TokenType.LParen,"("),
            new Token(TokenType.LParen,"("),
            new Token(TokenType.LParen,"("),
            new Token(TokenType.Number,"1"),
            new Token(TokenType.RParen,")"),
            new Token(TokenType.RParen,")"),
            new Token(TokenType.RParen,")"),
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