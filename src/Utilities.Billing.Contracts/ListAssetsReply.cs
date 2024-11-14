﻿
namespace Utilities.Billing.Contracts
{
    [GenerateSerializer]
    public class ListAssetsReply
    {
        [Id(0)]
        public IList<Item> Items { get; set; } = new List<Item>();

        [GenerateSerializer]
        public class Item
        {
            [Id(0)]
            public Guid Id { get; set; }
            [Id(1)]
            public string Code { get; set; }
            [Id(2)]
            public string Issuer { get; set; }
        }
    }
}