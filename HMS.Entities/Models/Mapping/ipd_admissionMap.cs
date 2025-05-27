using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class ipd_admissionMap : EntityTypeConfiguration<ipd_admission>
    {
        public ipd_admissionMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AdmissionNo)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Location)
                .HasMaxLength(150);

            this.Property(t => t.ReasonForVisit)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("ipd_admission");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.AdmissionNo).HasColumnName("AdmissionNo");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.AdmissionTypeId).HasColumnName("AdmissionTypeId");
            this.Property(t => t.AdmissionTypeDropdownId).HasColumnName("AdmissionTypeDropdownId");
            this.Property(t => t.DoctorId).HasColumnName("DoctorId");
            this.Property(t => t.AdmissionDate).HasColumnName("AdmissionDate");
            this.Property(t => t.AdmissionTime).HasColumnName("AdmissionTime");
            this.Property(t => t.DischargeDate).HasColumnName("DischargeDate");
            this.Property(t => t.DischargeTime).HasColumnName("DischargeTime");
            this.Property(t => t.Location).HasColumnName("Location");
            this.Property(t => t.ReasonForVisit).HasColumnName("ReasonForVisit");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            this.Property(t => t.TypeId).HasColumnName("TypeId");
            this.Property(t => t.WardTypeId).HasColumnName("WardTypeId");
            this.Property(t => t.WardTypeDropdownId).HasColumnName("WardTypeDropdownId");
            this.Property(t => t.BedId).HasColumnName("BedId");
            this.Property(t => t.BedDropdownId).HasColumnName("BedDropdownId");
            this.Property(t => t.RoomId).HasColumnName("RoomId");
            this.Property(t => t.RoomDropdownId).HasColumnName("RoomDropdownId");


            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.ipd_admission)
                .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.ipd_admission)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.ipd_admission1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasRequired(t => t.emr_patient_mf)
                .WithMany(t => t.ipd_admission)
                .HasForeignKey(d => new { d.PatientId, d.CompanyId });
            this.HasOptional(t => t.sys_drop_down_value)
                .WithMany(t => t.ipd_admission)
                .HasForeignKey(d => new { d.AdmissionTypeId, d.AdmissionTypeDropdownId });


            this.HasOptional(t => t.sys_drop_down_value1)
               .WithMany(t => t.ipd_admission1)
               .HasForeignKey(d => new { d.WardTypeId, d.WardTypeDropdownId });
            this.HasOptional(t => t.sys_drop_down_value2)
             .WithMany(t => t.ipd_admission2)
             .HasForeignKey(d => new { d.BedId, d.BedDropdownId });
            this.HasOptional(t => t.sys_drop_down_value3)
          .WithMany(t => t.ipd_admission3)
          .HasForeignKey(d => new { d.RoomId, d.RoomDropdownId });

        }
    }
}
