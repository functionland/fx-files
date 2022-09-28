namespace Functionland.FxFiles.Client.Shared.Models;

public class BloxSyncItem
{
    public long Id { get; set; }
    public string BloxPath { get; set; }
    public string LocalPath { get; set; }
    public SyncStatus SyncStatus { get; set; }
}


