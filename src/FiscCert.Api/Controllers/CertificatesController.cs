using FiscCert.Api.Requests;
using FiscCert.Application.Abstractions;
using FiscCert.Application.DTO;
using FiscCert.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FiscCert.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CertificatesController : ControllerBase
{
    private readonly ICertificateService _certificateService;

    public CertificatesController(ICertificateService certificateService)
    {
        _certificateService = certificateService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid tenantId, CancellationToken cancellationToken)
    {
        if (tenantId == Guid.Empty)
        {
            return BadRequest(new { Error = "Não foi possível identificar o usuário desta solicitação, contate o suporte." });
        }

        var certificates = await _certificateService.GetCertificatesAsync(tenantId, cancellationToken);

        return Ok(certificates);
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] UploadCertificateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            using var stream = request.File.OpenReadStream();

            var dto = new UploadCertificateDto(
                request.TenantId,
                stream,
                request.Password,
                request.EconomicGroupId);

            var certificateId = await _certificateService.UploadCertificateAsync(dto, cancellationToken);

            return Ok(new { Message = "Certificado salvo com sucesso!", CertificateId = certificateId });
        }
        catch (InvalidCertificatePasswordException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Erro interno ao processar o certificado.", Detalhe = ex.Message });
        }
    }
}

