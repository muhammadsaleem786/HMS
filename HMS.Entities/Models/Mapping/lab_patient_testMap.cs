using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{

    public class lab_patient_testMap : EntityTypeConfiguration<lab_patient_test>
    {
        public lab_patient_testMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });
            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(t => t.ClinicalNotes);
            // Table & Column Mappings
            this.ToTable("lab_patient_test");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.PriorityId).HasColumnName("PriorityId");
            this.Property(t => t.PriorityDropdownId).HasColumnName("PriorityDropdownId");
            this.Property(t => t.StatusId).HasColumnName("StatusId");
            this.Property(t => t.StatusDropdownId).HasColumnName("StatusDropdownId");
            this.Property(t => t.CatalogId).HasColumnName("CatalogId");
            this.Property(t => t.Age).HasColumnName("Age");
            this.Property(t => t.SerialNo).HasColumnName("SerialNo");
            this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo");
            this.Property(t => t.SampleDate).HasColumnName("SampleDate");
            this.Property(t => t.CompletedDate).HasColumnName("CompletedDate");
            this.Property(t => t.InProgressDate).HasColumnName("InProgressDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ReferringPhysician).HasColumnName("ReferringPhysician");
            this.Property(t => t.DiscountAmount).HasColumnName("DiscountAmount");
            this.Property(t => t.Discount).HasColumnName("Discount");
            this.Property(t => t.Total).HasColumnName("Total");
            this.Property(t => t.AccessCode).HasColumnName("AccessCode");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.lab_patient_test)
                .HasForeignKey(d => d.CompanyId);

            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.lab_patient_test)
                .HasForeignKey(d => d.CreatedBy);

            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.lab_patient_test1)
                .HasForeignKey(d => d.ModifiedBy);

            this.HasRequired(t => t.sys_drop_down_value)
                .WithMany(t => t.lab_patient_test)
                .HasForeignKey(d => new { d.PriorityId, d.PriorityDropdownId });

            this.HasRequired(t => t.sys_drop_down_value1)
              .WithMany(t => t.lab_patient_test1)
              .HasForeignKey(d => new { d.StatusId, d.StatusDropdownId });

            this.HasRequired(t => t.emr_patient_mf)
                .WithMany(t => t.lab_patient_test)
                .HasForeignKey(d => new { d.PatientId, d.CompanyId });

            this.HasRequired(t => t.emr_investigation)
               .WithMany(t => t.lab_patient_test)
               .HasForeignKey(d => new { d.CatalogId, d.CompanyId });
        }
    }
}
