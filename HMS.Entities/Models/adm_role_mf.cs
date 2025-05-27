using System;
using System.Collections.Generic;
using Repository.Pattern.Ef6;
namespace HMS.Entities.Models
{
    public partial class adm_role_mf : Entity
    {
        public adm_role_mf()
        {
            this.adm_role_dt = new List<adm_role_dt>();
            this.adm_user_company = new List<adm_user_company>();
        }

        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool SystemGenerated { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public decimal ModifiedBy { get; set; }
        public bool IsUpdateText { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual ICollection<adm_role_dt> adm_role_dt { get; set; }
        public virtual ICollection<adm_user_company> adm_user_company { get; set; }
    }
}
