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

        var cnMatch = Regex.Match(subject, @"CN=([^,]+)");

        if (cnMatch.Success)
        {
            var commonName = cnMatch.Groups[1].Value;
            var partes = commonName.Split(':');

            ownerName = partes[0].Trim();

            if (partes.Length >= 2)
            {
                var possivelInscricao = Regex.Replace(partes[1], "[^0-9]", "");

                if (possivelInscricao.Length == 11 || possivelInscricao.Length == 14)
                {
                    federalInscription = possivelInscricao;
                }
            }
        }

        if (string.IsNullOrWhiteSpace(federalInscription))
        {
            // Pega todas as ocorrências de OU=alguma_coisa
            var ouMatches = Regex.Matches(subject, @"OU=([^,]+)");

            foreach (Match match in ouMatches)
            {
                var ouValue = match.Groups[1].Value.Trim();

                var possivelInscricao = Regex.Replace(ouValue, "[^0-9]", "");

                if (possivelInscricao.Length == 11 || possivelInscricao.Length == 14)
                {
                    federalInscription = possivelInscricao;
                    break;
                }
            }
        }

        return (ownerName, federalInscription);
    }
}
