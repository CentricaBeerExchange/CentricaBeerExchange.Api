namespace CentricaBeerExchange.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class MinimumRoleAttribute : Attribute
{
    public MinimumRoleAttribute(ERole minimumRequiredUserRole)
    {
        MinimumRequiredUserRole = minimumRequiredUserRole;
    }

    public ERole MinimumRequiredUserRole { get; }
}
