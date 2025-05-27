using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class LoanAdjustmentModel
    {
        public decimal LoanMfID { get; set; }
        public decimal LoanDtID { get; set; }
        public string EmployeeName { get; set; }
        public string LoanScreenType { get; set; }
        public string AdjustmentTitle { get; set; }
        public string AdjustmentText { get; set; }
        public decimal AdjustmentValue { get; set; }
        public string OrignalText { get; set; }
        public decimal OrignalValue { get; set; }
        public DateTime AdjustmentDate { get; set; }
        public string AdjustmentType { get; set; }
        public decimal AdjustmentAmount { get; set; }
        public string AdjustmentComments { get; set; }
        public decimal AdjustmentBy { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
