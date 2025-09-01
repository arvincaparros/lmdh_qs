using LMDH_QS.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LMDH_QS
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<PatientsInformation> PatientsInformation { get; set; }
        public DbSet<Queue> Queues { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<DoctorNote> DoctorNotes { get; set; }

        public DbSet<PatientVitalRecord> PatientVitalRecord { get; set; }
    }
}
