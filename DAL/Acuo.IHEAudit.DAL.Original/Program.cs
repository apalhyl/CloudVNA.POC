using Acuo.IHEAudit.DAL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Acuo.IHEAudit.DAL
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Hello, World!");
                Console.WriteLine("Done");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }

    public class BloggingContextFactory : IDesignTimeDbContextFactory<AcuoMedIHEAuditContext>
    {
        public AcuoMedIHEAuditContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AcuoMedIHEAuditContext>();

            var isSqlServer = false;
            var SqlServer = "Application Name=AcuoMedIHEAudit;server=(local);integrated security=SSPI;database=AcuoMedIHEAudit;TrustServerCertificate=True;MultipleActiveResultSets=True;Transaction Binding=Explicit Unbind;Min Pool Size=1;Max Pool Size = 250";
            var Postgres = "User ID=postgres;Password=$ecureDefault2017;Server=localhost;Port=5432;Database=AcuoMedIHEAudit;";

            if (isSqlServer)
                optionsBuilder.UseSqlServer(SqlServer);
            else
                optionsBuilder.UseNpgsql(Postgres);

            return new AcuoMedIHEAuditContext(optionsBuilder.Options);
        }
    }
}
