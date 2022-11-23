using Dapper;
using Dapper.Contrib.Extensions;
using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Extensions;
using Functionland.FxFiles.Client.Shared.Models;
using System.Xml.Linq;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class LocalDbArtifactService : ILocalDbArtifactService
{
    // TODO: Check all of the input properties and the properties that will modify through the query. Like their names, types, or if that property is needed or not.

    IFxLocalDbService FxLocalDbService { get; set; }

    public LocalDbArtifactService(IFxLocalDbService fxLocalDbService)
    {
        FxLocalDbService = fxLocalDbService;
    }

    public async Task<List<FsArtifact>> GetChildrenArtifactsAsync(string localPath, string userToken)
    {
        using var LocalDb = FxLocalDbService.CreateConnection();

        var artifacts = await LocalDb.QueryAsync<FsArtifactTable>(
            $"SELECT * FROM FsArtifactTable WHERE LocalFullPath = '{localPath}' AND DId = '{userToken}'");

        var artifactList = new List<FsArtifact>();

        foreach(var artifact in artifacts)
        {
            artifactList.Add(new FsArtifact(artifact.FullPath, artifact.Name, artifact.ArtifactType, artifact.ProviderType)
            {
                Capacity = artifact.Capacity,
                ContentHash = artifact.ContentHash,
                CreateDateTime = artifact.CreateDateTime,
                FileExtension = artifact.FileExtension,
                LastModifiedDateTime = artifact.LastModifiedDateTime,
                LocalFullPath = artifact.LocalFullPath,
                ParentFullPath = artifact.ParentFullPath,
                OriginDevice = artifact.OriginDevice,
                Size = artifact.Size,
                ThumbnailPath = artifact.ThumbnailPath,
                WhoMadeLastEdit = artifact.WhoMadeLastEdit,
                PersistenceStatus = artifact.PersistenceStatus,
                DId = artifact.DId,
                IsAvailableOfflineRequested = artifact.IsAvailableOfflineRequested,
                IsSharedWithMe = artifact.IsSharedWithMe,
                IsSharedByMe = artifact.IsSharedByMe
            });
        }

        return artifactList;
    }

    public async Task<FsArtifact> GetArtifactAsync(string localPath, string userToken)
    {
        using var LocalDb = FxLocalDbService.CreateConnection();

        var artifact = await LocalDb.QuerySingleAsync<FsArtifactTable>(
            $"SELECT * FROM FsArtifactTable WHERE LocalFullPath = '{localPath}' AND DId = '{userToken}'");

        return new FsArtifact(artifact.FullPath, artifact.Name, artifact.ArtifactType, artifact.ProviderType)
        {
            Capacity = artifact.Capacity,
            ContentHash = artifact.ContentHash,
            CreateDateTime = artifact.CreateDateTime,
            FileExtension = artifact.FileExtension,
            LastModifiedDateTime = artifact.LastModifiedDateTime,
            LocalFullPath = artifact.LocalFullPath,
            ParentFullPath = artifact.ParentFullPath,
            OriginDevice = artifact.OriginDevice,
            Size = artifact.Size,
            ThumbnailPath = artifact.ThumbnailPath,
            WhoMadeLastEdit = artifact.WhoMadeLastEdit,
            PersistenceStatus = artifact.PersistenceStatus,
            DId = artifact.DId,
            IsAvailableOfflineRequested = artifact.IsAvailableOfflineRequested,
            IsSharedWithMe = artifact.IsSharedWithMe,
            IsSharedByMe = artifact.IsSharedByMe
        };
    }

    public async Task<FsArtifact> CreateArtifactAsync(FsArtifact fsArtifact, ArtifactPersistenceStatus uploadStatus, string localPath, string userToken)
    {
        var LocalDb = FxLocalDbService.CreateConnection();

        var artifactTable = new FsArtifactTable()
        {
            Name = fsArtifact.Name,
            ArtifactType = fsArtifact.ArtifactType,
            Capacity = fsArtifact.Capacity,
            ContentHash = fsArtifact.ContentHash,
            CreateDateTime = fsArtifact.CreateDateTime,
            FileExtension = fsArtifact.FileExtension,
            FullPath = fsArtifact.FullPath,
            OriginDevice = fsArtifact.OriginDevice,
            ParentFullPath = fsArtifact.ParentFullPath,
            ThumbnailPath = fsArtifact.ThumbnailPath,
            ProviderType = fsArtifact.ProviderType,
            Size = fsArtifact.Size,
            WhoMadeLastEdit = fsArtifact.WhoMadeLastEdit,
            LocalFullPath = localPath,
            LastModifiedDateTime = fsArtifact.LastModifiedDateTime,
            PersistenceStatus = uploadStatus,
            DId = userToken,
            IsAvailableOfflineRequested = fsArtifact.IsAvailableOfflineRequested,
            IsSharedWithMe = fsArtifact.IsSharedWithMe,
            IsSharedByMe = fsArtifact.IsSharedByMe
        };

        await LocalDb.InsertAsync(artifactTable);

        return artifactTable.ToFsArtifact();
    }

    public async Task RemoveArtifactAsync(string localPath, string userToken)
    {
        using var LocalDb = FxLocalDbService.CreateConnection();

        await LocalDb.ExecuteAsync(
            $"DELETE FROM FsArtifactTable WHERE LocalFullPath = '{localPath}' AND DId = '{userToken}'");
    }

    public async Task UpdateFileAsync(FsArtifact fsArtifact, string localPath, string userToken)
    {
        var localDb = FxLocalDbService.CreateConnection();

        // TODO: If the fsArtifact property is null, what would it mean to us?

        await localDb.ExecuteAsync(UpdateQuery(),
            new
            {
                FullPath = fsArtifact.FullPath,
                UserToken = userToken,
                LocalFullPath = fsArtifact.LocalFullPath,
                Name = fsArtifact.Name,
                FileExtension = fsArtifact.FileExtension,
                ArtifactType = fsArtifact.ArtifactType,
                Size = fsArtifact.Size,
                Capacity = fsArtifact.Capacity,
                ContentHash = fsArtifact.ContentHash,
                ParentFullPath = fsArtifact.ParentFullPath,
                OriginDevice = fsArtifact.OriginDevice,
                CreateDateTime = fsArtifact.CreateDateTime,
                LastModifiedDateTime = fsArtifact.LastModifiedDateTime,
                WhoMadeLastEdit = fsArtifact.WhoMadeLastEdit,
                ArtifactUploadStatus = fsArtifact.PersistenceStatus,
                IsAvailableOfflineRequested = fsArtifact.IsAvailableOfflineRequested,
                IsSharedWithMe = fsArtifact.IsSharedWithMe,
                IsSharedByMe = fsArtifact.IsSharedByMe
            });
    }

    public async Task UpdateFolderAsync(FsArtifact fsArtifact, string localPath, string userToken)
    {
        var localDb = FxLocalDbService.CreateConnection();

        // TODO: If the fsArtifact property is null, what would it mean to us?

        await localDb.ExecuteAsync(UpdateQuery(),
        new
        {
            FullPath = fsArtifact.FullPath,
            UserToken = userToken,
            LocalFullPath = fsArtifact.LocalFullPath,
            Name = fsArtifact.Name,
            FileExtension = fsArtifact.FileExtension,
            ArtifactType = fsArtifact.ArtifactType,
            Size = fsArtifact.Size,
            Capacity = fsArtifact.Capacity,
            ContentHash = fsArtifact.ContentHash,
            ParentFullPath = fsArtifact.ParentFullPath,
            OriginDevice = fsArtifact.OriginDevice,
            CreateDateTime = fsArtifact.CreateDateTime,
            LastModifiedDateTime = fsArtifact.LastModifiedDateTime,
            WhoMadeLastEdit = fsArtifact.WhoMadeLastEdit,
            ArtifactUploadStatus = fsArtifact.PersistenceStatus,
            IsAvailableOfflineRequested = fsArtifact.IsAvailableOfflineRequested,
            IsSharedWithMe = fsArtifact.IsSharedWithMe,
            IsSharedByMe = fsArtifact.IsSharedByMe
        });
    }

    private static string UpdateQuery()
    {
        return @$"UPDATE FsArtifactTable SET
FullPath = @FullPath
LocalFullPath = @LocalFullPath
Name = @Name
FileExtension = @FileExtension
ArtifactType = @ArtifactType
Size = @Size
Capacity = @Capacity
ContentHash = @ContentHash
ParentFullPath = @ParentFullPath
OriginDevice = @OriginDevice
CreateDateTime = @CreateDateTime
LastModifiedDateTime = @LastModifiedDateTime
WhoMadeLastEdit = @WhoMadeLastEdit
ArtifactUploadStatus = @ArtifactUploadStatus
IsAvailableOfflineRequested = @IsAvailableOfflineRequested
IsSharedWithMe = @IsSharedWithMe
IsSharedByMe = @IsSharedByMe

WHERE FullPath = @FullPath AND DId = @UserToken";
    }
}
