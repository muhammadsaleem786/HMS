using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class ipd_input_outputMap : EntityTypeConfiguration<ipd_input_output>
    {
        public ipd_input_outputMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("ipd_input_output");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.AdmissionId).HasColumnName("AdmissionId");
            this.Property(t => t.AppointmentId).HasColumnName("AppointmentId");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.IntakeValue).HasColumnName("IntakeValue");
            this.Property(t => t.Time).HasColumnName("Time");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.IntakeId).HasColumnName("IntakeId");
            this.Property(t => t.IntakeDropdownId).HasColumnName("IntakeDropdownId");
            this.Property(t => t.OutputId).HasColumnName("OutputId");
            this.Property(t => t.OutputDropdownId).HasColumnName("OutputDropdownId");
            this.Property(t => t.Value).HasColumnName("Value");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.adm_company)
               .WithMany(t => t.ipd_input_output)
               .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.ipd_input_output)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.ipd_input_output1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasRequired(t => t.ipd_admission)
                .WithMany(t => t.ipd_input_output)
                .HasForeignKey(d => new { d.AdmissionId, d.CompanyId });
                      

        }
    }
}
