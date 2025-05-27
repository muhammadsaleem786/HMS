using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class ipd_admission_notes : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public decimal AdmissionId { get; set; }
        public Nullable<decimal> AppointmentId { get; set; }
        public decimal PatientId { get; set; }
        public string OnBehalfOf { get; set; }
        public string Note { get; set; }
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
