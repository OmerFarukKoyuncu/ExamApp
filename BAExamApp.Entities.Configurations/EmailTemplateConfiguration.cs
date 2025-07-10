using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Entities.Configurations;
public class EmailTemplateConfiguration : AuditableEntityConfiguration<EmailTemplate>
{
    public override void Configure(EntityTypeBuilder<EmailTemplate> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.ModelName).IsRequired();
        builder.Property(x => x.Template).IsRequired();
    }
}
