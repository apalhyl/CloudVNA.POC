using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Hyland.SOAP.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Hyland.SOAP.Service
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServiceModelServices();
            services.AddServiceModelMetadata();
            services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseServiceModel(serviceBuilder =>
            {
                serviceBuilder.AddService<Service>();
                serviceBuilder.AddServiceEndpoint<Service, IService>(ServiceUtils.GetBinding(true, false), "/service", serviceEndpoint =>
                {
                    serviceEndpoint.EndpointBehaviors.Add(new XUABehaviorInspector());
                });
                serviceBuilder.ConfigureServiceHostBase<Service>(options =>
                {
                    options.Credentials.ServiceCertificate.Certificate = Utils.GetCertificateCollectionUsingThumbprint("7316dab70f17e863f3eb7f81bb7752b83459ed7f")[0];
                    options.Credentials.ClientCertificate.Authentication.CertificateValidationMode = CoreWCF.Security.X509CertificateValidationMode.Custom;
                    options.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new CustomClientCertificateValidator();
                    options.Authorization.ServiceAuthorizationManager = new CustomServiceAuthorizationManager();
                });

                var serviceMetadataBehavior = app.ApplicationServices.GetRequiredService<ServiceMetadataBehavior>();
                serviceMetadataBehavior.HttpsGetEnabled = true;
                serviceMetadataBehavior.HttpGetUrl = new Uri("/service/metadata", UriKind.Relative);
            });
        }
    }
}
