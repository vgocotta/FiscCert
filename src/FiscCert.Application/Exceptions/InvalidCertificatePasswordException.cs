namespace FiscCert.Application.Exceptions;

public class InvalidCertificatePasswordException : Exception
{
    public InvalidCertificatePasswordException() : base("The provided certificate password is invalid or the file is corrupted.")
    {
    }

    public InvalidCertificatePasswordException(string message) : base(message)
    {
    }

    public InvalidCertificatePasswordException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
