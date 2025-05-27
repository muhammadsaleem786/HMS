using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
   public class PatientList
    {
        public decimal ID { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public Nullable<DateTime> AppointmentDate { get; set; }
        public Nullable<TimeSpan> AppointmentTime { get; set; }
        public Nullable<decimal> DoctorId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string CNIC { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public decimal PatientId { get; set; }
        public string CreatedBy { get; set; }
        public string Note { get; set; }
        public int Gender { get; set; }
        public string Image { get; set; }
    }
}
