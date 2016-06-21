using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using ReportApp.Core.Entities;

namespace ReportApp.Core.Concrete
{
    public class EfDbContext: IdentityDbContext<Staff>
    {
        public EfDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<AttachedFile> AttachedFiles { get; set; }

        public static EfDbContext Create()
        {
            return new EfDbContext();
        }
    }
}
