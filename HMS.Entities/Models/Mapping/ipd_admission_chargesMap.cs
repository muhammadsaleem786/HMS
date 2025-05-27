using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class ipd_admission_chargesMap : EntityTypeConfiguration<ipd_admission_charges>
    {
        public ipd_admission_chargesMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("ipd_admission_charges");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.AdmissionId).HasColumnName("AdmissionId");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.AppointmentId).HasColumnName("AppointmentId");
            this.Property(t => t.AnnualPE).HasColumnName("AnnualPE");
            this.Property(t => t.General).HasColumnName("General");
            this.Property(t => t.Medical).HasColumnName("Medical");
            this.Property(t => t.ICUCharges).HasColumnName("ICUCharges");
            this.Property(t => t.ExamRoom).HasColumnName("ExamRoom");
            this.Property(t => t.PrivateWard).HasColumnName("PrivateWard");
            this.Property(t => t.RIP).HasColumnName("RIP");
            this.Property(t => t.OtherAllCharges).HasColumnName("OtherAllCharges");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.ipd_admission_charges)
                .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.ipd_admission_charges)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.ipd_admission_charges1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasOptional(t => t.emr_appointment_mf)
                .WithMany(t => t.ipd_admission_charges)
                .HasForeignKey(d => new { d.AppointmentId, d.CompanyId });
            this.HasRequired(t => t.ipd_admission)
                .WithMany(t => t.ipd_admission_charges)
                .HasForeignKey(d => new { d.AdmissionId, d.CompanyId });

        }
    }
}
