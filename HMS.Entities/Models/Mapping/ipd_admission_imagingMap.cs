using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class ipd_admission_imagingMap : EntityTypeConfiguration<ipd_admission_imaging>
    {
        public ipd_admission_imagingMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Notes)
                .HasMaxLength(250);

            this.Property(t => t.Image)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("ipd_admission_imaging");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.AdmissionId).HasColumnName("AdmissionId");
            this.Property(t => t.AppointmentId).HasColumnName("AppointmentId");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.ImagingTypeId).HasColumnName("ImagingTypeId");
            this.Property(t => t.ImagingTypeDropdownId).HasColumnName("ImagingTypeDropdownId");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.StatusId).HasColumnName("StatusId");
            this.Property(t => t.StatusDropdownId).HasColumnName("StatusDropdownId");
            this.Property(t => t.ResultId).HasColumnName("ResultId");
            this.Property(t => t.ResultDropdownId).HasColumnName("ResultDropdownId");
            this.Property(t => t.Image).HasColumnName("Image");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.ipd_admission_imaging)
                .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.ipd_admission_imaging)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.ipd_admission_imaging1)
                .HasForeignKey(d => d.ModifiedBy);            
            this.HasRequired(t => t.ipd_admission)
                .WithMany(t => t.ipd_admission_imaging)
                .HasForeignKey(d => new { d.AdmissionId, d.CompanyId });
            this.HasRequired(t => t.sys_drop_down_value)
                .WithMany(t => t.ipd_admission_imaging)
                .HasForeignKey(d => new { d.ImagingTypeId, d.ImagingTypeDropdownId });
            this.HasRequired(t => t.sys_drop_down_value1)
                .WithMany(t => t.ipd_admission_imaging1)
                .HasForeignKey(d => new { d.StatusId, d.StatusDropdownId });
            this.HasRequired(t => t.sys_drop_down_value2)
                .WithMany(t => t.ipd_admission_imaging2)
                .HasForeignKey(d => new { d.ResultId, d.ResultDropdownId });

        }
    }
}
