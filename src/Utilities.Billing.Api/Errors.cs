using Utilities.Common;

namespace Utilities.Billing.Api;

public static class Errors
{
    public static Exception NotFound(string entityName, ICollection<long> ids) =>
        throw new NotFoundException($"Entity not found {entityName}:{string.Join(",", ids)}");

    public static Exception NotFound(string entityName, ICollection<string> ids) =>
        throw new NotFoundException($"Entity not found {entityName}:{string.Join(",", ids)}");

    public static Exception GrainIsNotInitialized(string grainName, Guid id) =>
        throw new OperationException($"Grain uninitialized {grainName}:{id}");

    public static Exception EntityExists()
    {
        throw new AlreadyExistsException($"Entity already exists");
    }

    public static Exception BelongsAnotherTenant(string entityName, string id)
    {
        throw new OperationException($"Entity {entityName}:{id} belongs another Tenant");
    }

    internal static Exception InvalidValue(string field, string value)
    {
        throw new OperationException($"{field} has ivalid value: {value}");
    }

    internal static Exception IncorrectState(string entityName, string state)
    {
        throw new OperationException($"{entityName} has incorrecct state: {state}");
    }
}

