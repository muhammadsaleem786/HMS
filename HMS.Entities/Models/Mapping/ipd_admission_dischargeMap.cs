using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class ipd_admission_dischargeMap : EntityTypeConfiguration<ipd_admission_discharge>
    {
        public ipd_admission_dischargeMap()
        {
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("ipd_admission_discharge");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.AdmissionId).HasColumnName("AdmissionId");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.Weight).HasColumnName("Weight");
            this.Property(t => t.Temperature).HasColumnName("Temperature");
            this.Property(t => t.DiagnosisAdmission).HasColumnName("DiagnosisAdmission");
            this.Property(t => t.ComplaintSummary).HasColumnName("ComplaintSummary");
            this.Property(t => t.ConditionAdmission).HasColumnName("ConditionAdmission");
            this.Property(t => t.GeneralCondition).HasColumnName("GeneralCondition");
            this.Property(t => t.RespiratoryRate).HasColumnName("RespiratoryRate");
            this.Property(t => t.BP).HasColumnName("BP");
            this.Property(t => t.Other).HasColumnName("Other");
            this.Property(t => t.Systemic).HasColumnName("Systemic");
            this.Property(t => t.PA).HasColumnName("PA");
            this.Property(t => t.PV).HasColumnName("PV");
            this.Property(t => t.UrineProteins).HasColumnName("UrineProteins");
            this.Property(t => t.Sugar).HasColumnName("Sugar");
            this.Property(t => t.Microscopy).HasColumnName("Microscopy");
            this.Property(t => t.BloodHB).HasColumnName("BloodHB");
            this.Property(t => t.TLC).HasColumnName("TLC");
            this.Property(t => t.P).HasColumnName("P");
            this.Property(t => t.L).HasColumnName("L");
            this.Property(t => t.E).HasColumnName("E");
            this.Property(t => t.ESR).HasColumnName("ESR");
            this.Property(t => t.BloodSugar).HasColumnName("BloodSugar");
            this.Property(t => t.BloodGroup).HasColumnName("BloodGroup");
            this.Property(t => t.Ultrasound).HasColumnName("Ultrasound");
            this.Property(t => t.UltrasoundRemark).HasColumnName("UltrasoundRemark");
            this.Property(t => t.XRay).HasColumnName("XRay");
            this.Property(t => t.XRayRemark).HasColumnName("XRayRemark");
            this.Property(t => t.Conservative).HasColumnName("Conservative");
            this.Property(t => t.Operation).HasColumnName("Operation");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.Surgeon).HasColumnName("Surgeon");
            this.Property(t => t.Assistant).HasColumnName("Assistant");
            this.Property(t => t.Anaesthesia).HasColumnName("Anaesthesia");
            this.Property(t => t.Incision).HasColumnName("Incision");
            this.Property(t => t.OperativeFinding).HasColumnName("OperativeFinding");
            this.Property(t => t.OprationNotes).HasColumnName("OprationNotes");
            this.Property(t => t.OPMedicines).HasColumnName("OPMedicines");
            this.Property(t => t.OPProgress).HasColumnName("OPProgress");
            this.Property(t => t.ConditionDischarge).HasColumnName("ConditionDischarge");
            this.Property(t => t.RemovalDate).HasColumnName("RemovalDate");
            this.Property(t => t.ConditionWound).HasColumnName("ConditionWound");
            this.Property(t => t.AdviseMedicine).HasColumnName("AdviseMedicine");
            this.Property(t => t.FollowUpDate).HasColumnName("FollowUpDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.OtherRemarks).HasColumnName("OtherRemarks");
            this.Property(t => t.CheckedBy).HasColumnName("CheckedBy");          

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.ipd_admission_discharge)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.ipd_admission_discharge)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.ipd_admission_discharge1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasRequired(t => t.ipd_admission)
                .WithMany(t => t.ipd_admission_discharge)
                .HasForeignKey(d => new { d.AdmissionId, d.CompanyID });
        }
    }
}
