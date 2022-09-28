using System.Xml.Linq;

namespace Functionland.FxFiles.Client.Shared.Services;

public partial class FakeBloxServiceFactory
{
    [AutoInject] public IServiceProvider ServiceProvider { get; set; }

    public FakeBloxService CreateTypical(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var bloxs = new List<Blox>
        {
            new Blox{ Id = "Blox 1",  Name = "Blox 1" }
        };

        var invitedBloxs = new List<Blox>
        {
            new Blox{ Id = "My Friend Blox 1", Name = "My Friend Blox 1" },
            new Blox{ Id = "My Friend Blox 2", Name = "My Friend Blox 2" }
        };

        return new FakeBloxService(bloxs, invitedBloxs);
    }
}
