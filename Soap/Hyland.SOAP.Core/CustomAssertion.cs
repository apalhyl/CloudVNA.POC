using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using static System.Reflection.Metadata.BlobBuilder;

namespace Hyland.SOAP.Core
{
    [XmlRoot(Constants.Assertion_Root, Namespace = Constants.Assertion_Namespace)]
    public class CustomAssertion
    {
        [XmlAttribute("ID")]
        public string Id { get; set; }

        [XmlAttribute]
        public string IssueInstant { get; set; }

        [XmlAttribute]
        public string Version { get; set; }

        [XmlElement]
        public Issuer Issuer { get; set; }

        [XmlElement]
        public Subject Subject { get; set; }

        [XmlElement]
        public Conditions Conditions { get; set; }

        [XmlElement]
        public AttributeStatement AttributeStatement { get; set; }

        [XmlElement]
        public AuthnStatement AuthnStatement { get; set; }
    }

    public class Issuer
    {
        [XmlAttribute]
        public string Format { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class Subject
    {
        [XmlElement(Constants.NameId_Element)]
        public NameId NameId { get; set; }

        [XmlElement]
        public SubjectConfirmation SubjectConfirmation { get; set; }
    }

    public class NameId
    {
        [XmlAttribute]
        public string Format { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class SubjectConfirmation
    {
        [XmlAttribute]
        public string Method { get; set; }
    }

    public class Conditions
    {
        [XmlAttribute]
        public string NotBefore { get; set; }

        [XmlAttribute]
        public string NotOnOrAfter { get; set; }

        [XmlElement]
        public AudienceRestriction AudienceRestriction { get; set; }
    }

    public class AudienceRestriction
    {
        [XmlElement(nameof(Audience))]
        public List<Audience> Audiences { get; set; }
    }

    public class Audience
    {
        [XmlText]
        public string Value;
    }

    public class AuthnStatement
    {
        [XmlAttribute]
        public string AuthnInstant { get; set; }

        [XmlAttribute]
        public string SessionNotOnOrAfter { get; set; }

        [XmlElement]
        public AuthnContext AuthnContext { get; set; }
    }

    public class AuthnContext
    {
        [XmlElement]
        public AuthnContextClassRef AuthnContextClassRef { get; set; }
    }

    public class AuthnContextClassRef
    {
        [XmlText]
        public string Value;
    }

    public class AttributeStatement
    {
        [XmlElement(nameof(Attribute))]

        public List<Attribute> Attributes { get; set; }
    }

    public class Attribute : IXmlSerializable
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string NameFormat { get; set; }

        [XmlElement(nameof(AttributeValue))]

        public List<AttributeValue> AttributeValues { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            #region Attributes

            if (reader.HasAttributes)
            {
                reader.MoveToNextAttribute();

                do
                {
                    switch (reader.LocalName)
                    {
                        case nameof(Name):
                            Name = reader.ReadContentAsString();
                            break;

                        case nameof(NameFormat):
                            NameFormat = reader.ReadContentAsString();
                            break;

                        default:
                            break;
                    }
                }
                while (reader.MoveToNextAttribute());
            }

            #endregion

            #region Elements

            AttributeValues = new List<AttributeValue>();

            reader.Read();
            reader.MoveToContent();

            bool continueReading = true;
            var serializer = Utils.GetSerializer<AttributeValue>();

            while (continueReading)
            {
                if (reader.LocalName.Equals(nameof(AttributeValue)) && reader.NodeType == XmlNodeType.Element)
                {
                    if (Name.Equals(Constants.AttributeName_PurposeOfUse, StringComparison.OrdinalIgnoreCase))
                    {
                        reader.Read();
                        var codedValue = Utils.GetCodedValue(reader);
                        var attributeValue = new AttributeValue();
                        attributeValue.Value = Utils.GetSerializedXmlElement(codedValue as PurposeOfUse);
                        AttributeValues.Add(attributeValue);
                    }
                    else if (Name.Equals(Constants.AtrributeName_SubjectRole, StringComparison.OrdinalIgnoreCase))
                    {
                        reader.Read();
                        var codedValue = Utils.GetCodedValue(reader);
                        var attributeValue = new AttributeValue();
                        attributeValue.Value = Utils.GetSerializedXmlElement(codedValue as SubjectRole);
                        AttributeValues.Add(attributeValue);
                    }
                    else
                    {
                        // Generic
                        AttributeValues.Add((AttributeValue)serializer.Deserialize(reader));
                    }
                }

                if (reader.LocalName.Equals(nameof(Attribute)) && reader.NodeType == XmlNodeType.EndElement)
                {
                    reader.ReadEndElement();
                    continueReading = false;
                }

                else if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName.Equals(nameof(Attribute)))
                        continueReading = false;
                    else
                        continueReading = reader.LocalName.Equals(nameof(AttributeValue));
                }

                else if (reader.NodeType == XmlNodeType.None)
                    continueReading = false;
                else
                {
                    reader.Read();
                    reader.MoveToContent();
                }
            }

            #endregion
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString(nameof(Name), Name);
            writer.WriteAttributeString(nameof(NameFormat), NameFormat);



            foreach (var item in AttributeValues)
            {
                writer.WriteStartElement(nameof(AttributeValue));

                if (Name.Equals(Constants.AttributeName_PurposeOfUse, StringComparison.OrdinalIgnoreCase) ||
                    Name.Equals(Constants.AtrributeName_SubjectRole, StringComparison.OrdinalIgnoreCase))
                {
                    XmlReader x = null;
                    try
                    {
                        x = XmlReader.Create(new StringReader(item.Value));
                        writer.WriteNode(x, true);
                    }
                    catch
                    {
                        writer.WriteString(item.Value);
                    }
                    finally
                    {
                        x.Close();
                    }
                }
                else
                {
                    writer.WriteString(item.Value);
                }


                writer.WriteEndElement();
            }
        }
    }

    public class AttributeValue
    {
        [XmlText]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = nameof(PurposeOfUse), Namespace = Constants.CodedValue_Namespace)]
    public class PurposeOfUse : CodedValue
    {
    }

    [XmlRoot(ElementName = nameof(SubjectRole), Namespace = Constants.CodedValue_Namespace)]
    public class SubjectRole : CodedValue
    {
    }

    public class CodedValue
    {
        [XmlAttribute("code")]
        public string Code { get; set; }

        [XmlAttribute("codeSystem")]
        public string CodeSystem { get; set; }

        [XmlAttribute("displayName")]
        public string DisplayName { get; set; }

        [XmlAttribute("codeSystemName")]
        public string CodeSystemName { get; set; }
    }

    public class XUADetails
    {
        public string Alias { get; set; }

        public string User { get; set; }

        public string NameIdFormat { get; set; }

        public string Issuer { get; set; }

        public string OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public string HomeCommunityId { get; set; }

        public X509DomainInfo DomainInfo { get; set; }

        public List<SubjectRole> Roles { get; set; }

        public List<PurposeOfUse> Purposes { get; set; }
    }

    public class X509DomainInfo
    {
        public string IncomingDomainName { get; set; }

        public string UserName { get; set; }

        public string UserId { get; set; }
    }
}
