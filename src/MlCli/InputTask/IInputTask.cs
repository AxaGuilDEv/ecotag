﻿namespace AxaGuilDEv.MlCli.InputTask;

public interface IInputTask
{
    public string Id { get; }
    public string Type { get; }
    public bool Enabled { get; }
}