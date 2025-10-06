using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acuo.CloudVNA.Models
{
    public class VNADbContext : DbContext
    {
        public VNADbContext(DbContextOptions<VNADbContext> options) : base(options) { }

        public DbSet<UserActivity> UserActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.ApplyConfiguration(new UserActivityConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
