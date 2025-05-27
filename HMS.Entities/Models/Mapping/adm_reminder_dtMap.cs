using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class adm_reminder_dtMap : EntityTypeConfiguration<adm_reminder_dt>
    {
        public adm_reminder_dtMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });
            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            // Table & Column Mappings
            this.ToTable("adm_reminder_dt");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ReminderId).HasColumnName("ReminderId");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.SMSTypeId).HasColumnName("SMSTypeId");
            this.Property(t => t.SMSTypeDropDownId).HasColumnName("SMSTypeDropDownId");
            this.Property(t => t.Value).HasColumnName("Value");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.TimeTypeId).HasColumnName("TimeTypeId");
            this.Property(t => t.TimeTypeDropDownId).HasColumnName("TimeTypeDropDownId");
            this.Property(t => t.BeforeAfter).HasColumnName("BeforeAfter");

            // Relationships
            this.HasRequired(t => t.adm_reminder_mf)
                .WithMany(t => t.adm_reminder_dt)
                .HasForeignKey(d => new { d.ReminderId, d.CompanyId });
            this.HasRequired(t => t.sys_drop_down_value)
                .WithMany(t => t.adm_reminder_dt)
                .HasForeignKey(d => new { d.SMSTypeId, d.SMSTypeDropDownId });

            this.HasRequired(t => t.sys_drop_down_value1)
               .WithMany(t => t.adm_reminder_dt1)
               .HasForeignKey(d => new { d.TimeTypeId, d.TimeTypeDropDownId });

            this.HasRequired(t => t.adm_user_mf)
               .WithMany(t => t.adm_reminder_dt)
               .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.adm_reminder_dt1)
                .HasForeignKey(d => d.ModifiedBy);
        }
    }
}
