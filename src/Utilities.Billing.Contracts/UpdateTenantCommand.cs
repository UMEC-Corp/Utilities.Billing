﻿namespace Utilities.Billing.Contracts
{
    public class UpdateTenantCommand
    {
        public string Name { get; set; }
        public string Account { get; set; }
        public string TenantId { get; set; }
    }
}