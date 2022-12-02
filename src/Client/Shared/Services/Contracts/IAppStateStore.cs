namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IAppStateStore
{
    ViewModeEnum ViewMode { get; set; }
    FileCategoryType? CurrentFileCategoryFilter { get; set; }
    bool IsAvailableForTest { get; set; }
    FsArtifact? CurrentFulaArtifact { get; set; }
    FsArtifact? CurrentMyDeviceArtifact { get; set; }
    string CurrentPagePath { get; set; }
    string? IntentFileUrl { get; set; }
    string? ArtifactListScrollTopValue { get; set; }
}