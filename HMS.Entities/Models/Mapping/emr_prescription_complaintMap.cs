using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class emr_prescription_complaintMap : EntityTypeConfiguration<emr_prescription_complaint>
    {
        public emr_prescription_complaintMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Complaint)
                .IsRequired()
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("emr_prescription_complaint");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.ComplaintId).HasColumnName("ComplaintId");
            this.Property(t => t.Complaint).HasColumnName("Complaint");
            this.Property(t => t.PrescriptionId).HasColumnName("PrescriptionId");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.emr_prescription_complaint)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.emr_prescription_complaint)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.adm_user_mf1)
                .WithMany(t => t.emr_prescription_complaint1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasRequired(t => t.emr_prescription_mf)
                .WithMany(t => t.emr_prescription_complaint)
                .HasForeignKey(d => new { d.PrescriptionId, d.CompanyID });
        }
    }
}
