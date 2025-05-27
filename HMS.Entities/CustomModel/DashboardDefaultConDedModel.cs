using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class DashboardDefaultConDedModel
    {
        public decimal EmployeeEOBIID { get; set; }
        public decimal EmployerEOBIID { get; set; }
        public decimal EmployeePFID { get; set; }
        public decimal EmployerPFID { get; set; }
        public decimal EmployeeGOSIID { get; set; }
        public decimal EmployerGOSIID { get; set; }

        public decimal[] VacationIds { get; set; }
        public decimal[] SickIds { get; set; }
    }
}
