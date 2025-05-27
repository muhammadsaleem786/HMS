using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class ipd_admission_notesMap : EntityTypeConfiguration<ipd_admission_notes>
    {
        public ipd_admission_notesMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.OnBehalfOf)
                .HasMaxLength(50);

            this.Property(t => t.Note)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("ipd_admission_notes");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.AdmissionId).HasColumnName("AdmissionId");
            this.Property(t => t.AppointmentId).HasColumnName("AppointmentId");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.OnBehalfOf).HasColumnName("OnBehalfOf");
            this.Property(t => t.Note).HasColumnName("Note");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.ipd_admission_notes)
                .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.ipd_admission_notes)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.ipd_admission_notes1)
                .HasForeignKey(d => d.ModifiedBy);            
            this.HasRequired(t => t.ipd_admission)
                .WithMany(t => t.ipd_admission_notes)
                .HasForeignKey(d => new { d.AdmissionId, d.CompanyId });

        }
    }
}
