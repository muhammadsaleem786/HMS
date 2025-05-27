using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class sys_drop_down_valueMap : EntityTypeConfiguration<sys_drop_down_value>
    {
        public sys_drop_down_valueMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.DropDownID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DropDownID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("sys_drop_down_value");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.DropDownID).HasColumnName("DropDownID");
            this.Property(t => t.Value).HasColumnName("Value");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.DependedDropDownID).HasColumnName("DependedDropDownID");
            this.Property(t => t.DependedDropDownValueID).HasColumnName("DependedDropDownValueID");
            this.Property(t => t.SystemGenerated).HasColumnName("SystemGenerated");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.Unit).HasColumnName("Unit");            
            // Relationships
            this.HasRequired(t => t.sys_drop_down_mf)
                .WithMany(t => t.sys_drop_down_value)
                .HasForeignKey(d => d.DropDownID);

        }
    }
}
