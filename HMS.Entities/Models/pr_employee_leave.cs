using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pr_employee_leave : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public decimal EmployeeID { get; set; }
        public decimal LeaveTypeID { get; set; }
        public double Hours { get; set; }
        public virtual pr_employee_mf pr_employee_mf { get; set; }
        public virtual pr_leave_type pr_leave_type { get; set; }
    }
}
