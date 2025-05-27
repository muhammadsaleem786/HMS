using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class EmpBulkUpdateModel
    {
        public string BulkUpdateCategoryType { get; set; }
        public decimal AllConDedId { get; set; }
        public string AllConDedValueType { get; set; }
        public double Amount { get; set; }
        public bool Taxable { get; set; }
        public List<object> AllConDedTypes { get; set; }
        public decimal?[] EmployeeIds { get; set; }
        public decimal?[] LocationsIds { get; set; }
        public decimal?[] DepartmentsIds { get; set; }
        public decimal?[] DesignationsIds { get; set; }
        public decimal?[] EmployeeTypeIds { get; set; }
    }
}
