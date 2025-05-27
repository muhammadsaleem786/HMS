using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pr_time_entry : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public decimal EmployeeID { get; set; }
        public DateTime AttendanceDate {  get; set; }
        public Nullable<decimal> LocationID { get; set; }
        public System.DateTime TimeIn { get; set; }
        public System.DateTime TimeOut { get; set; }
        [NotMapped]
        public string TimIntxt { get; set; }
        [NotMapped]
        public string TimOuttxt { get; set; }
        public int StatusDropDownID { get; set; }
        public int StatusID { get; set; }
        public string Remarks { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public decimal ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
        public virtual pr_employee_mf pr_employee_mf { get; set; }
    }
}
