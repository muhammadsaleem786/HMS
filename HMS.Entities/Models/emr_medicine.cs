using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class emr_medicine:Entity
    {
        public emr_medicine()
        {
            this.ipd_procedure_medication = new List<ipd_procedure_medication>();
           
        }
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public string Medicine { get; set; }
        public string Measure {  get; set; }
        public Nullable<decimal> InstructionId {  get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> UnitId { get; set; }
        public Nullable<int> UnitDropdownId { get; set; }
        public Nullable<int> TypeId { get; set; }
        public Nullable<int> TypeDropdownId { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public decimal ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value1 { get; set; }
        public virtual ICollection<ipd_procedure_medication> ipd_procedure_medication { get; set; }       
        public virtual emr_instruction emr_instruction { get; set; }

    }
}
