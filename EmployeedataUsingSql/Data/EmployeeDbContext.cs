using EmployeedataUsingSql.Model;
using Microsoft.EntityFrameworkCore;

namespace EmployeedataUsingSql.Data
{
    public class EmployeeDbContext: DbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options) { }
        public DbSet<EmployeeData> EmployeesData { get; set; }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<EmployeeData>()
        //         .HasNoKey();
        //}
    }
}
