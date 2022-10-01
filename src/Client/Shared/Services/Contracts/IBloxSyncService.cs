using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Services.Contracts
{
    public interface IBloxSyncService
    {
        Task<List<BloxSyncItem>> SyncItemsAsync();
        Task<List<FsArtifact>> SyncContentsAsync();
        Task SyncContentAsync(FsArtifact artifact);
    }
}
