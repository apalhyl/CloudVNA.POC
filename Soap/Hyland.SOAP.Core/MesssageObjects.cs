using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Hyland.SOAP.Core
{
    [XmlRoot("AdhocQueryRequest", Namespace = "urn:oasis:names:tc:ebxml-regrep:xsd:query:3.0")]
    public class AdhocQueryRequest
    {

        [XmlElement("AdhocQuery", Namespace = "urn:oasis:names:tc:ebxml-regrep:xsd:rim:3.0")]
        public AdhocQuery Query { get; set; }

        [XmlElement("ResponseOption", Namespace = "urn:oasis:names:tc:ebxml-regrep:xsd:query:3.0")]
        public ResponseOption Response { get; set; }

        public AdhocQueryRequest()
        {
            Query = new AdhocQuery();
            Query.Home = "urn:oid:1.2.3";
        }
    }

    [XmlRoot("AdhocQueryResponse", Namespace = "urn:oasis:names:tc:ebxml-regrep:xsd:query:3.0")]
    public class AdhocQueryResponse : IXmlSerializable
    {
        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces m_xmlns;

        public AdhocQueryResponse()
        {
            m_xmlns = new XmlSerializerNamespaces();
            m_xmlns.Add("query", "urn:oasis:names:tc:ebxml-regrep:xsd:query:3.0");
            m_status = new ResponseStatus(ResponseStatus.ResponseStatusType.SUCCESS);
            m_registryErrorList = new RegistryErrorList();
        }

        private ResponseStatus m_status;
        private RegistryErrorList m_registryErrorList;

        [XmlAttribute("status")]
        public string Status
        {
            get { return m_status.Status; }
            set { m_status.Status = value; }
        }

        [XmlElement("RegistryErrorList", IsNullable = false)]
        public RegistryErrorList RegErrorList
        {
            get { return m_registryErrorList; }
            set
            {
                if (value == null) return;
                m_registryErrorList = value;
            }
        }

        public void AddError(string errorCode, string codeContext)
        {
            if (m_registryErrorList == null)
                m_registryErrorList = new RegistryErrorList();

            m_registryErrorList.AddRegistryError(new RegistryError(errorCode, codeContext));
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.HasAttributes)
            {
                reader.MoveToNextAttribute();

                do
                {
                    if (reader.LocalName.Equals("status"))
                    {
                        m_status.Status = reader.ReadContentAsString();
                        break;
                    }
                }
                while (reader.MoveToNextAttribute());

            }

            while (reader.Read())
            {
                reader.MoveToContent();

                if (reader.LocalName.Equals("RegistryErrorList") && reader.NodeType == XmlNodeType.Element)
                {
                    var serializer = Utils.GetSerializer<RegistryErrorList>();
                    m_registryErrorList = (RegistryErrorList)serializer.Deserialize(reader);
                }
            }
        }

        /// <summary>
        /// Serializes AdhocQueryResponse
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("status", this.Status);
            if (m_registryErrorList != null && m_registryErrorList.RegistryErrors != null && m_registryErrorList.RegistryErrors.Count > 0)
            {
                var serializer = Utils.GetSerializer<RegistryErrorList>();
                serializer.Serialize(writer, m_registryErrorList);
            }
        }
    }

    [XmlRoot("AdhocQuery", Namespace = "urn:oasis:names:tc:ebxml-regrep:xsd:rim:3.0")]
    public class AdhocQuery
    {
        public AdhocQuery()
        {
            Slots = new List<Slot>();
        }

        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("home")]
        public string Home { get; set; }

        [XmlElement("Slot")]
        public List<Slot> Slots { get; set; }
    }

    public class ResponseOption
    {
        public ResponseOption()
        {
            responseType = "LeafClass";
            returnComposedObjects = "true";
        }

        private string responseType;
        [XmlAttribute("returnType")]
        public string ResponseType
        {
            get { return responseType; }
            set { responseType = value; }
        }

        private string returnComposedObjects;
        [XmlAttribute("returnComposedObjects")]
        public string ReturnComposedObjects
        {
            get { return returnComposedObjects; }
            set { returnComposedObjects = value; }
        }
    }

    [XmlRoot("Slot", Namespace = "urn:oasis:names:tc:ebxml-regrep:xsd:rim:3.0")]
    public class Slot
    {

        private String name;
        private ValueList values;


        public Slot()
        {
            values = new ValueList();
        }
        
        public Slot(String name)
        {
            Name = name;
            Values = new ValueList();
        }

        [XmlAttribute("name")]
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public void AddValue(String value)
        {
            this.values.AddValue(value);
        }

        public void RemoveValue(String value)
        {
            this.values.RemoveValue(value);
        }

        [XmlElement("ValueList")]
        public ValueList Values
        {
            get
            {
                return values;
            }
            set
            {
                values = value;
            }
        }

        public override string ToString()
        {
            return "\n\tSlot:\n\t\tName = " + Name + "\n\t\tValues = " + Values;
        }
    }

    public class ValueList
    {

        private List<String> m_Values;

        public ValueList()
        {
            m_Values = new List<String>();
        }


        public void AddValue(String value)
        {
            this.m_Values.Add(value);
        }

        public void RemoveValue(string value)
        {
            this.m_Values.Remove(value);
        }

        public bool EditValue(int index, string item)
        {
            if (m_Values == null || m_Values.Count < index + 1)
                return false;
            else
            {
                m_Values[index] = item;
                return true;
            }
        }

        public string GetValue(int item)
        {
            return m_Values[item];
        }

        [XmlElement("Value")]
        public string[] Values
        {
            get
            {
                string[] values = new string[m_Values.Count];
                m_Values.CopyTo(values);
                return values;
            }
            set
            {
                if (value == null) return;

                string[] values = value;
                m_Values.Clear();

                foreach (string stringvalue in values)
                {
                    m_Values.Add(stringvalue);
                }
            }
        }

        public override string ToString()
        {
            return "ValueList: = " + String.Join(",", m_Values);
        }
    }

    public class ResponseStatus
    {
        public enum ResponseStatusType
        {
            SUCCESS = 0,
            PARTIALSUCCESS = 1,
            FAILURE = 2
        }

        public ResponseStatus(ResponseStatusType type)
        {
            m_type = type;
            switch (type)
            {
                case ResponseStatusType.FAILURE:
                    m_status = RESPONSE_STATUS_TYPE_FAILURE;
                    break;
                case ResponseStatusType.SUCCESS:
                    m_status = RESPONSE_STATUS_TYPE_SUCCESS;
                    break;
                case ResponseStatusType.PARTIALSUCCESS:
                    m_status = CONST_RESPONSE_STATUS_TYPE_PARTIALSUCCESS;
                    break;
            }
        }

        private ResponseStatusType m_type;
        public ResponseStatusType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }


        private String m_status;
        public String Status
        {
            get { return m_status; }
            set { m_status = value; }
        }

        public const string RESPONSE_STATUS_TYPE_FAILURE = "urn:oasis:names:tc:ebxml-regrep:ResponseStatusType:Failure";
        public const string RESPONSE_STATUS_TYPE_SUCCESS = "urn:oasis:names:tc:ebxml-regrep:ResponseStatusType:Success";
        public const string CONST_RESPONSE_STATUS_TYPE_PARTIALSUCCESS = "urn:ihe:iti:2007:ResponseStatusType:PartialSuccess";
    }

    [XmlRoot("RegistryErrorList", Namespace = "urn:oasis:names:tc:ebxml-regrep:xsd:rs:3.0")]
    public class RegistryErrorList
    {
        public RegistryErrorList()
        {
            RegistryErrors = new List<RegistryError>();
            m_highestSeverity = new RegistryError();
        }

        private RegistryError m_highestSeverity;
        [XmlAttribute("highestSeverity")]
        public string HighestSeverity
        {
            get { return m_highestSeverity.Severity; }
            set { m_highestSeverity.Severity = value; }
        }

        [XmlElement("RegistryError")]
        public List<RegistryError> RegistryErrors { get; set; }


        /// <summary>
        /// Adds a fully formed slot.
        /// </summary>
        public void AddRegistryError(RegistryError registryError)
        {
            m_highestSeverity = registryError;

            if (RegistryErrors == null)
                RegistryErrors = new List<RegistryError>();

            RegistryErrors.Add(registryError);
        }
    }

    [XmlRoot("RegistryError")]
    public class RegistryError
    {
        public const string SEVERITY_TYPE_ERROR = "urn:oasis:names:tc:ebxml-regrep:ErrorSeverityType:Error";

        public RegistryError()
        {
            Severity = SEVERITY_TYPE_ERROR;
        }

        public RegistryError(string errorCode, string codeContext)
        {
            ErrorCode = errorCode;
            CodeContext = codeContext;
            Severity = SEVERITY_TYPE_ERROR;
        }

        [XmlAttribute("errorCode")]
        public string ErrorCode { get; set; }

        [XmlAttribute("codeContext")]
        public string CodeContext { get; set; }

        [XmlAttribute("location")]
        public string Location { get; set; }

        [XmlAttribute("severity")]
        public string Severity { get; set; }
    }
}
