using Nike.Framework.Domain;

namespace Nike.CustomerManagement.Domain.Customers.ValueObjects;

public class FullName : ValueObject<FullName>
{
    /// <inheritdoc />
    public FullName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    // FOR ORM !
    private FullName()
    {
    }

    public string FirstName { get; }

    public string LastName { get; }

    /// <inheritdoc />
    protected override void Validate()
    {
    }
}