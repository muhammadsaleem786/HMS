using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pr_loan : Entity
    {
        public pr_loan()
        {
            this.pr_loan_payment_dt = new List<pr_loan_payment_dt>();
        }

        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public decimal EmployeeID { get; set; }
        public Nullable<int> PaymentMethodDropdownID { get; set; }
        public Nullable<int> PaymentMethodID { get; set; }
        public Nullable<int> LoanTypeDropdownID { get; set; }
        public string ApprovalStatusID {  get; set; }
        public int LoanTypeID { get; set; }
        public int LoanCode {  get; set; }
        public string Description { get; set; }
        public System.DateTime PaymentStartDate { get; set; }
        public System.DateTime LoanDate { get; set; }
        public double LoanAmount { get; set; }
        public string DeductionType { get; set; }
        public double DeductionValue { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public decimal ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public Nullable<System.DateTime> AdjustmentDate { get; set; }
        public string AdjustmentType { get; set; }
        public Nullable<decimal> AdjustmentAmount { get; set; }
        public string AdjustmentComments { get; set; }
        public Nullable<decimal> AdjustmentBy { get; set; }
        public Nullable<double> InstallmentByBaseSalary { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual pr_employee_mf pr_employee_mf { get; set; }
        public virtual ICollection<pr_loan_payment_dt> pr_loan_payment_dt { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
    }
}
