using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class ipd_admission_labMap : EntityTypeConfiguration<ipd_admission_lab>
    {
        public ipd_admission_labMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("ipd_admission_lab");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.AdmissionId).HasColumnName("AdmissionId");
            this.Property(t => t.AppointmentId).HasColumnName("AppointmentId");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.PriorityId).HasColumnName("PriorityId");
            this.Property(t => t.PriorityDropdownId).HasColumnName("PriorityDropdownId");
            this.Property(t => t.ClinicalNotes).HasColumnName("ClinicalNotes");
            this.Property(t => t.CatalogId).HasColumnName("CatalogId");
            this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo");
            this.Property(t => t.ReferringPhysician).HasColumnName("ReferringPhysician");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            // Relationships
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.ipd_admission_lab)
                .HasForeignKey(d => d.CreatedBy);

            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.ipd_admission_lab1)
                .HasForeignKey(d => d.ModifiedBy);

            this.HasRequired(t => t.ipd_admission)
                .WithMany(t => t.ipd_admission_lab)
                .HasForeignKey(d => new { d.AdmissionId, d.CompanyId });

            this.HasRequired(t => t.sys_drop_down_value)
                .WithMany(t => t.ipd_admission_lab)
                .HasForeignKey(d => new { d.PriorityId, d.PriorityDropdownId });

            this.HasRequired(t => t.adm_company)
               .WithMany(t => t.ipd_admission_lab)
               .HasForeignKey(d => d.CompanyId);

            this.HasRequired(t => t.emr_investigation)
              .WithMany(t => t.ipd_admission_lab)
              .HasForeignKey(d => new { d.CatalogId, d.CompanyId });
        }
    }
}
