namespace Functionland.FxFiles.Client.Shared.Models;

public class ArtifactUserPermission
{
    public ArtifactUserPermission(FulaUser fulaUser)
    {
        FulaUser = fulaUser;
    }
    public FulaUser FulaUser { get; set; }
    public ArtifactPermissionLevel PermissionLevel { get; set; }
}
