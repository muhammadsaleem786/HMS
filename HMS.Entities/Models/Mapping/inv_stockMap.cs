using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class inv_stockMap : EntityTypeConfiguration<inv_stock>
    {
        public inv_stockMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);      

            // Table & Column Mappings
            this.ToTable("inv_stock");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.ItemID).HasColumnName("ItemID");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.BatchSarialNumber).HasColumnName("BatchSarialNumber");
            this.Property(t => t.ExpiredWarrantyDate).HasColumnName("ExpiredWarrantyDate");
           this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.inv_stock)
                .HasForeignKey(d => d.CompanyId);

            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.inv_stock)
                .HasForeignKey(d => d.CreatedBy);

            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.inv_stock1)
                .HasForeignKey(d => d.ModifiedBy);

            this.HasRequired(t => t.adm_item)
               .WithMany(t => t.inv_stock)
              .HasForeignKey(d => new { d.ItemID, d.CompanyId });
        }
    }
}
