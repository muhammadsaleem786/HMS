using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
  public  class PrescriptionModel
    {
        public decimal ID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string PatientName { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal OutAmount { get; set; }
        public object medicine { get; set; }
    }
}
