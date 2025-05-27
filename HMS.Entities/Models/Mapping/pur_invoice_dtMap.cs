using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class pur_invoice_dtMap : EntityTypeConfiguration<pur_invoice_dt>
    {
        public pur_invoice_dtMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.InvoiceID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ItemDescription)
                .HasMaxLength(250);           
            // Table & Column Mappings
            this.ToTable("pur_invoice_dt");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.InvoiceID).HasColumnName("InvoiceID");
            this.Property(t => t.ItemID).HasColumnName("ItemID");
            this.Property(t => t.ItemDescription).HasColumnName("ItemDescription");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.Rate).HasColumnName("Rate");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.Discount).HasColumnName("Discount");
            this.Property(t => t.DiscountAmount).HasColumnName("DiscountAmount");
            this.Property(t => t.BatchSarialNumber).HasColumnName("BatchSarialNumber");
            this.Property(t => t.ExpiredWarrantyDate).HasColumnName("ExpiredWarrantyDate");
            // Relationships          
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.pur_invoice_dt)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.adm_item)
                .WithMany(t => t.pur_invoice_dt)
                .HasForeignKey(d => new { d.ItemID, d.CompanyID });

            this.HasRequired(t => t.pur_invoice_mf)
                .WithMany(t => t.pur_invoice_dt)
                .HasForeignKey(d => new { d.InvoiceID, d.CompanyID });          
        }
    }
}
