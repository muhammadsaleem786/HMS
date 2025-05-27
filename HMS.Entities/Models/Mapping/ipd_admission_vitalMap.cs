using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class ipd_admission_vitalMap : EntityTypeConfiguration<ipd_admission_vital>
    {
        public ipd_admission_vitalMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.BP)
                .HasMaxLength(50);

            this.Property(t => t.SPO2)
                .HasMaxLength(50);

            this.Property(t => t.HeartRate)
                .HasMaxLength(50);

            this.Property(t => t.RespiratoryRate)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ipd_admission_vital");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.AdmissionId).HasColumnName("AdmissionId");
            this.Property(t => t.AppointmentId).HasColumnName("AppointmentId");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.DateRecorded).HasColumnName("DateRecorded");
            this.Property(t => t.Temperature).HasColumnName("Temperature");
            this.Property(t => t.Weight).HasColumnName("Weight");
            this.Property(t => t.Height).HasColumnName("Height");
            this.Property(t => t.BP).HasColumnName("BP");
            this.Property(t => t.SPO2).HasColumnName("SPO2");
            this.Property(t => t.HeartRate).HasColumnName("HeartRate");
            this.Property(t => t.RespiratoryRate).HasColumnName("RespiratoryRate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.TimeRecorded).HasColumnName("TimeRecorded");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.ipd_admission_vital)
                .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.ipd_admission_vital)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.ipd_admission_vital1)
                .HasForeignKey(d => d.ModifiedBy);            
            this.HasRequired(t => t.ipd_admission)
                .WithMany(t => t.ipd_admission_vital)
                .HasForeignKey(d => new { d.AdmissionId, d.CompanyId });

        }
    }
}
