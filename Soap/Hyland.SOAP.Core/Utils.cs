using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Hyland.SOAP.Core
{
    public enum KeyClauseType
    {
        RSA,
        X509
    }

    public static class Utils
    {
        public static X509Certificate2Collection GetCertificateCollectionUsingThumbprint(string certificateThumbprint, StoreName storeName = StoreName.My, StoreLocation storeLocation = StoreLocation.LocalMachine, bool validOnly = true)
        {
            X509Store certStore = new X509Store(storeName, storeLocation);

            try
            {
                // Try to open the store.
                certStore.Open(OpenFlags.ReadOnly);

                // Find the certificate that matches the thumbprint.
                X509Certificate2Collection certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, certificateThumbprint, validOnly);
                return certCollection;
            }
            finally
            {
                certStore.Close();
            }
        }

        #region Public Methods

        public static XmlSerializer GetSerializer<T>()
        {
            XmlRootAttribute rootAttribute = null;

            var expectedType = typeof(T);

            if (expectedType == typeof(AdhocQueryRequest))
                rootAttribute = new XmlRootAttribute("AdhocQueryRequest") { Namespace = "urn:oasis:names:tc:ebxml-regrep:xsd:query:3.0" };
            else if (expectedType == typeof(AdhocQueryResponse))
                rootAttribute = new XmlRootAttribute("AdhocQueryResponse") { Namespace = "urn:oasis:names:tc:ebxml-regrep:xsd:query:3.0" };
            else if (expectedType == typeof(RegistryErrorList))
                rootAttribute = new XmlRootAttribute("RegistryErrorList") { Namespace = "urn:oasis:names:tc:ebxml-regrep:xsd:rs:3.0" };
            else if (expectedType == typeof(CustomAssertion))
                rootAttribute = new XmlRootAttribute(Constants.Assertion_Root) { Namespace = Constants.Assertion_Namespace };
            else if (expectedType == typeof(AttributeValue))
                rootAttribute = new XmlRootAttribute(Constants.AttributeValue) { Namespace = Constants.Assertion_Namespace };
            else if (expectedType == typeof(PurposeOfUse))
                rootAttribute = new XmlRootAttribute(nameof(PurposeOfUse)) { Namespace = Constants.CodedValue_Namespace };
            else if (expectedType == typeof(SubjectRole))
                rootAttribute = new XmlRootAttribute(nameof(SubjectRole)) { Namespace = Constants.CodedValue_Namespace };
            else
                return null;

            var type = typeof(T);

            if (rootAttribute == null)
                return new XmlSerializer(type);
            else
                return new XmlSerializer(type, rootAttribute);
        }

        public static CustomAssertion GetDummyCustomAssertion(string subjectName)
        {
            var currentTime = DateTime.UtcNow;
            var currentTimeText = currentTime.ToString(Constants.Xml_DateFormat);
            var endTime = currentTime.AddHours(1);
            var endTimeText = endTime.ToString(Constants.Xml_DateFormat);
            var assertionId = $"_{Guid.NewGuid()}";            

            var assertion = new CustomAssertion();
            assertion.Id = assertionId;
            assertion.IssueInstant = currentTimeText;
            assertion.Version = "2.0";

            #region Issuer

            assertion.Issuer = new Issuer();
            assertion.Issuer.Format = Constants.Issuer_Format;
            assertion.Issuer.Value = subjectName;

            #endregion

            #region Subject

            assertion.Subject = new Subject();
            assertion.Subject.NameId = new NameId();
            assertion.Subject.NameId.Format = Constants.NameId_Format;
            assertion.Subject.NameId.Value = subjectName;
            assertion.Subject.SubjectConfirmation = new SubjectConfirmation();
            assertion.Subject.SubjectConfirmation.Method = Constants.SubjectConfirmationMethod;

            #endregion

            #region Conditions

            assertion.Conditions = new Conditions();
            assertion.Conditions.NotBefore = currentTimeText;
            assertion.Conditions.NotOnOrAfter = endTimeText;
            assertion.Conditions.AudienceRestriction = new AudienceRestriction();
            assertion.Conditions.AudienceRestriction.Audiences =
            [
                new Audience() { Value = @"https://www.microsoft.com" },
                new Audience() { Value =@"https://www.google.com" },
            ];

            #endregion

            #region Attributes

            assertion.AttributeStatement = new AttributeStatement();
            assertion.AttributeStatement.Attributes = new List<Attribute>();

            #region SubjectId

            var subjecIdAttribute = new Attribute();
            subjecIdAttribute.Name = "urn:oasis:names:tc:xspa:1.0:subject:subject-id";
            subjecIdAttribute.NameFormat = Constants.AttributeName_Format;
            subjecIdAttribute.AttributeValues = new List<AttributeValue>();
            subjecIdAttribute.AttributeValues.Add(new AttributeValue() { Value = Environment.UserName });

            #endregion

            #region Org Id

            var orgIdAttribute = new Attribute();
            orgIdAttribute.Name = Constants.AtrributeName_OrganizationId;
            orgIdAttribute.NameFormat = Constants.AttributeName_Format;
            orgIdAttribute.AttributeValues = new List<AttributeValue>();
            orgIdAttribute.AttributeValues.Add(new AttributeValue() { Value = "Org Id" });

            #endregion

            #region Org Name

            var orgNameAttribute = new Attribute();
            orgNameAttribute.Name = Constants.AtrributeName_OrganizationName;
            orgNameAttribute.NameFormat = Constants.AttributeName_Format;
            orgNameAttribute.AttributeValues = new List<AttributeValue>();
            orgNameAttribute.AttributeValues.Add(new AttributeValue() { Value = "Org Name" });

            #endregion

            #region Home Community Id

            var hcIdAttribute = new Attribute();
            hcIdAttribute.Name = Constants.AtrributeName_HomeCommunityId;
            hcIdAttribute.NameFormat = Constants.AttributeName_Format;
            hcIdAttribute.AttributeValues = new List<AttributeValue>();
            hcIdAttribute.AttributeValues.Add(new AttributeValue() { Value = "HC Id" });

            #endregion

            #region Purpose

            var purpose = new PurposeOfUse();
            purpose.Code = "99-102";
            purpose.CodeSystem = "1.3.6.1.4.1.21367.3000.4.1";
            purpose.CodeSystemName = "connectathon PurposeOfUse codes";
            purpose.DisplayName = "EMERGENCY";

            var purposeAttribute = new Attribute();
            purposeAttribute.Name = Constants.AttributeName_PurposeOfUse;
            purposeAttribute.NameFormat = Constants.AttributeName_Format;
            purposeAttribute.AttributeValues = new List<AttributeValue>();
            purposeAttribute.AttributeValues.Add(new AttributeValue() { Value = GetSerializedXmlElement(purpose) });

            #endregion

            #region Role

            var role = new SubjectRole();
            role.Code = "007";
            role.CodeSystem = "MIG";
            role.CodeSystemName = "Bond, James Bond";
            role.DisplayName = "B-)";

            var subjectRoleAttribute = new Attribute();
            subjectRoleAttribute.Name = Constants.AtrributeName_SubjectRole;
            subjectRoleAttribute.NameFormat = Constants.AttributeName_Format;
            subjectRoleAttribute.AttributeValues = new List<AttributeValue>();
            subjectRoleAttribute.AttributeValues.Add(new AttributeValue() { Value = GetSerializedXmlElement(role) });

            #endregion

            assertion.AttributeStatement.Attributes.Add(subjecIdAttribute);
            assertion.AttributeStatement.Attributes.Add(orgIdAttribute);
            assertion.AttributeStatement.Attributes.Add(orgNameAttribute);
            assertion.AttributeStatement.Attributes.Add(hcIdAttribute);
            assertion.AttributeStatement.Attributes.Add(subjectRoleAttribute);
            assertion.AttributeStatement.Attributes.Add(purposeAttribute);

            #endregion

            #region Auth Statement

            assertion.AuthnStatement = new AuthnStatement();
            assertion.AuthnStatement.AuthnInstant = currentTimeText;
            assertion.AuthnStatement.SessionNotOnOrAfter = endTimeText;
            assertion.AuthnStatement.AuthnContext = new AuthnContext();
            assertion.AuthnStatement.AuthnContext.AuthnContextClassRef = new AuthnContextClassRef();
            assertion.AuthnStatement.AuthnContext.AuthnContextClassRef.Value = Constants.AuthContextClassReference;

            #endregion

            return assertion;
        }

        public static bool ValidateAssertionSignature(string assertionXml)
        {
            var xmlDoc = new XmlDocument()
            {
                PreserveWhitespace = true,
            };

            xmlDoc.LoadXml(assertionXml);

            var signatureNodes = xmlDoc.GetElementsByTagName(Constants.Signature_ElementName);

            // There should be one and only one signature for assertion
            if (signatureNodes.Count <= 0)
                throw new CryptographicException("Verification failed: No Signature was found in the document.");

            if (signatureNodes.Count >= 2)
                throw new CryptographicException("Verification failed: More that one signature was found for the document.");

            var signedXml = new SignedXml(xmlDoc);
            signedXml.LoadXml(signatureNodes[0] as XmlElement);
            var isValid = signedXml.CheckSignature(); // signedXml.CheckSignature(rsa);

            //var keyInfo = signedXml.KeyInfo;
            //var xe = keyInfo.GetEnumerator(typeof(KeyInfoX509Data));
            //var ye = keyInfo.GetEnumerator(typeof(RSAKeyValue));
            //if (xe.MoveNext()) {
            //    X509Certificate2 cert;
            //    var x = xe.Current as KeyInfoX509Data;
            //    if (x != null && x.Certificates != null)
            //        cert = x.Certificates[0] as X509Certificate2;
            //}

            //if (ye.MoveNext())
            //{
            //    var y = ye.Current as RSAKeyValue;
            //    RSA rsa;
            //    if (y != null && y.Key != null)
            //        rsa = y.Key;
            //}

            return isValid;
        }

        public static T Deserialize<T>(string xmlString, XmlSerializer xmlSerializer = null)
        {
            if (!string.IsNullOrEmpty(xmlString))
            {
                T deserializedObject;

                using (TextReader reader = new StringReader(xmlString))
                {
                    try
                    {
                        if (xmlSerializer == null)
                            xmlSerializer = Utils.GetSerializer<T>();

                        deserializedObject = (T)xmlSerializer.Deserialize(reader);
                        return deserializedObject;
                    }
                    catch { }
                }
            }

            return default(T);
        }

        public static string GetSerializedXmlElement<T>(T obj, XmlSerializer xmlSerializer = null)
        {
            XmlDocument doc = new XmlDocument();
            using (MemoryStream stream = new MemoryStream())
            {
                if (xmlSerializer == null)
                    xmlSerializer = GetSerializer<T>();
                xmlSerializer.Serialize(stream, obj);
                stream.Position = 0;
                doc.Load(stream);
                RemoveUnwantedAttribute(doc.DocumentElement);
                return doc.DocumentElement.OuterXml;
            }
        }

        public static CodedValue GetCodedValue(XmlReader reader)
        {
            CodedValue codedValue = null;

            if (reader.LocalName.Equals(nameof(PurposeOfUse)))
                codedValue = new PurposeOfUse();
            else if (reader.LocalName.Equals(nameof(SubjectRole)))
                codedValue = new SubjectRole();
            else
                throw new InvalidOperationException("No coded value");

            if (reader.HasAttributes)
            {
                reader.MoveToNextAttribute();

                do
                {
                    switch (reader.LocalName)
                    {
                        case "code":
                            codedValue.Code = reader.ReadContentAsString();
                            break;

                        case "codeSystem":
                            codedValue.CodeSystem = reader.ReadContentAsString();
                            break;

                        case "codeSystemName":
                            codedValue.CodeSystemName = reader.ReadContentAsString();
                            break;

                        case "displayName":
                            codedValue.DisplayName = reader.ReadContentAsString();
                            break;

                        default:
                            break;
                    }
                }
                while (reader.MoveToNextAttribute());

                reader.Read();
            }

            return codedValue;
        }

        #endregion

        #region Extension Methods

        public static XUADetails GetXUADetailsFromAssertion(this CustomAssertion assertion)
        {
            var details = new XUADetails();

            #region Issuer

            details.Issuer = assertion.Issuer.Value;

            #endregion

            #region User details

            if (assertion.Subject == null || assertion.Subject.NameId == null || string.IsNullOrEmpty(assertion.Subject.NameId.Value))
                throw new ApplicationException("Assertion missing Subject");

            if (!string.IsNullOrEmpty(assertion.Subject.NameId.Value))
            {
                details.Alias = assertion.Subject.NameId.Value;
                details.NameIdFormat = assertion.Subject.NameId.Format;
            }

            if (!string.IsNullOrEmpty(details.NameIdFormat))
            {
                if (details.NameIdFormat.EndsWith("X509SubjectName"))
                {
                    SetDomainInfoFromDistinguisedUserName(assertion.Subject.NameId.Value, details);
                }
                else if (details.NameIdFormat.EndsWith("emailAddress"))
                    details.User = details.Alias;
                else
                    details.User = details.Alias;
            }
            else
                details.User = details.Alias;

            #endregion

            #region Misc

            if (assertion.TryGetValues(Constants.AtrributeName_OrganizationId, out List<string> orgIds))
                details.OrganizationId = orgIds.First();

            if (assertion.TryGetValues(Constants.AtrributeName_OrganizationName, out List<string> orgNames))
                details.OrganizationName = orgNames.First();

            if (assertion.TryGetValues(Constants.AtrributeName_HomeCommunityId, out List<string> communities))
                details.HomeCommunityId = communities.First();

            if (assertion.TryGetValues(Constants.AttributeName_PurposeOfUse, out List<PurposeOfUse> purposes))
                details.Purposes = purposes;

            if (assertion.TryGetValues(Constants.AtrributeName_SubjectRole, out List<SubjectRole> roles))
                details.Roles = roles;

            #endregion

            return details;
        }

        public static bool VerifyValidityPeriod(this CustomAssertion assertion)
        {
            if (assertion != null)
            {
                if (!string.IsNullOrEmpty(assertion.IssueInstant))
                {
                    if (!IsValidTimestamp(assertion.IssueInstant, true))
                        return false;
                }

                if (assertion.Conditions != null)
                {
                    if (!string.IsNullOrEmpty(assertion.Conditions.NotBefore))
                    {
                        if (!IsValidTimestamp(assertion.Conditions.NotBefore, true))
                            return false;
                    }

                    if (!string.IsNullOrEmpty(assertion.Conditions.NotOnOrAfter))
                    {
                        if (!IsValidTimestamp(assertion.Conditions.NotOnOrAfter, false))
                            return false;
                    }
                }

                if (assertion.AuthnStatement != null)
                {
                    if (!string.IsNullOrEmpty(assertion.AuthnStatement.AuthnInstant))
                    {
                        if (!IsValidTimestamp(assertion.AuthnStatement.AuthnInstant, true))
                            return false;
                    }

                    if (!string.IsNullOrEmpty(assertion.AuthnStatement.SessionNotOnOrAfter))
                    {
                        if (!IsValidTimestamp(assertion.AuthnStatement.SessionNotOnOrAfter, false))
                            return false;
                    }
                }
            }

            return true;
        }

        public static string GetSignedAssertionXmlAsText(this CustomAssertion assertion, X509Certificate2 signingCertificate, KeyClauseType keyClauseType)
        {
            #region Get Assertion Xml

            var serializer = GetSerializer<CustomAssertion>();
            var xmlText = string.Empty;
            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    serializer.Serialize(writer, assertion);
                    xmlText = sww.ToString();
                }
            }

            var xmlDoc = new XmlDocument() { PreserveWhitespace = true };
            xmlDoc.LoadXml(xmlText);
            RemoveUnwantedAttribute(xmlDoc.DocumentElement);

            #endregion

            #region Compute Signature

            var signedXml = new SignedXml(xmlDoc);
            signedXml.KeyInfo = new KeyInfo();
            signedXml.KeyInfo.AddClause(keyClauseType == KeyClauseType.X509 ?
                new KeyInfoX509Data(signingCertificate, X509IncludeOption.EndCertOnly) :
                new RSAKeyValue(signingCertificate.GetRSAPublicKey()));

            signedXml.SigningKey = signingCertificate.GetRSAPrivateKey();
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;

            // Create a reference to be signed.
            Reference reference = new Reference();
            reference.DigestMethod = SignedXml.XmlDsigSHA1Url;
            reference.Uri = $"#{assertion.Id}";

            // Add Transformation
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigExcC14NTransform());
            signedXml.AddReference(reference);

            signedXml.ComputeSignature();

            #endregion

            #region Add signature to assertion

            var signature = signedXml.GetXml();
            var signatureFragment = xmlDoc.CreateDocumentFragment();
            signatureFragment.AppendChild(signature);
            var issuerNode = xmlDoc.ChildNodes[1].ChildNodes[0];
            xmlDoc.ChildNodes[1].InsertAfter(signatureFragment, issuerNode);

            #endregion

            var output = xmlDoc.DocumentElement.OuterXml;
            return output;
        }

        private static bool TryGetValues<T>(this CustomAssertion assertion, string key, out List<T> data) where T : class
        {
            data = new List<T>();
            var typeName = typeof(T).Name.ToLowerInvariant();

            if (assertion != null && assertion.AttributeStatement != null && assertion.AttributeStatement.Attributes != null)
            {
                var attributes = assertion.AttributeStatement.Attributes.Where(x => x.Name == key);

                foreach (var attribute in attributes)
                {
                    if (attribute.AttributeValues != null)
                    {
                        foreach (var attributeValue in attribute.AttributeValues)
                        {
                            switch (typeName)
                            {
                                case "string":
                                    data.Add(attributeValue.Value as T);
                                    break;

                                default:
                                    data.Add(Deserialize<T>(attributeValue.Value));
                                    break;
                            }
                        }
                    }
                }
            }

            return data.Count > 0;
        }

        #endregion

        #region Private Methods

        private static bool IsValidTimestamp(string timestamp, bool isStart)
        {
            try
            {
                var dateTimeOffset = XmlConvert.ToDateTimeOffset(timestamp);
                return isStart ? dateTimeOffset < DateTimeOffset.UtcNow : dateTimeOffset > DateTimeOffset.UtcNow;
            }
            catch
            {
                return false;
            }
        }

        private static void SetDomainInfoFromDistinguisedUserName(string value, XUADetails details)
        {
            var domainInfo = new X509DomainInfo();
            X500DistinguishedName dName = new X500DistinguishedName(value);
            details.User = dName.Decode(X500DistinguishedNameFlags.Reversed);
            
            var relativeNames = new Dictionary<string, string>();

            var subParts = value.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var subPart in subParts) 
            {
                var dataParts = subPart.Split('=', StringSplitOptions.RemoveEmptyEntries);

                if (dataParts.Length != 2)
                    continue;

                var key = dataParts[0].TrimStart().TrimEnd();
                var data = dataParts[1].TrimStart().TrimEnd();

                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(data))
                    continue;

                if (relativeNames.ContainsKey(key))
                {
                    if (key.ToUpperInvariant() == "DC")
                    {
                        // Append
                        relativeNames[key] = $"{relativeNames[key]}.{data}";
                    }
                    else
                    {
                        // Overwrite
                        relativeNames[key] = data;
                    }
                }
                else
                {
                    relativeNames.Add(key, data);
                }
            }

            //get the user name and domain
            foreach (var relativeName in relativeNames)
            {
                switch (relativeName.Key.ToUpperInvariant())
                {
                    case "DC":
                        domainInfo.IncomingDomainName = relativeName.Value;
                        break;
                    case "UID":
                        domainInfo.UserId = relativeName.Value;
                        break;
                    case "CN":
                        domainInfo.UserName = relativeName.Value;
                        break;
                }
            }

            details.DomainInfo = domainInfo;
        }        

        private static void RemoveUnwantedAttribute(XmlElement element)
        {
            element.RemoveAttribute(Constants.xsiAttribute);
            element.RemoveAttribute(Constants.xsdAttribute);
        }

        #endregion
    }
}
