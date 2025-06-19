using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSyncService
{
    public class AttendanceModel
    {
        public decimal CompanyID { get; set; }
        public string LocationCode { get; set; }
        public string EmployeeCode { get; set; }
        public Int16 AttendanceMode { get; set; }
        public string AttendanceTime { get; set; }
    }
}
