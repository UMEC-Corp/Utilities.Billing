namespace Utilities.Billing.Data.Entities
{
    public enum AccountState
    {
        Unknown = 0,
        Ok = 1,
        Creating = 2,
        Updating = 4,
        Deleting = 5,
        Deleted = 6,
    }
}