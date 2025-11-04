using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class ipd_paymentMap : EntityTypeConfiguration<ipd_payment>
    {
        public ipd_paymentMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("ipd_payment");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.AdmissionId).HasColumnName("AdmissionId");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.PaymentDate).HasColumnName("PaymentDate");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");          

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.ipd_payment)
                .HasForeignKey(d => d.CompanyId);

            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.ipd_payment)
                .HasForeignKey(d => d.CreatedBy);

            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.ipd_payment1)
                .HasForeignKey(d => d.ModifiedBy);

        }
    }
}
