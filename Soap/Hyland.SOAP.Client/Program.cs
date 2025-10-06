using Hyland.SOAP.Core;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Xml;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;

namespace Hyland.SOAP.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                
                var certificate = Utils.GetCertificateCollectionUsingThumbprint("7316dab70f17e863f3eb7f81bb7752b83459ed7f")[0];
                string xml = string.Empty;
                bool isValid = false;

                // Create and sign assertion => Client side functionality
                var createdAssertion = Utils.GetDummyCustomAssertion(certificate.SubjectName.Name);
                xml = createdAssertion.GetSignedAssertionXmlAsText(certificate, KeyClauseType.RSA);
                //xml = createdAssertion.GetSignedAssertionXmlAsText(certificate, KeyClauseType.RSA);

                // Validate and deserialize => Server side functionality
                isValid = Utils.ValidateAssertionSignature(xml);
                var serializedAssertion = Utils.Deserialize<CustomAssertion>(xml);
                isValid = serializedAssertion.VerifyValidityPeriod();
                var details = serializedAssertion.GetXUADetailsFromAssertion();

                //ServicePointManager.ServerCertificateValidationCallback = OnServerCertificateValidationCallback;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var requestObject = new AdhocQueryRequest();
                var responseObject = ProcessRequest(requestObject);

                Console.WriteLine($"Transaction successful at {DateTimeOffset.Now}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.WriteLine("Type to exit");
                Console.ReadLine();
            }
        }

        private static bool OnServerCertificateValidationCallback(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors errors)
        {
            return true;
        }

        private static AdhocQueryResponse ProcessRequest(AdhocQueryRequest request)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = Utils.GetSerializer<AdhocQueryRequest>();
                var requestMessageSerializer = new CustomMessageSerializer(serializer);
                serializer.Serialize(stream, request);
                stream.Flush();
                stream.Position = 0;

                var certificate = Utils.GetCertificateCollectionUsingThumbprint("7316dab70f17e863f3eb7f81bb7752b83459ed7f")[0];
                var createdAssertion = Utils.GetDummyCustomAssertion(certificate.SubjectName.Name);
                var xml = createdAssertion.GetSignedAssertionXmlAsText(certificate, KeyClauseType.X509);

                var requestMessage = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "urn:ihe:iti:2007:RegistryStoredQuery", request, requestMessageSerializer);

                MessageHeader header = new SecurityHeader() { Assertion = xml };

                if (header != null)
                    requestMessage.Headers.Add(header);

                requestMessage.Headers.MessageId = new UniqueId(Guid.NewGuid());

                using (var proxy = ClientUtils.GetServiceProxy<IServiceContract>("https://localhost:5001/Service", false))
                {
                    var responseMessage = proxy.Channel.Query(requestMessage);
                    if (responseMessage.IsFault)
                        throw new Exception(responseMessage.ToString());

                    var response = GetQueryResponseFromMessage(responseMessage);
                    return response;
                }
            }
        }

        private static AdhocQueryResponse GetQueryResponseFromMessage(Message message)
        {
            AdhocQueryResponse response = null;

            using (var reader = message.GetReaderAtBodyContents())
            {
                var serializer = Utils.GetSerializer<AdhocQueryResponse>();
                response = (AdhocQueryResponse)serializer.Deserialize(reader);
            }

            return response;
        }
    }
}
