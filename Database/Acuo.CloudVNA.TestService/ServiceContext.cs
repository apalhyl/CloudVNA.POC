using Acuo.CloudVNA.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Acuo.CloudVNA.TestService
{
    public class ServiceContext
    {
        private ConcurrentDictionary<int, PooledDbContextFactory<VNADbContext>> dicomDbContextPools = 
            new ConcurrentDictionary<int, PooledDbContextFactory<VNADbContext>>();
        private object _lock = new object();

        public ServiceContext()
        {

        }

        #region Public methods

        public void SetupConnectionPools(List<DicomDbConnection> connections)
        {
            foreach (var conn in connections)
            {
                DbContextOptions<VNADbContext> options;

                if (conn.DatabaseProvider == DatabaseProvider.SqlServer)
                    options = new DbContextOptionsBuilder<VNADbContext>().UseSqlServer(conn.ConnectionString).Options;
                else
                    options = new DbContextOptionsBuilder<VNADbContext>().UseNpgsql(conn.ConnectionString).Options;

                dicomDbContextPools.TryAdd(conn.Id, new PooledDbContextFactory<VNADbContext>(options));
            }
        }

        public VNADbContext GetVNADbContext(int id)
        {

            lock (_lock)
            {
                if (dicomDbContextPools.TryGetValue(id, out PooledDbContextFactory<VNADbContext> pool))
                    return pool.CreateDbContext();
                else
                    throw new InvalidOperationException($"Dicom Db with id {id} not found");
            }
        }


        #endregion

        #region Singleton Design Pattern

        public static ServiceContext Current
        {
            get { return ServiceContextManager.instance; }
        }

        private class ServiceContextManager
        {
            static ServiceContextManager() { }
            internal static readonly ServiceContext instance = new ServiceContext();
        }

        # endregion
    }
}
