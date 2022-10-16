using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Utils;
using System.Linq;
using System.Security.AccessControl;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public partial class FulaSyncService : IFulaSyncService
{
    [AutoInject] IFulaFileClient FulaFileClient { get; set; }

    [AutoInject] IIdentityService IdentityService { get; set; }

    [AutoInject] ILocalDbArtifactService LocalDbArtifactService { get; set; }

    [AutoInject] ILocalDbFulaSyncItemService LocalDbFulaSyncItemService { get; set; }

    [AutoInject] ILocalDeviceFileService LocalDeviceFileService { get; set; }

    static string CurrentToken { get; set; } = default!;

    public Task EnsureInitializedAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public async Task InitAsync(CancellationToken? cancellationToken = null)
    {     
        while (true)
        {
            CurrentToken = await GetCurrentUserTokenAsync();

            try
            {
                await SyncItemsAsync(cancellationToken);
            }
            catch { }

            await Task.Delay(TimeSpan.FromMinutes(1));
        }
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
        var syncItems = await GetSyncItemsAsync(CurrentToken);

        if (!syncItems.Any())
        {
            var fulaSyncItem = new FulaSyncItem()
            {
                LocalPath = GetLocalRootPath(),
                FulaPath = FulaConvention.FulaRootPath,
                LastSyncStatus = SyncStatus.Inprogress,
                SyncType = FulaSyncType.FullSync,
                UserToken = CurrentToken
            };

            await LocalDbFulaSyncItemService.CreateSyncItemAsync(fulaSyncItem, CurrentToken);
            syncItems.Add(fulaSyncItem);
        }

        var updatedArtifacts = new List<FsArtifact>();
        var syncedArtifactList = new List<FsArtifact>();

        foreach (var item in syncItems)
        {
            await LocalDbFulaSyncItemService.UpdateSyncItemAsync(item, SyncStatus.Inprogress, CurrentToken);
            var token = await GetCurrentUserTokenAsync();

            if (token != CurrentToken)
            {
                CurrentToken = token;
                break;
            }
            
            try
            {
                syncedArtifactList = await SyncAsync(item);
                await LocalDbFulaSyncItemService.UpdateSyncItemAsync(item, SyncStatus.Success, CurrentToken);
            }
            catch (Exception)
            {
                await LocalDbFulaSyncItemService.UpdateSyncItemAsync(item, SyncStatus.Fail, CurrentToken);
            }

            updatedArtifacts.AddRange(syncedArtifactList);       
        }

        return updatedArtifacts;
    }

    private async Task<List<FsArtifact>> SyncAsync(FulaSyncItem syncItem)
    {
        if (syncItem.SyncType == FulaSyncType.FullSync)
        {
            var fulaArtifact = await FulaFileClient.GetArtifactAsync(CurrentToken, syncItem.FulaPath);
            return await SyncByFulaArtifactAsync(fulaArtifact, syncItem.LocalPath);
        }
        else if (syncItem.SyncType == FulaSyncType.LocalToFulaJustAdd)
        {
            List<FsArtifact> addedArtifacts = new();
            return await SyncLocalToFulaJustAddAsync(syncItem.LocalPath, syncItem.FulaPath, addedArtifacts);
        }
        else
        {
            throw new InvalidOperationException($"Invalid FulaSyncItem.SyncType: {syncItem.SyncType}");
        }
    }

    private static string GetLocalPathBasedOnFulaPath(string rootLocalPath, string rootFulaPath, string fulaPath)
    {
        // syncItem.FulaPath: fula://Documents
        // syncItem.LocalPath: C:\Users\Mehran\Fula\Documents
        // fulaArtifact.FullPath: fula://Documents/Telegram
        // fula://Documents/Telegram    -----> C:\Users\Mehran\Fula\Documents\Telegram

        return Path.Combine(rootLocalPath, fulaPath.Replace(rootFulaPath, string.Empty).TrimStart(Path.DirectorySeparatorChar));
    }   

    private Task RemoveArtifactAsync(FsArtifact toRemoveArtifact)
    {
        return LocalDbArtifactService.RemoveArtifactAsync(toRemoveArtifact.LocalFullPath, CurrentToken);
    }

    private async Task<List<FsArtifact>> SyncByFulaArtifactAsync(FsArtifact fulaArtifact, string localPath)
    {
        var localArtifact = await LocalDbArtifactService.GetArtifactAsync(localPath, CurrentToken);

        if (localArtifact is null)
        {
            List<FsArtifact> addedArtifacts = new();
            return await CreateLocalArtifactByFulaArtifactAsync(fulaArtifact, localPath, addedArtifacts);
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

        var token = await GetCurrentUserTokenAsync();
        if (CurrentToken != token) return updatedArtifacts;

        if (fulaArtifact.ArtifactType is FsArtifactType.Folder or FsArtifactType.Drive)
        {
            var fulaChildArtifacts = await FulaFileClient.GetChildrenArtifactsAsync(CurrentToken, fulaArtifact.FullPath).ToListAsync();

            foreach (var childFulaArtifact in fulaChildArtifacts)
            {
                var localPath = GetLocalPathBasedOnFulaPath(localArtifact.LocalFullPath, fulaArtifact.FullPath, childFulaArtifact.FullPath);
                var syncedArtifacts = await SyncByFulaArtifactAsync(childFulaArtifact, localPath);
                updatedArtifacts.AddRange(syncedArtifacts);
            }

            var localArtifacts = await LocalDbArtifactService.GetChildrenArtifactsAsync(localArtifact.FullPath, CurrentToken);
            var toRemoveArtifacts = localArtifacts
                .Where(local => !fulaChildArtifacts.Any(fula => local.FullPath == fula.FullPath));

            foreach (var toRemoveArtifact in toRemoveArtifacts)
            {
                if (toRemoveArtifact.ArtifactUploadStatus == ArtifactUploadStatus.Uploaded)
                {
                    await RemoveArtifactAsync(toRemoveArtifact);
                    updatedArtifacts.Add(toRemoveArtifact);
                }
            }

            await LocalDbArtifactService.UpdateFolderAsync(fulaArtifact, localArtifact.LocalFullPath, CurrentToken);
            updatedArtifacts.Add(localArtifact);
        }
        else if (fulaArtifact.ArtifactType is FsArtifactType.File)
        {
            await LocalDbArtifactService.UpdateFileAsync(fulaArtifact, localArtifact.LocalFullPath, CurrentToken);
            updatedArtifacts.Add(localArtifact);
        }
        else
        {
            throw new InvalidOperationException($"ArtifactType not supported for sync: {fulaArtifact.ArtifactType}");
        }

        // TODO: What is this? Why we should update contenthash here again?

        //localArtifact.ContentHash = fulaArtifact.ContentHash;
        // Save in Db

        return updatedArtifacts;
    }

    private async Task<List<FsArtifact>> CreateLocalArtifactByFulaArtifactAsync(FsArtifact fulaArtifact, string localPath, List<FsArtifact> addedArtifacts)
    {
        var artifact = await LocalDbArtifactService.GetArtifactAsync(localPath, CurrentToken);

        var token = await GetCurrentUserTokenAsync();
        if (token != CurrentToken) return addedArtifacts;

        if (artifact is not null)
        {
            addedArtifacts.AddRange(await UpdateLocalArtifactByFulaArtifactAsync(fulaArtifact, artifact));
        }
        else
        {
            if (fulaArtifact.ArtifactType is FsArtifactType.Folder or FsArtifactType.Drive)
            {
                var fulaChildArtifacts = await FulaFileClient.GetChildrenArtifactsAsync(CurrentToken, fulaArtifact.FullPath).ToListAsync();

                foreach (var childFulaArtifact in fulaChildArtifacts)
                {
                    var childLocalPathBasedOnFulaPath = GetLocalPathBasedOnFulaPath(localPath, fulaArtifact.FullPath, childFulaArtifact.FullPath);
                    await CreateLocalArtifactByFulaArtifactAsync(childFulaArtifact, childLocalPathBasedOnFulaPath, addedArtifacts);
                }

                var localArtifact = 
                    await LocalDbArtifactService.CreateArtifactAsync(fulaArtifact, ArtifactUploadStatus.Uploaded, localPath, CurrentToken);
                addedArtifacts.Add(localArtifact);
            }
            else if (fulaArtifact.ArtifactType is FsArtifactType.File)
            {
                var localArtifact = 
                    await LocalDbArtifactService.CreateArtifactAsync(fulaArtifact, ArtifactUploadStatus.Uploaded, localPath, CurrentToken);
                addedArtifacts.Add(localArtifact);
            }
            else
            {
                throw new InvalidOperationException($"ArtifactType not supported for sync: {fulaArtifact.ArtifactType}");
            }
        }

        return addedArtifacts;
    }

    private async Task<List<FulaSyncItem>> GetSyncItemsAsync(string userToken)
    {
        return await LocalDbFulaSyncItemService.GetSyncItemsAsync(userToken);
    }

    private async Task<string> GetCurrentUserTokenAsync()
    {
        // TODO: How to get token?

        return "token,01";
    }
    
    private async Task<List<FsArtifact>> SyncLocalToFulaJustAddAsync(string localRootPath, string fulaRootPath, List<FsArtifact> addedArtifacts)
    {
        var token = await GetCurrentUserTokenAsync();
        if (CurrentToken != token) return addedArtifacts;

        var localChildArtifacts = await LocalDeviceFileService.GetArtifactsAsync(localRootPath).ToListAsync();
        var fulaChildArtifacts = await FulaFileClient.GetChildrenArtifactsAsync(fulaRootPath).ToListAsync();

        var fulaChildDic = fulaChildArtifacts.ToDictionary(f => f.FullPath, f => f);

        foreach (var localChild in localChildArtifacts)
        {
            var fulaPathBasedOnLocalPath = GetFulaPathBasedOnLocalPath(localChild.FullPath, localRootPath, fulaRootPath);           

            if (localChild.ArtifactType is FsArtifactType.Folder or FsArtifactType.Drive)
            {
                var fulaChild = fulaChildDic[fulaPathBasedOnLocalPath];

                if (!fulaChildDic.ContainsKey(fulaPathBasedOnLocalPath))
                {
                    var childLocalPath = GetLocalPathBasedOnFulaPath(GetLocalRootPath(), GetFulaRootPath(), fulaChild.FullPath);
                    var artifact = 
                        await LocalDbArtifactService.CreateArtifactAsync(localChild, ArtifactUploadStatus.PendingToUpload, childLocalPath, CurrentToken);
                    addedArtifacts.Add(artifact);
                }

                await SyncLocalToFulaJustAddAsync(localChild.FullPath, fulaPathBasedOnLocalPath, addedArtifacts);
            }
            else if (localChild.ArtifactType == FsArtifactType.File)
            {
                var fulaChild = fulaChildDic[fulaPathBasedOnLocalPath];
                
                if (fulaChildDic.ContainsKey(fulaPathBasedOnLocalPath))
                {
                    if (localChild.LastModifiedDateTime > fulaChild.LastModifiedDateTime)
                    {
                        // ToDo : what about time differences?
                        var name = localChild.Name + "_" + fulaChild.LastModifiedDateTime; 
                        localChild.Name = name;

                        // TODO : Check this localDbArtifact properties content.
                        var childLocalPath = GetLocalPathBasedOnFulaPath(GetLocalRootPath(), GetFulaRootPath(), fulaChild.FullPath);
                        var artifact = 
                            await LocalDbArtifactService.CreateArtifactAsync(localChild, ArtifactUploadStatus.PendingToUpload, childLocalPath, CurrentToken);
                        addedArtifacts.Add(artifact);
                    }
                }
                else
                {
                    var childLocalPath = GetLocalPathBasedOnFulaPath(GetLocalRootPath(), GetFulaRootPath(), fulaChild.FullPath);
                    var artifact = 
                        await LocalDbArtifactService.CreateArtifactAsync(localChild, ArtifactUploadStatus.PendingToUpload, childLocalPath, CurrentToken);
                    addedArtifacts.Add(artifact);
                }
            }
            else
            {
                throw new InvalidOperationException($"ArtifactType not supported for sync: {localChild.ArtifactType}");
            }
        }

        return addedArtifacts;
    }

    private string GetFulaPathBasedOnLocalPath(string childPath, string rootLocalPath, string rootFulaPath)
    {
        return Path.Combine(rootFulaPath, childPath.Replace(rootLocalPath, string.Empty));
    }

    private string GetLocalRootPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
    }

    private string GetFulaRootPath()
    {
        return "/";
    }
}
