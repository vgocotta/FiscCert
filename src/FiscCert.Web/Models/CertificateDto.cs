namespace FiscCert.Web.Models;

public record CertificateDto(
    Guid Id,
    string OwnerName,
    string FederalInscription,
    DateTime ExpirationDate,
    bool IsRevoked
);