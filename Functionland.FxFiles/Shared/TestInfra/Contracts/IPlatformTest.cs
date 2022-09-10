using Functionland.FxFiles.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.Shared.Services.Contracts
{
    public interface IPlatformTest
    {
        string Title { get; }
        string Description { get; }
        Task RunAsync();
        public event EventHandler<TestProgressChangedEventArgs> ProgressChanged;
    }
}
