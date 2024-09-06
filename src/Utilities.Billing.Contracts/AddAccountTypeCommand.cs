﻿namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public sealed class AddAccountTypeCommand
{
    [Id(0)]
    public string Name { get; set; }
    [Id(1)]
    public string Token { get; set; }
}
