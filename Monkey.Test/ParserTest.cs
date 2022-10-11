using Monkey.Core;

namespace Monkey.Test;

[TestClass]
public class ParserTest
{
    private Lexer lexer = new Lexer();
    private Parser? parser;

    [TestInitialize]
    public void Setup()
    {
        parser = new(lexer);
    }

    [TestMethod]
    public void TestBasicParser()
    {
        var source = "1+1+\"Hello\"--A+!true*+false";
        lexer.Set(source);
        var program = parser!.ParseProgram();

        Assert.AreEqual("1 + 1 + \"Hello\" - - A + ! true * + false", program.String);
    }

    [TestMethod]
    public void TestGroupParser()
    {
        var source = "(1+2+5)*((10)+5)-(((1)))";
        lexer.Set(source);
        var program = parser!.ParseProgram();

        Assert.AreEqual("1 + 2 + 5 * 10 + 5 - 1", program.String);
    }

    [TestMethod]
    public void TestLetParser()
    {
        var source = "let a = 10+11+12+13";
        lexer.Set(source);
        var program = parser!.ParseProgram();

        Assert.AreEqual("a = 10 + 11 + 12 + 13", program.String);
    }

    [TestMethod]
    public void TestErrorParser()
    {
        var testCase = new List<String>()
        {
            "let a+10+23",
            "let 10=13",
            "a++5",
            "a*+10",
            "a+*10"
        };

        var result = new List<String>()
        {
            "",
            "",
            "a + + 5",
            "a * + 10",
            ""
        };

        for (int i = 0; i < testCase.Count; i++)
        {
            lexer.Set(testCase[i]);
            var program = parser!.ParseProgram();
            Assert.AreEqual(result[i], program.String);
        }
    }

    [TestMethod]
    public void TestIfParser()
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
            "a = 1 if a == 1 { a = a + 1 } else {  }",
            "a = 1 if a == 1 { a = a + 1 } else {  }",
            "a = 1 if a > 5 { a = a + 1 } else { a = a + 10 }",
            "a = 1 if a > 5 { a = a + 1 } else { a = a + 5 }",
            "a = 1 if a > 5 { a = a + 1 } else { if a == 5 { a = a + 5 } else { a = 0 } }"
        };
        for (int i = 0; i < testCase.Count; i++)
        {
            lexer.Set(testCase[i]);
            var program = parser!.ParseProgram();
            Assert.AreEqual(result[i], program.String);
        }
    }
}