using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public partial class ReportModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public Nullable<decimal> PatientId { get; set; }
        public Nullable<decimal> ItemId { get; set; }
        public Nullable<decimal> DoctorId { get; set; }
        public Nullable<decimal> ProcedureId { get; set; }
        public int Type { get; set; }
        public string base64 { get; set; }
        public string FileName {get;set;}
        public string Email { get; set; }
}
}
