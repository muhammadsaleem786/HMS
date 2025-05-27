using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class ipd_diagnosisMap : EntityTypeConfiguration<ipd_diagnosis>
    {
        public ipd_diagnosisMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Description)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("ipd_diagnosis");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.AdmissionId).HasColumnName("AdmissionId");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.IsVisitType).HasColumnName("IsVisitType");            
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.IsType).HasColumnName("IsType");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            // Relationships
            this.HasRequired(t => t.ipd_admission)
               .WithMany(t => t.ipd_diagnosis)
               .HasForeignKey(d => new { d.AdmissionId, d.CompanyID });
            this.HasRequired(t => t.adm_company)
              .WithMany(t => t.ipd_diagnosis)
              .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.ipd_diagnosis)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.adm_user_mf1)
                .WithMany(t => t.ipd_diagnosis1)
                .HasForeignKey(d => d.ModifiedBy);
        }        
    }
}
