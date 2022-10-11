namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakeBloxService : IBloxService
{
    private readonly List<Blox> _bloxs;
    private readonly List<Blox> _invitedBlox;
    public TimeSpan? ActionLatency { get; set; }
    public TimeSpan? EnumerationLatency { get; set; }

    public FakeBloxService(IEnumerable<Blox> bloxs, IEnumerable<Blox> invitedBlox)
    {
        _bloxs = new List<Blox>(bloxs);
        _invitedBlox = new List<Blox>(invitedBlox);
    }

    public FakeBloxService(IServiceProvider serviceProvider, IEnumerable<Blox> bloxs, TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        ActionLatency = actionLatency ?? TimeSpan.FromSeconds(2);
        EnumerationLatency = enumerationLatency ?? TimeSpan.FromMilliseconds(10);
        foreach (var blox in bloxs)
        {
            _bloxs.Add(blox);
        }
    }

    public async Task AcceptBloxInvitationAsync(string bloxId, CancellationToken? cancellationToken = null)
    {
        var blox = _invitedBlox.FirstOrDefault(b => b.Id == bloxId);

        if (blox is null) return;//TODO

        _invitedBlox.Remove(blox);
        _bloxs.Add(blox);
    }

    public async Task FillBloxStatsAsync(Blox blox, CancellationToken? cancellationToken = null)
    {
        blox.TotalSpace = 100000000000; 

        blox.VideosUsed = 35000000000;
        blox.AudiosUsed = 16000000000;
        blox.DocsUsed = 3000000000;
        blox.PhotosUsed = 10000000000;
        blox.OtherUsed = 5000000000;

        blox.FreeSpace = 31000000000;
        blox.UsedSpace = 69000000000;
    }

    public async Task<List<Blox>> GetBloxesAsync(CancellationToken? cancellationToken = null)
    {
        return _bloxs.ToList();
    }

    public async Task<List<Blox>> GetBloxInvitationsAsync(CancellationToken? cancellationToken = null)
    {
        return _invitedBlox.ToList();
    }

    public async Task RejectBloxInvitationAsync(string bloxId, CancellationToken? cancellationToken = null)
    {
        var blox = _invitedBlox.FirstOrDefault(b => b.Id == bloxId);

        if (blox is null) return; //TODO

        _invitedBlox.Remove(blox);
    }
}
