using Acuo.CloudVNA.Models;
using Medallion.Threading;
using Quartz;

namespace Acuo.CloudVNA.TestService.Jobs
{
    [DisallowConcurrentExecution]
    public class ProcessUnderDbLockJob : IJob
    {
        private readonly VNADbContext _dbContext;
        private readonly ILogger _logger;
        private readonly IDistributedLock _lock;

        public ProcessUnderDbLockJob(VNADbContext dbContext, ILogger<ProcessUnderDbLockJob> logger, IDistributedLock distributedLock)
        {
            _logger = logger;
            _dbContext = dbContext;
            _lock = distributedLock;
        }

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                Parallel.For(0, 100, (i) => ExecuteSubTasks(i.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error executing ProcessUnderDbLockJob");
                _logger.LogError(ex.ToString());
            }

            return Task.CompletedTask;
        }

        private void ExecuteSubTasks(string userData)
        {
            using (_lock.Acquire())
            {
                var userActivity = new UserActivity()
                {
                    AppName = "TestService",
                    MachineName = Environment.MachineName,
                    Start = DateTimeOffset.UtcNow,
                    ProcessId = System.Diagnostics.Process.GetCurrentProcess().Id.ToString(),
                    UserData = userData
                };

                _dbContext.UserActivities.Add(userActivity);
                _dbContext.SaveChanges();
                
                // Mimicking long running functionality
                Thread.Sleep(30);
                
                userActivity.End = DateTimeOffset.UtcNow;
                _dbContext.SaveChanges();
            }
        }
    }
}
