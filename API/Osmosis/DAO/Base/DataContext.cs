using Microsoft.EntityFrameworkCore;
using Osmosis.Models;

namespace Osmosis.DAO.Base
{
    public class DataContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActiveGuids>().ToTable(tb => tb.HasTrigger("updateExpirationDate"));
            modelBuilder.Entity<Doctors>().ToTable(tb => tb.HasTrigger("updateDailyApointments"));
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Users> Users => Set<Users>();
        public DbSet<UserRoles> UserRoles => Set<UserRoles>();
        public DbSet<ActiveGuids> ActiveGuids => Set<ActiveGuids>();
        public DbSet<Doctors> Doctors => Set<Doctors>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Status> Status => Set<Status>();
        public DbSet<DailyAppointments> DailyAppointments => Set<DailyAppointments>();
    }
}
