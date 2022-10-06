namespace Functionland.FxFiles.Client.Shared.Models;

public class DIdDocument
{
    public DIdDocument(string dId)
    {
        DId = dId;
    }

    public string DId { get; set; }
    public string? Name { get; set; }
}
