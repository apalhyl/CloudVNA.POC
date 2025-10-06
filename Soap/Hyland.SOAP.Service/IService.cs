using CoreWCF;
using Hyland.SOAP.Core;
using System;
using System.Runtime.Serialization;

namespace Hyland.SOAP.Service
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract(Action = "urn:ihe:iti:2007:RegistryStoredQuery", ReplyAction = "urn:ihe:iti:2007:RegistryStoredQueryResponse")]
        Message Query(Message request);
    }

    public class Service : IService
    {
        public Message Query(Message request)
        {
            var details = XUAOperationContext.Current.XUADetails;
            var requestObject = ServiceUtils.GetQueryRequestFromMessage(request);
            var responseObject = new AdhocQueryResponse();
            responseObject.AddError("errCode", "errContext");
            var response = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "urn:ihe:iti:2007:RegistryStoredQueryResponse", responseObject,
                    new CustomMessageSerializer(Utils.GetSerializer<AdhocQueryResponse>()));
            return response;
        }
    }
}
