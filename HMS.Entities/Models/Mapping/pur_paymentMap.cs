using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class pur_paymentMap : EntityTypeConfiguration<pur_payment>
    {
        public pur_paymentMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("pur_payment");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.InvoiveId).HasColumnName("InvoiveId");
            this.Property(t => t.PaymentMethodDropdownID).HasColumnName("PaymentMethodDropdownID");
            this.Property(t => t.PaymentMethodID).HasColumnName("PaymentMethodID");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.PaymentDate).HasColumnName("PaymentDate");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");          

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.pur_payment)
                .HasForeignKey(d => d.CompanyId);

            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.pur_payment)
                .HasForeignKey(d => d.CreatedBy);

            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.pur_payment1)
                .HasForeignKey(d => d.ModifiedBy);

            this.HasRequired(t => t.pur_invoice_mf)
                .WithMany(t => t.pur_payment)
                .HasForeignKey(d => new { d.InvoiveId, d.CompanyId });

            this.HasRequired(t => t.sys_drop_down_value)
               .WithMany(t => t.pur_payment)
               .HasForeignKey(d => new { d.PaymentMethodID, d.PaymentMethodDropdownID });

        }
    }
}
