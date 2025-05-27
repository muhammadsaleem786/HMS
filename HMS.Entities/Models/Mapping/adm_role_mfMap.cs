using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using Repository.Pattern.Ef6;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities.Models.Mapping
{
    public class adm_role_mfMap : EntityTypeConfiguration<adm_role_mf>
    {
        public adm_role_mfMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.RoleName)
                .IsRequired()
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("adm_role_mf");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.RoleName).HasColumnName("RoleName");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.SystemGenerated).HasColumnName("SystemGenerated");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.IsUpdateText).HasColumnName("IsUpdateText");
            
            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.adm_role_mf)
                .HasForeignKey(d => d.CompanyID);

        }
    }
}
