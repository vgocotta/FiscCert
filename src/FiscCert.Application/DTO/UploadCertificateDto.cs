namespace FiscCert.Application.DTO;

public record UploadCertificateDto(Guid TenantId, Stream FileStream, string Password, Guid? EconomicGroupId = null);
