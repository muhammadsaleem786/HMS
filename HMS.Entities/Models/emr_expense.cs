using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class emr_expense:Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public int CategoryId { get; set; }
        public int CategoryDropdownId { get; set; }
        public System.DateTime Date { get; set; }
        public string Remark { get; set; }
        public decimal Amount { get; set; }
        public decimal ClinicId { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string Vendor { get; set; }
        public Nullable<int> PaymentStatusId { get; set; }
        public Nullable<int> PaymentStatusDropdownId { get; set; }
        public string PaymentRemrks { get; set; }
        public string Attachment { get; set; }
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
