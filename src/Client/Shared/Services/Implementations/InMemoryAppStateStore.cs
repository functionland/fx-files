namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class InMemoryAppStateStore : IAppStateStore
{
    public ViewModeEnum ViewMode { get; set; }
    public FileCategoryType? CurrentFileCategoryFilter { get; set; }
    public bool IsAvailableForTest { get; set; }
    public FsArtifact? CurrentFulaArtifact { get; set; }
    public FsArtifact? CurrentMyDeviceArtifact { get; set; }
    public string CurrentPagePath { get; set; } = "/";
    public string? IntentFileUrl { get; set; }
    public string? ArtifactListScrollTopValue { get; set; }
    public string? IntentType { get; set; }
}