namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakeBloxService : IBloxService
{
    private readonly List<Blox> _bloxs = new();
    private readonly List<Blox> _invitedBlox = new();
    public TimeSpan? ActionLatency { get; set; }
    public TimeSpan? EnumerationLatency { get; set; }
    public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

    public FakeBloxService(IEnumerable<Blox>? bloxs = null,
                           IEnumerable<Blox>? invitedBloxs = null,
                           TimeSpan? actionLatency = null,
                           TimeSpan? enumerationLatency = null)
    {
        _bloxs.Clear();
        _invitedBlox.Clear();

        ActionLatency = actionLatency ?? TimeSpan.FromSeconds(2);
        EnumerationLatency = enumerationLatency ?? TimeSpan.FromMilliseconds(10);

        if(bloxs is not null)
        {
            foreach (var blox in bloxs)
            {
                _bloxs.Add(blox);
            }
        }
        else
        {
            _bloxs = new List<Blox>();
        }

        if(invitedBloxs is not null)
        {
            foreach (var invitedBlox in invitedBloxs)
            {
                _invitedBlox.Add(invitedBlox);
            }
        }
        else
        {
            _invitedBlox = new List<Blox>();
        }
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
