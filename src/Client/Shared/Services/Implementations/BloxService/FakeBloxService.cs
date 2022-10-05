
namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakeBloxService : IBloxService
{
    private readonly List<Blox> _bloxs;
    private readonly List<Blox> _invitedBlox;

    public FakeBloxService(IEnumerable<Blox> bloxs, IEnumerable<Blox> invitedBlox)
    {
        _bloxs = new List<Blox>(bloxs);
        _invitedBlox = new List<Blox>(invitedBlox);
    }

    public async Task AcceptBloxInvitationAsync(string bloxId, CancellationToken? cancellationToken = null)
    {
        var blox = _invitedBlox.First(b => b.Id == bloxId);
        _invitedBlox.Remove(blox);
        _bloxs.Add(blox);
    }

    public async Task ClearBloxDataAsync(string bloxId, CancellationToken? cancellationToken = null)
    {
        _bloxs.Clear();
    }

    public async Task FillBloxStatsAsync(Blox blox, CancellationToken? cancellationToken = null)
    {

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
        var blox = _invitedBlox.First(b => b.Id == bloxId);
        _invitedBlox.Remove(blox);
    }
}
