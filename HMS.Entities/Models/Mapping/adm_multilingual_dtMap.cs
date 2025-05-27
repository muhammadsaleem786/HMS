using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class adm_multilingual_dtMap : EntityTypeConfiguration<adm_multilingual_dt>
    {
        public adm_multilingual_dtMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Module)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Form)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Keyword)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Value)
                .IsRequired()
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("adm_multilingual_dt");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.MultilingualId).HasColumnName("MultilingualId");
            this.Property(t => t.Module).HasColumnName("Module");
            this.Property(t => t.Form).HasColumnName("Form");
            this.Property(t => t.Keyword).HasColumnName("Keyword");
            this.Property(t => t.Value).HasColumnName("Value");

            // Relationships
            this.HasRequired(t => t.adm_multilingual_mf)
                .WithMany(t => t.adm_multilingual_dt)
                .HasForeignKey(d => d.MultilingualId);

        }
    }
}
