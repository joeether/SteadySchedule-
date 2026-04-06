using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace SteadySchedule.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Schedule> Schedules { get; set; }
    }
}
