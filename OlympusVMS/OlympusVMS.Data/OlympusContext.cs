using Microsoft.EntityFrameworkCore;
using OlympusVMS.Utils.Models;

namespace OlympusVMS.Data;

public class OlympusContext : DbContext
{
    public OlympusContext(DbContextOptions<OlympusContext> options) :  base(options)
    {
        
    }
    
    public DbSet<GuestRecord> GuestRecords => Set<GuestRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Olympus");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OlympusContext).Assembly);
    }
}