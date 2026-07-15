using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class emr_investigationMap : EntityTypeConfiguration<emr_investigation>
    {
        public emr_investigationMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Investigation)
                .IsRequired()
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("emr_investigation");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.Investigation).HasColumnName("Investigation");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.CategoryId).HasColumnName("CategoryId");
            this.Property(t => t.CategoryDropdownId).HasColumnName("CategoryDropdownId");
            this.Property(t => t.SampleId).HasColumnName("SampleId");
            this.Property(t => t.SampleDropdownId).HasColumnName("SampleDropdownId");
            this.Property(t => t.TAT).HasColumnName("TAT");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.IsItem).HasColumnName("IsItem");
            // Relationships
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.emr_investigation)
                .HasForeignKey(d => d.CreatedBy);

            this.HasRequired(t => t.adm_user_mf1)
                .WithMany(t => t.emr_investigation1)
                .HasForeignKey(d => d.ModifiedBy);

            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.emr_investigation)
                .HasForeignKey(d => d.CompanyID);

            this.HasRequired(t => t.sys_drop_down_value)
                .WithMany(t => t.emr_investigation)
                .HasForeignKey(d => new { d.CategoryId, d.CategoryDropdownId });

            this.HasRequired(t => t.sys_drop_down_value1)
                .WithMany(t => t.emr_investigation1)
                .HasForeignKey(d => new { d.SampleId, d.SampleDropdownId });
        }
    }
}
