using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{

    public class adm_integrationMap : EntityTypeConfiguration<adm_integration>
    {
        public adm_integrationMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.Password)
                .HasMaxLength(500);

            this.Property(t => t.Masking)
                .HasMaxLength(500);
            this.Property(t => t.SMTP)
                .HasMaxLength(500);


            // Table & Column Mappings
            this.ToTable("adm_integration");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.Masking).HasColumnName("Masking");
            this.Property(t => t.SMTP).HasColumnName("SMTP");
            this.Property(t => t.PortNo).HasColumnName("PortNo");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.adm_integration)
                .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.adm_integration)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.adm_integration1)
                .HasForeignKey(d => d.ModifiedBy);
        }
    }
}
