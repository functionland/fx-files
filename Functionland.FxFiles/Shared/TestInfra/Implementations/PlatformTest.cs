using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Functionland.FxFiles.Shared.TestInfra.Implementations
{
    public abstract class PlatformTest : IPlatformTest
    {
        public abstract string Title { get; }

        public abstract string Description { get; }

        public event EventHandler<TestProgressChangedEventArgs>? ProgressChanged;

        public void OnTestProgressChanged(object? sender, TestProgressChangedEventArgs eventArgs)
        {
            ProgressChanged?.Invoke(this, eventArgs);
        }

        protected abstract Task OnRunAsync();

        public async Task RunAsync()
        {
            await OnRunAsync();
        }
    }
}
