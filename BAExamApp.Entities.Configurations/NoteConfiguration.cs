using Microsoft.EntityFrameworkCore;

namespace BAExamApp.Entities.Configurations;
public class NoteConfiguration:AuditableEntityConfiguration<Note>
{
    public override void Configure(EntityTypeBuilder<Note> builder)
    {
        base.Configure(builder);

        builder.Property(x=>x.Content).IsRequired();
        builder.Property(x=>x.Date).HasColumnType("datetime2").IsRequired();

        builder.HasOne(x=>x.Admin).WithMany(x=>x.Notes).HasForeignKey(x=>x.AdminId);
    }
}
