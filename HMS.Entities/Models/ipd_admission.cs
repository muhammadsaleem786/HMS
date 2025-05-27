using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class ipd_admission:Entity
    {
        public ipd_admission()
        {
            this.ipd_admission_charges = new List<ipd_admission_charges>();
            this.ipd_admission_imaging = new List<ipd_admission_imaging>();
            this.ipd_admission_lab = new List<ipd_admission_lab>();
            this.ipd_admission_medication = new List<ipd_admission_medication>();
            this.ipd_admission_notes = new List<ipd_admission_notes>();
            this.ipd_admission_vital = new List<ipd_admission_vital>();
            this.ipd_procedure_mf = new List<ipd_procedure_mf>();
            this.ipd_diagnosis = new List<ipd_diagnosis>();
            this.emr_patient_bill = new List<emr_patient_bill>();
            this.ipd_admission_discharge = new List<ipd_admission_discharge>();
            this.ipd_input_output = new List<ipd_input_output>();
            this.ipd_medication_log = new List<ipd_medication_log>();

        }

        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public string AdmissionNo { get; set; }
        public decimal PatientId { get; set; }
        public int AdmissionTypeId { get; set; }
        public Nullable<int> AdmissionTypeDropdownId { get; set; }
        public Nullable<int> TypeId { get; set; }
        public Nullable<int> WardTypeId { get; set; }
        public Nullable<int> WardTypeDropdownId { get; set; }
        public Nullable<int> BedId { get; set; }
        public Nullable<int> BedDropdownId { get; set; }
        public Nullable<int> RoomId { get; set; }
        public Nullable<int> RoomDropdownId { get; set; }
        public int DoctorId { get; set; }
        public System.DateTime AdmissionDate { get; set; }
        public System.TimeSpan AdmissionTime { get; set; }
        public Nullable<System.DateTime> DischargeDate { get; set; }
        public Nullable<System.TimeSpan> DischargeTime { get; set; }
        public string Location { get; set; }
        public string ReasonForVisit { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual emr_patient_mf emr_patient_mf { get; set; }
        public virtual ICollection<ipd_admission_charges> ipd_admission_charges { get; set; }
        public virtual ICollection<ipd_admission_imaging> ipd_admission_imaging { get; set; }
        public virtual ICollection<ipd_admission_lab> ipd_admission_lab { get; set; }
        public virtual ICollection<ipd_diagnosis> ipd_diagnosis { get; set; }
        public virtual ICollection<ipd_admission_medication> ipd_admission_medication { get; set; }
        public virtual ICollection<ipd_admission_notes> ipd_admission_notes { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value1 { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value2 { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value3 { get; set; }
        public virtual ICollection<ipd_admission_vital> ipd_admission_vital { get; set; }
        public virtual ICollection<ipd_procedure_mf> ipd_procedure_mf { get; set; }
        public virtual ICollection<emr_patient_bill> emr_patient_bill { get; set; }
        public virtual ICollection<ipd_admission_discharge> ipd_admission_discharge { get; set; }
        public virtual ICollection<ipd_input_output> ipd_input_output { get; set; }
        public virtual ICollection<ipd_medication_log> ipd_medication_log { get; set; }
    }
}
