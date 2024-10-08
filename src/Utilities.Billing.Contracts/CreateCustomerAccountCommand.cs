﻿namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public class CreateCustomerAccountCommand
{
    [Id(0)]
    public string AssetId { get; set; }
    [Id(1)]
    public string DeviceSerial { get; set; }
    [Id(2)]
    public bool CreateMuxed { get; set; }
    [Id(3)]
    public string InputCode { get; set; }
}
