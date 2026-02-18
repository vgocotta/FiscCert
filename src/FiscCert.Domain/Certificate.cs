using FiscCert.Domain.Abstractions;

namespace FiscCert.Domain;

public sealed class Certificate : Entity
{
    public Guid? EconomicGroupId { get; private set; }

    public string BlobPath { get; private set; } = null!;

    public string OwnerName { get; private set; } = null!;
    public string FederalInscription { get; private set; } = null!;

    public string SerialNumber { get; private set; } = null!;

    public DateTime ExpirationDate { get; private set; }
    public bool IsRevoked { get; private set; }

    private readonly List<RepresentedCompany> _representedCompanies = [];
    public IReadOnlyCollection<RepresentedCompany> RepresentedCompanies => _representedCompanies.AsReadOnly();

    private Certificate() { }

    public Certificate(
        Guid tenantId,
        string blobPath,
        string ownerName,
        string federalInscription,
        string serialNumber,
        DateTime expirationDate,
        Guid? economicGroupId = null)
        : base(tenantId)
    {
        // Validações de integridade
        if (string.IsNullOrWhiteSpace(blobPath)) throw new ArgumentException("BlobPath is required");
        if (string.IsNullOrWhiteSpace(ownerName)) throw new ArgumentException("Owner Name is required");
        if (string.IsNullOrWhiteSpace(federalInscription)) throw new ArgumentException("Federal Inscription is required");
        if (string.IsNullOrWhiteSpace(serialNumber)) throw new ArgumentException("Serial Number is required");

        EconomicGroupId = economicGroupId;
        BlobPath = blobPath;
        OwnerName = ownerName;
        FederalInscription = federalInscription;
        SerialNumber = serialNumber;
        ExpirationDate = expirationDate;
        IsRevoked = false;
    }

    public void AssignToEconomicGroup(Guid groupId)
    {
        if (groupId == Guid.Empty) throw new ArgumentException("Group ID cannot be empty");
        EconomicGroupId = groupId;
    }

    public void RemoveFromEconomicGroup() => EconomicGroupId = null;

    public void AddRepresentedCompany(string name, string cnpj)
    {
        _representedCompanies.Add(new RepresentedCompany(this.TenantId, name, cnpj));
    }

    public void Revoke() => IsRevoked = true;

    public bool IsValid() => IsValid(DateTime.UtcNow);

    public bool IsValid(DateTime referenceDate)
    {
        return !IsRevoked && referenceDate <= ExpirationDate;
    }
}