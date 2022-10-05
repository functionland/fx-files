namespace Functionland.FxFiles.Client.Shared.Models;

public class FsArtifactActivity
{
    public DIdDocument? Performer { get; set; }
    public DateTimeOffset? ActionDateTime { get; set; }
    public ActionType? ActionType { get; set; }
    public KeyValuePair<string,string>? Properties { get; set; }
}
