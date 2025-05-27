using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class emr_patient_billMap : EntityTypeConfiguration<emr_patient_bill>
    {
        public emr_patient_billMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("emr_patient_bill");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.AppointmentId).HasColumnName("AppointmentId");
            this.Property(t => t.AdmissionId).HasColumnName("AdmissionId");            
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.ServiceId).HasColumnName("ServiceId");
            this.Property(t => t.BillDate).HasColumnName("BillDate");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.Discount).HasColumnName("Discount");
            this.Property(t => t.PaidAmount).HasColumnName("PaidAmount");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.DoctorId).HasColumnName("DoctorId");
            this.Property(t => t.OutstandingBalance).HasColumnName("OutstandingBalance");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.RefundAmount).HasColumnName("RefundAmount");
            this.Property(t => t.RefundDate).HasColumnName("RefundDate");
            // Relationships 
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.emr_patient_bill)
                .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.emr_patient_bill)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.emr_patient_bill1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasRequired(t => t.emr_service_mf)
                .WithMany(t => t.emr_patient_bill)
                .HasForeignKey(d => new { d.ServiceId, d.CompanyId });
            this.HasRequired(t => t.emr_patient_mf)
                .WithMany(t => t.emr_patient_bill)
                .HasForeignKey(d => new { d.PatientId, d.CompanyId });


            this.HasOptional(t => t.ipd_admission)
              .WithMany(t => t.emr_patient_bill)
              .HasForeignKey(d => new { d.AdmissionId, d.CompanyId });
        }
    }
}
