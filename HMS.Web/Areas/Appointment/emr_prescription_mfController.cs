using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
using HMS.Service;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Appointment;
using HMS.Service.Services.Employee;
using HMS.Service.Services.Items;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using HMS.Web.API.Interface;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NPOI.Util;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace HMS.Web.API.Areas.Appointment.Controllers
{
    [JwtAuthentication]
    public class emr_prescription_mfController : ApiController, IERPAPIInterface<emr_prescription_mf>, IDisposable
    {
        private readonly Iemr_prescription_mfService _service;
        private readonly Iemr_medicineService _emr_medicineService;
        private readonly Iemr_prescription_complaintService _emr_prescription_complaintService;
        private readonly Iemr_prescription_diagnosService _emr_prescription_diagnosService;
        private readonly Iemr_prescription_investigationService _emr_prescription_investigationService;
        private readonly Iemr_prescription_observationService _emr_prescription_observationService;
        private readonly Iemr_prescription_treatmentService _emr_prescription_treatmentService;
        private readonly Iemr_prescription_treatment_templateService _emr_prescription_treatment_templateService;
        private readonly Iemr_appointment_mfService _emr_appointment_mfService;
        private readonly Iadm_user_companyService _adm_user_companyService;
        private readonly Iadm_role_mfService _adm_role_mfService;
        private readonly Iadm_user_tokenService _adm_user_tokenService;
        private readonly Isys_notification_alertService _sys_notification_alertService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly Iadm_userService _adm_userService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iemr_patientService _emr_patientService;
        private readonly IStoredProcedureService _procedureService;
        private readonly Iemr_vitalService _emr_vitalService;
        private readonly Iemr_complaintService _emr_complaintService;
        private readonly Iemr_diagnosService _emr_diagnosService;
        private readonly Iemr_investigationService _emr_investigationService;
        private readonly Iemr_observationService _emr_observationService;
        private readonly Iemr_instructionService _emr_instructionService;
        private readonly Iemr_service_mfService _emr_service_mfService;
        private readonly Iadm_companyService _adm_companyService;
        private readonly Iadm_itemService _adm_itemService;
        private readonly Iemr_notes_favoriteService _emr_notes_favoriteService;
        private readonly Iadm_reminder_mfService _adm_reminder_mf;
        private readonly Ipr_employee_mfService _pr_employee_mfService;


        public emr_prescription_mfController(IUnitOfWorkAsync unitOfWorkAsync, Iemr_service_mfService emr_service_mfService, Iemr_vitalService emr_vitalService, Iemr_patientService emr_patientService, Iemr_prescription_mfService service, Iemr_medicineService emr_medicineService,
            Iemr_prescription_complaintService emr_prescription_complaintService, Iemr_prescription_diagnosService emr_prescription_diagnosService,
            Iemr_prescription_investigationService emr_prescription_investigationService,
            Iemr_prescription_observationService emr_prescription_observationService,
            Iemr_complaintService emr_complaintService, Iadm_reminder_mfService adm_reminder_mf,
            Iemr_diagnosService emr_diagnosService,
            Iemr_investigationService emr_investigationService,
            Iemr_observationService emr_observationService,
            Iemr_instructionService emr_instructionService,
            Iadm_companyService adm_companyService,
            Iemr_notes_favoriteService emr_notes_favoriteService,
            Ipr_employee_mfService pr_employee_mfService,
                      Iemr_prescription_treatmentService emr_prescription_treatmentService, Iemr_prescription_treatment_templateService emr_prescription_treatment_templateService,
            Iadm_userService adm_userService, Iemr_appointment_mfService emr_appointment_mfService, Iadm_user_companyService adm_user_companyService,
            Iadm_role_mfService adm_role_mfService, Iadm_user_tokenService adm_user_tokenService, Isys_drop_down_valueService sys_drop_down_valueService,
            Isys_notification_alertService sys_notification_alertService, IStoredProcedureService procedureService, Iadm_itemService adm_itemService)

        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _emr_medicineService = emr_medicineService;
            _service = service; _emr_service_mfService = emr_service_mfService;
            _emr_vitalService = emr_vitalService;
            _procedureService = procedureService;
            _adm_companyService = adm_companyService;
            _adm_reminder_mf = adm_reminder_mf;
            _emr_prescription_complaintService = emr_prescription_complaintService;
            _emr_prescription_diagnosService = emr_prescription_diagnosService;
            _emr_prescription_investigationService = emr_prescription_investigationService;
            _emr_prescription_observationService = emr_prescription_observationService;
            _emr_prescription_treatmentService = emr_prescription_treatmentService;
            _emr_prescription_treatment_templateService = emr_prescription_treatment_templateService;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _adm_user_companyService = adm_user_companyService;
            _adm_userService = adm_userService;
            _emr_appointment_mfService = emr_appointment_mfService;
            _adm_role_mfService = adm_role_mfService;
            _adm_user_tokenService = adm_user_tokenService;
            _emr_patientService = emr_patientService;
            _sys_notification_alertService = sys_notification_alertService;
            _emr_complaintService = emr_complaintService;
            _emr_diagnosService = emr_diagnosService;
            _emr_investigationService = emr_investigationService;
            _emr_observationService = emr_observationService;
            _emr_instructionService = emr_instructionService;
            _adm_itemService = adm_itemService;
            _emr_notes_favoriteService = emr_notes_favoriteService;
            _pr_employee_mfService = pr_employee_mfService;
        }
        [HttpPost]
        [HttpGet]
        [ActionName("Save")]
        public async Task<ResponseInfo> Save(emr_prescription_mf Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                List<emr_prescription_complaint> emr_prescription_complaint = new List<emr_prescription_complaint>();
                emr_prescription_complaint.AddRange(Model.emr_prescription_complaint);

                List<emr_prescription_diagnos> emr_prescription_diagnos = new List<emr_prescription_diagnos>();
                emr_prescription_diagnos.AddRange(Model.emr_prescription_diagnos);

                List<emr_prescription_investigation> emr_prescription_investigation = new List<emr_prescription_investigation>();
                emr_prescription_investigation.AddRange(Model.emr_prescription_investigation);

                List<emr_prescription_observation> emr_prescription_observation = new List<emr_prescription_observation>();
                emr_prescription_observation.AddRange(Model.emr_prescription_observation);

                List<emr_prescription_treatment> emr_prescription_treatment = new List<emr_prescription_treatment>();
                emr_prescription_treatment.AddRange(Model.emr_prescription_treatment);
                decimal ID = 1;
                if (_service.Queryable().Count() > 0)
                    ID = _service.Queryable().Max(e => e.ID) + 1;

                Model.ID = ID;
                Model.CompanyID = CompanyID;
                Model.CreatedBy = UserID;
                Model.CreatedDate = Request.DateTimes();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Modified;
                Model.emr_prescription_complaint = null;
                Model.emr_prescription_diagnos = null;
                Model.emr_prescription_investigation = null;
                Model.emr_prescription_observation = null;
                Model.emr_prescription_treatment = null;
                Model.emr_prescription_treatment_template = null;
                Model.adm_user_mf = null;
                Model.adm_user_mf1 = null;
                Model.emr_patient_mf = null;
                _service.Insert(Model);
                //insert complaint
                decimal complaintID = 1;
                if (_emr_prescription_complaintService.Queryable().Count() > 0)
                    complaintID = _emr_prescription_complaintService.Queryable().Max(e => e.ID) + 1;

                foreach (emr_prescription_complaint item in emr_prescription_complaint)
                {
                    item.ID = complaintID;
                    item.PrescriptionId = Model.ID;
                    item.PatientId = Model.PatientId;
                    item.CompanyID = CompanyID;
                    item.Complaint = item.Complaint;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.ObjectState = ObjectState.Added;
                    item.emr_prescription_mf = null;
                    item.adm_user_mf = null;
                    item.adm_user_mf1 = null;
                    _emr_prescription_complaintService.Insert(item);
                    complaintID++;
                }
                //insert diagnos
                decimal diagnosID = 1;
                if (_emr_prescription_diagnosService.Queryable().Count() > 0)
                    diagnosID = _emr_prescription_diagnosService.Queryable().Max(e => e.ID) + 1;

                foreach (emr_prescription_diagnos item in emr_prescription_diagnos)
                {
                    item.ID = diagnosID;
                    item.PrescriptionId = Model.ID;
                    item.PatientId = Model.PatientId;
                    item.CompanyID = CompanyID;
                    item.Diagnos = item.Diagnos;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.ObjectState = ObjectState.Added;
                    item.emr_prescription_mf = null;
                    item.adm_user_mf = null;
                    item.adm_user_mf1 = null;
                    _emr_prescription_diagnosService.Insert(item);
                    diagnosID++;
                }
                //insert investigation
                decimal investigationID = 1;
                if (_emr_prescription_investigationService.Queryable().Count() > 0)
                    investigationID = _emr_prescription_investigationService.Queryable().Max(e => e.ID) + 1;

                foreach (emr_prescription_investigation item in emr_prescription_investigation)
                {
                    item.ID = investigationID;
                    item.PrescriptionId = Model.ID;
                    item.PatientId = Model.PatientId;
                    item.CompanyID = CompanyID;
                    item.Investigation = item.Investigation;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.ObjectState = ObjectState.Added;
                    item.emr_prescription_mf = null;
                    item.adm_user_mf = null;
                    item.adm_user_mf1 = null;
                    _emr_prescription_investigationService.Insert(item);
                    investigationID++;
                }
                //insert Observation
                decimal observationID = 1;
                if (_emr_prescription_observationService.Queryable().Count() > 0)
                    observationID = _emr_prescription_observationService.Queryable().Max(e => e.ID) + 1;

                foreach (emr_prescription_observation item in emr_prescription_observation)
                {
                    item.ID = observationID;
                    item.PrescriptionId = Model.ID;
                    item.PatientId = Model.PatientId;
                    item.CompanyID = CompanyID;
                    item.Observation = item.Observation;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.emr_prescription_mf = null;
                    item.adm_user_mf = null;
                    item.adm_user_mf1 = null;
                    item.ObjectState = ObjectState.Added;
                    _emr_prescription_observationService.Insert(item);
                    observationID++;
                }
                //insert treatment
                decimal treatmentID = 1;
                if (_emr_prescription_treatmentService.Queryable().Count() > 0)
                    treatmentID = _emr_prescription_treatmentService.Queryable().Max(e => e.ID) + 1;

                foreach (emr_prescription_treatment item in emr_prescription_treatment)
                {
                    item.ID = treatmentID;
                    item.PrescriptionId = Model.ID;
                    item.PatientId = Model.PatientId;
                    item.CompanyID = CompanyID;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    if (item.Instructions == null)
                        item.Instructions = "";
                    item.ObjectState = ObjectState.Added;
                    item.emr_prescription_mf = null;
                    item.adm_user_mf = null;
                    item.adm_user_mf1 = null;
                    _emr_prescription_treatmentService.Insert(item);
                    treatmentID++;
                }
                if (Model.IsTemplate)
                {
                    decimal TemplateID = 1;
                    if (_emr_prescription_treatment_templateService.Queryable().Count() > 0)
                        TemplateID = _emr_prescription_treatment_templateService.Queryable().Max(e => e.ID) + 1;

                    emr_prescription_treatment_template obj = new emr_prescription_treatment_template();
                    obj.ID = TemplateID;

                    obj.TemplateName = Model.TemplateName;
                    obj.PrescriptionId = Model.ID;
                    obj.CompanyID = Request.CompanyID();
                    obj.CreatedBy = Request.LoginID();
                    obj.CreatedDate = Request.DateTimes();
                    obj.ModifiedBy = Request.LoginID();
                    obj.emr_prescription_mf = null;
                    obj.adm_user_mf = null;
                    obj.adm_user_mf1 = null;
                    obj.ModifiedDate = Request.DateTimes();
                    _emr_prescription_treatment_templateService.Insert(obj);
                }

                var appinfo = _emr_appointment_mfService.Queryable().Where(a => a.PatientId == Model.PatientId && DbFunctions.TruncateTime(a.AppointmentDate) == DbFunctions.TruncateTime(Model.FollowUpDate)).FirstOrDefault();
                if (Model.IsCreateAppointment && appinfo == null && Model.FollowUpDate != null)
                {
                    emr_appointment_mf appObj = new emr_appointment_mf();
                    decimal AppID = 0;
                    if (_emr_appointment_mfService.Queryable().Count() > 0)
                        AppID = _emr_appointment_mfService.Queryable().Max(e => e.ID) + 1;
                    appObj.ID = AppID;
                    appObj.StatusId = (int)sys_dropdown_mfEnum.StatusId;
                    appObj.CompanyId = Request.CompanyID();
                    appObj.PatientId = Model.PatientId;
                    appObj.DoctorId = Model.DoctorId;
                    appObj.AppointmentDate =Convert.ToDateTime(Model.FollowUpDate);
                    appObj.AppointmentTime = (TimeSpan)Model.FollowUpTime;
                    appObj.Notes = Model.Notes;
                    appObj.CreatedBy = Request.LoginID();
                    appObj.CreatedDate = Request.DateTimes();
                    appObj.ModifiedBy = Request.LoginID();
                    appObj.ModifiedDate = Request.DateTimes();
                    //Model = appObj;
                    appObj.ObjectState = ObjectState.Added;
                    _emr_appointment_mfService.Insert(appObj);
                }
                else if (Model.IsCreateAppointment && appinfo != null)
                {
                    appinfo.DoctorId = Model.DoctorId;
                    appinfo.AppointmentDate = Convert.ToDateTime(Model.FollowUpDate);
                    appinfo.AppointmentTime = (TimeSpan)Model.FollowUpTime;
                    appinfo.Notes = Model.Notes;
                    appinfo.ObjectState = ObjectState.Modified;
                    _emr_appointment_mfService.Update(appinfo);
                }

                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Save;
                    objResponse.IsSuccess = true;

                    var ids = Model?.emr_prescription_treatment?.Select(a => a.MedicineId).ToList() ?? new List<decimal?>();
                    var medicineList = _emr_medicineService.Queryable().Where(a => a.CompanyID == CompanyID && ids.Contains(a.ID)).Select(a => new { Id = a.ID, Medicine = a.Medicine, Type = a.sys_drop_down_value1.Value });
                    var itemList = _adm_itemService.Queryable().Where(a => a.CompanyId == CompanyID && ids.Contains(a.ID)).Select(a => new { Id = a.ID, Medicine = a.Name, Type = a.sys_drop_down_value1.Value });
                    var MedicineList = itemList.Union(medicineList).ToList();
                    var patientInfo = _emr_patientService.Queryable().Where(a => a.CompanyId == CompanyID && a.ID == Model.PatientId)
                        .Select(z => new
                        {
                            z.PatientName,
                            z.Mobile,
                            z.Age,
                            ClinicIogo = z.adm_company.CompanyLogo,
                            Gender = z.Gender == 1 ? "Male" : z.Gender == 2 ? "Female" : "Other",
                        }).FirstOrDefault();
                    var role = _adm_user_companyService.Queryable()
                     .Where(u => u.UserID == UserID && u.CompanyID == CompanyID)
                     .Select(u => u.adm_role_mf.RoleName)
                     .FirstOrDefault();
                    var doctorUsers = _adm_user_companyService.Queryable()
            .Where(u => u.CompanyID == CompanyID && u.adm_role_mf.RoleName == "Doctor")
            .Select(u => u.adm_user_mf)
            .OrderBy(u => u.ID)
            .ToList();
                    var doctors = doctorUsers.Select(mf =>
                    {
                        var emp = _pr_employee_mfService.Queryable().FirstOrDefault(e => e.ID == mf.EmployeeID);

                        return (object)new
                        {
                            ID = mf.ID,
                            Name = mf.Name,
                            StartTime = emp != null ? emp.StartTime : null,
                            EndTime = emp != null ? emp.EndTime : null,
                            SlotTime = mf.SlotTime,
                            WDSunday = emp?.WDSunday ?? false,
                            WDMonday = emp?.WDMonday ?? false,
                            WDTuesday = emp?.WDTuesday ?? false,
                            WDWednesday = emp?.WDWednesday ?? false,
                            WDThursday = emp?.WDThursday ?? false,
                            WDFriday = emp?.WDFriday ?? false,
                            WDSaturday = emp?.WDSaturday ?? false,
                            IsShowDoctor = mf.IsShowDoctor,
                            IsDoctor = false,
                            Qualification = mf.Qualification,
                            Designation = mf.Designation,
                            PhoneNo = mf.PhoneNo,
                            Footer = mf.RepotFooter,
                            TemplateId = mf.TemplateId,
                            QualificationUrdu = mf.QualificationUrdu,
                            DesignationUrdu = mf.DesignationUrdu,
                            NameUrdu = mf.NameUrdu,
                            HeaderDescription = mf.HeaderDescription,
                            HeaderDescriptionUrdu = mf.HeaderDescriptionUrdu,
                            IsOverLap = mf.IsOverLap
                        };
                    }).ToList();
                    List<object> result;
                    if (role == "Administrator")
                    {
                        result = doctors.Where(d => ((dynamic)d).ID == Model.DoctorId).ToList<object>();
                    }
                    else if (role == "Doctor")
                    {
                        result = doctors.Where(d => ((dynamic)d).ID == UserID).ToList<object>();
                    }
                    else
                    {
                        result = doctors;
                    }
                    var vitallist = _emr_vitalService.Queryable().Where(a => a.CompanyID == CompanyID && a.PatientId == Model.PatientId && DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(Model.AppointmentDate)).Select(a => new
                    {
                        Measure = a.Measure,
                        Unit = a.sys_drop_down_value.Unit,
                        Name = a.sys_drop_down_value.Value
                    }).ToList();
                    objResponse.ResultSet = new
                    {
                        Model = Model,
                        patientInfo = patientInfo,
                        doctor = result,
                        vitallist = vitallist,
                        MedicineList = MedicineList,
                    };

                }
                catch (DbUpdateException)
                {
                    if (ModelExists(Model.ID.ToString()))
                    {
                        objResponse.IsSuccess = false;
                        objResponse.ErrorMessage = MessageStatement.Conflict;
                        return objResponse;
                    }
                    throw;
                }
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        public string WorkingHours(DateTime? TimeIn, decimal? DoctorId)
        {
            var slotTime = _adm_userService.Queryable().Where(a => a.ID == DoctorId).FirstOrDefault().SlotTime;
            var aaa = slotTime.Value.Minutes;
            TimeSpan TimIn = new TimeSpan(TimeIn.Value.Hour, TimeIn.Value.Minute, 0);

            var Tim = TimeIn.Value.AddMinutes(aaa);
            var timeformate = Tim.ToString("HH\\:mm\\:ss");
            return timeformate.ToString();

        }
        [HttpPut]
        [HttpGet]
        [ActionName("Update")]
        public async Task<ResponseInfo> Update(emr_prescription_mf Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    objResponse.IsSuccess = false;
                //    objResponse.ErrorMessage = MessageStatement.BadRequest;
                //    return objResponse;
                //}
                decimal CompanyID = Request.CompanyID();
                int userid = Convert.ToInt32(Request.LoginID());
                List<emr_prescription_complaint> emr_prescription_complaint = new List<emr_prescription_complaint>();
                emr_prescription_complaint.AddRange(Model.emr_prescription_complaint);

                List<emr_prescription_diagnos> emr_prescription_diagnos = new List<emr_prescription_diagnos>();
                emr_prescription_diagnos.AddRange(Model.emr_prescription_diagnos);

                List<emr_prescription_investigation> emr_prescription_investigation = new List<emr_prescription_investigation>();
                emr_prescription_investigation.AddRange(Model.emr_prescription_investigation);

                List<emr_prescription_observation> emr_prescription_observation = new List<emr_prescription_observation>();
                emr_prescription_observation.AddRange(Model.emr_prescription_observation);

                List<emr_prescription_treatment> emr_prescription_treatment = new List<emr_prescription_treatment>();
                emr_prescription_treatment.AddRange(Model.emr_prescription_treatment);

                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Modified;
                Model.emr_prescription_complaint = null;
                Model.emr_prescription_diagnos = null;
                Model.emr_prescription_investigation = null;
                Model.emr_prescription_observation = null;
                Model.emr_prescription_treatment = null;
                Model.emr_prescription_treatment_template = null;
                Model.adm_user_mf = null;
                Model.adm_user_mf1 = null;
                Model.emr_patient_mf = null;
                _service.Update(Model);
                //delete complaint
                var DelAlLcomplaint = _emr_prescription_complaintService.Queryable().Where(e => e.PrescriptionId == Model.ID && e.CompanyID == CompanyID).ToList();
                foreach (var obj in DelAlLcomplaint)
                {
                    obj.ObjectState = ObjectState.Deleted;
                    _emr_prescription_complaintService.Delete(obj);
                }
                //delete diagnos
                var DelAlLdiagnos = _emr_prescription_diagnosService.Queryable().Where(e => e.PrescriptionId == Model.ID && e.CompanyID == CompanyID).ToList();
                foreach (var objdia in DelAlLdiagnos)
                {
                    objdia.ObjectState = ObjectState.Deleted;
                    _emr_prescription_diagnosService.Delete(objdia);
                }

                //delete observation
                var DelAlLobservation = _emr_prescription_observationService.Queryable().Where(e => e.PrescriptionId == Model.ID && e.CompanyID == CompanyID).ToList();
                foreach (var objObs in DelAlLobservation)
                {
                    objObs.ObjectState = ObjectState.Deleted;
                    _emr_prescription_observationService.Delete(objObs);
                }
                //delete complaint
                var DelAlLinvestigation = _emr_prescription_investigationService.Queryable().Where(e => e.PrescriptionId == Model.ID && e.CompanyID == CompanyID).ToList();
                foreach (var objComp in DelAlLinvestigation)
                {
                    objComp.ObjectState = ObjectState.Deleted;
                    _emr_prescription_investigationService.Delete(objComp);
                }
                //delete treatment
                var DelAlLtreatment = _emr_prescription_treatmentService.Queryable().Where(e => e.PrescriptionId == Model.ID && e.CompanyID == CompanyID).ToList();
                foreach (var objtreat in DelAlLtreatment)
                {
                    objtreat.ObjectState = ObjectState.Deleted;
                    _emr_prescription_treatmentService.Delete(objtreat);
                }
                //delete template
                if (Model.IsTemplate)
                {
                    var DelAlLtemplate = _emr_prescription_treatment_templateService.Queryable().Where(e => e.PrescriptionId == Model.ID && e.CompanyID == CompanyID).ToList();
                    foreach (var objtemplate in DelAlLtemplate)
                    {
                        objtemplate.ObjectState = ObjectState.Deleted;
                        _emr_prescription_treatment_templateService.Delete(objtemplate);
                    }
                }
                //insert complaint
                decimal complaintID = 1;
                if (_emr_prescription_complaintService.Queryable().Count() > 0)
                    complaintID = _emr_prescription_complaintService.Queryable().Max(e => e.ID) + 1;

                foreach (emr_prescription_complaint item in emr_prescription_complaint)
                {
                    item.ID = complaintID;
                    item.PrescriptionId = Model.ID;
                    item.PatientId = Model.PatientId;
                    item.CompanyID = CompanyID;
                    item.Complaint = item.Complaint;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.ObjectState = ObjectState.Added;
                    item.emr_prescription_mf = null;
                    item.adm_user_mf = null;
                    item.adm_user_mf1 = null;
                    _emr_prescription_complaintService.Insert(item);
                    complaintID++;
                }
                //insert diagnos
                decimal diagnosID = 1;
                if (_emr_prescription_diagnosService.Queryable().Count() > 0)
                    diagnosID = _emr_prescription_diagnosService.Queryable().Max(e => e.ID) + 1;

                foreach (emr_prescription_diagnos item in emr_prescription_diagnos)
                {
                    item.ID = diagnosID;
                    item.PrescriptionId = Model.ID;
                    item.PatientId = Model.PatientId;
                    item.CompanyID = CompanyID;
                    item.Diagnos = item.Diagnos;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.ObjectState = ObjectState.Added;
                    item.emr_prescription_mf = null;
                    item.adm_user_mf = null;
                    item.adm_user_mf1 = null;
                    _emr_prescription_diagnosService.Insert(item);
                    diagnosID++;
                }
                //insert investigation
                decimal investigationID = 1;
                if (_emr_prescription_investigationService.Queryable().Count() > 0)
                    investigationID = _emr_prescription_investigationService.Queryable().Max(e => e.ID) + 1;

                foreach (emr_prescription_investigation item in emr_prescription_investigation)
                {
                    item.ID = investigationID;
                    item.PrescriptionId = Model.ID;
                    item.PatientId = Model.PatientId;
                    item.CompanyID = CompanyID;
                    item.Investigation = item.Investigation;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.ObjectState = ObjectState.Added;
                    item.emr_prescription_mf = null;
                    item.adm_user_mf = null;
                    item.adm_user_mf1 = null;
                    _emr_prescription_investigationService.Insert(item);
                    investigationID++;
                }
                //insert Observation
                decimal observationID = 1;
                if (_emr_prescription_observationService.Queryable().Count() > 0)
                    observationID = _emr_prescription_observationService.Queryable().Max(e => e.ID) + 1;

                foreach (emr_prescription_observation item in emr_prescription_observation)
                {
                    item.ID = observationID;
                    item.PrescriptionId = Model.ID;
                    item.PatientId = Model.PatientId;
                    item.CompanyID = CompanyID;
                    item.Observation = item.Observation;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.emr_prescription_mf = null;
                    item.adm_user_mf = null;
                    item.adm_user_mf1 = null;
                    item.ObjectState = ObjectState.Added;
                    _emr_prescription_observationService.Insert(item);
                    observationID++;
                }
                //insert treatment
                decimal treatmentID = 1;
                if (_emr_prescription_treatmentService.Queryable().Count() > 0)
                    treatmentID = _emr_prescription_treatmentService.Queryable().Max(e => e.ID) + 1;

                foreach (emr_prescription_treatment item in emr_prescription_treatment)
                {
                    item.ID = treatmentID;
                    item.PrescriptionId = Model.ID;
                    item.PatientId = Model.PatientId;
                    item.CompanyID = CompanyID;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    if (item.Instructions == null)
                        item.Instructions = "";
                    item.ObjectState = ObjectState.Added;
                    item.emr_prescription_mf = null;
                    item.adm_user_mf = null;
                    item.adm_user_mf1 = null;
                    _emr_prescription_treatmentService.Insert(item);
                    treatmentID++;
                }

                if (Model.IsTemplate)
                {
                    decimal TemplateID = 1;
                    if (_emr_prescription_treatment_templateService.Queryable().Count() > 0)
                        TemplateID = _emr_prescription_treatment_templateService.Queryable().Max(e => e.ID) + 1;

                    emr_prescription_treatment_template obj = new emr_prescription_treatment_template();
                    obj.ID = TemplateID;

                    obj.TemplateName = Model.TemplateName;
                    obj.PrescriptionId = Model.ID;
                    obj.CompanyID = Request.CompanyID();
                    obj.CreatedBy = Request.LoginID();
                    obj.CreatedDate = Request.DateTimes();
                    obj.ModifiedBy = Request.LoginID();
                    obj.emr_prescription_mf = null;
                    obj.adm_user_mf = null;
                    obj.adm_user_mf1 = null;
                    obj.ModifiedDate = Request.DateTimes();
                    _emr_prescription_treatment_templateService.Insert(obj);
                }

                var appinfo = _emr_appointment_mfService.Queryable().Where(a => a.PatientId == Model.PatientId && DbFunctions.TruncateTime(a.AppointmentDate) == DbFunctions.TruncateTime(Model.FollowUpDate)).FirstOrDefault();
                if (Model.IsCreateAppointment && appinfo == null && Model.FollowUpDate != null)
                {
                    emr_appointment_mf appObj = new emr_appointment_mf();
                    decimal AppID = 0;
                    if (_emr_appointment_mfService.Queryable().Count() > 0)
                        AppID = _emr_appointment_mfService.Queryable().Max(e => e.ID) + 1;
                    appObj.ID = AppID;
                    appObj.StatusId = (int)sys_dropdown_mfEnum.StatusId;
                    appObj.CompanyId = Request.CompanyID();
                    appObj.PatientId = Model.PatientId;
                    appObj.DoctorId = Model.DoctorId;
                    appObj.AppointmentDate = Convert.ToDateTime(Model.FollowUpDate);
                    appObj.AppointmentTime = (TimeSpan)Model.FollowUpTime;
                    appObj.Notes = Model.Notes;
                    appObj.CreatedBy = Request.LoginID();
                    appObj.CreatedDate = Request.DateTimes();
                    appObj.ModifiedBy = Request.LoginID();
                    appObj.ModifiedDate = Request.DateTimes();
                    //Model = appObj;
                    appObj.ObjectState = ObjectState.Added;
                    _emr_appointment_mfService.Insert(appObj);
                }
                else if (Model.IsCreateAppointment && appinfo != null)
                {
                    appinfo.DoctorId = Model.DoctorId;
                    appinfo.AppointmentDate = Convert.ToDateTime(Model.FollowUpDate);
                    appinfo.AppointmentTime = (TimeSpan)Model.FollowUpTime;
                    appinfo.Notes = Model.Notes;
                    appinfo.ObjectState = ObjectState.Modified;
                    _emr_appointment_mfService.Update(appinfo);
                }
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;
                    objResponse.IsSuccess = true;

                    var patientInfo = _emr_patientService.Queryable().Where(a => a.CompanyId == CompanyID && a.ID == Model.PatientId)
                        .Select(z => new
                        {
                            z.PatientName,
                            z.Age,
                            Gender = z.Gender == 1 ? "Male" : z.Gender == 2 ? "Female" : "Other",
                            ClinicIogo = z.adm_company.CompanyLogo,
                            z.Mobile,
                        }).FirstOrDefault();
                    var role = _adm_user_companyService.Queryable()
                     .Where(u => u.UserID == userid && u.CompanyID == CompanyID)
                     .Select(u => u.adm_role_mf.RoleName)
                     .FirstOrDefault();
                    var doctorUsers = _adm_user_companyService.Queryable()
            .Where(u => u.CompanyID == CompanyID && u.adm_role_mf.RoleName == "Doctor")
            .Select(u => u.adm_user_mf)
            .OrderBy(u => u.ID)
            .ToList();
                    var doctors = doctorUsers.Select(mf =>
                    {
                        var emp = _pr_employee_mfService.Queryable().FirstOrDefault(e => e.ID == mf.EmployeeID);

                        return (object)new
                        {
                            ID = mf.ID,
                            Name = mf.Name,
                            StartTime = emp != null ? emp.StartTime : null,
                            EndTime = emp != null ? emp.EndTime : null,
                            SlotTime = mf.SlotTime,
                            WDSunday = emp?.WDSunday ?? false,
                            WDMonday = emp?.WDMonday ?? false,
                            WDTuesday = emp?.WDTuesday ?? false,
                            WDWednesday = emp?.WDWednesday ?? false,
                            WDThursday = emp?.WDThursday ?? false,
                            WDFriday = emp?.WDFriday ?? false,
                            WDSaturday = emp?.WDSaturday ?? false,
                            IsShowDoctor = mf.IsShowDoctor,
                            IsDoctor = false,
                            Qualification = mf.Qualification,
                            Designation = mf.Designation,
                            PhoneNo = mf.PhoneNo,
                            Footer = mf.RepotFooter,
                            TemplateId = mf.TemplateId,
                            QualificationUrdu = mf.QualificationUrdu,
                            DesignationUrdu = mf.DesignationUrdu,
                            NameUrdu = mf.NameUrdu,
                            HeaderDescription = mf.HeaderDescription,
                            HeaderDescriptionUrdu = mf.HeaderDescriptionUrdu,
                            IsOverLap = mf.IsOverLap
                        };
                    }).ToList();
                    List<object> result;
                    if (role == "Administrator")
                    {
                        result = doctors.Where(d => ((dynamic)d).ID == Model.DoctorId).ToList<object>();
                    }
                    else if (role == "Doctor")
                    {
                        result = doctors.Where(d => ((dynamic)d).ID == userid).ToList<object>();
                    }
                    else
                    {
                        result = doctors;
                    }
                    var vitallist = _emr_vitalService.Queryable().Where(a => a.CompanyID == CompanyID && a.PatientId == Model.PatientId && DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(Model.AppointmentDate)).Select(a => new
                    {
                        Measure = a.Measure,
                        Unit = a.sys_drop_down_value.Unit,
                        Name = a.sys_drop_down_value.Value
                    }).ToList();

                    var ids = Model?.emr_prescription_treatment?.Select(a => a.MedicineId).ToList() ?? new List<decimal?>();
                    var medicineList = _emr_medicineService.Queryable().Where(a => a.CompanyID == CompanyID && ids.Contains(a.ID)).Select(a => new { Id = a.ID, Medicine = a.Medicine, Type = a.sys_drop_down_value1.Value });
                    var itemList = _adm_itemService.Queryable().Where(a => a.CompanyId == CompanyID && ids.Contains(a.ID)).Select(a => new { Id = a.ID, Medicine = a.Name, Type = a.sys_drop_down_value1.Value });
                    var MedicineList = itemList.Union(medicineList).ToList();

                    objResponse.ResultSet = new
                    {
                        Model = Model,
                        patientInfo = patientInfo,
                        doctor = result,
                        vitallist = vitallist,
                        MedicineList = MedicineList,
                    };

                }
                catch (DbUpdateException)
                {
                    if (!ModelExists(Model.ID.ToString()))
                    {
                        objResponse.IsSuccess = false;
                        objResponse.ErrorMessage = MessageStatement.NotFound;
                        return objResponse;
                    }
                    throw;
                }
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }

        [HttpGet]
        [ActionName("GetList")]
        public ResponseInfo GetList()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var GenderType = _sys_drop_down_valueService.Queryable().Where(e => e.DropDownID == 2).ToList();
                objResponse.ResultSet = new
                {
                    GenderList = GenderType
                };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        private bool ModelExists(string key)
        {
            return _service.Query(e => e.ID.ToString() == key).Select().Any();
        }
        [HttpGet]
        [ActionName("GetByPrescriptionId")]
        public async Task<ResponseInfo> GetByPrescriptionId(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal companyId = Request.CompanyID();
                var result = _service.Queryable().Where(e => e.ID.ToString() == Id && e.CompanyID == companyId).Include(a => a.emr_prescription_treatment_template).Include(a => a.emr_prescription_complaint).Include(a => a.emr_prescription_diagnos).Include(a => a.emr_prescription_investigation).Include(a => a.emr_prescription_observation).Include(a => a.emr_prescription_treatment).FirstOrDefault();
                decimal doctorId = Convert.ToDecimal(result.DoctorId);
                decimal patientId = Convert.ToDecimal(result.PatientId);
                decimal prescriptionId = Convert.ToDecimal(result.ID);
                int userid = Convert.ToInt32(Request.LoginID());
                var ids = result?.emr_prescription_treatment?.Select(a => a.MedicineId).ToList() ?? new List<decimal?>();
                var medicine = _emr_medicineService.Queryable().Where(a => a.CompanyID == companyId && ids.Contains(a.ID)).Select(a => new { Id = a.ID, Medicine = a.Medicine, Type = a.sys_drop_down_value1.Value });
                var itemList = _adm_itemService.Queryable().Where(a => a.CompanyId == companyId && ids.Contains(a.ID)).Select(a => new { Id = a.ID, Medicine = a.Name, Type = a.sys_drop_down_value1.Value });
                var medicineList = itemList.Union(medicine).ToList();

                var vitalObj = new List<object>();
                if (result != null)
                {
                    vitalObj = _emr_vitalService.Queryable()
                        .Where(a => a.CompanyID == companyId
                            && a.PatientId == patientId
                            && DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(result.AppointmentDate))
                        .Select(z => new
                        {
                            z.ID,
                            z.Date,
                            z.Measure,
                            z.VitalId,
                            Name = z.sys_drop_down_value.Value,
                            Unit = z.sys_drop_down_value.Unit
                        })
                        .ToList<object>();
                }

                var patientInfo = await _emr_patientService.Queryable()
    .Where(p => p.ID == patientId && p.CompanyId == companyId)
    .FirstOrDefaultAsync();

                var reslt = await _emr_appointment_mfService.Queryable()
     .Where(a => a.PatientId == patientId
              && a.CompanyId == companyId
              && a.DoctorId == doctorId)
     .GroupBy(a => new { a.PatientId, Date = DbFunctions.TruncateTime(a.AppointmentDate) }) // group by patient + date only
     .Select(g => new
     {
         g.Key.PatientId,
         AppointmentDate = g.Key.Date
     })
     .OrderByDescending(x => x.AppointmentDate)
     .ToListAsync();

                // if you really need string format "yyyy-MM-dd"
                var appointmentDates = reslt.Select(a => new
                {
                    a.PatientId,
                    AppointmentDate = a.AppointmentDate.Value.ToString("yyyy-MM-dd")
                }).ToList();
                var complaintList = await GetTopComplaintsAsync(companyId, doctorId, patientId);
                var observationList = await GetTopObservationsAsync(companyId, doctorId, patientId);
                var investigationList = await GetTopInvestigationsAsync(companyId, doctorId, patientId);
                var diagnosisList = await GetTopDiagnosisAsync(companyId, doctorId, patientId);
                var dropDownIds = new[] { 14, 15, 16, 2, 17 };
                var allDropDownValues = await _sys_drop_down_valueService.Queryable()
    .Where(x => dropDownIds.Contains(x.DropDownID)
                && (x.CompanyID == companyId || x.CompanyID == null)).Select(z => new
                {
                    z.ID,
                    z.Value,
                    z.DropDownID,
                })
    .ToListAsync();
                var unitList = allDropDownValues
      .Where(x => x.DropDownID == 14).ToList();
                var medicineTypeList = allDropDownValues
    .Where(x => x.DropDownID == 15).ToList();
                var doseList = allDropDownValues
    .Where(x => x.DropDownID == 16).ToList();

                var genderList = allDropDownValues
    .Where(x => x.DropDownID == 2).ToList();
                var bloodList = allDropDownValues
    .Where(x => x.DropDownID == 17).ToList();
                var isShowDoctorIds = await _adm_userService.Queryable()
    .Where(u => u.ID == userid)
    .Select(u => u.IsShowDoctor)
    .FirstOrDefaultAsync();
                var ht = new Hashtable();
                ht.Add("@CompanyId", companyId);
                ht.Add("@UserId", userid);
                DataAccessManager dataAccessManager = new DataAccessManager();
                var doctorList = dataAccessManager.GetDataSet("SP_GetDoctorList", ht);

                var PatientInfo = patientInfo;
                var UnitList = unitList;
                var MadicineTypeList = medicineTypeList;
                var DoesList = doseList;
                var MedicineList = medicineList;
                var CurntPrevDateList = appointmentDates;
                var ComplaintList = complaintList;
                var ObservationList = observationList;
                var InvestigationsList = investigationList;
                var DiagnosisList = diagnosisList;
                var ClinicList = _adm_user_companyService.Queryable()
                            .Where(e => e.UserID == userid).Include(x => x.adm_company).Select(x => new { x.CompanyID, x.adm_company.CompanyName }).ToList();
                var BloodList = bloodList;
                var GenderType = genderList;
                var DoctorList = doctorList.Tables[0];
                var DoctorCalander = doctorList.Tables[0];
                decimal[] IdList = null;
                var isShowIds = isShowDoctorIds;
                if (!string.IsNullOrWhiteSpace(isShowIds))
                    IdList = isShowIds.Split(',').Select(decimal.Parse).ToArray();

                objResponse.ResultSet = new
                {
                    PatientInfo = PatientInfo,
                    GenderList = GenderType,
                    DoctorList = DoctorList,
                    DoctorCalander = DoctorCalander,
                    IsShowDoctorIds = IdList,
                    ClinicList = ClinicList,
                    BloodList = BloodList,
                    MedicineList = MedicineList,
                    result = result,
                    UnitList = UnitList,
                    MadicineTypeList = MadicineTypeList,
                    DoesList = DoesList,
                    CurntPrevDateList = CurntPrevDateList,
                    ComplaintList = ComplaintList,
                    ObservationList = ObservationList,
                    InvestigationsList = InvestigationsList,
                    DiagnosisList = DiagnosisList,
                    vitalList = vitalObj,
                };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        public async Task<List<object>> GetTopComplaintsAsync(
    decimal companyId,
    decimal doctorId,
    decimal patientId)
        {
            // Step 1: Get favorite complaint IDs
            var favorites = await _emr_notes_favoriteService.Queryable()
                .Where(ft => ft.ReferenceType == "C"
                          && ft.DoctorId == doctorId
                          && ft.CompanyId == companyId)
                .Select(ft => new { ft.ID, ft.ReferenceId })
                .ToListAsync();
            var favoriteIds = favorites.Select(f => f.ReferenceId).ToList();
            // Step 2: Get up to 10 complaints (favorites first, then others)
            var complaints = await _emr_complaintService.Queryable()
        .Where(c => c.CompanyID == companyId)
        .Select(c => new
        {
            c.ID,
            c.Complaint,
            Isfavorite = favoriteIds.Contains(c.ID) ? 1 : 0
        })
        .OrderByDescending(c => c.Isfavorite)
        .ThenBy(c => c.ID)
        .Take(10)
        .ToListAsync();

            // Step 3: Extract complaint IDs
            var complaintIds = complaints.Select(c => (decimal?)c.ID).ToList();

            // Step 4: Get related prescription complaints in one go
            var prescriptionComplaints = await _emr_prescription_complaintService.Queryable()
        .Where(pc => pc.CompanyID == companyId
                  && pc.PatientId == patientId
                  && complaintIds.Contains(pc.ComplaintId))
        .Select(pc => new { pc.ComplaintId, pc.Complaint })
        .ToListAsync();

            // Step 5: Merge in-memory
            var complaintList = complaints
        .Select(c =>
        {
            var fav = favorites.FirstOrDefault(f => f.ReferenceId == c.ID);
            var pc = prescriptionComplaints.FirstOrDefault(x => x.ComplaintId == c.ID);
            return new
            {
                id = c.ID,
                Complaint = pc?.Complaint ?? c.Complaint,
                Isfavorite = c.Isfavorite,
                favoriteId = fav?.ID
            };
        })
        .ToList<object>();

            return complaintList;
        }
        public async Task<List<object>> GetTopObservationsAsync(
   decimal companyId,
   decimal doctorId,
   decimal patientId)
        {
            // Step 1: Get favorite complaint IDs
            var favorites = await _emr_notes_favoriteService.Queryable()
                .Where(ft => ft.ReferenceType == "O"
                          && ft.DoctorId == doctorId
                          && ft.CompanyId == companyId)
                .Select(ft => new { ft.ID, ft.ReferenceId })
                .ToListAsync();
            var favoriteIds = favorites.Select(f => f.ReferenceId).ToList();
            // Step 2: Get up to 10 complaints (favorites first, then others)
            var observations = await _emr_observationService.Queryable()
        .Where(c => c.CompanyID == companyId)
        .Select(c => new
        {
            c.ID,
            c.Observation,
            Isfavorite = favoriteIds.Contains(c.ID) ? 1 : 0
        })
        .OrderByDescending(c => c.Isfavorite)
        .ThenBy(c => c.ID)
        .Take(10)
        .ToListAsync();

            // Step 3: Extract complaint IDs
            var observationIds = observations.Select(c => (decimal?)c.ID).ToList();

            // Step 4: Get related prescription complaints in one go
            var prescriptionComplaints = await _emr_prescription_observationService.Queryable()
        .Where(pc => pc.CompanyID == companyId
                  && pc.PatientId == patientId
                  && observationIds.Contains(pc.ObservationId))
        .Select(pc => new { pc.ObservationId, pc.Observation })
        .ToListAsync();

            // Step 5: Merge in-memory
            var observationList = observations
        .Select(c =>
        {
            var fav = favorites.FirstOrDefault(f => f.ReferenceId == c.ID);
            var pc = prescriptionComplaints.FirstOrDefault(x => x.ObservationId == c.ID);
            return new
            {
                id = c.ID,
                Observation = pc?.Observation ?? c.Observation,
                Isfavorite = c.Isfavorite,
                favoriteId = fav?.ID
            };
        })
        .ToList<object>();

            return observationList;
        }
        public async Task<List<object>> GetTopInvestigationsAsync(
   decimal companyId,
   decimal doctorId,
   decimal patientId)
        {
            // Step 1: Get favorite complaint IDs
            var favorites = await _emr_notes_favoriteService.Queryable()
                .Where(ft => ft.ReferenceType == "I"
                          && ft.DoctorId == doctorId
                          && ft.CompanyId == companyId)
                .Select(ft => new { ft.ID, ft.ReferenceId })
                .ToListAsync();
            var favoriteIds = favorites.Select(f => f.ReferenceId).ToList();
            // Step 2: Get up to 10 complaints (favorites first, then others)
            var investigations = await _emr_investigationService.Queryable()
        .Where(c => c.CompanyID == companyId)
        .Select(c => new
        {
            c.ID,
            c.Investigation,
            Isfavorite = favoriteIds.Contains(c.ID) ? 1 : 0
        })
        .OrderByDescending(c => c.Isfavorite)
        .ThenBy(c => c.ID)
        .Take(10)
        .ToListAsync();

            // Step 3: Extract complaint IDs
            var investigationIds = investigations.Select(c => (decimal?)c.ID).ToList();

            // Step 4: Get related prescription complaints in one go
            var prescriptionComplaints = await _emr_prescription_investigationService.Queryable()
        .Where(pc => pc.CompanyID == companyId
                  && pc.PatientId == patientId
                  && investigationIds.Contains(pc.InvestigationId))
        .Select(pc => new { pc.InvestigationId, pc.Investigation })
        .ToListAsync();

            // Step 5: Merge in-memory
            var investigationList = investigations
        .Select(c =>
        {
            var fav = favorites.FirstOrDefault(f => f.ReferenceId == c.ID);
            var pc = prescriptionComplaints.FirstOrDefault(x => x.InvestigationId == c.ID);
            return new
            {
                id = c.ID,
                Investigation = pc?.Investigation ?? c.Investigation,
                Isfavorite = c.Isfavorite,
                favoriteId = fav?.ID
            };
        })
        .ToList<object>();

            return investigationList;
        }
        public async Task<List<object>> GetTopDiagnosisAsync(
   decimal companyId,
   decimal doctorId,
   decimal patientId)
        {
            // Step 1: Get favorite complaint IDs
            var favorites = await _emr_notes_favoriteService.Queryable()
                .Where(ft => ft.ReferenceType == "D"
                          && ft.DoctorId == doctorId
                          && ft.CompanyId == companyId)
                .Select(ft => new { ft.ID, ft.ReferenceId })
                .ToListAsync();
            var favoriteIds = favorites.Select(f => f.ReferenceId).ToList();
            // Step 2: Get up to 10 complaints (favorites first, then others)
            var diagnosis = await _emr_diagnosService.Queryable()
        .Where(c => c.CompanyID == companyId)
        .Select(c => new
        {
            c.ID,
            c.Diagnos,
            Isfavorite = favoriteIds.Contains(c.ID) ? 1 : 0
        })
        .OrderByDescending(c => c.Isfavorite)
        .ThenBy(c => c.ID)
        .Take(10)
        .ToListAsync();

            // Step 3: Extract complaint IDs
            var diagnosisIds = diagnosis.Select(c => (decimal?)c.ID).ToList();

            // Step 4: Get related prescription complaints in one go
            var prescriptionComplaints = await _emr_prescription_diagnosService.Queryable()
        .Where(pc => pc.CompanyID == companyId
                  && pc.PatientId == patientId
                  && diagnosisIds.Contains(pc.DiagnosId))
        .Select(pc => new { pc.DiagnosId, pc.Diagnos })
        .ToListAsync();

            // Step 5: Merge in-memory
            var diagnosisList = diagnosis
        .Select(c =>
        {
            var fav = favorites.FirstOrDefault(f => f.ReferenceId == c.ID);
            var pc = prescriptionComplaints.FirstOrDefault(x => x.DiagnosId == c.ID);
            return new
            {
                id = c.ID,
                Diagnos = pc?.Diagnos ?? c.Diagnos,
                Isfavorite = c.Isfavorite,
                favoriteId = fav?.ID
            };
        })
        .ToList<object>();

            return diagnosisList;
        }
        [HttpGet]
        [ActionName("TemplateLoadById")]
        public ResponseInfo TemplateLoadById(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var result = _service.Queryable().Where(e => e.ID.ToString() == Id && e.CompanyID == CompanyID).Include(a => a.emr_prescription_complaint).Include(a => a.emr_prescription_diagnos).Include(a => a.emr_prescription_investigation).Include(a => a.emr_prescription_observation).Include(a => a.emr_prescription_treatment).Include(a => a.emr_prescription_treatment_template).Include(a => a.emr_patient_mf).FirstOrDefault();
                var ids = result?.emr_prescription_treatment?.Select(a => a.MedicineId).ToList() ?? new List<decimal?>();
                var medicineList = _emr_medicineService.Queryable().Where(a => a.CompanyID == CompanyID && ids.Contains(a.ID)).Select(a => new { Id = a.ID, Medicine = a.Medicine, Type = a.sys_drop_down_value1.Value });
                var itemList = _adm_itemService.Queryable().Where(a => a.CompanyId == CompanyID && ids.Contains(a.ID)).Select(a => new { Id = a.ID, Medicine = a.Name, Type = a.sys_drop_down_value1.Value });
                var MedicineList = itemList.Union(medicineList).ToList();
                objResponse.ResultSet = new
                {
                    MedicineList = MedicineList,
                    result = result,
                };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }

        [HttpGet]
        [ActionName("PrintRXById")]
        public ResponseInfo PrintRXById(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var result = _service.Queryable().Where(e => e.ID.ToString() == Id && e.CompanyID == CompanyID).Include(a => a.emr_prescription_complaint).Include(a => a.emr_prescription_diagnos).Include(a => a.emr_prescription_investigation).Include(a => a.emr_prescription_observation).Include(a => a.emr_prescription_treatment).Include(a => a.emr_prescription_treatment_template)
                    .Select(z => new
                    {
                        z.DoctorId,
                        z.PatientId,
                        z.AppointmentDate,
                        z.Notes,
                        z.FollowUpDate,
                        z.FollowUpTime,
                        z.ClinicalNote,
                        z.PlanNote,
                        PatientName = z.emr_patient_mf.PatientName,
                        Mobile = z.emr_patient_mf.Mobile,
                        Gender = z.emr_patient_mf.Gender == 1 ? "Male" : z.emr_patient_mf.Gender == 2 ? "Female" : "Other",
                        ClinicIogo = z.adm_company.CompanyLogo,
                        Age = z.emr_patient_mf.Age,
                        z.emr_prescription_complaint,
                        z.emr_prescription_diagnos,
                        z.emr_prescription_investigation,
                        z.emr_prescription_observation,
                        z.emr_prescription_treatment,
                        z.adm_user_mf
                    }).FirstOrDefault();
                int userid = Convert.ToInt32(Request.LoginID());
                var role = _adm_user_companyService.Queryable()
                     .Where(u => u.UserID == userid && u.CompanyID == CompanyID)
                     .Select(u => u.adm_role_mf.RoleName)
                     .FirstOrDefault();
                var doctorUsers = _adm_user_companyService.Queryable()
        .Where(u => u.CompanyID == CompanyID && u.adm_role_mf.RoleName == "Doctor")
        .Select(u => u.adm_user_mf)
        .OrderBy(u => u.ID)
        .ToList();
                var doctors = doctorUsers.Select(mf =>
                {
                    var emp = _pr_employee_mfService.Queryable().FirstOrDefault(e => e.ID == mf.EmployeeID);

                    return (object)new
                    {
                        ID = mf.ID,
                        Name = mf.Name,
                        StartTime = emp != null ? emp.StartTime : null,
                        EndTime = emp != null ? emp.EndTime : null,
                        SlotTime = mf.SlotTime,
                        WDSunday = emp?.WDSunday ?? false,
                        WDMonday = emp?.WDMonday ?? false,
                        WDTuesday = emp?.WDTuesday ?? false,
                        WDWednesday = emp?.WDWednesday ?? false,
                        WDThursday = emp?.WDThursday ?? false,
                        WDFriday = emp?.WDFriday ?? false,
                        WDSaturday = emp?.WDSaturday ?? false,
                        IsShowDoctor = mf.IsShowDoctor,
                        IsDoctor = false,
                        Qualification = mf.Qualification,
                        Designation = mf.Designation,
                        PhoneNo = mf.PhoneNo,
                        Footer = mf.RepotFooter,
                        TemplateId = mf.TemplateId,
                        QualificationUrdu = mf.QualificationUrdu,
                        DesignationUrdu = mf.DesignationUrdu,
                        NameUrdu = mf.NameUrdu,
                        HeaderDescription = mf.HeaderDescription,
                        HeaderDescriptionUrdu = mf.HeaderDescriptionUrdu,
                        IsOverLap = mf.IsOverLap
                    };
                }).ToList();
                List<object> doctorList;
                if (role == "Administrator")
                {
                    doctorList = doctors.Where(d => ((dynamic)d).ID == result.DoctorId).ToList<object>();
                }
                else if (role == "Doctor")
                {
                    doctorList = doctors.Where(d => ((dynamic)d).ID == userid).ToList<object>();
                }
                else
                {
                    doctorList = doctors;
                }
                var vitallist = _emr_vitalService.Queryable().Where(a => a.CompanyID == CompanyID && a.PatientId == result.PatientId && DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(result.AppointmentDate)).Select(a => new
                {
                    Measure = a.Measure,
                    Unit = a.sys_drop_down_value.Unit,
                    Name = a.sys_drop_down_value.Value
                }).ToList();
                var ids = result?.emr_prescription_treatment?.Select(a => a.MedicineId).ToList() ?? new List<decimal?>();
                var medicineList = _emr_medicineService.Queryable().Where(a => a.CompanyID == CompanyID && ids.Contains(a.ID)).Select(a => new { Id = a.ID, Medicine = a.Medicine, TypeId = a.TypeId, Type = a.sys_drop_down_value1.Value });
                var itemList = _adm_itemService.Queryable().Where(a => a.CompanyId == CompanyID && ids.Contains(a.ID)).Select(a => new { Id = a.ID, Medicine = a.Name, TypeId = a.CategoryID, Type = a.sys_drop_down_value1.Value });
                var MedicineList = itemList.Union(medicineList).ToList();
                objResponse.ResultSet = new
                {
                    doctor = doctorList,
                    result = result,
                    vitallist = vitallist,
                    MedicineList = MedicineList,
                };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        public ResponseInfo GetById(string Id, int NextPreviousIndex)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                objResponse.ResultSet = _service.Queryable().Where(e => e.ID.ToString() == Id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        [HttpGet]
        [ActionName("Pagination")]
        public PaginationResult Pagination(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();

                string TotalRecord = "0";
                if (SearchText == null)
                    SearchText = "";
                DataAccessManager dataAccessManager = new DataAccessManager();
                var ht = new Hashtable();
                ht.Add("@CompanyID", CompanyID);
                ht.Add("@CurrentPageNo", CurrentPageNo);
                ht.Add("@RecordPerPage", RecordPerPage);
                ht.Add("@SearchText", SearchText.ToString());

                var AllData = dataAccessManager.GetDataSet("SP_PrescriptionList", ht);
                var DataList = AllData.Tables[0];
                if (DataList.Rows.Count > 0)
                    TotalRecord = DataList.Rows[0]["TotalRecord"].ToString();
                objResult.OtherDataModel = DataList;
                objResult.TotalRecord = Convert.ToInt32(TotalRecord);

            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }
        [HttpGet]
        [ActionName("GetPrescription")]
        public async Task<PaginationResult> GetPrescription(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal companyId = Request.CompanyID();
                string TotalRecord = "0";
                if (SearchText == null)
                    SearchText = "";

                var query = _service.Queryable()
        .Where(mf => mf.CompanyID == companyId)
        .Select(mf => new
        {
            mf.ID,
            mf.AppointmentDate,
            PatientName = mf.emr_patient_mf.PatientName,   // via navigation property
            PaidAmount = mf.emr_patient_mf.emr_patient_bill
                .Where(b => b.CompanyId == companyId
                            && b.DoctorId == mf.DoctorId
                            && DbFunctions.TruncateTime(b.CreatedDate) == DbFunctions.TruncateTime(mf.CreatedDate))
                .Sum(b => (decimal?)b.PaidAmount) ?? 0,
            OutstandingBalance = mf.emr_patient_mf.emr_patient_bill
                .Where(b => b.CompanyId == companyId
                            && b.DoctorId == mf.DoctorId
                            && DbFunctions.TruncateTime(b.CreatedDate) == DbFunctions.TruncateTime(mf.CreatedDate))
                .Sum(b => (decimal?)b.OutstandingBalance) ?? 0,
            MedicineIds = mf.emr_prescription_treatment
                        .Where(t => t.CompanyID == companyId && t.PrescriptionId == mf.ID)
                        .Select(t => t.MedicineId)
                        .Distinct()
                        .ToList()
        });
                if (!string.IsNullOrEmpty(SearchText))
                {
                    query = query.Where(x => x.PatientName.Contains(SearchText));
                }
                var totalRecords = await query.CountAsync();
                var paged = await query
    .OrderByDescending(x => x.ID)
    .Skip((CurrentPageNo - 1) * RecordPerPage)
    .Take(RecordPerPage)
    .ToListAsync();
                var result = paged.Select(r => new
                {
                    r.ID,
                    r.AppointmentDate,
                    r.PatientName,
                    r.PaidAmount,
                    r.OutstandingBalance,
                    MedicineName = string.Join(", ",
        _emr_medicineService.Queryable()
            .Where(m => r.MedicineIds.Contains(m.ID))
            .Select(m => m.Medicine)
            .ToList()),
                    TotalRecord = totalRecords
                }).ToList();

                //string TotalRecord = "0";
                //if (SearchText == null)
                //    SearchText = "";
                //DataAccessManager dataAccessManager = new DataAccessManager();
                //var ht = new Hashtable();
                //ht.Add("@CompanyID", CompanyID);
                //ht.Add("@CurrentPageNo", CurrentPageNo);
                //ht.Add("@RecordPerPage", RecordPerPage);
                //ht.Add("@SearchText", SearchText.ToString());

                //var AllData = dataAccessManager.GetDataSet("SP_PrescriptionList", ht);
                //var DataList = AllData.Tables[0];
                //if (DataList.Rows.Count > 0)
                //    TotalRecord = DataList.Rows[0]["TotalRecord"].ToString();
                objResult.OtherDataModel = result;
                objResult.TotalRecord = totalRecords;

            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }

        [HttpGet]
        [ActionName("ExportData")]
        public ResponseInfo ExportData(int ExportType, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var ObjList = Pagination(0, 0, VisibleColumnInfo, SortName, SortOrder, SearchText, true);
                objResponse.FilePath = Documents.ExportWithType(ExportType, VisibleColumnInfo, ObjList.DataList);

            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<ResponseInfo> Delete(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal ID = Convert.ToDecimal(Id);
                emr_prescription_mf model = _service.Queryable().Where(e => e.CompanyID == CompanyID && e.ID == ID).FirstOrDefault();
                if (model == null)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
                }
                //delete complaint
                var DelAlLcomplaint = _emr_prescription_complaintService.Queryable().Where(e => e.PrescriptionId == model.ID && e.CompanyID == CompanyID).ToList();
                foreach (var obj in DelAlLcomplaint)
                {
                    obj.ObjectState = ObjectState.Deleted;
                    _emr_prescription_complaintService.Delete(obj);
                }
                //delete diagnos
                var DelAlLdiagnos = _emr_prescription_diagnosService.Queryable().Where(e => e.PrescriptionId == model.ID && e.CompanyID == CompanyID).ToList();
                foreach (var objdia in DelAlLdiagnos)
                {
                    objdia.ObjectState = ObjectState.Deleted;
                    _emr_prescription_diagnosService.Delete(objdia);
                }

                //delete observation
                var DelAlLobservation = _emr_prescription_observationService.Queryable().Where(e => e.PrescriptionId == model.ID && e.CompanyID == CompanyID).ToList();
                foreach (var objObs in DelAlLobservation)
                {
                    objObs.ObjectState = ObjectState.Deleted;
                    _emr_prescription_observationService.Delete(objObs);
                }
                //delete complaint
                var DelAlLinvestigation = _emr_prescription_investigationService.Queryable().Where(e => e.PrescriptionId == model.ID && e.CompanyID == CompanyID).ToList();
                foreach (var objComp in DelAlLinvestigation)
                {
                    objComp.ObjectState = ObjectState.Deleted;
                    _emr_prescription_investigationService.Delete(objComp);
                }
                //delete treatment
                var DelAlLtreatment = _emr_prescription_treatmentService.Queryable().Where(e => e.PrescriptionId == model.ID && e.CompanyID == CompanyID).ToList();
                foreach (var objtreat in DelAlLtreatment)
                {
                    objtreat.ObjectState = ObjectState.Deleted;
                    _emr_prescription_treatmentService.Delete(objtreat);
                }

                //delete treatment template
                var DelAlLtemplate = _emr_prescription_treatment_templateService.Queryable().Where(e => e.PrescriptionId == model.ID && e.CompanyID == CompanyID).ToList();
                foreach (var temp in DelAlLtemplate)
                {
                    temp.ObjectState = ObjectState.Deleted;
                    _emr_prescription_treatment_templateService.Delete(temp);
                }

                model.ObjectState = ObjectState.Deleted;
                _service.Delete(model);
                await _unitOfWorkAsync.SaveChangesAsync();
                objResponse.Message = MessageStatement.Delete;
            }
            catch (Exception ex)
            {
                string Message = "";
                objResponse.IsSuccess = false;

                if (ex.InnerException == null)
                    Message = ex.Message;
                else if (ex.InnerException.InnerException == null)
                    Message = ex.InnerException.Message;
                else
                    Message = ex.InnerException.InnerException.Message;

                if (Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                    Message = MessageStatement.RelationExists;

                objResponse.ErrorMessage = Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWorkAsync.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpGet]
        [ActionName("Load")]
        public ResponseInfo Load()
        {

            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                int userid = Request.LoginID();
                List<TemplateModel> template = _procedureService.GetAlLTemplate(CompanyID).ToList();
                var result = template.AsEnumerable().Select(z => new
                {
                    MfId = z.MfId,
                    Name = z.TemplateName,
                    medicine = z.Medicine.Split(',').ToList(),
                }).ToList();

                objResponse.ResultSet = new
                {
                    result = result,
                };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        [HttpGet]
        [ActionName("LoadDropdown")]
        public async Task<ResponseInfo> LoadDropdown()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal companyId = Request.CompanyID();
                int userid = Convert.ToInt32(Request.LoginID());

                var ht = new Hashtable();
                ht.Add("@CompanyId", companyId);
                ht.Add("@UserId", userid);
                DataAccessManager dataAccessManager = new DataAccessManager();
                var doctorData = dataAccessManager.GetDataSet("SP_GetDoctorList", ht);

                var dropDownIds = new[] { 2, 17, 1 };
                var allDropDownValues = await _sys_drop_down_valueService.Queryable()
    .Where(x => dropDownIds.Contains(x.DropDownID)
                && (x.CompanyID == companyId || x.CompanyID == null)).Select(z => new
                {
                    z.ID,
                    z.Value,
                    z.DropDownID,
                })
    .ToListAsync();
                var genderList = allDropDownValues
    .Where(x => x.DropDownID == 2).ToList();
                var bloodList = allDropDownValues
    .Where(x => x.DropDownID == 17).ToList();
                var statusList = allDropDownValues
    .Where(x => x.DropDownID == 1)
    .ToList();
                var isShowDoctorIds = await _adm_userService.Queryable()
.Where(x => x.ID == userid)
.Select(x => x.IsShowDoctor)
.FirstOrDefaultAsync();


                var companyInfo = await _adm_user_companyService.Queryable()
    .Where(x => x.UserID == userid)
    .Select(x => new
    {
        x.CompanyID,
        CompanyName = x.adm_company.CompanyName
    }).ToListAsync();

                var serviceType = await _emr_service_mfService.Queryable()
                    .Where(x => x.CompanyId == companyId && x.ServiceName == "Consultation")
                    .Select(x => new { x.ID, x.ServiceName, x.Price })
                    .ToListAsync();


                var nextToken = GetNextToken(companyId);
                var isBackDatedAppointment = await _adm_companyService.Queryable()
    .Where(x => x.ID == companyId)
    .Select(x => x.IsBackDatedAppointment)
    .FirstOrDefaultAsync();

                var reminders = await _adm_reminder_mf.Queryable()
   .Where(r => r.CompanyId == companyId)
   .Select(r => new { r.ID, Value = r.Name })
   .ToListAsync();


                var followUps = await _service.Queryable()
    .Where(mf => mf.CompanyID == companyId
              && DbFunctions.TruncateTime(mf.FollowUpDate) >= DbFunctions.TruncateTime(DateTime.Now))
    .Select(mf => new
    {
        // Convert TimeSpan (or DateTime) to string in-memory after query
        AppointmentTime = mf.FollowUpTime, // keep raw for now

        PatientName = mf.emr_patient_mf.PatientName, // navigation property
        PatientId = mf.PatientId,
        mf.IsCreateAppointment,
        Mobile = mf.emr_patient_mf.Mobile,
        mf.FollowUpNotes,
        mf.FollowUpDate
    })
    .OrderByDescending(mf => mf.FollowUpDate)
    .Distinct()
    .ToListAsync();



                // Format AppointmentTime after query
                var followUpsResult = followUps
                    .Select(f => new
                    {
                        AppointmentTime = f.AppointmentTime.HasValue ? f.FollowUpDate.Value.Add(f.AppointmentTime.Value).ToString("hh:mm tt") : null,
                        f.PatientName,
                        f.PatientId,
                        f.IsCreateAppointment,
                        f.Mobile,
                        f.FollowUpNotes,
                        f.FollowUpDate
                    })
                    .ToList();
                var doctorId = Convert.ToInt64(doctorData.Tables[0].Rows[0]["ID"]);
                var prescriptionComplaints = new List<emr_prescription_complaint>();

                var complaints = await _emr_complaintService.Queryable().Where(a => a.CompanyID == companyId).Take(10).ToListAsync();
                var favorites = await _emr_notes_favoriteService.Queryable()
      .Where(ft => ft.ReferenceType == "C"
                && ft.DoctorId == doctorId
                && ft.CompanyId == companyId)
      .ToListAsync();
                var complaintList = complaints
    .Select(cmp =>
    {
        var pc = prescriptionComplaints.FirstOrDefault(x => x.ComplaintId == cmp.ID);
        var fav = favorites.FirstOrDefault(x => x.ReferenceId == cmp.ID);

        return new
        {
            id = cmp.ID,
            Complaint = pc?.Complaint ?? cmp.Complaint,
            favoriteId = fav?.ID,
            Isfavorite = fav != null ? 1 : 0
        };
    })
    .Distinct()
    .Take(10)
    .ToList();

                var prescriptionObservations = new List<emr_prescription_observation>();
                var observations = await _emr_observationService.Queryable().Where(a => a.CompanyID == companyId)
     .Take(10)
     .ToListAsync();
                var favoritesObser = await _emr_notes_favoriteService.Queryable()
.Where(ft => ft.ReferenceType == "O"
&& ft.DoctorId == doctorId
&& ft.CompanyId == companyId)
.ToListAsync();


                var observationList = observations
    .Select(ob =>
    {
        var po = prescriptionObservations.FirstOrDefault(x => x.ObservationId == ob.ID);
        var fav = favoritesObser.FirstOrDefault(x => x.ReferenceId == ob.ID);

        return new
        {
            id = ob.ID,
            Observation = po?.Observation ?? ob.Observation,
            favoriteId = fav?.ID,
            Isfavorite = fav != null ? 1 : 0
        };
    })
    .Distinct()
    .Take(10)
    .ToList();
                var prescriptionInvestigations = new List<emr_prescription_investigation>();
                // 1. Base investigations
                var investigations = await _emr_investigationService.Queryable().Where(a => a.CompanyID == companyId)
                    .Take(10)
                    .ToListAsync();
                // 3. Favorites
                var investigationFavorites = await _emr_notes_favoriteService.Queryable()
                    .Where(ft => ft.ReferenceType == "I"
                              && ft.DoctorId == doctorId
                              && ft.CompanyId == companyId)
                    .ToListAsync();
                // 4. Merge
                var investigationList = investigations
                    .Select(inv =>
                    {
                        var pi = prescriptionInvestigations.FirstOrDefault(x => x.InvestigationId == inv.ID);
                        var fav = investigationFavorites.FirstOrDefault(x => x.ReferenceId == inv.ID);
                        return new
                        {
                            id = inv.ID,
                            Investigation = pi?.Investigation ?? inv.Investigation,
                            favoriteId = fav?.ID,
                            Isfavorite = fav != null ? 1 : 0
                        };
                    })
                    .Distinct()
                    .Take(10)
                    .ToList();
                // 1. Base diagnoses
                var diagnoses = await _emr_diagnosService.Queryable().Where(a => a.CompanyID == companyId)
                    .Take(10)
                    .ToListAsync();
                // 2. Prescription diagnoses
                var prescriptionDiagnoses = new List<emr_prescription_diagnos>();
                // 3. Favorites
                var diagnosisFavorites = await _emr_notes_favoriteService.Queryable()
                    .Where(ft => ft.ReferenceType == "D"
                              && ft.DoctorId == doctorId
                              && ft.CompanyId == companyId)
                    .ToListAsync();
                // 4. Merge
                var diagnosisList = diagnoses
                    .Select(dia =>
                    {
                        var pd = prescriptionDiagnoses.FirstOrDefault(x => x.DiagnosId == dia.ID);
                        var fav = diagnosisFavorites.FirstOrDefault(x => x.ReferenceId == dia.ID);
                        return new
                        {
                            id = dia.ID,
                            Diagnos = pd?.Diagnos ?? dia.Diagnos,
                            favoriteId = fav?.ID,
                            Isfavorite = fav != null ? 1 : 0
                        };
                    })
                    .Distinct()
                    .Take(10)
                    .ToList();




                var GenderType = genderList;
                var DoctorList = doctorData.Tables[0];
                decimal[] IdList = null;
                var isShowIds = isShowDoctorIds;
                if (!string.IsNullOrWhiteSpace(isShowIds))
                    IdList = isShowIds.Split(',').Select(decimal.Parse).ToArray();
                var DoctorCalander = doctorData.Tables[0];
                //var MedicineList = medicineList;
                var ClinicList = companyInfo;
                var BloodList = bloodList;
                var ServiceType = serviceType;
                var token = nextToken;
                decimal TokenNo = Convert.ToDecimal(token);
                var StatusList = statusList;
                var Followuplist = followUpsResult;
                var ReminderList = reminders;

                objResponse.ResultSet = new
                {
                    GenderList = GenderType,
                    DoctorList = DoctorList,
                    DoctorCalander = DoctorCalander,
                    IsShowDoctorIds = IdList,
                    ClinicList = ClinicList,
                    BloodList = BloodList,
                    //MedicineList = MedicineList,
                    ServiceType = ServiceType,
                    TokenNo = TokenNo,
                    StatusList = StatusList,
                    Followuplist = Followuplist,
                    ReminderList = ReminderList,
                    ComplaintList = complaintList,
                    ObservationList = observationList,
                    InvestigationsList = investigationList,
                    DiagnosisList = diagnosisList,
                };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        public int GetNextToken(decimal companyId)
        {
            // Step 1: Get the maximum TokenNo from adm_company
            int maxCompanyToken = _adm_companyService.Queryable()
                .Where(cm => cm.ID == companyId)
                .Max(cm => (int?)cm.TokenNo) ?? 0;

            // Step 2: Check if any records exist in emr_appointment_mf for today
            bool hasAppointmentsToday = _emr_appointment_mfService.Queryable()
                .Any(mf => mf.CompanyId == companyId && mf.AppointmentDate == DateTime.Today);

            // Step 3: Get the maximum TokenNo from emr_appointment_mf for today
            int maxAppointmentToken = hasAppointmentsToday
                ? _emr_appointment_mfService.Queryable()
                    .Where(mf => mf.CompanyId == companyId && mf.AppointmentDate == DateTime.Today)
                    .Max(mf => (int?)mf.TokenNo) ?? 0
                : 0;

            // Step 4: Calculate the next token
            int nextToken = hasAppointmentsToday
                ? Math.Max(maxAppointmentToken, maxCompanyToken) + 1
                : maxCompanyToken + 1;

            return nextToken;
        }

        [HttpGet]
        [ActionName("PrescriptionLoad")]
        public async Task<ResponseInfo> PrescriptionLoad(string id, string DocId, string Date)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal doctorId = Convert.ToDecimal(DocId);
                decimal companyId = Request.CompanyID();
                decimal patientid = Convert.ToDecimal(id);
                int userid = Convert.ToInt32(Request.LoginID());
                DateTime date = Convert.ToDateTime(Date);

                var vitalObj = _emr_vitalService.Queryable().Where(a => a.CompanyID == companyId && a.PatientId == patientid && DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(date)).Select(z => new
                {
                    z.ID,
                    z.Date,
                    z.Measure,
                    z.VitalId,
                    Name = z.sys_drop_down_value.Value,
                    Unit = z.sys_drop_down_value.Unit
                }).ToList();

                var reslt = await _emr_appointment_mfService.Queryable()
    .Where(a => a.PatientId == patientid
             && a.CompanyId == companyId
             && a.DoctorId == doctorId)
    .GroupBy(a => new { a.PatientId, Date = DbFunctions.TruncateTime(a.AppointmentDate) }) // group by patient + date only
    .Select(g => new
    {
        g.Key.PatientId,
        AppointmentDate = g.Key.Date
    })
    .OrderByDescending(x => x.AppointmentDate)
    .ToListAsync();

                // if you really need string format "yyyy-MM-dd"
                var appointmentDates = reslt.Select(a => new
                {
                    a.PatientId,
                    AppointmentDate = a.AppointmentDate.Value.ToString("yyyy-MM-dd"),
                }).ToList();
                var userObj = _adm_userService.Queryable().Where(a => a.ID == doctorId).FirstOrDefault();
                var result = _service.Queryable().Where(e => DbFunctions.TruncateTime(e.AppointmentDate) == DbFunctions.TruncateTime(date) && e.PatientId == patientid && e.CompanyID == companyId && e.DoctorId == doctorId).Include(a => a.emr_prescription_treatment_template).Include(a => a.emr_prescription_complaint).Include(a => a.emr_prescription_diagnos).Include(a => a.emr_prescription_investigation).Include(a => a.emr_prescription_observation).Include(a => a.emr_prescription_treatment).FirstOrDefault();

                var prescriptionComplaints = new List<emr_prescription_complaint>();

                var complaints = await _emr_complaintService.Queryable().Where(a => a.CompanyID == companyId).Take(10).ToListAsync();
                if (result != null)
                {
                    prescriptionComplaints = await _emr_prescription_complaintService.Queryable()
   .Where(pc => pc.PrescriptionId == result.ID && pc.CompanyID == companyId)
   .ToListAsync();
                }
                var favorites = await _emr_notes_favoriteService.Queryable()
                    .Where(ft => ft.ReferenceType == "C"
                              && ft.DoctorId == doctorId
                              && ft.CompanyId == companyId)
                    .ToListAsync();
                var complaintList = complaints
    .Select(cmp =>
    {
        var pc = prescriptionComplaints.FirstOrDefault(x => x.ComplaintId == cmp.ID);
        var fav = favorites.FirstOrDefault(x => x.ReferenceId == cmp.ID);

        return new
        {
            id = cmp.ID,
            Complaint = pc?.Complaint ?? cmp.Complaint,
            favoriteId = fav?.ID,
            Isfavorite = fav != null ? 1 : 0
        };
    })
    .Distinct()
    .Take(10)
    .ToList();

                var prescriptionObservations = new List<emr_prescription_observation>();
                var observations = await _emr_observationService.Queryable().Where(a => a.CompanyID == companyId)
     .Take(10)
     .ToListAsync();
                if (result != null)
                {
                    prescriptionObservations = await _emr_prescription_observationService.Queryable()
       .Where(po => po.CompanyID == companyId && po.PrescriptionId == result.ID)
       .ToListAsync();
                }
                var favoritesObser = await _emr_notes_favoriteService.Queryable()
    .Where(ft => ft.ReferenceType == "O"
              && ft.DoctorId == doctorId
              && ft.CompanyId == companyId)
    .ToListAsync();


                var observationList = observations
    .Select(ob =>
    {
        var po = prescriptionObservations.FirstOrDefault(x => x.ObservationId == ob.ID);
        var fav = favoritesObser.FirstOrDefault(x => x.ReferenceId == ob.ID);

        return new
        {
            id = ob.ID,
            Observation = po?.Observation ?? ob.Observation,
            favoriteId = fav?.ID,
            Isfavorite = fav != null ? 1 : 0
        };
    })
    .Distinct()
    .Take(10)
    .ToList();
                var prescriptionInvestigations = new List<emr_prescription_investigation>();
                // 1. Base investigations
                var investigations = await _emr_investigationService.Queryable().Where(a => a.CompanyID == companyId)
                    .Take(10)
                    .ToListAsync();
                // 2. Prescription investigations
                if (result != null)
                {
                    prescriptionInvestigations = await _emr_prescription_investigationService.Queryable()
                   .Where(pi => pi.CompanyID == companyId && pi.PrescriptionId == result.ID)
                   .ToListAsync();
                }
                // 3. Favorites
                var investigationFavorites = await _emr_notes_favoriteService.Queryable()
                    .Where(ft => ft.ReferenceType == "I"
                              && ft.DoctorId == doctorId
                              && ft.CompanyId == companyId)
                    .ToListAsync();
                // 4. Merge
                var investigationList = investigations
                    .Select(inv =>
                    {
                        var pi = prescriptionInvestigations.FirstOrDefault(x => x.InvestigationId == inv.ID);
                        var fav = investigationFavorites.FirstOrDefault(x => x.ReferenceId == inv.ID);
                        return new
                        {
                            id = inv.ID,
                            Investigation = pi?.Investigation ?? inv.Investigation,
                            favoriteId = fav?.ID,
                            Isfavorite = fav != null ? 1 : 0
                        };
                    })
                    .Distinct()
                    .Take(10)
                    .ToList();
                // 1. Base diagnoses
                var diagnoses = await _emr_diagnosService.Queryable().Where(a => a.CompanyID == companyId)
                    .Take(10)
                    .ToListAsync();
                // 2. Prescription diagnoses
                var prescriptionDiagnoses = new List<emr_prescription_diagnos>();
                if (result != null)
                {
                    prescriptionDiagnoses = await _emr_prescription_diagnosService.Queryable()
                    .Where(pd => pd.CompanyID == companyId && pd.PrescriptionId == result.ID)
                    .ToListAsync();
                }
                // 3. Favorites
                var diagnosisFavorites = await _emr_notes_favoriteService.Queryable()
                    .Where(ft => ft.ReferenceType == "D"
                              && ft.DoctorId == doctorId
                              && ft.CompanyId == companyId)
                    .ToListAsync();
                // 4. Merge
                var diagnosisList = diagnoses
                    .Select(dia =>
                    {
                        var pd = prescriptionDiagnoses.FirstOrDefault(x => x.DiagnosId == dia.ID);
                        var fav = diagnosisFavorites.FirstOrDefault(x => x.ReferenceId == dia.ID);
                        return new
                        {
                            id = dia.ID,
                            Diagnos = pd?.Diagnos ?? dia.Diagnos,
                            favoriteId = fav?.ID,
                            Isfavorite = fav != null ? 1 : 0
                        };
                    })
                    .Distinct()
                    .Take(10)
                    .ToList();
                var isBackDatedAppointment = await _adm_companyService.Queryable()
    .Where(x => x.ID == companyId)
    .Select(x => x.IsBackDatedAppointment)
    .FirstOrDefaultAsync();

                var query = _emr_appointment_mfService.Queryable()
                    .Where(mf => mf.CompanyId == companyId
                              && DbFunctions.TruncateTime(mf.AppointmentDate) == DbFunctions.TruncateTime(date)
                              && mf.DoctorId == doctorId
                              && mf.PatientId == patientid)
                    .Select(mf => new
                    {
                        mf.ID,
                        DoctorName = mf.Doctor.Name,
                        SlotTime = mf.Doctor.SlotTime,
                        PatientName = mf.emr_patient_mf.PatientName,       // navigation to Patient
                        mf.AppointmentDate,
                        mf.AppointmentTime,
                        mf.DoctorId,
                        mf.StatusId,
                        mf.PatientId,
                        CNIC = mf.emr_patient_mf.CNIC,
                        Note = mf.Notes,
                        CreatedBy = mf.adm_user_mf.Name,          // navigation to CreatedByUser
                        Color = mf.StatusId == 1 ? "#dddddd" :
                                mf.StatusId == 2 ? "#fd8d64" :
                                mf.StatusId == 5 ? "#fdca66" :
                                mf.StatusId == 6 ? "#dd2626" :
                                mf.StatusId == 7 ? "#65d3fd" :
                                mf.StatusId == 8 ? "#6597ff" :
                                mf.StatusId == 9 ? "#0bb8ab" :
                                mf.StatusId == 10 ? "#fb64a7" :
                                mf.StatusId == 25 ? "#6265fd" : ""
                    })
                    .ToList();
                var appointmentList = query.Select(mf => new
                {
                    mf.ID,
                    mf.DoctorName,
                    mf.PatientName,
                    mf.DoctorId,
                    mf.StatusId,
                    mf.CNIC,
                    mf.PatientId,
                    mf.CreatedBy,
                    mf.Note,
                    mf.Color,
                    AppointmentDate = mf.AppointmentDate.ToString("yyyy-MM-dd"),
                    AppointmentTime = mf.AppointmentTime.ToString(@"hh\:mm\:ss"),
                    StartDate = mf.AppointmentDate
                   .Add(mf.AppointmentTime)
                   .ToString("yyyy-MM-ddTHH:mm:ss"),
                    EndDate = mf.AppointmentDate
                  .Add(mf.AppointmentTime)
                  .Add(mf.SlotTime ?? TimeSpan.Zero)
                  .ToString("yyyy-MM-ddTHH:mm:ss"),
                }).ToList();

                var ids = result?.emr_prescription_treatment?.Select(a => a.MedicineId).ToList() ?? new List<decimal?>();
                var medicineList = _emr_medicineService.Queryable().Where(a => a.CompanyID == companyId && ids.Contains(a.ID)).Select(a => new { Id = a.ID, Medicine = a.Medicine, Type = a.sys_drop_down_value1.Value });
                var itemList = _adm_itemService.Queryable().Where(a => a.CompanyId == companyId && ids.Contains(a.ID)).Select(a => new { Id = a.ID, Medicine = a.Name, Type = a.sys_drop_down_value1.Value });
                var MedicineList = itemList.Union(medicineList).ToList();

                var patientInfo = _emr_patientService.Queryable().Where(a => a.ID == patientid && a.CompanyId == companyId).FirstOrDefault();

                var CurntPrevDate = appointmentDates;
                var AppointmentList = appointmentList;
                var ComplaintList = complaintList;
                var ObservationList = observationList;
                var InvestigationsList = investigationList;
                var DiagnosisList = diagnosisList;
                bool IsBackDatedAppointment = isBackDatedAppointment;
                objResponse.ResultSet = new
                {
                    patientInfo = patientInfo,
                    Doctorid = doctorId,
                    prescription_mf = result,
                    prescription_treatment = result?.emr_prescription_treatment ?? new List<emr_prescription_treatment>(),
                    prescription_complaint = result?.emr_prescription_complaint ?? new List<emr_prescription_complaint>(),
                    prescription_diagnos = result?.emr_prescription_diagnos ?? new List<emr_prescription_diagnos>(),
                    prescription_investigation = result?.emr_prescription_investigation ?? new List<emr_prescription_investigation>(),
                    prescription_observation = result?.emr_prescription_observation ?? new List<emr_prescription_observation>(),
                    CurntPrevDateList = CurntPrevDate,
                    AppointmentList = AppointmentList,
                    ComplaintList = ComplaintList,
                    ObservationList = ObservationList,
                    InvestigationsList = InvestigationsList,
                    DiagnosisList = DiagnosisList,
                    IsBackDatedAppointment = IsBackDatedAppointment,
                    vitalList = vitalObj,
                    MedicineList = MedicineList,
                    Instruction = userObj == null ? "" : userObj.Instruction,
                };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        [HttpGet]
        [ActionName("PrescriptionDataLoad")]
        public ResponseInfo PrescriptionDataLoad(string PatientId, string DocId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal Doctorid = Convert.ToDecimal(DocId);
                decimal CompanyID = Request.CompanyID();
                decimal patientid = Convert.ToDecimal(PatientId);
                int userid = Convert.ToInt32(Request.LoginID());
                var CurntPrevDate = _emr_appointment_mfService.Queryable().Where(a => a.CompanyId == CompanyID && a.PatientId == patientid && a.DoctorId == Doctorid)
                .GroupBy(a => new { a.PatientId, DateOnly = DbFunctions.TruncateTime(a.AppointmentDate) })
                .Select(z => new
                {
                    z.Key.PatientId,
                    AppointmentDate = z.Key.DateOnly.Value
                }).OrderByDescending(x => x.AppointmentDate).ToList();

                objResponse.ResultSet = CurntPrevDate.Select(z => new
                {
                    z.PatientId,
                    AppointmentDate = z.AppointmentDate.ToString("yyyy-MM-dd")
                });

            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        [HttpPost]
        [HttpGet]
        [ActionName("ComplaintList")]
        public async Task<ResponseInfo> ComplaintList()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var result = _emr_complaintService.Queryable().Where(a => (a.CompanyID == CompanyID || a.CompanyID == 0)).Select(z => new
                {
                    id = z.ID,
                    name = z.Complaint,
                }).Take(100).ToList();
                objResponse.ResultSet = new
                {
                    result = result
                };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;

        }
        [HttpPost]
        [HttpGet]
        [ActionName("InvestigationList")]
        public async Task<ResponseInfo> InvestigationList()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var result = _emr_investigationService.Queryable().Where(a => (a.CompanyID == CompanyID || a.CompanyID == 0)).Select(z => new
                {
                    id = z.ID,
                    name = z.Investigation,

                }).Take(100).ToList();
                objResponse.ResultSet = new
                {
                    result = result
                };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        [HttpPost]
        [HttpGet]
        [ActionName("ObservationList")]
        public async Task<ResponseInfo> ObservationList()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var result = _emr_observationService.Queryable().Where(a => (a.CompanyID == CompanyID || a.CompanyID == 0)).Select(z => new
                {
                    id = z.ID,
                    name = z.Observation,

                }).Take(100).ToList();
                objResponse.ResultSet = new
                {
                    result = result
                };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;

        }
        [HttpPost]
        [HttpGet]
        [ActionName("DiagnosisList")]
        public async Task<ResponseInfo> DiagnosisList()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var result = _emr_diagnosService.Queryable().Where(a => (a.CompanyID == CompanyID || a.CompanyID == 0)).Select(z => new
                {
                    id = z.ID,
                    name = z.Diagnos,

                }).Take(100).ToList();
                objResponse.ResultSet = new
                {
                    result = result
                };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;

        }
        [HttpPost]
        [HttpGet]
        [ActionName("ComplaintSearch")]
        public async Task<ResponseInfo> ComplaintSearch(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var result = _emr_complaintService.Queryable().Where(a => (a.CompanyID == CompanyID || a.CompanyID == 0) && (a.Complaint.Contains(term))).Select(z => new
                {
                    value = z.ID,
                    label = z.Complaint,

                }).ToList();
                objResponse.ResultSet = new
                {
                    result = result
                };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;

        }
        [HttpPost]
        [HttpGet]
        [ActionName("DiagnosSearch")]
        public async Task<ResponseInfo> DiagnosSearch(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var result = _emr_diagnosService.Queryable().Where(a => (a.CompanyID == CompanyID || a.CompanyID == 0) && (a.Diagnos.Contains(term))).Select(z => new
                {
                    value = z.ID,
                    label = z.Diagnos,

                }).ToList();
                objResponse.ResultSet = new
                {
                    result = result
                };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;

        }
        [HttpPost]
        [HttpGet]
        [ActionName("InstructionSearch")]
        public async Task<ResponseInfo> InstructionSearch(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var result = _emr_instructionService.Queryable().Where(a => (a.CompanyID == CompanyID || a.CompanyID == 0) && (a.Instructions.Contains(term))).Select(z => new
                {
                    value = z.ID,
                    label = z.Instructions,

                }).ToList();
                objResponse.ResultSet = new
                {
                    result = result
                };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;

        }
        [HttpPost]
        [HttpGet]
        [ActionName("ObservationSearch")]
        public async Task<ResponseInfo> ObservationSearch(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var result = _emr_observationService.Queryable().Where(a => (a.CompanyID == CompanyID || a.CompanyID == 0) && (a.Observation.Contains(term))).Select(z => new
                {
                    value = z.ID,
                    label = z.Observation,

                }).ToList();
                objResponse.ResultSet = new
                {
                    result = result
                };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;

        }
        [HttpPost]
        [HttpGet]
        [ActionName("InvestigationSearch")]
        public async Task<ResponseInfo> InvestigationSearch(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var result = _emr_investigationService.Queryable().Where(a => (a.CompanyID == CompanyID || a.CompanyID == 0) && (a.Investigation.Contains(term))).Select(z => new
                {
                    value = z.ID,
                    label = z.Investigation,

                }).ToList();
                objResponse.ResultSet = new
                {
                    result = result
                };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;

        }

        public List<AppointmentInfo> AppointmentInfo(decimal CompanyID)
        {
            var AppointmentList = _emr_appointment_mfService.Queryable().Where(a => a.CompanyId == CompanyID && DbFunctions.TruncateTime(a.AppointmentDate) == DbFunctions.TruncateTime(DateTime.Now))
                  .Include(a => a.adm_user_mf2).Include(a => a.emr_patient_mf).Select(z => new
                  {
                      ID = z.ID,
                      DoctorName = z.adm_user_mf2.Name,
                      PatientName = z.emr_patient_mf.PatientName,
                      AppointmentDate = z.AppointmentDate,
                      AppointmentTime = z.AppointmentTime,
                      DoctorId = z.DoctorId,
                      StatusId = z.StatusId,
                      PatientId = z.PatientId,
                      CNIC = z.emr_patient_mf.CNIC,
                      Note = z.Notes,
                      CreatedBy = z.adm_user_mf.Name,
                  }).OrderBy(d => d.ID).ToList();

            var LIST = AppointmentList.AsEnumerable().Select(m => new AppointmentInfo
            {
                ID = m.ID,
                DoctorName = m.DoctorName,
                PatientName = m.PatientName,
                DoctorId = m.DoctorId,
                Note = m.Note,
                StatusId = m.StatusId,
                CNIC = m.CNIC,
                PatientId = m.PatientId,
                CreatedBy = m.CreatedBy,
                Color = m.StatusId == 25 ? "#007bff" : m.StatusId == 5 ? "#00FF00" : m.StatusId == 6 ? "#FF0000" : m.StatusId == 8 ? "#808080" : "",
                StartDate = m.AppointmentDate.ToString("yyyy-MM-dd") + "T" + m.AppointmentTime.ToString("hh\\:mm\\:ss"),
                EndDate = m.AppointmentDate.ToString("yyyy-MM-dd") + "T" + WorkingHours(Convert.ToDateTime(m.AppointmentDate + m.AppointmentTime), m.DoctorId),
            }).OrderBy(a => a.DoctorId).ToList();
            return LIST.ToList();
        }

        public PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            throw new NotImplementedException();
        }

        ResponseInfo IERPAPIInterface<emr_prescription_mf>.GetById(string Id)
        {
            throw new NotImplementedException();
        }
    }
}
