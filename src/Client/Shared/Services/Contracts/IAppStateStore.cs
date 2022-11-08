namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IAppStateStore
{
    public ViewModeEnum ViewMode { get; set; }
    public bool IsAvailableForTest { get; set; }
    public FsArtifact? CurrentFulaArtifact { get; set; }
    public FsArtifact? CurrentMyDeviceArtifact { get; set; }
    public string CurrentPagePath { get; set; }
}