using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class BulkEmpModel
    {
        public decimal ID { get; set; }
        public string Employee { get; set; }
        public double ExistingAmount { get; set; }
        public double Amount { get; set; }
        public string Category { get; set; }
        public bool Taxable { get; set; }
        public bool Remove { get; set; }
        public bool UseExistingAmount { get; set; }
        public decimal AllwConDedBSalID { get; set; }
        public double BasicSalary { get; set; }
    }
}
