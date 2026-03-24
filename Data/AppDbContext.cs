using ae_reporting.api.Models;
using Microsoft.EntityFrameworkCore;

namespace ae_reporting.api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<AdverseEvent> AdverseEvents { get; set; }
}
