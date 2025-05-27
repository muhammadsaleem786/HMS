using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class adm_settingMap : EntityTypeConfiguration<adm_setting>
    {
        public adm_settingMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.SettingIdOrName)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.SettingIdOrNameValue)
                .IsRequired();

            this.Property(t => t.SettingIdOrNameDepAllowValue)
                .HasMaxLength(500);

            this.Property(t => t.SettingIdOrNameDepDedValue)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("adm_setting");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.SettingIdOrName).HasColumnName("SettingIdOrName");
            this.Property(t => t.SettingIdOrNameValue).HasColumnName("SettingIdOrNameValue");
            this.Property(t => t.SettingIdOrNameDepAllowValue).HasColumnName("SettingIdOrNameDepAllowValue");
            this.Property(t => t.SettingIdOrNameDepDedValue).HasColumnName("SettingIdOrNameDepDedValue");
        }
    }
}
