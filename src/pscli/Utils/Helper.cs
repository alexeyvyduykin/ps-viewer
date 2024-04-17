using pscli.Models;

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
}
