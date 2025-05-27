using System;
using System.Collections.Generic;
using Repository.Pattern.Ef6;
namespace HMS.Entities.Models
{
    public partial class adm_role_dt : Entity
    {
        public decimal ID { get; set; }
        public decimal RoleID { get; set; }
        public decimal CompanyID { get; set; }
        public int DropDownScreenID { get; set; }
        public int ScreenID { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public decimal ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public bool ViewRights { get; set; }
        public bool CreateRights { get; set; }
        public bool DeleteRights { get; set; }
        public bool EditRights { get; set; }
        public virtual adm_role_mf adm_role_mf { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
    }
}
