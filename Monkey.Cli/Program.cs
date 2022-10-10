using Monkey.Core;

public class Program
{
    public static void Main(string[] arg)
    {
        string? line;
        var code = new MonkeyCode();

        Console.WriteLine("===========================");
        Console.WriteLine("      Monkey Language");
        Console.WriteLine("===========================\n");
        
        do
        {
            Console.Write("> ");
            line = Console.ReadLine();

            if (line != null && line != "")
            {
                var result = code.Compile(line);
                Console.WriteLine(result?.String);
            }

        } while (line != null);
    }
}