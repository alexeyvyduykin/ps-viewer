using pscli.Models;
using System.Text.Json;

namespace pscli.Utils;

public static class AppOptions
{
    private static readonly string PsOptionsFile =
        Directory.GetCurrentDirectory()
        + Path.DirectorySeparatorChar
        + "temp"
        + Path.DirectorySeparatorChar
        + "PsOptions.json";

    private static void Init()
    {
        if (File.Exists(PsOptionsFile) == false)
        {
            Console.WriteLine("Options file not exist, creating...");
            
            var path = Path.GetDirectoryName(PsOptionsFile)!;
            _ = Directory.CreateDirectory(path);

            var res = JsonSerializer.Serialize(new PsOptions());

            // TODO: create 'temp' folder
            File.WriteAllText(PsOptionsFile, res);
        }
        else
        {
            Console.WriteLine("Options file exist");
        }
    }

    public static PsOptions Load()
    {
        Init();

        string jsonString = File.ReadAllText(PsOptionsFile);
        return JsonSerializer.Deserialize<PsOptions>(jsonString)!;
    }

    public static void Save(PsOptions options)
    {
        var jsonString = JsonSerializer.Serialize(options);

        File.WriteAllText(PsOptionsFile, jsonString);
    }

    public static void Update(Action<PsOptions> action)
    {
        var psOptions = Load();

        action.Invoke(psOptions);

        Save(psOptions);
    }
}
