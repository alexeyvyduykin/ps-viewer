using pscli.Builders;
using pscli.Utils;
using System.CommandLine;

namespace pscli.Commands;

public static class Create
{
    public static Command CreateCommand()
    {
        var nameOption = new Option<string>(name: "--name", description: "Satellite name.")
        {
            IsRequired = true
        };

        var gtsOption = new Option<int>(name: "--gts", description: "Count ground targets.")
        {
            IsRequired = true
        };

        var nodesOption = new Option<int?>(name: "--nodes", description: "Count nodes.");


        var command = new Command("create", "???") { nameOption, gtsOption, nodesOption };

        command.SetHandler(CreateAsync, nameOption, gtsOption, nodesOption);

        return command;
    }

    // TODO: make psData on nodes (now only 1 day)
    private static async Task CreateAsync(string name, int gtsCount, int? nodes)
    {
        var psOptions = AppOptions.Load();

        var sats = psOptions.Satellites;

        if (sats.Count == 0)
        {
            Console.WriteLine("Satellites array is empty.");
            return;
        }

        var gts = await GroundTargetBuilder.CreateAsync(gtsCount);
        var satellites = sats;
        var gss = await GroundStationBuilder.CreateDefaultAsync();

        var psData = await PlannedScheduleBuilder.CreateAsync(name, satellites, gts, gss);

        AppData.Save(psData);

        Console.WriteLine($"{name} created.");
    }
}
