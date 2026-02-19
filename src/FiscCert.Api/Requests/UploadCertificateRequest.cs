namespace FiscCert.Api.Requests;

public class UploadCertificateRequest
{
    public required IFormFile File { get; set; }
    public required string Password { get; set; }
    public Guid TenantId { get; set; }
    public Guid? EconomicGroupId { get; set; }
}
