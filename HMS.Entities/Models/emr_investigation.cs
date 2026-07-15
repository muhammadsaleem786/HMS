using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class emr_investigation : Entity
    {
        public emr_investigation()
        {
            this.lab_parameter = new List<lab_parameter>();
            this.lab_patient_test = new List<lab_patient_test>();
            this.lab_result_mf = new List<lab_result_mf>();
            this.emr_investigation_Item = new List<emr_investigation_Item>();
        }
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public string Investigation { get; set; }
        public int CategoryId { get; set; }
        public int CategoryDropdownId { get; set; }
        public int SampleId { get; set; }
        public int SampleDropdownId { get; set; }
        public bool IsItem { get; set; } = false;
        public string TAT { get; set; }
        public decimal Price { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public decimal ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value1 { get; set; }
        public virtual ICollection<lab_parameter> lab_parameter { get; set; }
        public virtual ICollection<lab_patient_test> lab_patient_test { get; set; }
        public virtual ICollection<lab_result_mf> lab_result_mf { get; set; }
        public virtual ICollection<emr_investigation_Item> emr_investigation_Item { get; set; }
    }
}
