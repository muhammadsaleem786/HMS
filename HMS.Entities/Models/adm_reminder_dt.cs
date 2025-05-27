using System;
using System.Collections.Generic;
using Repository.Pattern.Ef6;
namespace HMS.Entities.Models
{
    public partial class adm_reminder_dt : Entity
    {
        public decimal ID { get; set; }
        public decimal ReminderId { get; set; }
        public decimal CompanyId { get; set; }
        public int SMSTypeId { get; set; }
        public int SMSTypeDropDownId { get; set; }
        public int Value { get; set; }
        public int TimeTypeId { get; set; }
        public int TimeTypeDropDownId { get; set; }
        public string BeforeAfter { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual adm_reminder_mf adm_reminder_mf { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value1 { get; set; }

    }
}
