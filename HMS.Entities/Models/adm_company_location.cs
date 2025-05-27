using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class adm_company_location : Entity
    {
        public adm_company_location()
        {
            this.pr_employee_mf = new List<pr_employee_mf>();
        }

        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public int CountryDropDownID { get; set; }
        public Nullable<int> CountryID { get; set; }
        public int CityDropDownID { get; set; }
        public Nullable<int> CityID { get; set; }
        public string ZipCode { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value1 { get; set; }
        public virtual ICollection<pr_employee_mf> pr_employee_mf { get; set; }
    }
}
