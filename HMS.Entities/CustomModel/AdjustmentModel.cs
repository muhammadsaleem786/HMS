using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class AdjustmentModel
    {
        public decimal PayrollMfID { get; set; }
        public decimal PayrollDtID { get; set; }
        public decimal EmployeeID { get; set; }
        public decimal PayScheduleID { get; set; }
        public DateTime PayDate { get; set; }
        public decimal AllowDedConID { get; set; }
        public string AllDedContType { get; set; }
        public string AdjustmentTitle { get; set; }
        public string OrignalText { get; set; }
        public string AdjustmentText { get; set; }
        public decimal OrignalValue { get; set; }
        public decimal AdjustmentValue { get; set; }
        public string AdjustmentType { get; set; }
        public string AdjustmentComments { get; set; }
        public decimal AdjustmentAmount { get; set; }
        public decimal AdjustmentBy { get; set; }
        public DateTime AdjustmentDate { get; set; }
    }
}
