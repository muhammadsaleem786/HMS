using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class ipd_procedure_mfMap : EntityTypeConfiguration<ipd_procedure_mf>
    {
        public ipd_procedure_mfMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PatientProcedure)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Location)
                .HasMaxLength(250);

            this.Property(t => t.Physician)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Assistant)
                .HasMaxLength(50);

            this.Property(t => t.Notes)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("ipd_procedure_mf");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.AdmissionId).HasColumnName("AdmissionId");
            this.Property(t => t.AppointmentId).HasColumnName("AppointmentId");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.PatientProcedure).HasColumnName("PatientProcedure");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.Time).HasColumnName("Time");
            this.Property(t => t.CPTCodeId).HasColumnName("CPTCodeId");
            this.Property(t => t.CPTCodeDropdownId).HasColumnName("CPTCodeDropdownId");
            this.Property(t => t.Location).HasColumnName("Location");
            this.Property(t => t.Physician).HasColumnName("Physician");
            this.Property(t => t.Assistant).HasColumnName("Assistant");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.Discount).HasColumnName("Discount");
            this.Property(t => t.PaidAmount).HasColumnName("PaidAmount");
            this.Property(t => t.ServiceId).HasColumnName("ServiceId");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.ipd_procedure_mf)
                .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.ipd_procedure_mf)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.ipd_procedure_mf1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasRequired(t => t.ipd_admission)
                .WithMany(t => t.ipd_procedure_mf)
                .HasForeignKey(d => new { d.AdmissionId, d.CompanyId });
            this.HasOptional(t => t.sys_drop_down_value)
                .WithMany(t => t.ipd_procedure_mf)
                .HasForeignKey(d => new { d.CPTCodeId, d.CPTCodeDropdownId });
            this.HasRequired(t => t.emr_service_mf)
                .WithMany(t => t.ipd_procedure_mf)
                .HasForeignKey(d => new { d.ServiceId, d.CompanyId });

        }
    }
}
