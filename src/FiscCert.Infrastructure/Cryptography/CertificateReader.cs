using FiscCert.Application.Abstractions;
using FiscCert.Application.DTO;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace FiscCert.Infrastructure.Cryptography;

public class CertificateReader : ICertificateReader
{
    public async Task<CertificateMetadata> ReadCertificateAsync(Stream fileStream, string password, CancellationToken cancellationToken = default)
    {
        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream, cancellationToken);
        var certBytes = memoryStream.ToArray();

        using var certificate = X509CertificateLoader.LoadPkcs12(certBytes, password, X509KeyStorageFlags.EphemeralKeySet);

        var serialNumber = certificate.SerialNumber;
        var expirationDate = certificate.NotAfter.ToUniversalTime();

        var (ownerName, federalInscription) = ExtractMetadata(certificate.Subject);

        return new CertificateMetadata(ownerName, federalInscription, serialNumber, expirationDate);
    }

    private (string OwnerName, string FederalInscription) ExtractMetadata(string subject)
    {
        string ownerName = string.Empty;
        string federalInscription = string.Empty;

        var match = Regex.Match(subject, @"CN=([^,]+)");

        if (match.Success)
        {
            var commonName = match.Groups[1].Value;

            var partes = commonName.Split(':');

            if (partes.Length >= 2)
            {
                ownerName = partes[0].Trim();

                federalInscription = Regex.Replace(partes[1], "[^0-9]", "");
            }
            else
            {
                ownerName = commonName;
            }
        }

        return (ownerName, federalInscription);
    }
}
