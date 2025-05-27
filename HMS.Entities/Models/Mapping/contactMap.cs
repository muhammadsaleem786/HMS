using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class contactMap : EntityTypeConfiguration<contact>
    {
        public contactMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("contact");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Speciality).HasColumnName("Speciality");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
        }
    }
}
