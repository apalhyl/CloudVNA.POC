using Acuo.IHEAudit.DAL.DTO;
using Acuo.IHEAudit.DAL.Models;
using Medallion.Threading;
using Medallion.Threading.Postgres;
using Medallion.Threading.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acuo.IHEAudit.DAL
{
    internal class IHEAuditRepository
    {
        private string _sqlConnection;
        private bool _isSqlServer;
        private PooledDbContextFactory<AcuoMedIHEAuditContext> _dbContextPool;
        private bool _initialized = false;

        #region Private constructor

        private IHEAuditRepository()
        {

        }

        #endregion

        #region Public Methods

        public bool IsInialized { get { return _initialized; } }

        public void Initialize(string sqlConnection, bool isSqlServer = true)
        {
            _sqlConnection = sqlConnection;
            _isSqlServer = isSqlServer;

            DbContextOptions<AcuoMedIHEAuditContext> options;

            if (isSqlServer)
                options = new DbContextOptionsBuilder<AcuoMedIHEAuditContext>().UseSqlServer(sqlConnection).Options;
            else
                options = new DbContextOptionsBuilder<AcuoMedIHEAuditContext>().UseNpgsql(sqlConnection).Options;

            _dbContextPool = new PooledDbContextFactory<AcuoMedIHEAuditContext>(options);
        }

        public IDistributedLock GetDistributedLock(string context, int timeOutInSeconds = 30)
        {
            if (!IsInialized)
                throw new InvalidOperationException("Not initialized");

            if (_isSqlServer)
                return new SqlDistributedLock(context, _sqlConnection, options => { options.UseMultiplexing(); });
            else
                return new PostgresDistributedLock(new PostgresAdvisoryLockKey(context, allowHashing: true), 
                    _sqlConnection, options => { options.UseMultiplexing(); });
        }

        public AcuoMedIHEAuditContext GetDbContext()
        {
            if (!IsInialized)
                throw new InvalidOperationException("Not initialized");

            return _dbContextPool.CreateDbContext();
        }

        #endregion

        #region Stored Procedures

        /// <summary>
        /// [dbo].[SP_CONFIG_SELECT]
        /// </summary>
        /// <returns></returns>
        public Task<List<TConfig>> GetConfigAsync()
        {
            using (var dbContext = GetDbContext())
            {
                return dbContext.TConfigs.AsNoTracking().ToListAsync();
            }
        }

        /// <summary>
        /// [dbo].[SP_TASKS_GET_BY_TARGET_ID]
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="taskStatus"></param>
        /// <returns></returns>
        public Task<List<TaskResult>> GetTasksByTargetAsync(Guid targetId, int taskStatus = 1)
        {
            using (var dbContext = GetDbContext())
            {
                return dbContext.TTasks.AsNoTracking()
                    .Where(x => x.TaskTargetId.Equals(targetId) && x.TaskStatus == taskStatus)
                    .OrderBy(x => x.TaskId)
                    .Take(50)
                    .Select(x => new TaskResult()
                    {
                        TaskId = x.TaskId,
                        TaskStatus = x.TaskStatus,
                        TaskTargetId = x.TaskTargetId,
                        TaskIheAuditId = x.TaskIheAuditId,
                        TaskLastError = x.TaskLastError,
                        TaskQueuedTime = x.TaskQueuedTime,
                        TaskXmlMessage = x.TaskXmlMessage
                    })
                    .ToListAsync();
            }
        }

        /// <summary>
        /// [dbo].[SP_TASKS_UPDATE]
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskStatus"></param>
        /// <param name="taskLastError"></param>
        /// <returns></returns>
        public Task<int> UpdateTaskAsync(int taskId, int taskStatus, string taskLastError)
        {
            using (var dbContext = GetDbContext())
            {
                return dbContext.TTasks.AsNoTracking()
                    .Where(x => x.TaskId == taskId)
                    .ExecuteUpdateAsync(setters =>
                        setters
                            .SetProperty(b => b.TaskStatus, taskStatus)
                            .SetProperty(b => b.TaskLastError, taskLastError)
                            .SetProperty(b => b.TaskRunCount, b => b.TaskRunCount + 1));
            }
        }

        /// <summary>
        /// [dbo].[SP_EVENT_AUDIT_CHECK]
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="eventType"></param>
        /// <param name="eventAction"></param>
        /// <returns></returns>
        public async Task<bool?> IsEventSupported(int eventId, int eventType, int eventAction)
        {
            bool? result = null;

            if (eventAction >= 1 && eventAction <= 5)
            {
                using (var dbContext = GetDbContext())
                {
                    var tEvent = await dbContext.TEvents.AsNoTracking().FirstOrDefaultAsync(x => x.EvId == eventId && x.EvType == eventType);
                    
                    if (tEvent != null)
                    {
                        switch(eventAction)
                        {
                            case 1:
                                return tEvent.EvLogCreate;

                            case 2:
                                return tEvent.EvLogRead;

                            case 3:
                                return tEvent.EvLogUpdate;

                            case 4:
                                return tEvent.EvLogDelete;

                            case 5:
                                return tEvent.EvLogExecute;

                            default:
                                return null;
                        }
                    }
                }
            }

            return result;
        }

        #region Controversional Stored Proces

        //[dbo].[USP_RethrowError]

        #endregion

        #region Excluded Stored Procedures

        //[dbo].[GetSqlTransAppLock]
        //[dbo].[ReleaseSqlTransAppLock]

        #endregion

        #endregion

        #region Singleton Design Pattern

        public static IHEAuditRepository Current
        {
            get { return RepositoryManager.instance; }
        }

        private class RepositoryManager
        {
            static RepositoryManager() { }
            internal static readonly IHEAuditRepository instance = new IHEAuditRepository();
        }

        # endregion
    }
}
