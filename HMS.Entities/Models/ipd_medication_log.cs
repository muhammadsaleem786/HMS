using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class ipd_medication_log : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public decimal AdmissionId { get; set; }
        public decimal PatientId { get; set; }
        public Nullable<decimal> AppointmentId { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public decimal DrugId {  get; set; }
        public string Dose {  get; set; }
        public Nullable<int> RouteId { get; set; }
        public Nullable<int> RouteDropdownId { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual ipd_admission ipd_admission { get; set; }
        public virtual adm_item adm_item { get; set; }
    }
}
