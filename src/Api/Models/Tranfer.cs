namespace MetroPortoAPI.Api.Models;

public class Transfer
{
    public string Id { get; set; }
    public string FromStopId { get; set; }
    public string ToStopId { get; set; }
    public int? TransferType { get; set; }
}