namespace SMMTracker.Domain.Entities;

public class Entity
{
    public int Id { get; private set; }

    public override bool Equals(object? obj)
    {
        throw new NotImplementedException();

    }

    protected bool Equals(Entity other)
    {
        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        throw new NotImplementedException();
    }
}