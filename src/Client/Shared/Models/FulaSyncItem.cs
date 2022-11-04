﻿namespace Functionland.FxFiles.Client.Shared.Models;

public class FulaSyncItem
{
    public long Id { get; set; }
    public string FulaPath { get; set; }
    public string LocalPath { get; set; }

    //TODO: What is it?
    public SyncStatus SyncStatus { get; set; }

    public FulaSyncType SyncType { get; set; }
}


