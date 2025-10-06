using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Channels;
using System.Xml;
using Hyland.SOAP.Core;
using Microsoft.IdentityModel.Protocols.WsAddressing;
using Microsoft.IdentityModel.Protocols.WsTrust;
using Microsoft.IdentityModel.Tokens.Saml2;
using System.IdentityModel.Tokens;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Collections.ObjectModel;
using System.Security.Cryptography.Xml;
using System.IdentityModel.Selectors;

namespace Hyland.SOAP.Client
{
    internal class ClientUtils
    {
        public static Client<T> GetServiceProxy<T>(string endpointAddress, bool isMtom, IEndpointBehavior behavior = null, string certThumbprint = null)
        {
            var isSecure = endpointAddress.StartsWith("https", StringComparison.OrdinalIgnoreCase);

            //Initialize factory
            var factory = new ChannelFactory<T>(GetBinding(isSecure, isMtom), new EndpointAddress(endpointAddress));

            if (isSecure)
            {
                //Create new client credentials
                var clientCredentials = new ClientCredentials();
                clientCredentials.ClientCertificate.Certificate = Utils.GetCertificateCollectionUsingThumbprint("7316dab70f17e863f3eb7f81bb7752b83459ed7f")[0];
                //clientCredentials.ClientCertificate.Certificate = Utils.GetCertificateCollectionUsingThumbprint("decf0d5bb1cc44f0a7bb4424c19447c631448ccd")[0];
                clientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

                clientCredentials.ServiceCertificate.SslCertificateAuthentication = new X509ServiceCertificateAuthentication();
                clientCredentials.ServiceCertificate.SslCertificateAuthentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
                clientCredentials.ServiceCertificate.SslCertificateAuthentication.CustomCertificateValidator = new CustomSslCertValidator();

                //Remove default Client Credentials
                factory.Endpoint.EndpointBehaviors.Clear();

                //Add custom behaviors
                factory.Endpoint.EndpointBehaviors.Add(clientCredentials);
            }

            if (behavior != null)
                factory.Endpoint.EndpointBehaviors.Add(behavior);

            factory.Endpoint.EndpointBehaviors.Add(new ResponseInspector());

            return new Client<T>(factory);
        }

        public static Binding GetBinding(bool isSecure, bool isMtom)
        {
            var binding = new WSHttpBinding();

            binding.BypassProxyOnLocal = true;
            binding.TransactionFlow = false;
            binding.AllowCookies = false;
            binding.MessageEncoding = isMtom ? WSMessageEncoding.Mtom : WSMessageEncoding.Text;
            binding.TextEncoding = Encoding.UTF8;
            binding.UseDefaultWebProxy = false;

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferPoolSize = isMtom ? 0 : 65536 * 2;
            binding.OpenTimeout = TimeSpan.FromSeconds(30);
            binding.CloseTimeout = TimeSpan.FromSeconds(30);
            binding.SendTimeout = TimeSpan.FromSeconds(30);
            binding.ReceiveTimeout = TimeSpan.FromSeconds(30);

            binding.ReaderQuotas = new XmlDictionaryReaderQuotas();
            binding.ReaderQuotas.MaxArrayLength = binding.ReaderQuotas.MaxDepth = binding.ReaderQuotas.MaxNameTableCharCount =
                binding.ReaderQuotas.MaxStringContentLength = binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;

            binding.ReliableSession = new OptionalReliableSession();
            binding.ReliableSession.Ordered = true;
            //Keeping Inactivity timeout to default value of 10 mins.
            binding.ReliableSession.Enabled = false;

            binding.Security = new WSHttpSecurity();
            binding.Security.Mode = isSecure ? SecurityMode.Transport : SecurityMode.None;
            binding.Security.Transport = new HttpTransportSecurity();
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
            binding.Security.Transport.Realm = string.Empty;
            binding.Security.Message = new NonDualMessageSecurityOverHttp();
            binding.Security.Message.ClientCredentialType = MessageCredentialType.None;
            binding.Security.Message.NegotiateServiceCredential = false;
            binding.Security.Message.EstablishSecurityContext = false;

            return binding;
        }
    }
    public class CustomCertValidator : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            // throw new NotImplementedException();
        }
    }

    public class CustomSslCertValidator : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            // throw new NotImplementedException();
        }
    }

    public sealed class Client<T> : IDisposable
    {
        public T Channel { get; private set; }
        public ChannelFactory<T> Factory { get; set; }

        public Client(ChannelFactory<T> factory)
        {
            if (factory == null)
                throw new ArgumentException("factory");

            Factory = factory;
            Channel = Factory.CreateChannel();
        }

        public void Dispose()
        {
            try
            {
                if (Factory.State == CommunicationState.Opened)
                    Factory.Close();
            }
            catch { }
            finally
            {
                Factory = null;
            }
        }
    }
}
