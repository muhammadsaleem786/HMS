using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pr_loan_payment_dt : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public decimal LoanID { get; set; }
        public System.DateTime PaymentDate { get; set; }
        public string Comment { get; set; }
        public double Amount { get; set; }
        public Nullable<System.DateTime> AdjustmentDate { get; set; }
        public string AdjustmentType { get; set; }
        public Nullable<decimal> AdjustmentAmount { get; set; }
        public string AdjustmentComments { get; set; }
        public Nullable<decimal> AdjustmentBy { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual pr_loan pr_loan { get; set; }
    }
}
