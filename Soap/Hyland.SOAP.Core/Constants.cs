using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyland.SOAP.Core
{
    public class Constants
    {
        public const string Assertion_Root = "Assertion";
        public const string Signature_ElementName = "Signature";
        public const string Assertion_Namespace = "urn:oasis:names:tc:SAML:2.0:assertion";
        public const string CodedValue_Namespace = "urn:hl7-org:v3";
        public const string NameId_Element = "NameID";
        public const string AttributeValue = "AttributeValue";


        public const string xsdAttribute = "xmlns:xsd";
        public const string xsiAttribute = "xmlns:xsi";

        public const string Xml_DateFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
        public const string Issuer_Format = "urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName";
        public const string NameId_Format = "urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName";
        public const string AttributeName_Format = "urn:oasis:names:tc:SAML:2.0:attrname-format:uri";
        public const string SubjectConfirmationMethod = "urn:oasis:names:tc:SAML:2.0:cm:bearer";
        public const string AuthContextClassReference = "urn:oasis:names:tc:SAML:2.0:ac:classes:unspecified";

        public const string Security_HeaderName = "Security";
        public const string Security_HeaderNamespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
        public const string SoapEnvelope_Namespace = "http://www.w3.org/2003/05/soap-envelope";

        public const string AttributeName_PurposeOfUse = "urn:oasis:names:tc:xspa:1.0:subject:purposeofuse";
        public const string AtrributeName_SubjectId = "urn:oasis:names:tc:xspa:1.0:subject:subject-id";
        public const string AtrributeName_SubjectRole = "urn:oasis:names:tc:xacml:2.0:subject:role";
        public const string AtrributeName_OrganizationName = "urn:oasis:names:tc:xspa:1.0:subject:organization";
        public const string AtrributeName_OrganizationId = "urn:oasis:names:tc:xspa:1.0:subject:organization-id";
        public const string AtrributeName_HomeCommunityId = "urn:ihe:iti:xca:2010:homeCommunityId";
        public const string AtrributeName_PATIENT_PRIVACY_POLICY_ACKNOWLEDGEMENT_DOCUMENT = "urn:ihe:iti:bppc:2007:docid";
        public const string AtrributeName_PATIENT_PRIVACY_POLICY_IDENTIFIER = "urn:ihe:iti:xua:2012:acp";
        public const string AtrributeName_PATIENT_IDENTIFIER = "urn:oasis:names:tc:xacml:2.0:resource:resource-id";
        public const string NameIdFormat_Certificate = "urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName";
        public const string NameIdFormat_Email = "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress";
        public const string NameIdFormat_Unspecified = "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified";

    }
}
