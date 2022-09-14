using Dapper;
using Dapper.Contrib.Extensions;
using DbUp;
using DbUp.SQLite.Helpers;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


namespace Functionland.FxFiles.Shared.Services.Implementations.Db;

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

#if RELEASE
//TODO: Track version
        if (!VersionTracking.IsFirstLaunchEver && !VersionTracking.IsFirstLaunchForCurrentVersion && !VersionTracking.IsFirstLaunchForCurrentBuild)
            needsMigrate = false;
#endif

        if (needsMigrate)
        {
            MigrateDatabase();
        }

    }
    void MigrateDatabase()
    {
        var connection = new SharedConnection(CreateConnection());

        var upgrader =
            DeployChanges.To
                .SQLiteDatabase(connection)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

        var result = upgrader.PerformUpgrade();

        if (result.Successful is false)
            throw new InvalidOperationException(result.Error.Message, result.Error);
    }

    private SqliteConnection CreateConnection()
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
