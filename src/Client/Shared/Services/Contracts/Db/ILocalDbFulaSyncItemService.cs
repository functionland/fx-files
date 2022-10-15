using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface ILocalDbFulaSyncItemService
{
    Task<List<FulaSyncItem>> GetSyncItemsAsync(string userToken);
    Task<FulaSyncItem> CreateSyncItemAsync(FulaSyncItem fulaSyncItem, string userToken);
    Task UpdateSyncItemAsync(FulaSyncItem fulaSyncItem, SyncStatus syncStatus, string userToken);
}
