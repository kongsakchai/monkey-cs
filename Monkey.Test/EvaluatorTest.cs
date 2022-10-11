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

    [TestMethod]
    public void TestIfEval()
    {
        var testCase = new List<String>()
        {
            @"a=1
            if(a==1)
                a=a+1
            ",
            @"a=1
            if(a==1){
                a=a+1
            }
            "
            ,
            @"a=1
            if(a>5)
                a=a+1
            else
                a=a+10
            ",
            @"a=1
            if(a>5){
                a=a+1
            }else{
                a=a+5
            }
            "
            ,
            @"a=1
            if(a>5)
                a=a+1
            else if(a==5)
                a=a+5
            else
                a=0
            "
        };

        var result = new List<String>()
        {
            "2",
            "2",
            "11",
            "6",
            "0"
        };
        for (int i = 0; i < testCase.Count; i++)
        {
            var _result = code.Compile(testCase[i]);
            Assert.AreEqual(result[i], _result?.String);
        }
    }
}