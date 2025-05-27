using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{

    public class adm_itemMap : EntityTypeConfiguration<adm_item>
    {
        public adm_itemMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.SKU)
                .HasMaxLength(250);

            this.Property(t => t.Image)
                .HasMaxLength(500);


            // Table & Column Mappings
            this.ToTable("adm_item");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.ItemTypeDropDownID).HasColumnName("ItemTypeDropDownID");
            this.Property(t => t.ItemTypeId).HasColumnName("ItemTypeId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.SKU).HasColumnName("SKU");
            this.Property(t => t.Image).HasColumnName("Image");
            this.Property(t => t.UnitDropDownID).HasColumnName("UnitDropDownID");
            this.Property(t => t.UnitID).HasColumnName("UnitID");
            this.Property(t => t.CategoryDropDownID).HasColumnName("CategoryDropDownID");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.TrackInventory).HasColumnName("TrackInventory");
            this.Property(t => t.IsActive).HasColumnName("IsActive");            
            this.Property(t => t.CostPrice).HasColumnName("CostPrice");            
            this.Property(t => t.SalePrice).HasColumnName("SalePrice");
            this.Property(t => t.InventoryOpeningStock).HasColumnName("InventoryOpeningStock");
            this.Property(t => t.InventoryStockPerUnit).HasColumnName("InventoryStockPerUnit");
            this.Property(t => t.InventoryStockQuantity).HasColumnName("InventoryStockQuantity");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.GroupId).HasColumnName("GroupId");
            this.Property(t => t.GroupDropDownId).HasColumnName("GroupDropDownId");
            this.Property(t => t.POSItem).HasColumnName("POSItem");
            this.Property(t => t.InstructionId).HasColumnName("InstructionId");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.adm_item)
                .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.adm_item)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.adm_item1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasOptional(t => t.sys_drop_down_value)
                .WithMany(t => t.adm_item)
                .HasForeignKey(d => new { d.UnitID, d.UnitDropDownID });
            this.HasOptional(t => t.sys_drop_down_value1)
               .WithMany(t => t.adm_item1)
               .HasForeignKey(d => new { d.CategoryID, d.CategoryDropDownID });
            this.HasOptional(t => t.sys_drop_down_value2)
            .WithMany(t => t.adm_item2)
            .HasForeignKey(d => new { d.ItemTypeId, d.ItemTypeDropDownID });
            this.HasOptional(t => t.sys_drop_down_value3)
            .WithMany(t => t.adm_item3)
            .HasForeignKey(d => new { d.GroupId, d.GroupDropDownId });
            this.HasOptional(t => t.emr_instruction)
               .WithMany(t => t.adm_item)
               .HasForeignKey(d => new { d.InstructionId, d.CompanyId });
        }
    }
}
