namespace webapi.Services;

public interface IFeatureServiceOptions
{
    bool IsLonLat { get; init; }
}

public interface IGSOptions : IFeatureServiceOptions
{
    double[]? Angles { get; init; }
}

[Flags]
public enum SatelliteFeatureType
{
    Track = 1,
    IntervalTrack = 2,
    Left = 4,
    Right = 8,
    PsMarker = 16,
    MarkerTrack = 32,
}

[Flags]
public enum PreviewFeatureType
{
    Footprint = 1,
    PreviewTrack = 2,
    PreviewIntervalTrack = 4,
    PreviewSwath = 8,
}

public interface IFeatureService
{
    NodeFeatureMap GetSatelliteFeatures(string satelliteName, SatelliteFeatureType types, int[] nodes, IFeatureServiceOptions? options = null);

    FeatureMap GetGroundStation(string name, IGSOptions? options = null);

    FeatureMap GetGroundTargets(IFeatureServiceOptions? options = null);

    FeatureMap GetGroundTarget(string name, IFeatureServiceOptions? options = null);

    FeatureMap GetPreview(string observationTaskName, PreviewFeatureType types, IFeatureServiceOptions? options = null);
}
