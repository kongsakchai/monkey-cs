using Monkey.Core;

namespace Monkey.Test;

[TestClass]
public class EvaluatorTest
{
    [TestMethod]
    public void TestBasic()
    {
        var source = @"""Hello""+1";
        var lexer = new Lexer(source);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();
        var result = Evaluator.Eval(program);

        Assert.AreEqual("Hello1", result.String);
    }

    [TestMethod]
    public void TestGroupEval()
    {
        var source = @"(1+2+5)*((10)+5)";
        var lexer = new Lexer(source);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();
        var result = Evaluator.Eval(program);
        
        Assert.AreEqual("120", result.String);
    }
}