using Monkey.Core;
// See https://aka.ms/new-console-template for more information
var source = @"
a=10 * b + 2.5 + 3
b=10*b/3
c=""5555""
b=""111""+""1""
10+0+b
";
var lexer = new Lexer(source);
var parse = new Parser(lexer);

var p = parse.ParseProgram();
Console.WriteLine(p.Ast());
