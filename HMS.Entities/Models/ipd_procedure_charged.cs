using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class ipd_procedure_charged : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public decimal ProcedureId { get; set; }
        public Nullable<decimal> AppointmentId { get; set; }
        public decimal PatientId { get; set; }
        public decimal ItemId { get; set; }
        public bool IsBatch { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Item { get; set; }
        public string Batch {  get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual ipd_procedure_mf ipd_procedure_mf { get; set; }
        public virtual adm_item adm_item { get; set; }
    }
}
