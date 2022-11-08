using Functionland.FxFiles.Client.Shared.Exceptions;
using Functionland.FxFiles.Client.Shared.Services;
using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Functionland.FxFiles.Client.Test.UnitTests;

[TestClass]
public class FakeBloxServiceTest : TestBase
{
    public TestContext TestContext { get; set; } = default!;

    [TestMethod]
    public async Task FakeBloxService_MustWork()
    {
        var testHost = Host.CreateDefaultBuilder()
           .ConfigureServices((_, services) =>
           {
               services.AddClientSharedServices();
               services.AddClientTestServices(TestContext);
               services.AddSingleton<IBloxService>(
                   serviceScope =>
                   serviceScope.GetRequiredService<FakeBloxServiceFactory>()
                   .CreateTypical());
           }
        ).Build();

        var serviceScope = testHost.Services.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;

        // Get Bloxes
        var bloxService = serviceProvider.GetRequiredService<IBloxService>();

        var bloxes = await bloxService.GetBloxesAsync();
        Assert.AreEqual(1, bloxes.Count);

        var invitations = await bloxService.GetBloxInvitationsAsync();
        Assert.AreEqual(2, invitations.Count);

        // Accept an invitation
        await bloxService.AcceptBloxInvitationAsync(invitations.First().Id);
        invitations = await bloxService.GetBloxInvitationsAsync();
        Assert.AreEqual(1, invitations.Count);

        bloxes = await bloxService.GetBloxesAsync();
        Assert.AreEqual(2, bloxes.Count);

        // Reject an inviation
        await bloxService.RejectBloxInvitationAsync(invitations.First().Id);
        invitations = await bloxService.GetBloxInvitationsAsync();
        Assert.AreEqual(0, invitations.Count);

        bloxes = await bloxService.GetBloxesAsync();
        Assert.AreEqual(2, bloxes.Count);
    }

    [TestMethod]
    public async Task FakeBloxService_ShouldThrowBloxIsNotFoundInAcceptBloxInvitation()
    {
        var testHost = Host.CreateDefaultBuilder()
           .ConfigureServices((_, services) =>
           {
               services.AddClientSharedServices();
               services.AddClientTestServices(TestContext);
               services.AddSingleton<IBloxService>(
                   serviceScope =>
                   serviceScope.GetRequiredService<FakeBloxServiceFactory>()
                   .CreateBloxs());
           }
        ).Build();

        var serviceScope = testHost.Services.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;

        var bloxService = serviceProvider.GetRequiredService<IBloxService>();

        await Assert.ThrowsExceptionAsync<BloxIsNotFoundException>(async () =>
        {
            await bloxService.AcceptBloxInvitationAsync("Second Blox");
        });
    }

    [TestMethod]
    public async Task FakeBloxService_ShouldThrowBloxIsNotFoundInRejectBloxInvitation()
    {
        var testHost = Host.CreateDefaultBuilder()
           .ConfigureServices((_, services) =>
           {
               services.AddClientSharedServices();
               services.AddClientTestServices(TestContext);
               services.AddSingleton<IBloxService>(
                   serviceScope =>
                   serviceScope.GetRequiredService<FakeBloxServiceFactory>()
                   .CreateALotOfBloxs());
           }
        ).Build();

        var serviceScope = testHost.Services.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;

        var bloxService = serviceProvider.GetRequiredService<IBloxService>();

        await Assert.ThrowsExceptionAsync<BloxIsNotFoundException>(async () =>
        {
            await bloxService.RejectBloxInvitationAsync("InvitedBloxs");
        });
    }

    [TestMethod]
    public async Task BloxService_MustWork()
    {
        var testHost = Host.CreateDefaultBuilder()
           .ConfigureServices((_, services) =>
           {
               services.AddClientSharedServices();
               services.AddClientTestServices(TestContext);
               services.AddSingleton<IBloxService>(
                   serviceScope =>
                   serviceScope.GetRequiredService<FakeBloxServiceFactory>()
                   .CreateALotOfBloxs());
           }
        ).Build();

        var serviceScope = testHost.Services.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;
        var bloxService = serviceProvider.GetRequiredService<IBloxService>();

        var bloxes = await bloxService.GetBloxesAsync();
        Assert.AreEqual(11, bloxes.Count);
        var invitations = await bloxService.GetBloxInvitationsAsync();
        Assert.AreEqual(6, invitations.Count);
        await bloxService.RejectBloxInvitationAsync("My Father InvitedBloxs");
        await bloxService.RejectBloxInvitationAsync("My City InvitedBloxs");
        await bloxService.AcceptBloxInvitationAsync("Company InvitedBloxs");
        await bloxService.AcceptBloxInvitationAsync("My Friend InvitedBloxs");
        await bloxService.AcceptBloxInvitationAsync("China InvitedBloxs");
        bloxes = await bloxService.GetBloxesAsync();
        Assert.AreEqual(14, bloxes.Count);
        invitations = await bloxService.GetBloxInvitationsAsync();
        Assert.AreEqual(1, invitations.Count);
    }
}