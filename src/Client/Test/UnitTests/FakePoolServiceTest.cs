using Functionland.FxFiles.Client.Shared.Exceptions;
using Functionland.FxFiles.Client.Shared.Resources;
using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Functionland.FxFiles.Client.Shared.Services.Implementations;
using Functionland.FxFiles.Client.Shared.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Functionland.FxFiles.Client.Test.UnitTests;

[TestClass]
public class FakePoolServiceTest
{
    public TestContext TestContext { get; set; } = default!;
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
        var isJoined = await poolService.JoinToPoolAsync("zvgssdsdbjAAAEEEdjddkdk123654s");
        myPoolBloxs = await poolService.GetMyPoolsAsync();
        Assert.IsTrue(isJoined);
        Assert.AreEqual(1, myPoolBloxs.Count);

        var joinNewBlox = await poolService.JoinToPoolAsync("BloxPool2587428zxdarasbjdjsdbd");
        myPoolBloxs = await poolService.GetMyPoolsAsync();
        Assert.IsTrue(joinNewBlox);
        Assert.AreEqual(2, myPoolBloxs.Count);

        //search pool async
        var allPoolBloxs = await poolService.SearchPoolAsync().ToListAsync();
        Assert.AreEqual(4, allPoolBloxs.Count);
    }

    [TestMethod]
    public async Task FakePoolService_ShouldThrowBloxPoolIsNotFound_JoinToPool()
    {
        var testHost = Host.CreateDefaultBuilder()
               .ConfigureServices((_, services) =>
               {
                   services.AddClientSharedServices();
                   services.AddClientTestServices(TestContext);
                   services.AddSingleton<IFulaPoolSevice>(
                       serviceScope =>
                       serviceScope.GetRequiredService<FakePoolServiceFactory>()
                       .CreateBloxPools());
               }).Build();

        var serviceScope = testHost.Services.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;
        var poolService = serviceProvider.GetRequiredService<IFulaPoolSevice>();

        await Assert.ThrowsExceptionAsync<BloxPoolIsNotFoundException>(async () =>
        {
            await poolService.JoinToPoolAsync("a4s5d5df5ff5f51236654914952369");
        });
    }

    [TestMethod]
    public async Task FakePoolService_ShouldThrowBloxPoolAlreadyExists_JoinToPool()
    {
        var testHost = Host.CreateDefaultBuilder()
               .ConfigureServices((_, services) =>
               {
                   services.AddClientSharedServices();
                   services.AddClientTestServices(TestContext);
                   services.AddSingleton<IFulaPoolSevice>(
                       serviceScope =>
                       serviceScope.GetRequiredService<FakePoolServiceFactory>()
                       .CreateALotOfBloxPools());
               }).Build();

        var serviceScope = testHost.Services.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;
        var poolService = serviceProvider.GetRequiredService<IFulaPoolSevice>();

        await Assert.ThrowsExceptionAsync<BloxPoolAlreadyExistsException>(async () =>
        {
            await poolService.JoinToPoolAsync("BloxPool147852369987uyttrrNine");
        });
    }

    [TestMethod]
    public async Task FakePoolService_ShouldThrowBloxPoolIsNotFound_LeavePool()
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

        await Assert.ThrowsExceptionAsync<BloxPoolIsNotFoundException>(async () =>
        {
            await poolService.LeavePoolAsync("zvgssdsdbjAAAEEEdjddkdk123654s");
        });
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
        await poolService.JoinToPoolAsync("123654kklljjhhujkoiu123698cvbn");
        await poolService.JoinToPoolAsync("BloxPool2241111111111111111111");
        myPoolBloxs = await poolService.GetMyPoolsAsync();
        Assert.AreEqual(2, myPoolBloxs.Count);

        await poolService.JoinToPoolAsync("BloxPool226zaqwssxvcdertgvhhnj");
        await poolService.JoinToPoolAsync("BloxPool2258799999999999666666");
        myPoolBloxs = await poolService.GetMyPoolsAsync();
        Assert.AreEqual(4, myPoolBloxs.Count);

        var allPoolBloxs = await poolService.SearchPoolAsync().ToListAsync();
        Assert.AreEqual(12, allPoolBloxs.Count);

        await poolService.LeavePoolAsync("BloxPool226zaqwssxvcdertgvhhnj");
        myPoolBloxs = await poolService.GetMyPoolsAsync();
        Assert.AreEqual(3, myPoolBloxs.Count);
    }
}