using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Functionland.FxFiles.Shared.TestInfra.Implementations
{
    public abstract class PlatformTest : IPlatformTest
    {
        public abstract string Title { get; }

        public abstract string Description { get; }

        public event EventHandler<TestProgressChangedEventArgs>? ProgressChanged;

        protected void OnTestProgressChanged(object? sender, TestProgressChangedEventArgs eventArgs)
        {
            ProgressChanged?.Invoke(this, eventArgs);
        }

        protected void Progress(string title, string description, TestProgressType progressType)
        {
            OnTestProgressChanged(this, new TestProgressChangedEventArgs(title, description, progressType));
        }

        protected abstract Task OnRunAsync();

        public async Task RunAsync()
        {
            await OnRunAsync();
        }
    }
}
