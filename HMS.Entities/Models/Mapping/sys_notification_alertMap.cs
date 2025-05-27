using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class sys_notification_alertMap : EntityTypeConfiguration<sys_notification_alert>
    {
        public sys_notification_alertMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EmailFrom)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.EmailTo)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Subject)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.Body)
                .IsRequired();

            this.Property(t => t.AttachmentPath)
                .HasMaxLength(5000);

            // Table & Column Mappings
            this.ToTable("sys_notification_alert");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.TypeID).HasColumnName("TypeID");
            this.Property(t => t.EmailFrom).HasColumnName("EmailFrom");
            this.Property(t => t.EmailTo).HasColumnName("EmailTo");
            this.Property(t => t.Subject).HasColumnName("Subject");
            this.Property(t => t.Body).HasColumnName("Body");
            this.Property(t => t.SentTime).HasColumnName("SentTime");
            this.Property(t => t.FailureCount).HasColumnName("FailureCount");
            this.Property(t => t.IsRead).HasColumnName("IsRead");
            this.Property(t => t.AttachmentPath).HasColumnName("AttachmentPath");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");

            // Relationships
            this.HasOptional(t => t.adm_company)
                .WithMany(t => t.sys_notification_alert)
                .HasForeignKey(d => d.CompanyID);

        }
    }
}
