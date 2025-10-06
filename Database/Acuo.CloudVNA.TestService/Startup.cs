using Quartz;
using Serilog.Events;
using Serilog;
using Serilog.Core;
using System.Net;
using Acuo.CloudVNA.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Acuo.CloudVNA.TestService.Jobs;
using Medallion.Threading.SqlServer;
using Medallion.Threading.Postgres;
using Medallion.Threading;

namespace Acuo.CloudVNA.TestService
{
    public class Startup
    {
        private readonly IConfiguration config;

        public Startup(IWebHostEnvironment hostEnvironment)
        {
            config = Utilities.GetAppSettingsConfiguration();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(config);
            var connectionString = config.GetValue("ConnectionString",string.Empty);

            #region Add database context

            var provider = config.GetValue("provider", DatabaseProvider.SqlServer.ToString());
            if (Enum.TryParse(provider, true, out DatabaseProvider databaseProvider))
            {
                var migrationAssembly = Utilities.GetMigrationAssembly(databaseProvider);
                services.AddDbContext<VNADbContext>(options =>
                {
                    if (databaseProvider == DatabaseProvider.SqlServer)
                    {
                        options.UseSqlServer(connectionString,
                            x => x.MigrationsAssembly(migrationAssembly));
                    }
                    else
                    {
                        options.UseNpgsql(connectionString,
                            x => x.MigrationsAssembly(migrationAssembly));
                    }
                });
            }
            else
                throw new InvalidOperationException("Invalid database provider");

            #endregion

            #region Distributed Lock

            IDistributedLock distributedLock;
            if (databaseProvider == DatabaseProvider.SqlServer)
                distributedLock = new SqlDistributedLock("processUserActivity", connectionString);
            else
                distributedLock = new PostgresDistributedLock(new PostgresAdvisoryLockKey("processUserActivity", allowHashing: true), connectionString);

            services.AddSingleton(distributedLock);

            #endregion

            #region Web API

            //services.AddCors();
            //services.AddMvc(options =>
            //{
            //    options.EnableEndpointRouting = false;
            //    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            //});

            //services.AddEndpointsApiExplorer();

            #endregion

            #region Setup scheduled  jobs

            services.AddQuartz(q =>
            {
                foreach (Type jobType in Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(mytype => mytype.GetInterfaces()
                    .Contains(typeof(IJob))))
                {
                    var jobKey = new JobKey(jobType.Name);
                    q.AddJob(jobType, jobKey, c => c.StoreDurably(true));
                }
            });

            services.AddQuartzHostedService(options =>
            {
                options.AwaitApplicationStarted = true;
                options.WaitForJobsToComplete = true;
            });

            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IServiceProvider services)
        {
            ConfigureSeriLog();

            var logger = loggerFactory.CreateLogger<Startup>();
            logger.LogInformation("Starting Acuo Cloud VNA Test Service.");

            var dbContext = services.GetService<VNADbContext>();

            // DbContext migrate in debug mode only, production to use scripts.
            dbContext.Database.Migrate();

            //app.UseCors(builder => builder.AllowAnyOrigin());
            //app.UseMvc();

            #region Initiate jobs

            var schedulerFactory = services.GetService<ISchedulerFactory>();
            var scheduler = schedulerFactory.GetScheduler().Result;
            var jobName = nameof(ProcessUnderDbLockJob);
            var jobKey = new JobKey(jobName);
            if (scheduler.CheckExists(jobKey).Result)
                scheduler.TriggerJob(jobKey);

            #endregion

            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault | SecurityProtocolType.Tls12;
            ServicePointManager.UseNagleAlgorithm = false;

            logger.LogInformation("Started Acuo Cloud VNA Test Service.");
        }        

        private void ConfigureSeriLog()
        {
            var location = Utilities.GetBasePath();
            var file = Path.Combine(location, "logs", "test_service.log");
            Log.Logger = GetFileLogger(file);
        }

        private static Logger GetFileLogger(string file)
        {
            var loggingConfig = new LoggerConfiguration().MinimumLevel.Debug();
            return loggingConfig.WriteTo.File(file, rollingInterval: RollingInterval.Day).CreateLogger();
        }
    }
}
