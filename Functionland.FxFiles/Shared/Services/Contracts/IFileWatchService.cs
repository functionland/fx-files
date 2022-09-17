using Functionland.FxFiles.Shared.TestInfra.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Services.Contracts
{
    public interface IFileWatchService
    {
        Task InitialyzeAsync();
        delegate Task AsyncEventHandler<TEventArgs>(object? sender, TEventArgs e);
        public event AsyncEventHandler<ArtifactChangeEventArgs> ArtifactChangeEvent;
    }

}
