
namespace Utilities.Billing.Contracts;

public class CreateCustomerAccountCommand
{
    public string AssetId { get; set; }
    public string DeviceSerial { get; set; }
    public bool CreateMuxed { get; set; }
    public string InputCode { get; set; }
    public string TenantId { get; set; }
}
