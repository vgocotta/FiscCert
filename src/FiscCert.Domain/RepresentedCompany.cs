using FiscCert.Domain.Abstractions;

namespace FiscCert.Domain;

public sealed class RepresentedCompany : Entity
{
    public Guid CertificateId { get; private set; }
    public string CompanyName { get; private set; } = null!;
    public string Cnpj { get; private set; } = null!;

    private RepresentedCompany() { }

    public RepresentedCompany(Guid tenantId, string companyName, string cnpj) : base(tenantId)
    {
        CompanyName = companyName;
        Cnpj = cnpj;
    }
}