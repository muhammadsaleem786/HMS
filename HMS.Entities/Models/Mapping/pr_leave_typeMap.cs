using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class pr_leave_typeMap : EntityTypeConfiguration<pr_leave_type>
    {
        public pr_leave_typeMap()
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

            this.Property(t => t.TypeName)
                .IsRequired()
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("pr_leave_type");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.Category).HasColumnName("Category");
            this.Property(t => t.TypeName).HasColumnName("TypeName");
            this.Property(t => t.AccuralDropDownID).HasColumnName("AccuralDropDownID");
            this.Property(t => t.AccrualFrequencyID).HasColumnName("AccrualFrequencyID");
            this.Property(t => t.EarnedValue).HasColumnName("EarnedValue");
            this.Property(t => t.SystemGenerated).HasColumnName("SystemGenerated");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.pr_leave_type)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.sys_drop_down_value)
                .WithMany(t => t.pr_leave_type)
                .HasForeignKey(d => new { d.AccrualFrequencyID, d.AccuralDropDownID });

        }
    }
}
