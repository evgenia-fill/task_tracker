namespace SMMTracker.Domain.Entities;

public abstract class Entity
{
    public int Id { get; protected set; }

    public override bool Equals(object? obj)
    {
        return obj is Entity other && Equals(other);
    }

    public bool Equals(Entity? other)
    {
        if (other is null) return false;
        if (Id == 0 || other.Id == 0)
            return ReferenceEquals(this, other);

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override string ToString()
    {
       return $"Name: {GetType().Name} Id: {Id}";
    }

    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
}