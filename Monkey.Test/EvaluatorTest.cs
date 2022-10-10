using Monkey.Core;

namespace Monkey.Test;

[TestClass]
public class EvaluatorTest
{
    private MonkeyCode code = new MonkeyCode();

    [TestMethod]
    public void TestBasicEval()
    {
        var source = @"""Hello""+1";
        var result = code.Compile(source);
        Assert.AreEqual("Hello1", result?.String);
    }

    [TestMethod]
    public void TestGroupEval()
    {
        var source = @"(1+2+5)*((10)+5)";
        var result = code.Compile(source);
        Assert.AreEqual("120", result?.String);
    }

    [TestMethod]
    public void TestLetEval()
    {
        var source = @"let a=(10+5)*10";
        var result = code.Compile(source);
        Assert.AreEqual("150", result?.String);
    }
}