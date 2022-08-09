using Monkey.Core;

namespace Monkey.Test;

[TestClass]
public class ParseToken
{
    [TestMethod]
    public void TestExpression()
    {
        var source = @"a=10 * b + 2.5 + 3";
        var lexer = new Lexer(source);
        var parse = new Parser(lexer);

        var p = parse.ParseProgram();
    }
}