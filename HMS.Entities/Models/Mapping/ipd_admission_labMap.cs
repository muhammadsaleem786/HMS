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

            this.Property(t => t.Notes)
                .HasMaxLength(250);

            this.Property(t => t.OrderingPhysician)
                .HasMaxLength(50);

            this.Property(t => t.Parameter)
                .HasMaxLength(50);

            this.Property(t => t.ResultValues)
                .HasMaxLength(50);

            this.Property(t => t.ABN)
                .HasMaxLength(50);

            this.Property(t => t.Flags)
                .HasMaxLength(50);

            this.Property(t => t.Comment)
                .HasMaxLength(50);

            this.Property(t => t.TestPerformedAt)
                .HasMaxLength(50);

            this.Property(t => t.TestDescription)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("ipd_admission_lab");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.AdmissionId).HasColumnName("AdmissionId");
            this.Property(t => t.AppointmentId).HasColumnName("AppointmentId");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.LabTypeId).HasColumnName("LabTypeId");
            this.Property(t => t.LabTypeDropdownId).HasColumnName("LabTypeDropdownId");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.CollectDate).HasColumnName("CollectDate");
            this.Property(t => t.TestDate).HasColumnName("TestDate");
            this.Property(t => t.ReportDate).HasColumnName("ReportDate");
            this.Property(t => t.OrderingPhysician).HasColumnName("OrderingPhysician");
            this.Property(t => t.Parameter).HasColumnName("Parameter");
            this.Property(t => t.ResultValues).HasColumnName("ResultValues");
            this.Property(t => t.ABN).HasColumnName("ABN");
            this.Property(t => t.Flags).HasColumnName("Flags");
            this.Property(t => t.Comment).HasColumnName("Comment");
            this.Property(t => t.TestPerformedAt).HasColumnName("TestPerformedAt");
            this.Property(t => t.TestDescription).HasColumnName("TestDescription");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.StatusId).HasColumnName("StatusId");
            this.Property(t => t.StatusDropdownId).HasColumnName("StatusDropdownId");
            this.Property(t => t.ResultId).HasColumnName("ResultId");
            this.Property(t => t.ResultDropdownId).HasColumnName("ResultDropdownId");

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
            this.HasOptional(t => t.sys_drop_down_value)
                .WithMany(t => t.ipd_admission_lab)
                .HasForeignKey(d => new { d.LabTypeId, d.LabTypeDropdownId });
            this.HasRequired(t => t.sys_drop_down_value1)
                .WithMany(t => t.ipd_admission_lab1)
                .HasForeignKey(d => new { d.StatusId, d.StatusDropdownId });
            this.HasRequired(t => t.sys_drop_down_value2)
                .WithMany(t => t.ipd_admission_lab2)
                .HasForeignKey(d => new { d.ResultId, d.ResultDropdownId });

        }
    }
}
