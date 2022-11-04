namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public partial class FakeFulaIdentityClientFactory
{

	public FakeFulaIdentityClient CreateSyncScenario01()
	{
		return new FakeFulaIdentityClient(null);
	}
}
