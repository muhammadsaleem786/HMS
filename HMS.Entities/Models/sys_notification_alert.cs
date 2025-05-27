using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class sys_notification_alert:Entity
    {
        public decimal ID { get; set; }
        public Nullable<decimal> CompanyID { get; set; }
        public int TypeID { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public Nullable<System.DateTime> SentTime { get; set; }
        public int FailureCount { get; set; }
        public bool IsRead { get; set; }
        public string AttachmentPath { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
    }
}
