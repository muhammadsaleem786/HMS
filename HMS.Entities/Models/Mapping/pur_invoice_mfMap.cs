using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class pur_invoice_mfMap : EntityTypeConfiguration<pur_invoice_mf>
    {
        public pur_invoice_mfMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("pur_invoice_mf");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.VendorID).HasColumnName("VendorID");
            this.Property(t => t.BillNo).HasColumnName("BillNo");
            this.Property(t => t.OrderNo).HasColumnName("OrderNo");
            this.Property(t => t.BillDate).HasColumnName("BillDate");
            this.Property(t => t.DueDate).HasColumnName("DueDate");
            this.Property(t => t.Total).HasColumnName("Total");
            this.Property(t => t.DiscountAmount).HasColumnName("DiscountAmount");
            this.Property(t => t.Discount).HasColumnName("Discount");
            this.Property(t => t.IsItemLevelDiscount).HasColumnName("IsItemLevelDiscount");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");          

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.pur_invoice_mf)
                .HasForeignKey(d => d.CompanyID);

            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.pur_invoice_mf)
                .HasForeignKey(d => d.CreatedBy);

            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.pur_invoice_mf1)
                .HasForeignKey(d => d.ModifiedBy);

            this.HasRequired(t => t.pur_vendor)
                .WithMany(t => t.pur_invoice_mf)
                .HasForeignKey(d => new { d.VendorID, d.CompanyID });          

        }
    }
}
