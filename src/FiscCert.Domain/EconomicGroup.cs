using FiscCert.Domain.Abstractions;

namespace FiscCert.Domain;

public sealed class EconomicGroup : Entity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    private readonly List<Certificate> _certificates = [];
    public IReadOnlyCollection<Certificate> Certificates => _certificates.AsReadOnly();

    private EconomicGroup() { }

    public EconomicGroup(Guid tenantId, string name, string? description = null) : base(tenantId)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Group name is required");
        Name = name;
        Description = description;
    }
}