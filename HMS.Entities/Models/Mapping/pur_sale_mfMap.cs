using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class pur_sale_mfMap : EntityTypeConfiguration<pur_sale_mf>
    {
        public pur_sale_mfMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("pur_sale_mf");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.SubTotal).HasColumnName("SubTotal");
            this.Property(t => t.Discount).HasColumnName("Discount");
            this.Property(t => t.DiscountType).HasColumnName("DiscountType");
            this.Property(t => t.DiscountAmount).HasColumnName("DiscountAmount");
            this.Property(t => t.TaxAmount).HasColumnName("TaxAmount");
            this.Property(t => t.Total).HasColumnName("Total");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.CustomerId).HasColumnName("CustomerId");
            this.Property(t => t.SaleTypeDropDownId).HasColumnName("SaleTypeDropDownId");
            this.Property(t => t.SaleTypeID).HasColumnName("SaleTypeID");   
            this.Property(t => t.ReturnInvoiceId).HasColumnName("ReturnInvoiceId");
            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.pur_sale_mf)
                .HasForeignKey(d => d.CompanyID);

            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.pur_sale_mf)
                .HasForeignKey(d => d.CreatedBy);

            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.pur_sale_mf1)
                .HasForeignKey(d => d.ModifiedBy);

            this.HasRequired(t => t.sys_drop_down_value)
               .WithMany(t => t.pur_sale_mf)
               .HasForeignKey(d => new { d.SaleTypeID, d.SaleTypeDropDownId });
            this.HasRequired(t => t.emr_patient_mf)
               .WithMany(t => t.pur_sale_mf)
               .HasForeignKey(d => new { d.CustomerId, d.CompanyID });
        }
    }
}
