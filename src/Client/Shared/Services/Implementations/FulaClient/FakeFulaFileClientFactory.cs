namespace Functionland.FxFiles.Client.Shared.Services.Implementations;
public class FakeFulaFileClientFactory
{
    //FulaRootPath
    //SharedRootPath

    public FakeFulaFileClient CreateSyncScenario01()
    {
        return new FakeFulaFileClient(null);
    }

    public FakeFulaFileClient CreateSyncScenario02()
    {
        return new FakeFulaFileClient(null);
    }
}

