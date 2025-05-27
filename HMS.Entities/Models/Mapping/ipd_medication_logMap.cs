using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class ipd_medication_logMap : EntityTypeConfiguration<ipd_medication_log>
    {
        public ipd_medication_logMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("ipd_medication_log");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.AdmissionId).HasColumnName("AdmissionId");
            this.Property(t => t.AppointmentId).HasColumnName("AppointmentId");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.Time).HasColumnName("Time");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.Dose).HasColumnName("Dose");
            this.Property(t => t.DrugId).HasColumnName("DrugId");
            this.Property(t => t.RouteId).HasColumnName("RouteId");
            this.Property(t => t.RouteDropdownId).HasColumnName("RouteDropdownId");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.adm_company)
               .WithMany(t => t.ipd_medication_log)
               .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.ipd_medication_log)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.ipd_medication_log1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasRequired(t => t.ipd_admission)
                .WithMany(t => t.ipd_medication_log)
                .HasForeignKey(d => new { d.AdmissionId, d.CompanyId });
            this.HasRequired(t => t.adm_item)
       .WithMany(t => t.ipd_medication_log)
       .HasForeignKey(d => new { d.DrugId, d.CompanyId });

        }
    }
}
