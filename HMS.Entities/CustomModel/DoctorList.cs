using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
  public class DoctorList
    {
        public string Name { get; set; }
        public decimal ID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string OffDay { get; set; }
        public Nullable<int> SlotTime { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string IsShowDoctor { get; set; }
        public bool IsDoctor { get; set; }

        public string Qualification { get; set; }
        public string Designation { get; set; }
        public string PhoneNo { get; set; }
        public string DoctorName { get; set; }
        public decimal DoctorId { get; set; }
        public string Footer {  get; set; }
        public string TemplateId {  get; set; }
        public string NameUrdu {  get; set; }
        public string HeaderDescription {  get; set; }
        public string QualificationUrdu {  get; set; }
        public string DesignationUrdu {  get; set; }
    }
}
