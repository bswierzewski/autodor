namespace Web.Infrastructure;

public abstract class EndpointGroupBase
{
    public virtual string GroupName { get; } = null;
    public abstract void Map(WebApplication app);
}
