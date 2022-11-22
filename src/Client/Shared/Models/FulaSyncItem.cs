namespace Functionland.FxFiles.Client.Shared.Models;

[Dapper.Contrib.Extensions.Table("FulaSyncItem")]
public class FulaSyncItem
{
    [Key]
    public long Id { get; set; }
    public string FulaPath { get; set; } 
    public string LocalPath { get; set; } 
    public SyncStatus LastSyncStatus { get; set; }
    public RunningStatus RunningStatus { get; set; }
    public FulaSyncType SyncType { get; set; }
    public string DId { get; set; }
}