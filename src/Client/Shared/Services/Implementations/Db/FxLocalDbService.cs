﻿using Dapper;
using Dapper.Contrib.Extensions;
using DbUp;
using DbUp.SQLite.Helpers;
using Microsoft.Data.Sqlite;
using System.Data;


namespace Functionland.FxFiles.Client.Shared.Services.Implementations.Db;

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
                .WithScriptsEmbeddedInAssembly(typeof(PinnedArtifact).Assembly)
                .LogToNowhere()
                .Build();

        var result = upgrader.PerformUpgrade();

        if (result.Successful is false)
            throw new InvalidOperationException(result.Error.Message, result.Error);
    }

    private SqliteConnection CreateConnection()
    {
        return new SqliteConnection(ConnectionString);
    }

    public async Task AddPinAsync(FsArtifact artifact)
    {
        using var LocalDb = CreateConnection();
        var pinnedArtifact = new PinnedArtifact()
        {
            FullPath = artifact.FullPath,
            ProviderType = artifact.ProviderType,
            PinEpochTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ContentHash = artifact.LastModifiedDateTime.ToString(),
            ThumbnailPath = artifact.ThumbnailPath,
            ArtifactName = artifact.Name,
            FsArtifactType = artifact.ArtifactType
        };

        await Task.Run(() => LocalDb.Insert(pinnedArtifact));
    }

    public async Task UpdatePinAsync(PinnedArtifact pinnedArtifact, string? oldPath = null)
    {
        if (oldPath == null)
        {
            var localDb = CreateConnection();
            await Task.Run(() => localDb.Execute(
                $"UPDATE PinnedArtifact SET ThumbnailPath = @ThumbnailPath, ContentHash=@ContentHash WHERE FullPath = @FullPath ",
                new
                {
                    ThumbnailPath = pinnedArtifact.ThumbnailPath,
                    ContentHash = pinnedArtifact.ContentHash,
                    FullPath = pinnedArtifact.FullPath
                }));
        }
        else
        {
            var localDb = CreateConnection();
            await Task.Run(() => localDb.Execute(
                $"UPDATE PinnedArtifact SET FullPath =@FullPath, ThumbnailPath = @ThumbnailPath, ContentHash=@ContentHash WHERE FullPath = @OldPath ",
                new
                {
                    OldPath = oldPath,
                    ThumbnailPath = pinnedArtifact.ThumbnailPath,
                    ContentHash = pinnedArtifact.ContentHash,
                    FullPath = pinnedArtifact.FullPath
                }));
        }
    }

    public async Task RemovePinAsync(String FullPath)
    {
        if (string.IsNullOrEmpty(FullPath)) return;
        using var LocalDb = CreateConnection();

        await Task.Run(() => LocalDb.Execute($"DELETE FROM PinnedArtifact WHERE FullPath = '{FullPath}';"));
    }

    public async Task<List<PinnedArtifact>> GetPinnedArticatInfos()
    {
        using var LocalDb = CreateConnection();

        var list = await Task.Run(() => LocalDb.Query<PinnedArtifact>($"SELECT * FROM PinnedArtifact"));
        return list.ToList();
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
