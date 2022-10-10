using Monkey.Core;

namespace Monkey.Test;

[TestClass]
public class ParserTest
{
    [TestMethod]
    public void TestBasic()
    {
        var source = "1+1+\"Hello\"--A+!true*+false";
        var lexer = new Lexer(source);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        Assert.AreEqual("1 + 1 + \"Hello\" - - A + ! true * + false", program.String);
    }

    [TestMethod]
    public void TestGroupParser()
    {
        var source = "(1+2+5)*((10)+5)-(((1)))";
        var lexer = new Lexer(source);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        Assert.AreEqual("1 + 2 + 5 * 10 + 5 - 1", program.String);
    }
}