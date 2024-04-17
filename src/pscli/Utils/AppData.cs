using NetTopologySuite.IO;
using Newtonsoft.Json;
using Shared.Models;

namespace pscli.Utils;

public static class AppData
{
    private static readonly string PsDataFile =
        Directory.GetCurrentDirectory()
        + Path.DirectorySeparatorChar
        + "temp"
        + Path.DirectorySeparatorChar
        + "PsData.json";

    public static void Save(PlannedScheduleObject psData)
    {
        var serializer = GeoJsonSerializer.Create();

        using (var stringWriter = new StringWriter())
        using (var jsonWriter = new JsonTextWriter(stringWriter))
        {
            serializer.Serialize(jsonWriter, psData);
            var jsonString = stringWriter.ToString();

            File.WriteAllText(PsDataFile, jsonString);
        }
    }
}
