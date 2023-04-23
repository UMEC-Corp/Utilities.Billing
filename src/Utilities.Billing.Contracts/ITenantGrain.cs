namespace Utilities.Billing.Contracts;
public interface ITenantGrain : IGrainWithGuidKey
{
    Task<long> AddAccountTypeAsync(AddAccountTypeCommand command);
}

[GenerateSerializer]
public class AddAccountTypeCommand
{
}
