namespace Functionland.FxFiles.Client.Shared.Services.Contracts
{
    public interface IBloxService
    {
        Task<List<Blox>> GetBloxesAsync();
        Task<List<Blox>> GetBloxesJoinInvitationAsync();
        Task AcceptBloxInvitationAsync(string bloxId);
        Task RejectBloxInvitationAsync(string bloxId);
        Task ClearBloxData(string bloxId);



        Task<List<Pool>> GetPoolsAsync();
        Task LeavePoolAsync(string poolId);
        Task<(double? DueNowPaymentRequired, double? PerMounthPaymentRequired)> RequestJoinToPoolAsync(string poolId);
        Task<bool> ConfirmJoinToPoolAsync(string poolId);
        Task<List<Pool>> SearchPoolAsync(PoolFilter filter, double? Distance);

        Task<List<(string? Did, string? Name)>> GetUsersForShare();


        Task<Stream> GetAvatarAsync();
    }
}
