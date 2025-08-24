namespace Autodor.Modules.Contractors.Domain.Common;

public abstract class Entity<TId> : IEquatable<Entity<TId>> where TId : notnull
{
    public TId Id { get; protected set; }
    public DateTime CreatedDate { get; protected set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; protected set; }

    protected Entity(TId id)
    {
        Id = id;
    }

    protected Entity() { } // EF Constructor

    protected void SetModifiedDate()
    {
        ModifiedDate = DateTime.UtcNow;
    }

    public bool Equals(Entity<TId>? other)
    {
        return Equals((object?)other);
    }

    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> entity && Id.Equals(entity.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity<TId> left, Entity<TId> right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<TId> left, Entity<TId> right)
    {
        return !Equals(left, right);
    }
}