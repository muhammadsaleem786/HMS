using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class emr_incomeMap : EntityTypeConfiguration<emr_income>
    {
        public emr_incomeMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Remark)
                .IsRequired()
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("emr_income");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.CategoryId).HasColumnName("CategoryId");
            this.Property(t => t.CategoryDropdownId).HasColumnName("CategoryDropdownId");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.DueAmount).HasColumnName("DueAmount");
            this.Property(t => t.ReceivedAmount).HasColumnName("ReceivedAmount");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.Image).HasColumnName("Image");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.emr_income)
                .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.emr_income)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.emr_income1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasRequired(t => t.sys_drop_down_value)
                .WithMany(t => t.emr_income)
                .HasForeignKey(d => new { d.CategoryId, d.CategoryDropdownId });

        }
    }
}
