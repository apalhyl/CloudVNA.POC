using CoreWCF.IdentityModel.Selectors;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Hyland.SOAP.Service
{
    public class CustomClientCertificateValidator : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            Console.WriteLine(certificate);
        }
    }
}
