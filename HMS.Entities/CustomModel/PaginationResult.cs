using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class PaginationResult
    {
        public int TotalRecord { get; set; }
        public List<object> DataList { get; set; }
        public object OtherDataModel { get; set; }
        public object OtherData2Model { get; set; }
        public object OtherData3Model { get; set; }
        public object DoctorList { get; set; }
        public object PatientList { get; set; }
        public object GenderList { get; set; }
        public object AppointmentList { get; set; }
        public object AllStatusList { get; set; }
        public object PatientInfo { get; set; }
        public object DoctorCalander { get; set; }
        public object IsShowDoctorIds { get; set; }
        public object ClinicList { get; set; }
        public object BloodList { get; set; }
        public object MedicineList { get; set; }
        public object ServiceType { get; set; }
        public string TokenNo { get; set; }
        public object Reminderlist {  get; set; }
        public object Followuplist {  get; set; }
        public bool IsBackDatedAppointment { get; set; }
       public List<DoctorList> DoctList { get; set; }

    }
}
