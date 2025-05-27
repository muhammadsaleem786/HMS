using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class emr_patient_mfMap : EntityTypeConfiguration<emr_patient_mf>
    {
        public emr_patient_mfMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PatientName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Email)
                .HasMaxLength(50);

            this.Property(t => t.Mobile)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.CNIC)
                .HasMaxLength(50);

            this.Property(t => t.Notes)
                .HasMaxLength(500);

            this.Property(t => t.MRNO)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("emr_patient_mf");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.PatientName).HasColumnName("PatientName");
            this.Property(t => t.Gender).HasColumnName("Gender");
            this.Property(t => t.DOB).HasColumnName("DOB");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Mobile).HasColumnName("Mobile");
            this.Property(t => t.CNIC).HasColumnName("CNIC");
            this.Property(t => t.Image).HasColumnName("Image");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.MRNO).HasColumnName("MRNO");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.Age).HasColumnName("Age");
            this.Property(t => t.BloodGroupId).HasColumnName("BloodGroupId");
            this.Property(t => t.BloodGroupDropDownId).HasColumnName("BloodGroupDropDownId");
            this.Property(t => t.EmergencyNo).HasColumnName("EmergencyNo");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.ReferredBy).HasColumnName("ReferredBy");
            this.Property(t => t.AnniversaryDate).HasColumnName("AnniversaryDate");
            this.Property(t => t.Illness_Diabetes).HasColumnName("Illness_Diabetes");
            this.Property(t => t.Illness_Tuberculosis).HasColumnName("Illness_Tuberculosis");
            this.Property(t => t.Illness_HeartPatient).HasColumnName("Illness_HeartPatient");
            this.Property(t => t.Illness_LungsRelated).HasColumnName("Illness_LungsRelated");
            this.Property(t => t.Illness_BloodPressure).HasColumnName("Illness_BloodPressure");
            this.Property(t => t.Illness_Migraine).HasColumnName("Illness_Migraine");
            this.Property(t => t.Illness_Other).HasColumnName("Illness_Other");
            this.Property(t => t.Allergies_Food).HasColumnName("Allergies_Food");
            this.Property(t => t.Allergies_Drug).HasColumnName("Allergies_Drug");
            this.Property(t => t.Allergies_Other).HasColumnName("Allergies_Other");
            this.Property(t => t.Habits_Smoking).HasColumnName("Habits_Smoking");
            this.Property(t => t.Habits_Drinking).HasColumnName("Habits_Drinking");
            this.Property(t => t.Habits_Tobacco).HasColumnName("Habits_Tobacco");
            this.Property(t => t.Habits_Other).HasColumnName("Habits_Other");
            this.Property(t => t.MedicalHistory).HasColumnName("MedicalHistory");
            this.Property(t => t.CurrentMedication).HasColumnName("CurrentMedication");
            this.Property(t => t.HabitsHistory).HasColumnName("HabitsHistory");
            this.Property(t => t.BillTypeId).HasColumnName("BillTypeId");
            this.Property(t => t.BillTypeDropdownId).HasColumnName("BillTypeDropdownId");
            this.Property(t => t.PrefixTittleId).HasColumnName("PrefixTittleId");
            this.Property(t => t.PrefixDropdownId).HasColumnName("PrefixDropdownId");
            this.Property(t => t.Father_Husband).HasColumnName("Father_Husband");
            this.Property(t => t.ContactNo).HasColumnName("ContactNo");
            this.Property(t => t.ReminderId).HasColumnName("ReminderId");


            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.emr_patient_mf)
                .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.emr_patient_mf)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.emr_patient_mf1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasRequired(t => t.sys_drop_down_value)
               .WithMany(t => t.emr_patient_mf)
               .HasForeignKey(d => new { d.BloodGroupId, d.BloodGroupDropDownId });

            this.HasRequired(t => t.sys_drop_down_value1)
              .WithMany(t => t.emr_patient_mf1)
              .HasForeignKey(d => new { d.PrefixTittleId, d.PrefixDropdownId });
            this.HasRequired(t => t.sys_drop_down_value2)
              .WithMany(t => t.emr_patient_mf2)
              .HasForeignKey(d => new { d.BillTypeId, d.BillTypeDropdownId });


            this.HasOptional(t => t.adm_reminder_mf)
                .WithMany(t => t.emr_patient_mf)
                .HasForeignKey(d => new { d.ReminderId, d.CompanyId });
        }
    }
}
