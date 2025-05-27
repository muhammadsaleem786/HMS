using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pr_employee_document : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public decimal EmployeeID { get; set; }
        public int DocumentTypeID { get; set; }
        public int DocumentTypeDropdownID { get; set; }
        public string Description { get; set; }
        public string AttachmentPath { get; set; }
        public System.DateTime UploadDate { get; set; }
        public System.DateTime ExpireDate { get; set; }
        public virtual pr_employee_mf pr_employee_mf { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
    }
}
