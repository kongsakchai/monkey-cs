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

        Assert.AreEqual("Hello", result.String);
    }
}