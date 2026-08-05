using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class ShareRecord
    {
        public decimal ID { get; set; }
        public DateTime? BillDate { get; set; }
        public decimal? DoctorId { get; set; }
        public string Category { get; set; }
        public decimal? ServiceId { get; set; }
        public decimal? LabId { get; set; }
        public decimal? ItemId { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public string ServiceName { get; set; }
        public string CreatedBy { get; set; }
        public decimal? PatientId { get; set; }
        public string PatientName { get; set; }
        public string MrNo { get; set; }
        public string Mobile { get; set; }
    }
}
