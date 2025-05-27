using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class ipd_admission_medicationMap : EntityTypeConfiguration<ipd_admission_medication>
    {
        public ipd_admission_medicationMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Prescription)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Refills)
                .HasMaxLength(50);

            this.Property(t => t.BillTo)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ipd_admission_medication");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.AdmissionId).HasColumnName("AdmissionId"); this.Property(t => t.AppointmentId).HasColumnName("AppointmentId");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.ItemId).HasColumnName("ItemId");
            this.Property(t => t.Prescription).HasColumnName("Prescription");
            this.Property(t => t.PrescriptionDate).HasColumnName("PrescriptionDate");
            this.Property(t => t.QuantityRequested).HasColumnName("QuantityRequested");
            this.Property(t => t.Refills).HasColumnName("Refills");
            this.Property(t => t.IsRequestNow).HasColumnName("IsRequestNow");
            this.Property(t => t.BillTo).HasColumnName("BillTo");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.ipd_admission_medication)
                .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.ipd_admission_medication)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.ipd_admission_medication1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasRequired(t => t.adm_item)
                .WithMany(t => t.ipd_admission_medication)
                .HasForeignKey(d => new { d.ItemId, d.CompanyId });           
            this.HasRequired(t => t.ipd_admission)
                .WithMany(t => t.ipd_admission_medication)
                .HasForeignKey(d => new { d.AdmissionId, d.CompanyId });

        }
    }
}
