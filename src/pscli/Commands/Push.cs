using pscli.Services;
using pscli.Utils;
using System.CommandLine;

namespace pscli.Commands;

public static class Push
{
    public static Command CreateCommand()
    {
        var command = new Command("push", "???");

        command.SetHandler(PushAsync);

        return command;
    }

    private static async Task PushAsync()
    {
        var options = AppOptions.Load();

        if (string.IsNullOrEmpty(options.ConnectionString))
        {
            Console.WriteLine("Database ConnectString is empty.");
            return;
        }

        var psService = new PsService(options.ConnectionString);

        var psData = AppData.Load();

        if (psData is { })
        {
            await psService.AddAsync(psData);
        }
    }
}
