namespace Functionland.FxFiles.Client.Shared.Models;

public class FsArtifactActivity
{
    public FulaUser? Performer { get; set; }
    public DateTimeOffset? ActionDateTime { get; set; }
    public ActionType? ActionType { get; set; }
    public List<KeyValuePair<string, string>>? Properties { get; set; }
}
