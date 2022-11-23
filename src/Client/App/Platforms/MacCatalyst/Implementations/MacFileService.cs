using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Exceptions;
using Functionland.FxFiles.Client.Shared.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.App.Platforms.MacCatalyst.Implementations;

public partial class MacFileService : LocalDeviceFileService
{
    public override FsFileProviderType GetFsFileProviderType(string filePath)
    {
        return FsFileProviderType.InternalMemory;
    }

    public override string GetShowablePath(string artifactPath)
    {
        if (artifactPath is null)
            throw new ArtifactPathNullException(nameof(artifactPath));

        //ToDo: Implement Mac version of how to shape the fullPath to be shown in UI.
        return artifactPath;
    }

    public async override IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string? path = null, CancellationToken? cancellationToken = null)
    {
       
        if (string.IsNullOrWhiteSpace(path))
        {
            var userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) ;

            

            var fsArtifacts = new[] { new FsArtifact(userPath, Path.GetFileName(userPath), FsArtifactType.Folder, FsFileProviderType.InternalMemory) { CreateDateTime = Directory.GetCreationTime(userPath), LastModifiedDateTime = Directory.GetLastWriteTime(userPath)} };
            
            foreach (var item in fsArtifacts.Concat(GetDrives()))
            {
                yield return item;
            }

            yield break;
        }
        else
        {
            await foreach(var item in  base.GetArtifactsAsync(path, cancellationToken))
            {
                yield return item;
            }

            yield break;
        }
        
        
    }

    public override List<FsArtifact> GetDrives()
    {
        var allDerives = base.GetDrives();
        return allDerives.Where(d => d.FullPath.StartsWith("/Volumes/")).ToList();
    }
}
