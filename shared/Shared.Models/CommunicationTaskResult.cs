namespace Shared.Models;

public enum CommunicationType
{
    Uplink,
    Downlink
}

public class CommunicationTaskResult : BaseTaskResult
{
    public required CommunicationType Type { get; init; }
}
