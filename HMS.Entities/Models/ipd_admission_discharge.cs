using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public class ipd_admission_discharge : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }

        public decimal AdmissionId { get; set; }
        public decimal PatientId { get; set; }
        public string Weight { get; set; }
        public string Temperature { get; set; }
        public string DiagnosisAdmission { get; set; }
        public string ComplaintSummary { get; set; }
        public string ConditionAdmission { get; set; }
        public string GeneralCondition { get; set; }
        public string RespiratoryRate { get; set; }
        public string OtherRemarks { get; set; }
        public string CheckedBy { get; set; }
        public string BP { get; set; }
        public string Other { get; set; }
        public string Systemic { get; set; }
        public string PA { get; set; }
        public string PV { get; set; }
        public string UrineProteins { get; set; }
        public string Sugar { get; set; }
        public string Microscopy { get; set; }
        public string BloodHB { get; set; }
        public string TLC { get; set; }
        public string P { get; set; }
        public string L { get; set; }
        public string E { get; set; }
        public string ESR { get; set; }
        public string BloodSugar { get; set; }
        public string BloodGroup { get; set; }
        public string Ultrasound { get; set; }
        public string UltrasoundRemark { get; set; }
        public string XRay { get; set; }
        public string XRayRemark { get; set; }
        public string Conservative { get; set; }
        public string Operation { get; set; }
        public string Date { get; set; }
        public string Surgeon { get; set; }
        public string Assistant { get; set; }
        public string Anaesthesia { get; set; }
        public string Incision { get; set; }
        public string OperativeFinding { get; set; }
        public string OprationNotes { get; set; }
        public string OPMedicines { get; set; }
        public string OPProgress { get; set; }
        public string ConditionDischarge { get; set; }
        public string RemovalDate { get; set; }
        public string ConditionWound { get; set; }
        public string AdviseMedicine { get; set; }
        public Nullable<DateTime> FollowUpDate { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual ipd_admission ipd_admission { get; set; }
    }
}
