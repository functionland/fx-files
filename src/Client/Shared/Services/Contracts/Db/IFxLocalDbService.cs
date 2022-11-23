using Microsoft.Data.Sqlite;

namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IFxLocalDbService
{
    SqliteConnection CreateConnection();
    Task InitAsync();
}