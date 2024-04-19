using System.CommandLine;
using MongoDB.Bson;
using MongoDB.Driver;
using pscli.Commands;
using pscli.Services;

namespace Pscli;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand(
            "Pscli is a application for creating planned schedule(ps) data."
        );

        rootCommand.AddCommand(Sats.CreateCommand());
        rootCommand.AddCommand(Create.CreateCommand());
        rootCommand.AddCommand(Connect.CreateCommand());
        rootCommand.AddCommand(Push.CreateCommand());
        rootCommand.AddCommand(Info.CreateCommand());

        rootCommand.SetHandler(() =>
        {
            //var connectionString = $"mongodb://root:example@localhost:27017";
            var connectionString = $"mongodb://mongodb";
            var psService = new PsService(connectionString);
        });

        return await rootCommand.InvokeAsync(args);
    }
}
