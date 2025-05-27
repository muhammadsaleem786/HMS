using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class emr_prescription_mfMap : EntityTypeConfiguration<emr_prescription_mf>
    {
        public emr_prescription_mfMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Notes)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("emr_prescription_mf");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.IsTemplate).HasColumnName("IsTemplate");
            this.Property(t => t.AppointmentDate).HasColumnName("AppointmentDate");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.DoctorId).HasColumnName("DoctorId");
            this.Property(t => t.FollowUpDate).HasColumnName("FollowUpDate");
            this.Property(t => t.FollowUpTime).HasColumnName("FollowUpTime");
            this.Property(t => t.FollowUpNotes).HasColumnName("FollowUpNotes");
            this.Property(t => t.IsCreateAppointment).HasColumnName("IsCreateAppointment");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ClinicId).HasColumnName("ClinicId");
            this.Property(t => t.Day).HasColumnName("Day");
            
            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.emr_prescription_mf)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.emr_prescription_mf)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.adm_user_mf1)
                .WithMany(t => t.emr_prescription_mf1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasRequired(t => t.emr_patient_mf)
                .WithMany(t => t.emr_prescription_mf)
                .HasForeignKey(d => new { d.PatientId, d.CompanyID });

        }
    }
}
