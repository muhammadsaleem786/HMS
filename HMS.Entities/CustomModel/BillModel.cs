using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class BillModel
    {
        public decimal ID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }
        public string PatientName { get; set; }
        public string AdmisDate { get; set; }
        public string Room { get; set; }
        public string Ward { get; set; }
        public string MRNo { get; set; }
        public string PatientAddress { get; set; }
        public string PatientEmail { get; set; }
        public string PatientMobile { get; set; }
        public DateTime BillDate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string CompanyLogo { get; set; }
        public decimal DoctorId { get; set; }
    }
}
