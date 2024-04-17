using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pscli.Models;

public class PsOptions
{
    public bool Dirty { get; set; }
    public List<Satellite> Satellites { get; set; } = [];
    public SatelliteTemplate SatelliteTemplate { get; set; } = new();
}
