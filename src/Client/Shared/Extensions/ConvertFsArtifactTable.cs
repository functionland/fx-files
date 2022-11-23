using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Extensions
{
    public static class ConvertFsArtifactTable
    {
        public static FsArtifact ToFsArtifact(this FsArtifactTable fsArtifactTable)
        {
            // TODO: What will happen to other propeties. null?

            return new FsArtifact(fsArtifactTable.FullPath, fsArtifactTable.Name, fsArtifactTable.ArtifactType, fsArtifactTable.ProviderType)
            {
                Name = fsArtifactTable.Name,
                ArtifactType = fsArtifactTable.ArtifactType,
                Capacity = fsArtifactTable.Capacity,
                ContentHash = fsArtifactTable.ContentHash,
                CreateDateTime = fsArtifactTable.CreateDateTime,
                FileExtension = fsArtifactTable.FileExtension,
                FullPath = fsArtifactTable.FullPath,
                OriginDevice = fsArtifactTable.OriginDevice,
                ParentFullPath = fsArtifactTable.ParentFullPath,
                ThumbnailPath = fsArtifactTable.ThumbnailPath,
                ProviderType = fsArtifactTable.ProviderType,
                Size = fsArtifactTable.Size,
                WhoMadeLastEdit = fsArtifactTable.WhoMadeLastEdit,
                LocalFullPath = fsArtifactTable.LocalFullPath,
                LastModifiedDateTime = fsArtifactTable.LastModifiedDateTime,
                PersistenceStatus = fsArtifactTable.PersistenceStatus,
                DId = fsArtifactTable.DId,
                IsAvailableOfflineRequested = fsArtifactTable.IsAvailableOfflineRequested,
                IsSharedWithMe = fsArtifactTable.IsSharedWithMe,
                IsSharedByMe = fsArtifactTable.IsSharedByMe
            };
        }
    }
}
