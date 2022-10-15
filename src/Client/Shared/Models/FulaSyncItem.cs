namespace Functionland.FxFiles.Client.Shared.Models;

[Dapper.Contrib.Extensions.Table("FulaSyncItem")]
public class FulaSyncItem
{
    [Key]
    public long Id { get; set; }
    public string FulaPath { get; set; } 
    public string LocalPath { get; set; } 

    //TODO: What is it?
    public SyncStatus LastSyncStatus { get; set; }

    public FulaSyncType SyncType { get; set; }

    public string UserToken { get; set; }
}


