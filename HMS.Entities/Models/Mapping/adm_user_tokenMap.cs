using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class adm_user_tokenMap : EntityTypeConfiguration<adm_user_token>
    {
        public adm_user_tokenMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TokenKey)
                .IsRequired()
                .HasMaxLength(1000);

            this.Property(t => t.DeviceType)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("adm_user_token");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.TokenKey).HasColumnName("TokenKey");
            this.Property(t => t.ExpiryDate).HasColumnName("ExpiryDate");
            this.Property(t => t.IsExpired).HasColumnName("IsExpired");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.adm_user_token)
                .HasForeignKey(d => d.UserID);

        }
    }
}
