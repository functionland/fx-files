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
        // TODO: Correct the method inputs. Like userToken .. (in all of the methods)
        var userSyncItems =_syncItems.Where(a => a.DId == userToken).ToList();
        return userSyncItems;
    }

    public async Task UpdateSyncItemAsync(FulaSyncItem fulaSyncItem, SyncStatus syncStatus, string userToken)
    {
        // TODO: Correct the method inputs. Like userToken .. (in all of the methods)
        var update = _syncItems.FirstOrDefault(s => s.DId == userToken && s.FulaPath == fulaSyncItem.FulaPath && s.LocalPath == fulaSyncItem.LocalPath);
        _syncItems.Remove(update);
        fulaSyncItem.LastSyncStatus = syncStatus;
        _syncItems.Add(fulaSyncItem);
    }
}
