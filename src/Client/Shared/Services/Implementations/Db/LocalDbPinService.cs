using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class LocalDbPinService : ILocalDbPinService
{
    IFxLocalDbService FxLocalDbService { get; set; }

    public LocalDbPinService(IFxLocalDbService fxLocalDbService)
    {
        FxLocalDbService = fxLocalDbService;
    }

    public async Task AddPinAsync(FsArtifact artifact)
    {
        using var LocalDb = FxLocalDbService.CreateConnection();

        var pinnedArtifact = new PinnedArtifact()
        {
            FullPath = artifact.FullPath,
            ProviderType = artifact.ProviderType,
            PinEpochTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ContentHash = artifact.LastModifiedDateTime.ToString(),
            ThumbnailPath = artifact.ThumbnailPath,
            ArtifactName = artifact.Name,
            FsArtifactType = artifact.ArtifactType
        };

        await Task.Run(() => LocalDb.Insert(pinnedArtifact));
    }

    public async Task UpdatePinAsync(PinnedArtifact pinnedArtifact, string? oldPath = null)
    {
        if (oldPath == null)
        {
            var localDb = FxLocalDbService.CreateConnection();

            await Task.Run(() => localDb.Execute(
                $"UPDATE PinnedArtifact SET ThumbnailPath = @ThumbnailPath, ContentHash=@ContentHash WHERE FullPath = @FullPath ",
                new
                {
                    ThumbnailPath = pinnedArtifact.ThumbnailPath,
                    ContentHash = pinnedArtifact.ContentHash,
                    FullPath = pinnedArtifact.FullPath
                }));
        }
        else
        {
            var localDb = FxLocalDbService.CreateConnection();
            await Task.Run(() => localDb.Execute(
                $"UPDATE PinnedArtifact SET FullPath =@FullPath, ThumbnailPath = @ThumbnailPath, ContentHash=@ContentHash WHERE FullPath = @OldPath ",
                new
                {
                    OldPath = oldPath,
                    ThumbnailPath = pinnedArtifact.ThumbnailPath,
                    ContentHash = pinnedArtifact.ContentHash,
                    FullPath = pinnedArtifact.FullPath
                }));
        }
    }

    public async Task RemovePinAsync(String FullPath)
    {
        if (string.IsNullOrEmpty(FullPath)) return;
        using var LocalDb = FxLocalDbService.CreateConnection();

        await Task.Run(() => LocalDb.Execute($"DELETE FROM PinnedArtifact WHERE FullPath = '{FullPath}';"));
    }

    public async Task<List<PinnedArtifact>> GetPinnedArticatInfos()
    {
        using var LocalDb = FxLocalDbService.CreateConnection();

        var list = await Task.Run(() => LocalDb.Query<PinnedArtifact>($"SELECT * FROM PinnedArtifact"));
        return list.ToList();
    }
}
