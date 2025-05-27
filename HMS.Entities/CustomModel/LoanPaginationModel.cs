using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class LoanPaginationModel
    {
        public decimal ID { get; set; }
        public string EmpName { get; set; }
        public string Description { get; set; }
        public string LoanDate { get; set; }
        public double LoanAmount { get; set; }
        public double PaymentAmount { get; set; }
        public double Balance { get; set; }
    }
}
