using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class ipd_adverse_drug_reactionMap : EntityTypeConfiguration<ipd_adverse_drug_reaction>
    {
        public ipd_adverse_drug_reactionMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });
            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("ipd_adverse_drug_reaction");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.PatientName).HasColumnName("PatientName");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.Age).HasColumnName("Age");
            this.Property(t => t.Gender).HasColumnName("Gender");
            this.Property(t => t.Contact).HasColumnName("Contact");
            this.Property(t => t.MedicineGiven).HasColumnName("MedicineGiven");
            this.Property(t => t.ReactionObserved).HasColumnName("ReactionObserved");
            this.Property(t => t.ActionTaken).HasColumnName("ActionTaken");
            this.Property(t => t.Outcome).HasColumnName("Outcome");
            this.Property(t => t.DoctorSignature).HasColumnName("DoctorSignature");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.ipd_adverse_drug_reaction)
                .HasForeignKey(d => d.CompanyID);

            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.ipd_adverse_drug_reaction)
                .HasForeignKey(d => d.CreatedBy);

            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.ipd_adverse_drug_reaction1)
                .HasForeignKey(d => d.ModifiedBy);
        }
    }
}
