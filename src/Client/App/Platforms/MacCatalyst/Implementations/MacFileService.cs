using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.App.Platforms.MacCatalyst.Implementations;

public partial class MacFileService : LocalDeviceFileService
{
    public override FsFileProviderType GetFsFileProviderType(string filePath)
    {
        return FsFileProviderType.InternalMemory;
    }

    
   
}
