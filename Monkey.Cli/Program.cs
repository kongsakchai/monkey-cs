using Monkey.Core;

string? line;

do
{
    Console.Write("> ");
    line = Console.ReadLine();

    if (line != null && line != "")
    {
        var lexer = new Lexer(line);
        var parser = new Parser(lexer);

        var result = Evaluator.Eval(parser.ParseProgram());
        Console.WriteLine(result.String);
    }

} while (line != null);