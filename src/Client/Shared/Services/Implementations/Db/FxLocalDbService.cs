using Dapper;
using Dapper.Contrib.Extensions;
using DbUp;
using DbUp.SQLite.Helpers;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Reflection;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FxLocalDbService : IFxLocalDbService
{
    private string ConnectionString { get; set; }

    public FxLocalDbService(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public async Task InitAsync()
    {
        SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
        SqlMapper.AddTypeHandler(new GuidHandler());
        SqlMapper.AddTypeHandler(new TimeSpanHandler());
        bool needsMigrate = true;

#if RELEASE && BlazorHybrid
        if (!VersionTracking.IsFirstLaunchEver && !VersionTracking.IsFirstLaunchForCurrentVersion && !VersionTracking.IsFirstLaunchForCurrentBuild)
            needsMigrate = false;
#endif

#if Dev
    needsMigrate = true;
#endif

        if (needsMigrate)
        {
            MigrateDatabase();
        }

    }

    void MigrateDatabase()
    {
        var tryCount = 0;
        DbUp.Engine.DatabaseUpgradeResult? result = null;

        while (true)
        {
            tryCount++;
            try
            {
                var connection = new SharedConnection(CreateConnection());

                var upgrader =
                    DeployChanges.To
                        .SQLiteDatabase(connection)
                        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                        .LogToNowhere()
                        .Build();

                result = upgrader.PerformUpgrade();
            }
            catch (Exception)
            {
                if (tryCount > 5)
                {
                    throw;
                }
            }

            if (result?.Successful == true || tryCount > 5)
                break;
        }

        if (result?.Successful == false)
            throw new InvalidOperationException(result.Error.Message, result.Error);
    }

    public SqliteConnection CreateConnection()
    {
        return new SqliteConnection(ConnectionString);
    }
}

public abstract class SqliteTypeHandler<T> : SqlMapper.TypeHandler<T>
{
    // Parameters are converted by Microsoft.Data.Sqlite
    public override void SetValue(IDbDataParameter parameter, T value)
        => parameter.Value = value;
}

public class DateTimeOffsetHandler : SqliteTypeHandler<DateTimeOffset>
{
    public override DateTimeOffset Parse(object value)
        => DateTimeOffset.Parse((string)value);
}

public class GuidHandler : SqliteTypeHandler<Guid>
{
    public override Guid Parse(object value)
        => Guid.Parse((string)value);
}

public class TimeSpanHandler : SqliteTypeHandler<TimeSpan>
{
    public override TimeSpan Parse(object value)
        => TimeSpan.Parse((string)value);
}
