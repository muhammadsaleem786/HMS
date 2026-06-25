using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class LabReportDto
    {
        public LabReportHeaderDto Header { get; set; }
        public PatientInfoDto Patient { get; set; }
        public List<LabResultDto> Results { get; set; }
    }
    public class LabReportHeaderDto
    {
        public string ReportNo { get; set; }
        public DateTime? ReportDate { get; set; }
        public string DepartmentName { get; set; }
        public string ReportTitle { get; set; }
    }

    public class PatientInfoDto
    {
        public string MRNO { get; set; }
        public string PatientName { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public string OrderedBy { get; set; }
        public string ReferringPhysician { get; set; }
        public string CollectionCenter { get; set; }

        public DateTime? RequestedDate { get; set; }
        public DateTime? CollectedDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? ReportedDate { get; set; }
    }

    public class LabResultDto
    {
        public string TestName { get; set; }
        public string NormalRange { get; set; }
        public string Unit { get; set; }

        public string CurrentResult { get; set; }
        public string PreviousResult1 { get; set; }
        public string PreviousResult2 { get; set; }

        public bool IsAbnormal { get; set; }
    }
}
