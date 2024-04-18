using pscli.Models;
using Shared.Models;

namespace pscli.Utils;

public static class Helper
{
    public static int GetAvailableSatelliteIndex(PsOptions options)
    {
        var names = options.Satellites.Select(s => s.Name).ToList();

        var numbers = new List<int>();

        foreach (var item in names)
        {
            if (item.StartsWith("Satellite"))
            {
                var str = item.Substring("Satellite".Length);

                if (int.TryParse(str, out int number))
                {
                    numbers.Add(number);
                }
            }
        }

        return Enumerable.Range(1, numbers.Count + 1).First(x => !numbers.Contains(x));
    }

    public static void SatellitesInfo(List<Satellite> sats)
    {
        var count = sats.Count;

        if (count == 0)
        {
            Console.WriteLine("Satellite array is empty");
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("{0,-20} {1,12} {2,12} {3, 12} {4, 12} {5, 12} {6, 12} {7, 12} {8, 24} {9, 12} {10, 12}\n",
                "name", "sa, km", "ecc", "incl, deg", "argOfPer, deg", "lan, deg", "raan, deg", "period, sec", "epoch", "look, deg", "radar, deg");

            for (int i = 0; i < count; i++)
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

    // TODO: template default values replace to single place
    public static void SatelliteTemplateInfo(SatelliteTemplate template)
    {
        Console.WriteLine();
        Console.WriteLine("{0,12} {1,12} {2, 12} {3, 12} {4, 24} {5, 12} {6, 12}\n",
            "sa, km", "ecc", "incl, deg", "period, sec", "epoch", "look, deg", "radar, deg");

        var semiaxis2 = template.Semiaxis ?? 6945.03;
        var ecc2 = template.Ecc ?? 0.0;
        var incl2 = template.Incl ?? 97.65;
        var period2 = template.Period ?? 5760.0;
        var epoch2 = template.Epoch ?? Convert.ToDateTime("1 Jul 2007 12:00:00.000");
        var lookAngle2 = template.Look ?? 40.0;
        var radarAngle2 = template.Radar ?? 16.0;

        Console.WriteLine(
            "{0,12:N2} {1,12:N2} {2,12:N2} {3,12:N2} {4,24} {5,12:N2} {6,12:N2}",
            semiaxis2,
            ecc2,
            incl2,
            period2,
            epoch2,
            lookAngle2,
            radarAngle2
        );

        Console.WriteLine();
    }

    public static void ConnectionStringInfo(string connectionString)
    {
        Console.WriteLine();

        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine("MongoDB ConnectionString is empty");
        }
        else
        {
            Console.WriteLine($"{connectionString}");
        }

        Console.WriteLine();
    }

    public static void CurrentPsDataInfo(string? name, int? gts)
    {
        Console.WriteLine();

        if (name is null && gts is null)
        {
            Console.WriteLine("PsData not created");
        }
        else
        {
            Console.WriteLine("{0,-30} {1,12}\n", "name", "gts");
            Console.WriteLine("{0,-30} {1,12}", name, gts);
        }

        Console.WriteLine();
    }
}
