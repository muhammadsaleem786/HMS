using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class pr_deduction_contributionMap : EntityTypeConfiguration<pr_deduction_contribution>
    {
        public pr_deduction_contributionMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Category)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.DeductionContributionName)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.DeductionContributionType)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("pr_deduction_contribution");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.Category).HasColumnName("Category");
            this.Property(t => t.DeductionContributionName).HasColumnName("DeductionContributionName");
            this.Property(t => t.DeductionContributionType).HasColumnName("DeductionContributionType");
            this.Property(t => t.DeductionContributionValue).HasColumnName("DeductionContributionValue");
            this.Property(t => t.Default).HasColumnName("Default");
            this.Property(t => t.Taxable).HasColumnName("Taxable");
            this.Property(t => t.StartingBalance).HasColumnName("StartingBalance");
            this.Property(t => t.SystemGenerated).HasColumnName("SystemGenerated");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.pr_deduction_contribution)
                .HasForeignKey(d => d.CompanyID);

        }
    }
}
