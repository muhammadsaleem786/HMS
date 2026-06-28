using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{

    public class lab_parameterMap : EntityTypeConfiguration<lab_parameter>
    {
        public lab_parameterMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });
            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(t => t.ParameterName).IsRequired();
            // Table & Column Mappings
            this.ToTable("lab_parameter");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.ParameterName).HasColumnName("ParameterName");
            this.Property(t => t.UnitId).HasColumnName("UnitId");
            this.Property(t => t.UnitDropdownId).HasColumnName("UnitDropdownId");
            this.Property(t => t.CatalogId).HasColumnName("CatalogId");
            this.Property(t => t.ReferenceRange).HasColumnName("ReferenceRange");
            this.Property(t => t.NormalMinimum).HasColumnName("NormalMinimum");
            this.Property(t => t.NormalMaximum).HasColumnName("NormalMaximum");
            this.Property(t => t.CriticalLow).HasColumnName("CriticalLow");
            this.Property(t => t.CriticalHigh).HasColumnName("CriticalHigh");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.lab_parameter)
                .HasForeignKey(d => d.CompanyId);

            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.lab_parameter)
                .HasForeignKey(d => d.CreatedBy);

            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.lab_parameter1)
                .HasForeignKey(d => d.ModifiedBy);

            this.HasRequired(t => t.sys_drop_down_value)
                .WithMany(t => t.lab_parameter)
                .HasForeignKey(d => new { d.UnitId, d.UnitDropdownId });

            this.HasRequired(t => t.emr_investigation)
                .WithMany(t => t.lab_parameter)
                .HasForeignKey(d => new { d.CatalogId, d.CompanyId });
        }
    }
}
