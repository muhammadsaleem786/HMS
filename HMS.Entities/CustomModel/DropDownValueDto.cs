using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class DropDownValueDto
    {
        public int ID { get; set; }
        public int DropDownID { get; set; }
        public string Value { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> DependedDropDownID { get; set; }
        public Nullable<int> DependedDropDownValueID { get; set; }
        public string SystemGenerated { get; set; }
        public Nullable<decimal> CompanyID { get; set; }
        public string Unit { get; set; }
    }
}
