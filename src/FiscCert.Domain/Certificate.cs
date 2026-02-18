using FiscCert.Domain.Abstractions;

namespace FiscCert.Domain;

public sealed class Certificate : Entity
{
    public string OwnerName { get; private set; } = null!;
    public string Cnpj { get; private set; } = null!;
    public DateTime ExpirationDate { get; private set; }
    public bool IsRevoked { get; private set; }

    private Certificate()
    {
    }

    public Certificate(string ownerName, string cnpj, DateTime expirationDate)
    {

        if (string.IsNullOrWhiteSpace(ownerName))
            throw new ArgumentException("Owner name is required.", nameof(ownerName));

        if (string.IsNullOrWhiteSpace(cnpj))
            throw new ArgumentException("CNPJ is required.", nameof(cnpj));

        OwnerName = ownerName;
        Cnpj = cnpj;
        ExpirationDate = expirationDate;
        IsRevoked = false;
    }

    public void Revoke()
    {
        IsRevoked = true;
    }

    public bool IsValid()
    {
        return IsValid(DateTime.UtcNow);
    }

    public bool IsValid(DateTime referenceDate)
    {
        return !IsRevoked && referenceDate <= ExpirationDate;
    }

}
