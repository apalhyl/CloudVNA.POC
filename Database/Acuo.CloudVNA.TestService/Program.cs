using Acuo.CloudVNA.TestService;
using Serilog;
using System.Net;

namespace TestService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args);

            if (!Environment.UserInteractive)
                builder.UseWindowsService();

            builder.Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = Utilities.GetAppSettingsConfiguration();
            var httpPort = config.GetValue("Port_Http", 0);

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder =>
                {
                    builder
                    .ConfigureKestrel(serveroptions =>
                    {
                        if (httpPort > 0)
                            serveroptions.Listen(IPAddress.Any, httpPort);                        
                    })
                    .UseKestrel()
                    .ConfigureLogging(logBuilder =>
                    {
                        logBuilder.ClearProviders();
                        logBuilder.AddSerilog();
                    })
                    .UseStartup<Startup>();
                }
            );
        }
    }
}
