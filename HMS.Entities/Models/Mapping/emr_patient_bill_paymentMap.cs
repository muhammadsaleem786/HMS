using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class emr_patient_bill_paymentMap : EntityTypeConfiguration<emr_patient_bill_payment>
    {
        public emr_patient_bill_paymentMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Remarks)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("emr_patient_bill_payment");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.BillId).HasColumnName("BillId");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.PaymentDate).HasColumnName("PaymentDate");
            
            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.emr_patient_bill_payment)
                .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.emr_patient_bill_payment)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.adm_user_mf1)
                .WithMany(t => t.emr_patient_bill_payment1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasRequired(t => t.emr_patient_bill)
                .WithMany(t => t.emr_patient_bill_payment)
                .HasForeignKey(d => new { d.BillId, d.CompanyId });

        }
    }
}
