using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class PayrollMasterPaginationSearchSortModel
    {
        public string UniqueID { get; set; }
        public string ScheduleName { get; set; }
        public string Status { get; set; }
        public string PayPeriod { get; set; }
        public DateTime PayDate { get; set; }
        public decimal NoOfEmp { get; set; }
        public decimal Tax { get; set; }
        public decimal TestBaseAdjustment { get; set; }
        public decimal TestALLDEDConAdjustment { get; set; }
        public decimal TestALLDEDCon { get; set; }
        public decimal NetSalary { get; set; }
    }
}
