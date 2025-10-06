using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Hyland.SOAP.Core;

namespace Hyland.SOAP.Client
{
    public class SecurityHeader : MessageHeader
    {
        public string Assertion { get; set; }

        // Name of the header
        public override string Name
        {
            get { return Constants.Security_HeaderName; }
        }

        // Header namespace
        public override string Namespace
        {
            get { return Constants.Security_HeaderNamespace; }
        }

        protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteStartElement("wsse", Name, Namespace);
            writer.WriteXmlnsAttribute("wsse", Namespace);            
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            XmlDocument document = GetSecurityXmlDocument();

            foreach (XmlNode item in document.ChildNodes[0].ChildNodes)
            {
                writer.WriteNode(item.CreateNavigator(), false);
            }
        }

        public XmlDocument GetSecurityXmlDocument()
        {
            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            var securityElement = doc.CreateElement($"wsse:{Constants.Security_HeaderName}", Constants.Security_HeaderNamespace);
            securityElement.SetAttribute("xmlns:s", Constants.SoapEnvelope_Namespace);

            if (!string.IsNullOrEmpty(Assertion))
            {
                var fragment = doc.CreateDocumentFragment();
                fragment.InnerXml = Assertion;
                securityElement.AppendChild(fragment);
            }

            doc.AppendChild(securityElement);

            return doc;
        }
    }
}
