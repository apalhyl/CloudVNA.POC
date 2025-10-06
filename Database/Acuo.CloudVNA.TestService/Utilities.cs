using Microsoft.EntityFrameworkCore.Storage;

namespace Acuo.CloudVNA.TestService
{
    public static class Utilities
    {
        internal static string GetBasePath()
        {
            using var processModule = System.Diagnostics.Process.GetCurrentProcess().MainModule;
            return Path.GetDirectoryName(processModule?.FileName);
        }

        internal static string GetMigrationAssembly(DatabaseProvider databaseProvider)
        {
            if (databaseProvider == DatabaseProvider.SqlServer)
                return typeof(SqlServer.MigrationAssemblyLocator).Assembly.GetName().Name;
            else
                return typeof(Postgres.MigrationAssemblyLocator).Assembly.GetName().Name;
        }

        internal static IConfiguration GetAppSettingsConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(GetBasePath())
                .AddStandardSources()
                .Build();
        }

        public static IConfigurationBuilder AddStandardSources(this IConfigurationBuilder builder)
        {
            return builder.AddJsonFile("appsettings.json").AddEnvironmentVariables();
        }
    }

    public class DicomDbConnection
    {
        public int Id { get; set; }

        public string ConnectionString { get; set; }

        public DatabaseProvider DatabaseProvider { get; set; }
    }
}
