using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class LoanDetailModel
    {
        public decimal ID { get; set; }
        public string ScreenType { get; set; }
        public string Transaction { get; set; }
        public double LoanAmount { get; set; }
        public DateTime LoanDate { get; set; }
        public double Payment { get; set; }
        public double Balance { get; set; }
        public string EmpName { get; set; }
        public string Description { get; set; }
        public double TotalBalance { get; set; }
        public string DeductionType { get; set; }
        public double DeductionValue { get; set; }
        public Nullable<double> InstallmentByBaseSalary { get; set; }
        public Nullable<decimal> AdjustmentAmount { get; set; }
        public Nullable<decimal> AdjustmentBy { get; set; }
        public string AdjustmentComments { get; set; }
        public Nullable<DateTime> AdjustmentDate { get; set; }
        public string AdjustmentType { get; set; }
        public double TotalInstallmentAmount { get; set; }
        public List<object> DummyList { get; set; }
    }
}
