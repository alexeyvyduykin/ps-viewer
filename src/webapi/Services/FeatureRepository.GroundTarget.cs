using NetTopologySuite.Features;
using webapi.Extensions;
using webapi.Geometries;

namespace webapi.Services;

public partial class FeatureRepository
{
    private FeatureCollection CreateGroundTargets(bool isLonLat = true)
    {
        var ps = _dataService.GetPlannedScheduleObject();

        if (ps != null)
        {
            var gts = ps.GroundTargets.Select(s =>
            {
                var f = FeatureBuilder.CreateGroundTarget(s, isLonLat);

                f.Attributes = new AttributesTable
                {
                    { "Name", s.Name },
                    { "State", "Unselected" },
                    { "Highlight", true },
                    { "Type", s.Type.ToString() }
                };

                //f.Attributes["Name"] = s.Name;
                //f.Attributes["State"] = "Unselected";
                //f.Attributes["Highlight"] = true;
                //f.Attributes["Type"] = s.Type.ToString();

                return f;
            }).ToList();

            return gts.ToFeatureCollection();
        }

        return [];
    }

    private FeatureCollection CreateGroundTarget(string name, bool isLonLat = true)
    {
        var ps = _dataService.GetPlannedScheduleObject();

        if (ps != null)
        {
            var gt = ps.GroundTargets.Where(s => Equals(s.Name, name)).FirstOrDefault();

            if (gt != null)
            {
                var f = FeatureBuilder.CreateGroundTarget(gt, isLonLat);

                f.Attributes = new AttributesTable
                {
                    { "Name", gt.Name },
                    { "State", "Unselected" },
                    { "Highlight", true },
                    { "Type", gt.Type.ToString() }
                };

                //f.Attributes["Name"] = gt.Name;
                //f.Attributes["State"] = "Unselected";
                //f.Attributes["Highlight"] = true;
                //f.Attributes["Type"] = gt.Type.ToString();

                return new[] { f }.ToFeatureCollection();
            }
        }

        return [];
    }
}
