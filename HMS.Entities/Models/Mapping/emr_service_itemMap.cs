using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class emr_service_itemMap : EntityTypeConfiguration<emr_service_item>
    {
        public emr_service_itemMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("emr_service_item");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.ItemId).HasColumnName("ItemId");
            this.Property(t => t.ServiceId).HasColumnName("ServiceId");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.emr_service_item)
                .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.emr_service_item)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.emr_service_item1)
                .HasForeignKey(d => d.ModifiedBy);

            this.HasRequired(t => t.adm_item)
                .WithMany(t => t.emr_service_item)
                .HasForeignKey(d => new { d.ItemId, d.CompanyId });
            this.HasRequired(t => t.emr_service_mf)
                .WithMany(t => t.emr_service_item)
                .HasForeignKey(d => new { d.ServiceId, d.CompanyId });

        }
    }
}
