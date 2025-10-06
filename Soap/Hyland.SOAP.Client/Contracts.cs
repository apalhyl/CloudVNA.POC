using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyland.SOAP.Client
{
    [ServiceContract]
    public interface IServiceContract
    {
        [OperationContract(Action = "urn:ihe:iti:2007:RegistryStoredQuery", ReplyAction = "urn:ihe:iti:2007:RegistryStoredQueryResponse")]
        Message Query(Message request);
    }
}
