using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class emr_prescription_mf : Entity
    {
        public emr_prescription_mf()
        {
            this.emr_prescription_complaint = new List<emr_prescription_complaint>();
            this.emr_prescription_diagnos = new List<emr_prescription_diagnos>();
            this.emr_prescription_investigation = new List<emr_prescription_investigation>();
            this.emr_prescription_observation = new List<emr_prescription_observation>();
            this.emr_prescription_treatment = new List<emr_prescription_treatment>();
            this.emr_prescription_treatment_template = new List<emr_prescription_treatment_template>();
        }

        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public bool IsTemplate { get; set; }
        [NotMapped]
        public string Email { get; set; }
        public System.DateTime AppointmentDate { get; set; }
        public decimal PatientId { get; set; }
        public decimal ClinicId { get; set; }
        public decimal DoctorId { get; set; }
        public Nullable<System.DateTime> FollowUpDate { get; set; }
        public Nullable<System.TimeSpan> FollowUpTime { get; set; }
        public string FollowUpNotes {  get; set; }
        public bool IsCreateAppointment { get; set; }
        public string Notes { get; set; }
        public decimal CreatedBy { get; set; }
        [NotMapped]
        public string TemplateName { get; set; }
        [NotMapped]
        public Nullable<int> AppointmentId { get; set; }
        public Nullable<int> Day { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public decimal ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual emr_patient_mf emr_patient_mf { get; set; }
        public virtual ICollection<emr_prescription_complaint> emr_prescription_complaint { get; set; }
        public virtual ICollection<emr_prescription_diagnos> emr_prescription_diagnos { get; set; }
        public virtual ICollection<emr_prescription_investigation> emr_prescription_investigation { get; set; }
        public virtual ICollection<emr_prescription_observation> emr_prescription_observation { get; set; }
        public virtual ICollection<emr_prescription_treatment> emr_prescription_treatment { get; set; }
        public virtual ICollection<emr_prescription_treatment_template> emr_prescription_treatment_template { get; set; }
       
    }
}
