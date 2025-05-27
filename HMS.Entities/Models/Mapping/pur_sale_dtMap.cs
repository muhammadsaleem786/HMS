using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class pur_sale_dtMap : EntityTypeConfiguration<pur_sale_dt>
    {
        public pur_sale_dtMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SaleID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ItemDescription)
                .HasMaxLength(250);
            // Table & Column Mappings
            this.ToTable("pur_sale_dt");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.SaleID).HasColumnName("SaleID");
            this.Property(t => t.ItemID).HasColumnName("ItemID");
            this.Property(t => t.ItemDescription).HasColumnName("ItemDescription");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.Rate).HasColumnName("Rate");
            this.Property(t => t.DiscountType).HasColumnName("DiscountType");
            this.Property(t => t.Discount).HasColumnName("Discount");
            this.Property(t => t.DiscountAmount).HasColumnName("DiscountAmount");
            this.Property(t => t.TotalAmount).HasColumnName("TotalAmount");;
            this.Property(t => t.BatchSarialNumber).HasColumnName("BatchSarialNumber");;
            this.Property(t => t.ExpiredWarrantyDate).HasColumnName("ExpiredWarrantyDate");;
            // Relationships          
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.pur_sale_dt)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.adm_item)
                .WithMany(t => t.pur_sale_dt)
                .HasForeignKey(d => new { d.ItemID, d.CompanyID });
            this.HasRequired(t => t.pur_sale_mf)
                .WithMany(t => t.pur_sale_dt)
                .HasForeignKey(d => new { d.SaleID, d.CompanyID });
        }
    }
}
