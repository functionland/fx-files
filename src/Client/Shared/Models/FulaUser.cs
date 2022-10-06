namespace Functionland.FxFiles.Client.Shared.Models;

public class FulaUser
{
    public FulaUser(string dId)
    {
        DId = dId;
    }

    public string DId { get; set; }
    public string? Username { get; set; }
    public bool IsParent { get; set; }
}
