using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{

    public class adm_genericMap : EntityTypeConfiguration<adm_generic>
    {
        public adm_genericMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });
            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(t => t.GenericName).IsRequired();
            // Table & Column Mappings
            this.ToTable("adm_generic");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.SpecialtyDropDownID).HasColumnName("SpecialtyDropDownID");
            this.Property(t => t.SpecialtyId).HasColumnName("SpecialtyId");
            this.Property(t => t.GenericName).HasColumnName("GenericName");
            this.Property(t => t.CommonUse).HasColumnName("CommonUse");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.CategoryDropDownID).HasColumnName("CategoryDropDownID");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.adm_generic)
                .HasForeignKey(d => d.CompanyId);

            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.adm_generic)
                .HasForeignKey(d => d.CreatedBy);

            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.adm_generic1)
                .HasForeignKey(d => d.ModifiedBy);

            this.HasRequired(t => t.sys_drop_down_value)
                .WithMany(t => t.adm_generic)
                .HasForeignKey(d => new { d.SpecialtyId, d.SpecialtyDropDownID });

            this.HasRequired(t => t.sys_drop_down_value1)
               .WithMany(t => t.adm_generic1)
               .HasForeignKey(d => new { d.CategoryID, d.CategoryDropDownID });
        }
    }
}
