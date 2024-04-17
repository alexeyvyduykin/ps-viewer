using pscli.Models;
using pscli.Utils;
using Shared.Models;
using System.CommandLine;
using System.CommandLine.Binding;

namespace pscli.Commands;

public static class Sats
{
    public static Command CreateCommand()
    {
        var nameOption = new Option<string?>(name: "--name", description: "Satellite name.");

        var semiaxisOption = new Option<double?>(name: "--sa", description: "Satellite semiaxis.");

        var eccOption = new Option<double?>(name: "--ecc", description: "Satellite eccentricity.");

        var inclOption = new Option<double?>(name: "--incl", description: "Satellite inclination.");

        var lanOption = new Option<double?>(name: "--lan", description: "Satellite longitude ascending node.");

        var periodOption = new Option<double?>(name: "--period", description: "Satellite period.");

        var epochOption = new Option<DateTime?>(name: "--epoch", description: "Satellite epoch.");

        var lookOption = new Option<double?>(name: "--look", description: "Satellite look angle.");

        var radarOption = new Option<double?>(name: "--radar", description: "Satellite radar angle.");

        var countOption = new Option<int>(
            name: "--count",
            description: "Count generating satellites."
        )
        {
            IsRequired = true
        };

        var command = new Command("sats", "???");

        var clearCommand = new Command("clear", "???");
        var addCommand = new Command("add", "???") { nameOption, semiaxisOption, eccOption, inclOption, lanOption, periodOption, epochOption, lookOption, radarOption };
        var generateCommand = new Command("generate", "???") { countOption };
        var infoCommand = new Command("info", "???");

        var templateCommand = new Command("template", "???") { semiaxisOption, eccOption, inclOption, periodOption, epochOption, lookOption, radarOption };
        generateCommand.AddCommand(templateCommand);

        command.AddCommand(clearCommand);
        command.AddCommand(addCommand);
        command.AddCommand(generateCommand);
        command.AddCommand(infoCommand);

        clearCommand.SetHandler(Clear);
        addCommand.SetHandler(Add, new SatelliteBinder(nameOption, semiaxisOption, eccOption, inclOption, lanOption, periodOption, epochOption, lookOption, radarOption));
        generateCommand.SetHandler(Generate, countOption);
        templateCommand.SetHandler(Template, new SatelliteTemplateBinder(semiaxisOption, eccOption, inclOption, periodOption, epochOption, lookOption, radarOption));
        infoCommand.SetHandler(Info);

        return command;
    }

    public static void Clear()
    {
        AppOptions.Update(options =>
        {
            options.Satellites.Clear();
        });

        Console.WriteLine("Clear");
    }

    public static void Add(SatelliteOptions satOptions)
    {
        var satName = "";

        var semiaxis2 = satOptions.Semiaxis ?? 6945.03;
        var ecc2 = satOptions.Ecc ?? 0.0;
        var incl2 = satOptions.Incl ?? 97.65;
        var lan2 = satOptions.Lan ?? 0.0;
        var period2 = satOptions.Period ?? 5760.0;
        var epoch2 = satOptions.Epoch ?? Convert.ToDateTime("1 Jul 2007 12:00:00.000");
        var lookAngle2 = satOptions.Look ?? 40.0;
        var radarAngle2 = satOptions.Radar ?? 16.0;

        AppOptions.Update(options =>
        {
            var name2 = satOptions.Name ?? $"Satellite{Helper.GetAvailableSatelliteIndex(options)}";

            var sat = Factory.CreateSatellite(name2, semiaxis2, ecc2, incl2, lan2, period2, epoch2, lookAngle2, radarAngle2);

            satName = sat.Name;

            options.Satellites.Add(sat);
        });

        Console.WriteLine($"Add {satName}.");
    }

    public static void Generate(int count)
    {
        AppOptions.Update(options =>
        {
            var template = options.SatelliteTemplate;

            var list = new List<Satellite>();

            var d = 360.0 / count;

            var semiaxis2 = template.Semiaxis ?? 6945.03;
            var ecc2 = template.Ecc ?? 0.0;
            var incl2 = template.Incl ?? 97.65;
            var period2 = template.Period ?? 5760.0;
            var epoch2 = template.Epoch ?? Convert.ToDateTime("1 Jul 2007 12:00:00.000");
            var lookAngle2 = template.Look ?? 40.0;
            var radarAngle2 = template.Radar ?? 16.0;

            for (int i = 0; i < count; i++)
            {
                var lan2 = d * i;
                var sat = Factory.CreateSatellite($"Satellite{i + 1}", semiaxis2, ecc2, incl2, lan2, period2, epoch2, lookAngle2, radarAngle2);
                list.Add(sat);
            }

            options.Satellites = list;
        });

        Console.WriteLine("Generate");
    }

    public static void Info()
    {
        var psOptions = AppOptions.Load();

        var count = psOptions.Satellites.Count;

        if (count == 0)
        {
            Console.WriteLine("Satellite array is empty");
        }
        else
        {
            var sats = psOptions.Satellites;

            Console.WriteLine();
            Console.WriteLine("{0,-20} {1,12} {2,12} {3, 12} {4, 12} {5, 12} {6, 12} {7, 12} {8, 24} {9, 12} {10, 12}\n",
                "name", "sa, km", "ecc", "incl, deg", "argOfPer, deg", "lan, deg", "raan, deg", "period, sec", "epoch", "look, deg", "radar, deg");

            for (int i = 0; i < sats.Count; i++)
            {
                Console.WriteLine(
                    "{0,-20} {1,12:N2} {2,12:N2} {3,12:N2} {4,12:N2} {5,12:N2} {6,12:N2} {7,12:N2} {8,24} {9,12:N2} {10,12:N2}",
                    sats[i].Name,
                    sats[i].Semiaxis,
                    sats[i].Eccentricity,
                    sats[i].InclinationDeg,
                    sats[i].ArgumentOfPerigeeDeg,
                    sats[i].LongitudeAscendingNodeDeg,
                    sats[i].RightAscensionAscendingNodeDeg,
                    sats[i].Period,
                    sats[i].Epoch,
                    sats[i].LookAngleDeg,
                    sats[i].RadarAngleDeg
                );
            }

            Console.WriteLine();
        }
    }

    public static void Template(SatelliteTemplate template)
    {
        AppOptions.Update(options =>
        {
            options.SatelliteTemplate = template;
        });
    }
}

public class SatelliteOptions
{
    public string? Name { get; set; }

    public double? Semiaxis { get; set; }

    public double? Ecc { get; set; }

    public double? Incl { get; set; }

    public double? Lan { get; set; }

    public double? Period { get; set; }

    public DateTime? Epoch { get; set; }

    public double? Look { get; set; }

    public double? Radar { get; set; }
}

public class SatelliteBinder : BinderBase<SatelliteOptions>
{
    private readonly Option<string?> _nameOption;
    private readonly Option<double?> _semiaxisOption;
    private readonly Option<double?> _eccOption;
    private readonly Option<double?> _inclOption;
    private readonly Option<double?> _lanOption;
    private readonly Option<double?> _periodOption;
    private readonly Option<DateTime?> _epochOption;
    private readonly Option<double?> _lookOption;
    private readonly Option<double?> _radarOption;

    public SatelliteBinder(Option<string?> nameOption, Option<double?> semiaxisOption, Option<double?> eccOption, Option<double?> inclOption, Option<double?> lanOption, Option<double?> periodOption, Option<DateTime?> epochOption, Option<double?> lookOption, Option<double?> radarOption)
    {
        _nameOption = nameOption;
        _semiaxisOption = semiaxisOption;
        _eccOption = eccOption;
        _inclOption = inclOption;
        _lanOption = lanOption;
        _periodOption = periodOption;
        _epochOption = epochOption;
        _lookOption = lookOption;
        _radarOption = radarOption;
    }

    protected override SatelliteOptions GetBoundValue(BindingContext bindingContext) =>
        new SatelliteOptions
        {
            Name = bindingContext.ParseResult.GetValueForOption(_nameOption),
            Semiaxis = bindingContext.ParseResult.GetValueForOption(_semiaxisOption),
            Ecc = bindingContext.ParseResult.GetValueForOption(_eccOption),
            Incl = bindingContext.ParseResult.GetValueForOption(_inclOption),
            Lan = bindingContext.ParseResult.GetValueForOption(_lanOption),
            Period = bindingContext.ParseResult.GetValueForOption(_periodOption),
            Epoch = bindingContext.ParseResult.GetValueForOption(_epochOption),
            Look = bindingContext.ParseResult.GetValueForOption(_lookOption),
            Radar = bindingContext.ParseResult.GetValueForOption(_radarOption),
        };
}

public class SatelliteTemplateBinder : BinderBase<SatelliteTemplate>
{
    private readonly Option<double?> _semiaxisOption;
    private readonly Option<double?> _eccOption;
    private readonly Option<double?> _inclOption;
    private readonly Option<double?> _periodOption;
    private readonly Option<DateTime?> _epochOption;
    private readonly Option<double?> _lookOption;
    private readonly Option<double?> _radarOption;

    public SatelliteTemplateBinder(Option<double?> semiaxisOption, Option<double?> eccOption, Option<double?> inclOption, Option<double?> periodOption, Option<DateTime?> epochOption, Option<double?> lookOption, Option<double?> radarOption)
    {
        _semiaxisOption = semiaxisOption;
        _eccOption = eccOption;
        _inclOption = inclOption;
        _periodOption = periodOption;
        _epochOption = epochOption;
        _lookOption = lookOption;
        _radarOption = radarOption;
    }

    protected override SatelliteTemplate GetBoundValue(BindingContext bindingContext) =>
        new()
        {
            Semiaxis = bindingContext.ParseResult.GetValueForOption(_semiaxisOption),
            Ecc = bindingContext.ParseResult.GetValueForOption(_eccOption),
            Incl = bindingContext.ParseResult.GetValueForOption(_inclOption),
            Period = bindingContext.ParseResult.GetValueForOption(_periodOption),
            Epoch = bindingContext.ParseResult.GetValueForOption(_epochOption),
            Look = bindingContext.ParseResult.GetValueForOption(_lookOption),
            Radar = bindingContext.ParseResult.GetValueForOption(_radarOption),
        };
}