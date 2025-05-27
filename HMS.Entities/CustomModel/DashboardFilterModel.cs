using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class DashboardFilterModel
    {
        public string Range { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public decimal PayScheduleID { get; set; }
        public decimal LocationID { get; set; }
        public decimal DepartmentID { get; set; }
    }
}
