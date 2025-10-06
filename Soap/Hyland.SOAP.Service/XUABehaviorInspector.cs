using CoreWCF.Dispatcher;
using CoreWCF.IdentityModel.Tokens;
using CoreWCF;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Tracing;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System;
using Hyland.SOAP.Core;

namespace Hyland.SOAP.Service
{
    public class XUABehaviorInspector : IEndpointBehavior, IDispatchMessageInspector
    {
        #region IEndpointBehavior

        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            // Do nothing
        }

        void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
        {
            // Do nothing
        }

        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            // Do nothing
        }

        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
        }

        #endregion

        #region IDispatchMessageInspector

        void IDispatchMessageInspector.BeforeSendReply(ref Message reply, object correlationState)
        {
            // Do nothing
        }

        object IDispatchMessageInspector.AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            try
            {
                SetOperationContextValuesFromToken(request.Headers);
            }
            catch (Exception ex)
            {
                // Catch error and log for proper analysis
            }

            return null;
        }

        #endregion        

        private void SetOperationContextValuesFromToken(MessageHeaders messageHeaders)
        {
            // Retrieve Assertion text
            var assertionNodes = ServiceUtils.GetAssertionHeaderNode(messageHeaders);
            if (assertionNodes != null && assertionNodes.Length > 0)
            {
                var assertionXml = ServiceUtils.GetAssertionHeaderText(assertionNodes);

                if (!string.IsNullOrEmpty(assertionXml))
                {
                    var isSignatureValid = Utils.ValidateAssertionSignature(assertionXml);
                    if (isSignatureValid)
                    {
                        var serializedAssertion = Utils.Deserialize<CustomAssertion>(assertionXml);
                        var isPeriodValid = serializedAssertion.VerifyValidityPeriod();
                        if (isPeriodValid)
                        {
                            var details = serializedAssertion.GetXUADetailsFromAssertion();

                            // Any app specific validation or certificate level validation

                            if (details != null)
                                XUAOperationContext.Current.SetXUADetails(details);
                        }
                    }
                }
            }
        }        
    }
}
