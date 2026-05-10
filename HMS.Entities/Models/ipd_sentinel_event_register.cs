using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public class ipd_sentinel_event_register : Entity
    {
        public int ID { get; set; }
        public decimal CompanyID { get; set; }

        public DateTime EventDate { get; set; }

        public string EventType { get; set; }

        public string EventDescription { get; set; }

        public string ImmediateActionTaken { get; set; }

        public string OutcomeDetails { get; set; }

        public string ReportedBy { get; set; }

        public string ReviewedByClinicalManager { get; set; }

        public bool IsSigned { get; set; }

        public string Attachment { get; set; }
        public decimal CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
    }
}
