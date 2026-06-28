using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{

    public class lab_result_mfMap : EntityTypeConfiguration<lab_result_mf>
    {
        public lab_result_mfMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });
            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(t => t.CompanyId);
            // Table & Column Mappings
            this.ToTable("lab_result_mf");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.CatalogId).HasColumnName("CatalogId");
            this.Property(t => t.PatientTestId).HasColumnName("PatientTestId");          
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.lab_result_mf)
                .HasForeignKey(d => d.CompanyId);

            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.lab_result_mf)
                .HasForeignKey(d => d.CreatedBy);

            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.lab_result_mf1)
                .HasForeignKey(d => d.ModifiedBy);

            this.HasRequired(t => t.emr_patient_mf)
              .WithMany(t => t.lab_result_mf)
              .HasForeignKey(d => new { d.PatientId, d.CompanyId });

            this.HasRequired(t => t.emr_investigation)
             .WithMany(t => t.lab_result_mf)
             .HasForeignKey(d => new { d.CatalogId, d.CompanyId });

            this.HasRequired(t => t.lab_patient_test)
             .WithMany(t => t.lab_result_mf)
             .HasForeignKey(d => new { d.PatientTestId, d.CompanyId });
        }
    }
}
