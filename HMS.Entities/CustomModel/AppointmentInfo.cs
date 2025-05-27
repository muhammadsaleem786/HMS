using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
   public class AppointmentInfo
    {
        public decimal ID { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> DoctorId { get; set; }
        public string Note { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string CNIC { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public decimal PatientId { get; set; }
        public Nullable<double> Amount { get; set; }
        public string CreatedBy { get; set; }
        public string Color { get; set; }
        public Nullable<DateTime> AppointmentDate { get; set; }
    }
}
