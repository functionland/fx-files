namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakeBloxService : IBloxService
{
    private readonly List<Blox> _bloxs;
    private readonly List<Blox> _invitedBlox;
    public TimeSpan? ActionLatency { get; set; }
    public TimeSpan? EnumerationLatency { get; set; }
    public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

    public FakeBloxService(IEnumerable<Blox> bloxs, IEnumerable<Blox>? invitedBloxs = null, TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        _bloxs = bloxs.ToList();
        _invitedBlox = invitedBloxs?.ToList() ?? new List<Blox>();

        ActionLatency = actionLatency ?? TimeSpan.FromSeconds(2);
        EnumerationLatency = enumerationLatency ?? TimeSpan.FromMilliseconds(10);
    }


    public async Task AcceptBloxInvitationAsync(string bloxId, CancellationToken? cancellationToken = null)
    {
        if (ActionLatency != null)
        {
            await Task.Delay(ActionLatency.Value);
        }

        var blox = _invitedBlox.FirstOrDefault(b => b.Id == bloxId);

        if (blox is null)
            throw new BloxIsNotFoundException(StringLocalizer.GetString(AppStrings.BloxIsNotFoundException));

        _invitedBlox.Remove(blox);
        _bloxs.Add(blox);
    }


    public async Task<List<Blox>> GetBloxesAsync(CancellationToken? cancellationToken = null)
    {
        if (ActionLatency != null)
        {
            await Task.Delay(ActionLatency.Value);
        }

        return _bloxs.ToList();
    }

    public async Task<List<Blox>> GetBloxInvitationsAsync(CancellationToken? cancellationToken = null)
    {
        if (ActionLatency != null)
        {
            await Task.Delay(ActionLatency.Value);
        }

        return _invitedBlox.ToList();
    }

    public async Task RejectBloxInvitationAsync(string bloxId, CancellationToken? cancellationToken = null)
    {
        if (ActionLatency != null)
        {
            await Task.Delay(ActionLatency.Value);
        }

        var blox = _invitedBlox.FirstOrDefault(b => b.Id == bloxId);

        if (blox is null)
            throw new BloxIsNotFoundException(StringLocalizer.GetString(AppStrings.BloxIsNotFoundException));

        _invitedBlox.Remove(blox);
    }
}
