using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Hyland.SOAP.Client
{
    internal class ResponseInspector : IClientMessageInspector, IEndpointBehavior
    {
        #region IEndpointBehavior

        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            
        }

        void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
        {
            
        }

        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(this);
        }

        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            
        }

        #endregion

        #region IClientMessageInspector

        object IClientMessageInspector.BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            // No need to do anything
            return null;
        }

        void IClientMessageInspector.AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (reply.Headers.Count > 0)
            {
                // Check for security header
                int secHeaderIndex = reply.Headers.FindHeader("Security", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
                if (secHeaderIndex < 0)
                    return;
                else
                    reply.Headers.RemoveAt(secHeaderIndex);
            }
        }

        #endregion
    }
}
