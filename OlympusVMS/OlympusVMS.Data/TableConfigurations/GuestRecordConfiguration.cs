using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlympusVMS.Utils.Models;

namespace OlympusVMS.Data.TableConfigurations;

public class GuestRecordConfiguration : IEntityTypeConfiguration<GuestRecord>
{
    public void Configure(EntityTypeBuilder<GuestRecord> e)
    {
        e.ToTable("GuestRecord");
        e.HasKey(x => x.RecordId);

        e.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);
        
        e.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);
        
        e.Property(x => x.EmailAddress)
            .IsRequired()
            .HasMaxLength(320);
        
        e.Property(x => x.RegisteredBy)
            .IsRequired()
            .HasMaxLength(200);
        
        e.Property(x => x.RegisteredAt)
            .IsRequired()
            .HasDefaultValueSql("SYSDATETIMEOFFSET()");
        
        e.Property(x => x.MeetingDurationInHours)
            .IsRequired(false);

        e.Property(x => x.Visited)
            .IsRequired()
            .HasDefaultValue(false);
        
        e.HasIndex(x => x.EmailAddress);
    }
}
