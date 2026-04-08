using Microsoft.EntityFrameworkCore;
using SteadySchedule.Domain;

namespace SteadySchedule.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<WeekTemplate> WeekTemplates { get; set; }
        public DbSet<WeekTemplateShift> WeekTemplateShifts { get; set; }

        public DbSet<Position> Positions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Shift)
                .WithMany()
                .HasForeignKey(a => a.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WeekTemplateShift>()
                .HasOne<WeekTemplate>()
                .WithMany()
                .HasForeignKey(wts => wts.WeekTemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}