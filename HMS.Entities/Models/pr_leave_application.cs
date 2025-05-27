using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pr_leave_application : Entity
    {
        [Key]
        public decimal ID { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public double Hours { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public decimal ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public decimal CompanyID { get; set; }
        [ForeignKey("CompanyID")]
        public virtual adm_company adm_company { get; set; }
        public decimal EmployeeID { get; set; }
        [ForeignKey("EmployeeID")]
        public virtual pr_employee_mf pr_employee_mf { get; set; }
        public decimal LeaveTypeID { get; set; }
        [ForeignKey("LeaveTypeID")]
        public virtual pr_leave_type pr_leave_type { get; set; }
    }
}
