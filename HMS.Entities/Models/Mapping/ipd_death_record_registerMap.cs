using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class ipd_death_record_registerMap : EntityTypeConfiguration<ipd_death_record_register>
    {
        public ipd_death_record_registerMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });
            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("ipd_death_record_register");

            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.DeathDate).HasColumnName("DeathDate");
            this.Property(t => t.MRNo).HasColumnName("MRNo");
            this.Property(t => t.PatientName).HasColumnName("PatientName");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.FatherHusbandName).HasColumnName("FatherHusbandName");
            this.Property(t => t.Age).HasColumnName("Age");
            this.Property(t => t.Gender).HasColumnName("Gender");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.IDCardNo).HasColumnName("IDCardNo");
            this.Property(t => t.ContractNo).HasColumnName("ContractNo");
            this.Property(t => t.DateTimeOfDeath).HasColumnName("DateTimeOfDeath");
            this.Property(t => t.ProbableCauseOfDeath).HasColumnName("ProbableCauseOfDeath");
            this.Property(t => t.DeathCertificateHandedOverTo).HasColumnName("DeathCertificateHandedOverTo");
            this.Property(t => t.AttendantSignature).HasColumnName("AttendantSignature");
            this.Property(t => t.DoctorSignature).HasColumnName("DoctorSignature");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.ipd_death_record_register)
                .HasForeignKey(d => d.CompanyID);

            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.ipd_death_record_register)
                .HasForeignKey(d => d.CreatedBy);

            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.ipd_death_record_register1)
                .HasForeignKey(d => d.ModifiedBy);
        }
    }
}
