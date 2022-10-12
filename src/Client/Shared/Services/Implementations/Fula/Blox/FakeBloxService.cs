namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakeBloxService : IBloxService
{
    private readonly List<Blox> _bloxs;
    private readonly List<Blox> _invitedBlox;
    public TimeSpan? ActionLatency { get; set; }
    public TimeSpan? EnumerationLatency { get; set; }
    public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

    public FakeBloxService(IEnumerable<Blox> bloxs, IEnumerable<Blox> invitedBloxs, TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        _bloxs.Clear();
        _invitedBlox.Clear();

        ActionLatency = actionLatency ?? TimeSpan.FromSeconds(2);
        EnumerationLatency = enumerationLatency ?? TimeSpan.FromMilliseconds(10);

        foreach (var blox in bloxs)
        {
            _bloxs.Add(blox);
        }
        foreach (var invitedBlox in invitedBloxs)
        {
            _invitedBlox.Add(invitedBlox);
        }
    }

    public FakeBloxService(IEnumerable<Blox> bloxs, TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        _bloxs.Clear();
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

        if (blox is null)
            throw new BloxIsNotFoundException(StringLocalizer.GetString(AppStrings.BloxIsNotFoundException));

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

        if (blox is null)
            throw new BloxIsNotFoundException(StringLocalizer.GetString(AppStrings.BloxIsNotFoundException));

        _invitedBlox.Remove(blox);
    }
}
