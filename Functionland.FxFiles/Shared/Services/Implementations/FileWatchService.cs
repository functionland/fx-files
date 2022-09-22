using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Services.Implementations
{
    public abstract class FileWatchService : IFileWatchService
    {
        public abstract Task InitialyzeAsync();
    }
}
