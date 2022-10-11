namespace Monkey.Core;

public class MonkeyCode
{
    private Parser parser;
    private Lexer lexer;
    private Environment env;

    public MonkeyCode()
    {
        lexer = new Lexer();
        parser = new Parser(lexer);
        env = new Environment();
    }

    public IObject? Compile(string source)
    {
        lexer.Set(source);
        var program = parser.ParseProgram();
        if (program == null)
            return new ErrorObject("Not found program");
        return Evaluator.Eval(program, env);
    }

}