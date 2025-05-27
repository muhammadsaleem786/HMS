using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities.Models
{
    public partial class emr_appointment_mf : Entity
    {

        public emr_appointment_mf()
        {
            this.ipd_admission_charges = new List<ipd_admission_charges>();
        }
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public decimal PatientId { get; set; }
        public string PatientProblem { get; set; }
        public decimal DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public int TokenNo { get; set; }
        public string Notes { get; set; }
        public bool IsAdmission { get; set; }
        public Nullable<int> ReminderId { get; set; }
        public Nullable<decimal> AdmissionId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public bool IsAdmit { get; set; }

        //bill field
        [NotMapped]
        public Nullable<decimal> AppointmentId { get; set; }
        [NotMapped]
        public bool IsExistFollowUp { get; set; }
        [NotMapped]
        public string SecondaryDescription { get; set; }
        [NotMapped]
        public string PrimaryDescription { get; set; }
        [NotMapped]
        public decimal ServiceId { get; set; }
        [NotMapped]
        public System.DateTime BillDate { get; set; }
        [NotMapped]
        public decimal Price { get; set; }
        [NotMapped]
        public decimal Discount { get; set; }
        [NotMapped]
        public decimal PaidAmount { get; set; }
        [NotMapped]
        public decimal OutstandingBalance { get; set; }
        [NotMapped]
        public string Remarks { get; set; }
        //<-------------------------------------------->
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual adm_user_mf adm_user_mf2 { get; set; }
        public virtual emr_patient_mf emr_patient_mf { get; set; }
        public virtual ICollection<ipd_admission_charges> ipd_admission_charges { get; set; }
    }
}
