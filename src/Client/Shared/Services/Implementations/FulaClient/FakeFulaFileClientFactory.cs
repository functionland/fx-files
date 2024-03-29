﻿using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;
public partial class FakeFulaFileClientFactory
{
    [AutoInject] IStringLocalizer<AppStrings> StringLocalizer { get; set; }
    //TODO: Make sure for adding activity to fsartifact
    public FakeFulaFileClient CreateSyncScenario01()
    {
        var fsArtifacts = new Dictionary<FulaUser, List<KeyValuePair<FsArtifact, Stream?>>>
        {
            {
                new FulaUser("x"),
                new List<KeyValuePair<FsArtifact, Stream?>>
                {
                    CreateFolder($"{FulaConvention.FulaRootPath}"),
                    CreateFolder($"{FulaConvention.FulaRootPath}{FulaConvention.FulaFilesRootPath}"),
                    CreateFolder($"{FulaConvention.FulaRootPath}{FulaConvention.FulaSharedRootPath}"),

                    CreateFolder("\\MyFiles\\Documents"),
                    CreateFolder("\\MyFiles\\Music"),
                    CreateFolder("\\MyFiles\\Documents\\Work"),
                    CreateFolder("\\MyFiles\\Documents\\Home"),
                    CreateFile("\\MyFiles\\Documents\\fileD1.txt"),

                    CreateFolder("\\MyFiles\\Pictures"),
                    CreateFile("\\MyFiles\\Pictures\\p1.jpg"),
                    CreateFolder("\\MyFiles\\Pictures\\Winter"),
                    CreateFolder("\\MyFiles\\Pictures\\Spring"),
                    CreateFile("\\MyFiles\\Pictures\\Winter\\w1.jpg"),
                    CreateFile("\\MyFiles\\Pictures\\Spring\\s1.jpg"),
                }
            }
        };
        return new FakeFulaFileClient(fsArtifacts, StringLocalizer);
    }

    public FakeFulaFileClient CreateSyncScenario02()
    {
        var fsArtifacts = new Dictionary<FulaUser, List<KeyValuePair<FsArtifact, Stream?>>>
        {
            {
                new FulaUser("x"),
                new List<KeyValuePair<FsArtifact, Stream?>>
                {
                    CreateFolder($"{FulaConvention.FulaRootPath}"),
                    CreateFolder($"{FulaConvention.FulaRootPath}{FulaConvention.FulaFilesRootPath}"),

                    CreateFolder("\\MyFiles\\Documents"),
                    CreateFolder("\\MyFiles\\Music")
                }
            }
        };
        return new FakeFulaFileClient(fsArtifacts, StringLocalizer);
    }

    //ToDo: Cover create root folder and Shared/MyFiles.
    private static KeyValuePair<FsArtifact, Stream?> CreateFolder(string folderPath)
    {
        var folderName = Path.GetFileName(folderPath);
        //var finalFolderPath = FulaConvention.FulaFilesRootPath + folderPath;

        var fsArtifact = new FsArtifact(folderPath, folderName, FsArtifactType.Folder, FsFileProviderType.Fula)
        {
            ParentFullPath = Path.GetDirectoryName(folderPath),
            ContentHash = DateTimeOffset.UtcNow.ToString(),
            LastModifiedDateTime = DateTimeOffset.UtcNow,
            CreateDateTime = DateTimeOffset.UtcNow
        };

        return new KeyValuePair<FsArtifact, Stream?>(fsArtifact, CreateSimpleStream());
    }

    private static KeyValuePair<FsArtifact, Stream?> CreateFile(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var extension = Path.GetExtension(filePath);

        //var finalFilePath = FulaConvention.FulaFilesRootPath + filePath;
        var fsArtifact = new FsArtifact(filePath, fileName, FsArtifactType.File, FsFileProviderType.Fula)
        {
            ParentFullPath = Path.GetDirectoryName(filePath),
            FileExtension = extension,
            ContentHash = DateTimeOffset.UtcNow.ToString(),
            LastModifiedDateTime = DateTimeOffset.UtcNow,
            CreateDateTime = DateTimeOffset.UtcNow
        };

        return new KeyValuePair<FsArtifact, Stream?>(fsArtifact, CreateSimpleStream());
    }

    private static Stream CreateSimpleStream()
    {
        var sampleFilePath = GetSampleFileLocalPath();

        using FileStream fs = File.Open(Path.Combine(sampleFilePath, "fake-pic.jpg"), FileMode.Open);
        return fs;
    }

    private static string GetSampleFileLocalPath()
    {
        var baseAddress = AppDomain.CurrentDomain.BaseDirectory;
        var toRemove = $"Test\\bin\\Debug\\net6.0\\";
        var imgLocalAddress = $"Shared\\wwwroot\\images\\Files\\";

        return baseAddress.Replace(toRemove, imgLocalAddress);
    }

}

