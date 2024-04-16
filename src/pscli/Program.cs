using System.CommandLine;

namespace Pscli;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Pscli is a application for creating planned schedule(ps) data.");
   
        rootCommand.SetHandler(() => Console.WriteLine("Hello, World!"));

        return await rootCommand.InvokeAsync(args);
    }
}
