using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class emr_patient_bill : Entity
    {
        public emr_patient_bill()
        {
            this.emr_patient_bill_payment = new HashSet<emr_patient_bill_payment>();
        }
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public Nullable<decimal> AdmissionId { get; set; }
        public Nullable<decimal> AppointmentId { get; set; }
        public decimal DoctorId { get; set; }
        public decimal PatientId { get; set; }
        public decimal ServiceId { get; set; }
        public decimal OutstandingBalance { get; set; }
        public string Remarks { get; set; }
        public System.DateTime BillDate { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal CreatedBy { get; set; }
        [NotMapped]
        public decimal PartialAmount { get; set; }
        [NotMapped]
        public DateTime PaymentDate { get; set; }
        public Nullable<decimal> RefundAmount { get; set; }
        public Nullable<DateTime> RefundDate { get; set; }

        public System.DateTime CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual emr_service_mf emr_service_mf { get; set; }
        public virtual emr_patient_mf emr_patient_mf { get; set; }
        public virtual ipd_admission ipd_admission { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual ICollection<emr_patient_bill_payment> emr_patient_bill_payment { get; set; }

    }
}
