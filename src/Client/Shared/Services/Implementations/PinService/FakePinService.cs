﻿using Functionland.FxFiles.Client.Shared.Extensions;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakePinService : ILocalDevicePinService, IFulaPinService
{
    private List<FsArtifact> _pinnedArtifacts { get; } = new();
    private List<FsArtifact> _allArtifacts { get; } = new();
    public TimeSpan? ActionLatency { get; set; }
    public TimeSpan? EnumerationLatency { get; set; }
    public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;
    public FakePinService(IEnumerable<FsArtifact>? allArtifacts = null,
                          IEnumerable<FsArtifact>? pinnedArtifacts = null,
                          TimeSpan? actionLatency = null,
                          TimeSpan? enumerationLatency = null)
    {
        _pinnedArtifacts.Clear();
        _allArtifacts.Clear();
        ActionLatency = actionLatency ?? TimeSpan.FromSeconds(2);
        EnumerationLatency = enumerationLatency ?? TimeSpan.FromMilliseconds(10);

        if (pinnedArtifacts is not null)
        {
            foreach (var artifact in pinnedArtifacts)
            {
                _pinnedArtifacts.Add(artifact);
            }
        }
        else
        {
            _pinnedArtifacts = new List<FsArtifact>();
        }

        if (allArtifacts is not null)
        {
            foreach (var artifact in allArtifacts)
            {
                _allArtifacts.Add(artifact);
            }
        }
        else
        {
            _allArtifacts = new List<FsArtifact>();
        }
    }

    public Task InitializeAsync(CancellationToken? cancellationToken = null)
    {
        return Task.CompletedTask;
    }

    public Task EnsureInitializedAsync(CancellationToken? cancellationToken = null)
    {
        return Task.CompletedTask;
    }

    public async Task<List<FsArtifact>> GetPinnedArtifactsAsync(CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();

        return _pinnedArtifacts.ToList();
    }

    public async Task<bool> IsPinnedAsync(FsArtifact artifact, CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();

        return _pinnedArtifacts.Any(a => a.FullPath == artifact.FullPath);
    }

    public async Task SetArtifactsPinAsync(IEnumerable<FsArtifact> artifact, CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();

        var lowerCaseArtifact = AppStrings.Artifact.ToLowerFirstChar();

        foreach (var item in artifact)
        {
            var pinnedItem = _pinnedArtifacts?.FirstOrDefault(a => a.FullPath == item.FullPath);

            if (pinnedItem is not null)
                throw new ArtifactAlreadyPinnedException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyPinnedException, pinnedItem?.ArtifactType.ToString() ?? lowerCaseArtifact));

            _pinnedArtifacts?.Add(item);
        }
    }

    public async Task SetArtifactsUnPinAsync(IEnumerable<string> path, CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();

        var lowerCaseArtifact = AppStrings.Artifact.ToLowerFirstChar();

        if (!_pinnedArtifacts.Any())
            throw new ArtifactAlreadyDoseNotPinnedException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyDoseNotPinnedException));

        foreach (var itemPath in path)
        {
            var artifact = _pinnedArtifacts.FirstOrDefault(a => a.FullPath == itemPath);

            if (artifact is not null)
            {
                _pinnedArtifacts.Remove(artifact);
            }
        }
    }
    public async Task LatencyActionAsync()
    {
        if (ActionLatency is not null)
            await Task.Delay(ActionLatency.Value);
    }
}
