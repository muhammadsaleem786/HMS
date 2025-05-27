using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using Repository.Pattern.Ef6;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities.Models.Mapping
{
    public class adm_reminder_mfMap : EntityTypeConfiguration<adm_reminder_mf>
    {
        public adm_reminder_mfMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("adm_reminder_mf");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.IsEnglish).HasColumnName("IsEnglish");
            this.Property(t => t.IsUrdu).HasColumnName("IsUrdu");
            this.Property(t => t.MessageBody).HasColumnName("MessageBody");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
             
            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.adm_reminder_mf)
                .HasForeignKey(d => d.CompanyId);           

            this.HasRequired(t => t.adm_user_mf)
               .WithMany(t => t.adm_reminder_mf)
               .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.adm_reminder_mf1)
                .HasForeignKey(d => d.ModifiedBy);

        }
    }
}
