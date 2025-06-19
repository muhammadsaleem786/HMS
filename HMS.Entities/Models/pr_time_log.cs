using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{

    public partial class pr_time_log : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime AttendanceTime { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int16 AttendanceMode { get; set; }
        public string IPAddress {  get; set; }
        public string LocationCode {  get; set; }
    }
}
