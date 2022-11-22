using Dapper;
using Dapper.Contrib.Extensions;
using Functionland.FxFiles.Client.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class LocalDbFulaSyncItemService : ILocalDbFulaSyncItemService
{
    // TODO: Check all of the input properties and the properties that will modify through the query.Like their names, types, or if that property is needed or not.

    IFxLocalDbService FxLocalDbService { get; set; }

    public LocalDbFulaSyncItemService(IFxLocalDbService fxLocalDbService)
    {
        FxLocalDbService = fxLocalDbService;
    }

    public async Task<List<FulaSyncItem>> GetSyncItemsAsync(string userToken)
    {
        using var LocalDb = FxLocalDbService.CreateConnection();

        var fulaSyncItems = await LocalDb.QueryAsync<FulaSyncItem>(
            $"SELECT * FROM FulaSyncItem WHERE DId = '{userToken}'");

        return fulaSyncItems.ToList();
    }

    public async Task<FulaSyncItem> CreateSyncItemAsync(FulaSyncItem fulaSyncItem, string userToken)
    {
        var LocalDb = FxLocalDbService.CreateConnection();

        var syncItem = new FulaSyncItem()
        {
            FulaPath = fulaSyncItem.FulaPath,
            LastSyncStatus = fulaSyncItem.LastSyncStatus,
            LocalPath = fulaSyncItem.LocalPath,
            SyncType = fulaSyncItem.SyncType,
            DId = userToken,
            RunningStatus = fulaSyncItem.RunningStatus
        };

        await LocalDb.InsertAsync(syncItem);

        return syncItem;
    } 

    public async Task UpdateSyncItemAsync(FulaSyncItem fulaSyncItem, SyncStatus syncStatus, string userToken)
    {
        var localDb = FxLocalDbService.CreateConnection();

        await localDb.ExecuteAsync(
            @$"UPDATE FulaSyncItem SET LastSyncStatus = @LastSyncStatus 
WHERE FulaPath = @FulaPath AND DId = @UserToken AND LocalPath = @LocalPath",
        new
        {
            FulaPath = fulaSyncItem.FulaPath,
            LocalPath = fulaSyncItem.LocalPath,
            UserToken = userToken,
            LastSyncStatus = syncStatus//,
            //RunningStatus= syncStatus
        });
    }
}
