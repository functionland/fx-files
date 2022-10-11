using Functionland.FxFiles.Client.Shared.Services.FulaClient.Contracts;
using Functionland.FxFiles.Client.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public partial class FulaSyncService : IFulaSyncService
{
    [AutoInject]
    IFulaFileClient FulaFileClient { get; set; }

    [AutoInject]
    IIdentityService IIdentityService { get; set; }

    [AutoInject]
    ILocalDbArtifactService LocalDbArtifactService { get; set; }

    public Task EnsureInitializedAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task InitAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task SyncContentAsync(FsArtifact artifact, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<FsArtifact>> SyncContentsAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public async Task<List<FsArtifact>> SyncItemsAsync(CancellationToken? cancellationToken = null)
    {
        var syncItems = await GetSyncItemsAsync();
        
        var updatedArtifacts = new List<FsArtifact>();
        foreach(var item in syncItems)
        {
            var list = await SyncAsync(item);
            updatedArtifacts.AddRange(list);
        }
        
        return updatedArtifacts;
    }

    private async Task<List<FsArtifact>> SyncAsync(FulaSyncItem syncItem) 
    {
        var token = await GetCurrentUserTokenAsync();

        if (syncItem.SyncType == FulaSyncType.FullSync)
        {
            var fulaArtifact = await FulaFileClient.GetArtifactAsync(token, syncItem.FulaPath);
            return await SyncByFulaArtifactAsync(fulaArtifact, syncItem.LocalPath);
        }
        else
        {
            throw new InvalidOperationException($"Invalid FulaSyncItem.SyncType: {syncItem.SyncType}");
        }
    }

    private string GetLocalPathBasedOnFulaPath(string rootLocalPath, string rootFulaPath, string fulaPath)
    {
        // syncItem.FulaPath: fula://Documents
        // syncItem.LocalPath: C:\Users\Mehran\Fula\Documents
        // fulaArtifact.FullPath: fula://Documents/Telegram
        // fula://Documents/Telegram    -----> C:\Users\Mehran\Fula\Documents\Telegram
        return Path.Combine(rootLocalPath, fulaPath.Replace(rootFulaPath, string.Empty));
    }

    private Task<List<FsArtifact>> RemoveArtifactAsync(FsArtifact toRemoveArtifact)
    {
        throw new NotImplementedException();
    }

    private async Task<List<FsArtifact>> SyncByFulaArtifactAsync(FsArtifact fulaArtifact, string localPath)
    {
        var localArtifact = await LocalDbArtifactService.GetArtifactAsync(localPath);
        if (localArtifact is null)
        {
            return await CreateLocalArtifactByFulaArtifactAsync(fulaArtifact, localPath);
        }
        else
        {
            return await UpdateLocalArtifactByFulaArtifactAsync(fulaArtifact, localArtifact);
        }
    }

    private async Task<List<FsArtifact>> UpdateLocalArtifactByFulaArtifactAsync(FsArtifact fulaArtifact, FsArtifact localArtifact)
    {
        var updatedArtifacts = new List<FsArtifact>();
        if (fulaArtifact.ContentHash == localArtifact.ContentHash)
            return updatedArtifacts;

        if (fulaArtifact.ArtifactType is FsArtifactType.Folder or FsArtifactType.Drive)
        {
            var token = await GetCurrentUserTokenAsync();
            var fulaChildArtifacts = await FulaFileClient.GetChildrenArtifactsAsync(token, fulaArtifact.FullPath).ToListAsync();

            foreach(var childFulaArtifact in fulaChildArtifacts)
            {
                var localPath = GetLocalPathBasedOnFulaPath(localArtifact.FullPath, fulaArtifact.FullPath, childFulaArtifact.FullPath);
                var syncedArtifacts = await SyncByFulaArtifactAsync(childFulaArtifact, localPath);
                updatedArtifacts.AddRange(syncedArtifacts);
            }

            var localArtifacts = await LocalDbArtifactService.GetChildrenArtifactsAsync(localArtifact.FullPath);
            var toRemoveArtifacts = localArtifacts
                .Where(local => !fulaChildArtifacts.Any(fula => local.FullPath == fula.FullPath));

            foreach (var toRemoveArtifact in toRemoveArtifacts)
            {
                var removedArtifacts = await RemoveArtifactAsync(toRemoveArtifact);
                updatedArtifacts.AddRange(removedArtifacts);
            }

            // Todo copy new info to local and save to db.
        }
        else if (fulaArtifact.ArtifactType is FsArtifactType.File)
        {
            // Todo copy new info to local and save to db.
        }
        else
        {
            throw new InvalidOperationException($"ArtifactType not supported for sync: {fulaArtifact.ArtifactType}");
        }
        
        localArtifact.ContentHash = fulaArtifact.ContentHash;
        // Save in Db

        return updatedArtifacts;
    }

    private Task<List<FsArtifact>> CreateLocalArtifactByFulaArtifactAsync(FsArtifact fulaArtifact, string localPath)
    {
        throw new NotImplementedException();
    }

    private async Task<List<FulaSyncItem>> GetSyncItemsAsync()
    {
        throw new NotImplementedException();
    }

    private async Task<string> GetCurrentUserTokenAsync()
    {
        throw new NotImplementedException();
    }
}
