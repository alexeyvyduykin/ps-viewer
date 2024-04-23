using NetTopologySuite.Features;
using Shared.Models;
using webapi.Extensions;
using webapi.Geometries;

namespace webapi.Services;

public partial class FeatureRepository
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Удалите неиспользуемые закрытые члены", Justification = "<Ожидание>")]
    private FeatureCollection CreateFootprints(bool isLonLat = true)
    {
        var fc = new FeatureCollection();

        var res = _dataService.GetPlannedScheduleObject();

        if (res != null)
        {
            var ps = _mapper.Map<PlannedScheduleObject>(res);

            var footprints = ps.GetFootprints()
                .Select(s =>
                {
                    var feature = FeatureBuilder.CreateFootprint(s, isLonLat);

                    feature.Attributes = new AttributesTable
                {
                    { "Feature", "Footprint" },
                    { "Name", s.Name },
                    { "Satellite", s.SatelliteName },
                    { "Node", s.Node },
                    { "Direction", s.Direction.ToString() },
                    { "Target", s.TargetName },
                };

                    //feature.Attributes["Name"] = s.Name;
                    //feature.Attributes["Satellite"] = s.SatelliteName;
                    //feature.Attributes["Node"] = s.Node;
                    //feature.Attributes["Direction"] = s.Direction.ToString();
                    //feature.Attributes["Target"] = s.TargetName;

                    return feature;
                })
                .ToList();

            fc = footprints.ToFeatureCollection();
        }

        return fc;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Удалите неиспользуемые закрытые члены", Justification = "<Ожидание>")]
    private Feature? CreateFootprint(string observationTaskName, bool isLonLat = true)
    {
        Feature? f = null;

        var res = _dataService.GetPlannedScheduleObject();

        if (res != null)
        {
            var ps = _mapper.Map<PlannedScheduleObject>(res);

            var footprint = ps.GetFootprint(observationTaskName);

            if (footprint != null)
            {
                f = FeatureBuilder.CreateFootprint(footprint, isLonLat);

                f.Attributes = new AttributesTable
                {
                    { "Feature", "Footprint" },
                    { "Name", footprint.Name },
                    { "Satellite", footprint.SatelliteName },
                    { "Node", footprint.Node },
                    { "Direction", footprint.Direction.ToString() },
                    { "Target", footprint.TargetName },
                };

                //f.Attributes["Name"] = footprint.Name;
                //f.Attributes["Satellite"] = footprint.SatelliteName;
                //f.Attributes["Node"] = footprint.Node;
                //f.Attributes["Direction"] = footprint.Direction.ToString();
                //f.Attributes["Target"] = footprint.TargetName;
            }
        }

        return f;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Пометьте члены как статические", Justification = "<Ожидание>")]
    private Feature CreateFootprint(Footprint footprint, bool isLonLat = true)
    {
        var f = FeatureBuilder.CreateFootprint(footprint, isLonLat);

        f.Attributes = new AttributesTable
        {
            { "Feature", "Footprint" },
            { "Name", footprint.Name },
            { "Satellite", footprint.SatelliteName },
            { "Node", footprint.Node },
            { "Direction", footprint.Direction.ToString() },
            { "Target", footprint.TargetName }
        };

        //f.Attributes["Name"] = footprint.Name;
        //f.Attributes["Satellite"] = footprint.SatelliteName;
        //f.Attributes["Node"] = footprint.Node;
        //f.Attributes["Direction"] = footprint.Direction.ToString();
        //f.Attributes["Target"] = footprint.TargetName;

        return f;
    }
}
