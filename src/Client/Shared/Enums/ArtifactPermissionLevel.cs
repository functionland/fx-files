namespace Functionland.FxFiles.Client.Shared.Enums;

[Flags]
public enum ArtifactPermissionLevel
{
    None = 0,
    Read = 1,
    Write = 2,
    Delegate = 4,
}
