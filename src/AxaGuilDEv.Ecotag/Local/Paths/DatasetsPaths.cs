﻿using Ml.Cli.PathManager;

namespace Ml.Cli.WebApp.Local.Paths;

public class DatasetsPaths
{
    public DatasetsPaths(string paths)
    {
        Paths = PathAdapter.AdaptPathForCurrentOs(paths);
    }

    public string Paths { get; }
}