public sealed class Address : AuditableEntity
{
    private Address()
    {
    }

    public Address(
        Guid customerId,
        string fullName,
        string phoneNumber,
        string addressLine1,
        string? addressLine2,
        string city,
        string state,
        string postalCode,
        string country)
    {
        CustomerId = customerId;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }

    public Guid CustomerId { get; private set; }

    public string FullName { get; private set; } = default!;

    public string PhoneNumber { get; private set; } = default!;

    public string AddressLine1 { get; private set; } = default!;

    public string? AddressLine2 { get; private set; }

    public string City { get; private set; } = default!;

    public string State { get; private set; } = default!;

    public string PostalCode { get; private set; } = default!;

    public string Country { get; private set; } = default!;

    public bool IsDefault { get; private set; }

    public void Update(
        string fullName,
        string phoneNumber,
        string addressLine1,
        string? addressLine2,
        string city,
        string state,
        string postalCode,
        string country)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }

    public void SetDefault(bool isDefault)
    {
        IsDefault = isDefault;
    }
}