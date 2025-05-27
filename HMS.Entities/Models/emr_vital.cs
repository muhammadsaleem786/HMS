using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class emr_vital:Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public string Measure { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public int VitalId { get; set; }
        public int VitalDropdownId { get; set; }
        public Nullable<decimal> PatientId { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
    }
}
