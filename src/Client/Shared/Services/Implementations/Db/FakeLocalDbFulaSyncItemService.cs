using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakeLocalDbFulaSyncItemService : ILocalDbFulaSyncItemService
{
    private List<FulaSyncItem> _syncItems = new();
    public async Task<FulaSyncItem> CreateSyncItemAsync(FulaSyncItem fulaSyncItem, string userToken)
    {
        _syncItems.Add(fulaSyncItem);
        return fulaSyncItem;
    }

    public async Task<List<FulaSyncItem>> GetSyncItemsAsync(string userToken)
    {
        var userSyncItems =_syncItems.Where(a => a.UserToken == userToken).ToList();
        return userSyncItems;
    }

    public async Task UpdateSyncItemAsync(FulaSyncItem fulaSyncItem, SyncStatus syncStatus, string userToken)
    {
        var update = _syncItems.FirstOrDefault(s => s.UserToken == userToken && s.FulaPath == fulaSyncItem.FulaPath && s.LocalPath == fulaSyncItem.LocalPath);
        _syncItems.Remove(update);
        fulaSyncItem.LastSyncStatus = syncStatus;
        _syncItems.Add(fulaSyncItem);
    }
}
