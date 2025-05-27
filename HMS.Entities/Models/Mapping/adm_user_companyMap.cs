using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class adm_user_companyMap : EntityTypeConfiguration<adm_user_company>
    {
        public adm_user_companyMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("adm_user_company");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            this.Property(t => t.RoleID).HasColumnName("RoleID");
            this.Property(t => t.AdminID).HasColumnName("AdminID");
            this.Property(t => t.IsDefault).HasColumnName("IsDefault");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.adm_user_company)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.adm_role_mf)
                .WithMany(t => t.adm_user_company)
                .HasForeignKey(d => new { d.RoleID, d.CompanyID });
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.adm_user_company)
                .HasForeignKey(d => d.UserID);
            this.HasRequired(t => t.adm_user_mf1)
                .WithMany(t => t.adm_user_company1)
                .HasForeignKey(d => d.AdminID);

        }
    }
}
