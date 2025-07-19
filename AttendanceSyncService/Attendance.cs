using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSyncService
{
   public class Attendance
    {
        public decimal DeviceID { get; set; }
        public decimal CompanyID { get; set; }
        public string LocationCode { get; set; }
        public string IPAddress { get; set; }
        public string Password { get; set; }
        public int PortNo { get; set; }
        public string LastDataSync { get; set; }
    }
}
