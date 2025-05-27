using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class adm_role_dtMap : EntityTypeConfiguration<adm_role_dt>
    {
        public adm_role_dtMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.RoleID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.RoleID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("adm_role_dt");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.RoleID).HasColumnName("RoleID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.DropDownScreenID).HasColumnName("DropDownScreenID");
            this.Property(t => t.ScreenID).HasColumnName("ScreenID");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ViewRights).HasColumnName("ViewRights");
            this.Property(t => t.CreateRights).HasColumnName("CreateRights");
            this.Property(t => t.DeleteRights).HasColumnName("DeleteRights");
            this.Property(t => t.EditRights).HasColumnName("EditRights");

            // Relationships
            this.HasRequired(t => t.adm_role_mf)
                .WithMany(t => t.adm_role_dt)
                .HasForeignKey(d => new { d.RoleID, d.CompanyID });
            this.HasRequired(t => t.sys_drop_down_value)
                .WithMany(t => t.adm_role_dt)
                .HasForeignKey(d => new { d.ScreenID, d.DropDownScreenID });

        }
    }
}
