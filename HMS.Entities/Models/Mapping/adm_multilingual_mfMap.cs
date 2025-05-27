using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class adm_multilingual_mfMap : EntityTypeConfiguration<adm_multilingual_mf>
    {
        public adm_multilingual_mfMap()
        {
            // Primary Key
            this.HasKey(t => t.MultilingualId);

            // Properties
            this.Property(t => t.MultilingualName)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("adm_multilingual_mf");
            this.Property(t => t.MultilingualId).HasColumnName("MultilingualId");
            this.Property(t => t.MultilingualName).HasColumnName("MultilingualName");
        }
    }
}
