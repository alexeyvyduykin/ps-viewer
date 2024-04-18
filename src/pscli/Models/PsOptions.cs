﻿using Shared.Models;

namespace pscli.Models;

public class PsOptions
{
    public List<Satellite> Satellites { get; set; } = [];

    public SatelliteTemplate SatelliteTemplate { get; set; } = new();

    public string ConnectionString { get; set; } = "";
}
