using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class pr_designationMap : EntityTypeConfiguration<pr_designation>
    {
        public pr_designationMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DesignationName)
                .IsRequired()
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("pr_designation");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.DesignationName).HasColumnName("DesignationName");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.pr_designation)
                .HasForeignKey(d => d.CompanyID);

        }
    }
}
