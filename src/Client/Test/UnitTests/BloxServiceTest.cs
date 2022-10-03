using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Services;
using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;
using System.Text;

namespace Functionland.FxFiles.Client.Test.UnitTests
{
    [TestClass]
    public class BloxServiceTest : TestBase
    {
        public TestContext TestContext { get; set; }
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
    }
}