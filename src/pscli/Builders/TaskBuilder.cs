using Shared.Models;

namespace pscli.Builders;

public static class TaskBuilder
{
    public static async Task<List<ObservationTask>> CreateObservationTaskAsync(IList<GroundTarget> groundTargets)
        => await Task.Run(() => CreateObservationTask(groundTargets));

    public static async Task<List<CommunicationTask>> CreateCommunicationTaskAsync(IList<GroundStation> groundStations)
        => await Task.Run(() => CreateCommunicationTask(groundStations));

    public static List<ObservationTask> CreateObservationTask(IList<GroundTarget> groundTargets)
    {
        var list = new List<ObservationTask>();

        foreach (var (gt, i) in groundTargets.Select((s, index) => (s, index)))
        {
            list.Add(new ObservationTask()
            {
                Name = $"ObservationTask{(i + 1):0000}",
                GroundTargetName = gt.Name!
            });
        }

        return list;
    }

    public static List<CommunicationTask> CreateCommunicationTask(IList<GroundStation> groundStations)
    {
        var list = new List<CommunicationTask>();

        foreach (var (gs, i) in groundStations.Select((s, index) => (s, index)))
        {
            list.Add(new CommunicationTask()
            {
                Name = $"CommunicationTask{(i + 1):0000}",
                GroundStationName = gs.Name!
            });
        }

        return list;
    }
}
