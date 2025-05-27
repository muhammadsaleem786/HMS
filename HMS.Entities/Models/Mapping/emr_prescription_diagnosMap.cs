using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class emr_prescription_diagnosMap : EntityTypeConfiguration<emr_prescription_diagnos>
    {
        public emr_prescription_diagnosMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Diagnos)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("emr_prescription_diagnos");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.DiagnosId).HasColumnName("DiagnosId");
            this.Property(t => t.Diagnos).HasColumnName("Diagnos");
            this.Property(t => t.PrescriptionId).HasColumnName("PrescriptionId");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.emr_prescription_diagnos)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.emr_prescription_diagnos)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.adm_user_mf1)
                .WithMany(t => t.emr_prescription_diagnos1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasRequired(t => t.emr_prescription_mf)
                .WithMany(t => t.emr_prescription_diagnos)
                .HasForeignKey(d => new { d.PrescriptionId, d.CompanyID });
        }
    }
}
