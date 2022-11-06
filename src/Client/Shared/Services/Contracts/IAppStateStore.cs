namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IAppStateStore
{
    public ViewModeEnum ViewMode { get; protected set; }
    public bool IsAvailableForTest { get; protected set; }
    public FsArtifact? CurrentFulaArtifact { get; protected set; }
    public FsArtifact? CurrentMyDeviceArtifact { get; protected set; }

    public void SetViewMode(ViewModeEnum viewMode);
    public void SetAvailableForTest(bool isAvailable);
    public void SetCurrentFulaArtifact(FsArtifact? fulaArtifact);
    public void SetCurrentMyDeviceArtifact(FsArtifact? myDeviceArtifact);
}