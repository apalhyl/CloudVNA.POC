using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hyland.SOAP.Core;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Hyland.SOAP.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel(kestrelOptions =>
                {
                    kestrelOptions.ListenAnyIP(5001, listenOptions =>
                    {
                        listenOptions.UseHttps(httpsOptions =>
                        {
                            httpsOptions.ServerCertificate = Utils.GetCertificateCollectionUsingThumbprint("7316dab70f17e863f3eb7f81bb7752b83459ed7f")[0];
                            httpsOptions.ClientCertificateMode = Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode.AllowCertificate;
                            httpsOptions.ClientCertificateValidation = ClientCertificateValidation;

                        });
                    });
                });
                webBuilder.UseStartup<Startup>();
            });
        }
            

        private static bool ClientCertificateValidation(X509Certificate2 arg1, X509Chain arg2, SslPolicyErrors arg3)
        {
            return true;
        }
    }
}
