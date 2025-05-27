using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class emr_appointment_mfMap : EntityTypeConfiguration<emr_appointment_mf>
    {
        public emr_appointment_mfMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PatientProblem)
                .HasMaxLength(500);

            this.Property(t => t.Notes)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("emr_appointment_mf");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.PatientProblem).HasColumnName("PatientProblem");
            this.Property(t => t.DoctorId).HasColumnName("DoctorId");
            this.Property(t => t.AppointmentDate).HasColumnName("AppointmentDate");
            this.Property(t => t.AppointmentTime).HasColumnName("AppointmentTime");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.StatusId).HasColumnName("StatusId");
            this.Property(t => t.ReminderId).HasColumnName("ReminderId");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.TokenNo).HasColumnName("TokenNo");
            this.Property(t => t.IsAdmission).HasColumnName("IsAdmission");
            this.Property(t => t.IsAdmit).HasColumnName("IsAdmit");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.emr_appointment_mf)
                .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.emr_appointment_mf)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.emr_appointment_mf1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasRequired(t => t.adm_user_mf2)
             .WithMany(t => t.emr_appointment_mf2)
             .HasForeignKey(d => d.DoctorId);
            this.HasRequired(t => t.emr_patient_mf)
                .WithMany(t => t.emr_appointment_mf)
                .HasForeignKey(d => new { d.PatientId, d.CompanyId });

            

        }
    }
}
