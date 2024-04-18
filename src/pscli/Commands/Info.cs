using pscli.Utils;
using System.CommandLine;

namespace pscli.Commands;

public static class Info
{
    public static Command CreateCommand()
    {
        var command = new Command("info", "???");

        command.SetHandler(InfoImpl);

        return command;
    }

    public static void InfoImpl()
    {
        var psOptions = AppOptions.Load();

        Console.WriteLine("Satellites:");
        Helper.SatellitesInfo(psOptions.Satellites);

        Console.WriteLine("Satellite template:");
        Helper.SatelliteTemplateInfo(psOptions.SatelliteTemplate);

        Console.WriteLine("ConnectionString:");
        Helper.ConnectionStringInfo(psOptions.ConnectionString);

        Console.WriteLine("Current PsData:");
        Helper.CurrentPsDataInfo(psOptions.CurrentPsDataName, psOptions.CurrentGts);
    }
}
