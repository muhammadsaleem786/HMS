using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class pr_allowanceMap : EntityTypeConfiguration<pr_allowance>
    {
        public pr_allowanceMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AllowanceName)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.AllowanceType)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("pr_allowance");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.CategoryDropDownID).HasColumnName("CategoryDropDownID");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.AllowanceName).HasColumnName("AllowanceName");
            this.Property(t => t.AllowanceType).HasColumnName("AllowanceType");
            this.Property(t => t.AllowanceValue).HasColumnName("AllowanceValue");
            this.Property(t => t.Taxable).HasColumnName("Taxable");
            this.Property(t => t.Default).HasColumnName("Default");
            //this.Property(t => t.Formula).HasColumnName("Formula");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.pr_allowance)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.sys_drop_down_value)
           .WithMany(t => t.pr_allowance)
           .HasForeignKey(d => new { d.CategoryID, d.CategoryDropDownID });
        }
    }
}
