using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pr_employee_Dependent : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public decimal EmployeeID { get; set; }
        public Nullable<int> RelationshipDropdownID { get; set; }
        public Nullable<int> RelationshipTypeID { get; set; }
        public Nullable<bool> IsEmergencyContact { get; set; }
        public Nullable<bool> IsTicketEligible { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public Nullable<int> NationalityTypeDropdownID { get; set; }
        public Nullable<int> NationalityTypeID { get; set; }
        public string IdentificationNumber { get; set; }
        public string PassportNumber { get; set; }
        public Nullable<int> MaritalStatusTypeDropdownID { get; set; }
        public Nullable<int> MaritalStatusTypeID { get; set; }
        public System.DateTime? DOB { get; set; }
        public string Remarks { get; set; }
        public virtual pr_employee_mf pr_employee_mf { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value1 { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value2 { get; set; }
    }
}
