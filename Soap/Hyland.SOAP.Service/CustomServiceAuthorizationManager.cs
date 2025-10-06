using CoreWCF.IdentityModel.Claims;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Threading.Tasks;

namespace Hyland.SOAP.Service
{
    public class CustomServiceAuthorizationManager : ServiceAuthorizationManager
    {

        public override ValueTask<bool> CheckAccessAsync(OperationContext operationContext)
        {
            var requestEndpoint = operationContext.RequestContext.RequestMessage.Headers.To;
            Console.WriteLine(requestEndpoint);

            foreach (var claimSet in OperationContext.Current.ServiceSecurityContext.AuthorizationContext.ClaimSets)
            {
                X509CertificateClaimSet certificateClaimSet = claimSet as X509CertificateClaimSet;
                if (certificateClaimSet != null)
                {
                    //Get the actual certificate used by the client
                    Console.WriteLine(certificateClaimSet.X509Certificate);
                    break;
                }
            }

            return base.CheckAccessAsync(operationContext);
        }
    }
}
