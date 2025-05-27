using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;

namespace HMS.Entities.Models
{
    public partial class emr_patient_mf : Entity
    {
        public emr_patient_mf()
        {
            this.emr_appointment_mf = new List<emr_appointment_mf>();
            this.emr_prescription_mf = new List<emr_prescription_mf>();
            this.emr_patient_bill = new List<emr_patient_bill>();
            this.ipd_admission = new List<ipd_admission>();
            this.pur_sale_mf = new List<pur_sale_mf>();
            this.pur_sale_hold_mf = new List<pur_sale_hold_mf>();
        }

        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public string PatientName { get; set; }
        public int Gender { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string CNIC { get; set; }
        public string Image { get; set; }
        public string Notes { get; set; }
        public string MRNO { get; set; }
        public int BillTypeId { get; set; }
        public int BillTypeDropdownId { get; set; }
        public string ContactNo { get; set; }
        public int PrefixTittleId { get; set; }
        public int PrefixDropdownId { get; set; }
            public string Father_Husband { get; set; }
        public Nullable<int> BloodGroupId { get; set; }
        public Nullable<int> BloodGroupDropDownId { get; set; }
        public string EmergencyNo { get; set; }
        public string Address { get; set; }
        public string ReferredBy { get; set; }
        public Nullable<DateTime> AnniversaryDate { get; set; }
        public Boolean Illness_Diabetes { get; set; } = false;
        public Boolean Illness_Tuberculosis { get; set; } = false;
        public Boolean Illness_HeartPatient { get; set; } = false;
        public Boolean Illness_LungsRelated { get; set; } = false;
        public Boolean Illness_BloodPressure { get; set; } = false;
        public Boolean Illness_Migraine { get; set; } = false;
        public string Illness_Other { get; set; }
        public string Allergies_Food { get; set; }
        public string Allergies_Drug { get; set; }
        public string Allergies_Other { get; set; }
        public string Habits_Smoking { get; set; }
        public string Habits_Drinking { get; set; }
        public Nullable<decimal> ReminderId { get; set; }
        public string Habits_Tobacco { get; set; }
        public string Habits_Other { get; set; }
        public string MedicalHistory { get; set; }
        public string CurrentMedication { get; set; }
        public string HabitsHistory { get; set; }

        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<decimal> Age { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual ICollection<emr_appointment_mf> emr_appointment_mf { get; set; }
        public virtual ICollection<emr_prescription_mf> emr_prescription_mf { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value1 { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value2 { get; set; }
        public virtual ICollection<emr_patient_bill> emr_patient_bill { get; set; }
        public virtual ICollection<ipd_admission> ipd_admission { get; set; }
        public virtual ICollection<pur_sale_mf> pur_sale_mf { get; set; }
        public virtual ICollection<pur_sale_hold_mf> pur_sale_hold_mf { get; set; }
        public virtual adm_reminder_mf adm_reminder_mf { get; set; }

    }
}
