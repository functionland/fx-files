using Functionland.FxFiles.Client.Shared.Resources;
using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Functionland.FxFiles.Client.Shared.Services.Implementations;
using Functionland.FxFiles.Client.Shared.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Functionland.FxFiles.Client.Test.UnitTests;

[TestClass]
public class PoolServiceTest
{
    public TestContext TestContext { get; set; }
    public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;
    [TestMethod]
    public async Task FakePoolService_MustWork()
    {
        var testHost = Host.CreateDefaultBuilder()
               .ConfigureServices((_, services) =>
               {
                   services.AddClientSharedServices();
                   services.AddClientTestServices(TestContext);
                   services.AddSingleton<IFulaPoolSevice>(
                       serviceScope =>
                       serviceScope.GetRequiredService<FakePoolServiceFactory>()
                       .CreateTypicalBloxPools());
               }).Build();

        var serviceScope = testHost.Services.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;
        var poolService = serviceProvider.GetRequiredService<IFulaPoolSevice>();

        //Get my pools
        var myPoolBloxs = await poolService.GetMyPoolsAsync();
        Assert.AreEqual(1, myPoolBloxs.Count);

        //Leave pool
        await poolService.LeavePoolAsync(myPoolBloxs.First().Id);
        myPoolBloxs = await poolService.GetMyPoolsAsync();
        Assert.AreEqual(0, myPoolBloxs.Count);

        //Join to pool
        var isJoined = await poolService.JoinToPoolAsync("BloxPool 78975");
        myPoolBloxs = await poolService.GetMyPoolsAsync();
        Assert.IsTrue(isJoined);
        Assert.AreEqual(1, myPoolBloxs.Count);

        var joinNewBlox = await poolService.JoinToPoolAsync("BloxPool 2587428");
        myPoolBloxs = await poolService.GetMyPoolsAsync();
        Assert.IsTrue(joinNewBlox);
        Assert.AreEqual(2, myPoolBloxs.Count);

        //Assert.ThrowsExceptionAsync<BloxPoolIsNotFoundException>(async () => await poolService.JoinToPoolAsync("BloxPool"));

        //search pool async
        var allPoolBloxs = await poolService.SearchPoolAsync().ToListAsync();
        Assert.AreEqual(3, allPoolBloxs.Count);
    }

    [TestMethod]
    public async Task AllFulaBloxPool_MustWork()
    {
        var testHost = Host.CreateDefaultBuilder()
              .ConfigureServices((_, services) =>
              {
                  services.AddClientSharedServices();
                  services.AddClientTestServices(TestContext);
                  services.AddSingleton<IFulaPoolSevice>(
                      serviceScope =>
                      serviceScope.GetRequiredService<FakePoolServiceFactory>()
                      .CreateAllFulaBloxPool());
              }).Build();

        var serviceScope = testHost.Services.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;
        var poolService = serviceProvider.GetRequiredService<IFulaPoolSevice>();

        var myPoolBloxs = await poolService.GetMyPoolsAsync();
        Assert.AreEqual(0, myPoolBloxs.Count);
        await poolService.JoinToPoolAsync("The long number of characters in the bloxPool Id should be handled");
        await poolService.JoinToPoolAsync("BloxPool 225");
        myPoolBloxs = await poolService.GetMyPoolsAsync();
        Assert.AreEqual(2, myPoolBloxs.Count);

        await poolService.JoinToPoolAsync("BloxPool 16");
        await poolService.JoinToPoolAsync("BloxPool 17");
        myPoolBloxs = await poolService.GetMyPoolsAsync();
        Assert.AreEqual(4, myPoolBloxs.Count);

        var allPoolBloxs = await poolService.SearchPoolAsync().ToListAsync();
        Assert.AreEqual(16, allPoolBloxs.Count);

        await poolService.LeavePoolAsync("BloxPool 16");
        myPoolBloxs = await poolService.GetMyPoolsAsync();
        Assert.AreEqual(3, myPoolBloxs.Count);
    }
}