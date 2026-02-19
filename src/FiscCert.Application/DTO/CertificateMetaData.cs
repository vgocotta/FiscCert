namespace FiscCert.Application.DTO;

public record CertificateMetadata(string OwnerName, string FederalInscription, string SerialNumber, DateTime ExpirationDate);
