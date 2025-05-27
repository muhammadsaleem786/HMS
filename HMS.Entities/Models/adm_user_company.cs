using System;
using System.Collections.Generic;
using Repository.Pattern.Ef6;
namespace HMS.Entities.Models
{
    public partial class adm_user_company : Entity
    {
        public decimal ID { get; set; }
        public decimal UserID { get; set; }
        public decimal CompanyID { get; set; }
        public decimal EmployeeID { get; set; }
        public decimal RoleID { get; set; }
        public decimal AdminID { get; set; }
        public bool IsDefault { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_role_mf adm_role_mf { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
    }
}
