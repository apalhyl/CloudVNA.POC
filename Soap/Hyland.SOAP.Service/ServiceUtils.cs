using CoreWCF;
using System.Text;
using System.Xml;
using System;
using Hyland.SOAP.Core;
using System.Linq;

namespace Hyland.SOAP.Service
{
    public class ServiceUtils
    {
        public static WSHttpBinding GetBinding(bool isSecure, bool isMtom)
        {
            var binding = new WSHttpBinding();

            binding.TransactionFlow = false;
            binding.MessageEncoding = isMtom ? WSMessageEncoding.Mtom : WSMessageEncoding.Text;

            var timeSpan = TimeSpan.FromSeconds(30);
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferPoolSize = isMtom ? 0 : int.MaxValue;
            binding.OpenTimeout = timeSpan;
            binding.CloseTimeout = timeSpan;
            binding.SendTimeout = timeSpan;
            binding.ReceiveTimeout = timeSpan;

            binding.ReaderQuotas = new XmlDictionaryReaderQuotas();
            binding.ReaderQuotas.MaxArrayLength = binding.ReaderQuotas.MaxDepth = binding.ReaderQuotas.MaxNameTableCharCount =
                binding.ReaderQuotas.MaxStringContentLength = binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;

            binding.Security = new WSHTTPSecurity();
            binding.Security.Mode = isSecure ? SecurityMode.Transport : SecurityMode.None;
            binding.Security.Transport = new HttpTransportSecurity();
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            binding.Security.Transport.Realm = string.Empty;
            binding.Security.Message = new NonDualMessageSecurityOverHttp();
            binding.Security.Message.ClientCredentialType = MessageCredentialType.None;
            binding.Security.Message.NegotiateServiceCredential = false;
            binding.Security.Message.EstablishSecurityContext = false;

            return binding;
        }

        public static AdhocQueryRequest GetQueryRequestFromMessage(Message message)
        {
            AdhocQueryRequest request = null;

            using (var reader = message.GetReaderAtBodyContents())
            {
                var serializer = Utils.GetSerializer<AdhocQueryRequest>();
                request = (AdhocQueryRequest)serializer.Deserialize(reader);
            }

            return request;
        }

        public static XmlNode[] GetAssertionHeaderNode(MessageHeaders headers)
        {
            var index = headers.FindHeader(Constants.Security_HeaderName, Constants.Security_HeaderNamespace);
            return index >= 0 ? headers.GetHeader<XmlNode[]>(index) : null;
        }

        public static string GetAssertionHeaderText(XmlNode[] assertionNodes)
        {
            if (assertionNodes == null)
                return string.Empty;
            else
            {
                var doc = new XmlDocument();

                foreach (var node in assertionNodes.Where(x => x.NodeType == XmlNodeType.Element && x.LocalName == Constants.Assertion_Root))
                    doc.AppendChild(doc.ImportNode(node, true));

                return doc.OuterXml;
            }
        }
    }
}
