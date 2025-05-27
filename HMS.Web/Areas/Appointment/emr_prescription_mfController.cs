using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
using HMS.Service;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Appointment;
using HMS.Service.Services.Items;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using HMS.Web.API.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

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

        public emr_prescription_mfController(IUnitOfWorkAsync unitOfWorkAsync, Iemr_service_mfService emr_service_mfService, Iemr_vitalService emr_vitalService, Iemr_patientService emr_patientService, Iemr_prescription_mfService service, Iemr_medicineService emr_medicineService,
            Iemr_prescription_complaintService emr_prescription_complaintService, Iemr_prescription_diagnosService emr_prescription_diagnosService,
            Iemr_prescription_investigationService emr_prescription_investigationService,
            Iemr_prescription_observationService emr_prescription_observationService,
            Iemr_complaintService emr_complaintService,
            Iemr_diagnosService emr_diagnosService,
            Iemr_investigationService emr_investigationService,
            Iemr_observationService emr_observationService,
            Iemr_instructionService emr_instructionService,
            Iadm_companyService adm_companyService,
                      Iemr_prescription_treatmentService emr_prescription_treatmentService, Iemr_prescription_treatment_templateService emr_prescription_treatment_templateService,
            Iadm_userService adm_userService, Iemr_appointment_mfService emr_appointment_mfService, Iadm_user_companyService adm_user_companyService,
            Iadm_role_mfService adm_role_mfService, Iadm_user_tokenService adm_user_tokenService, Isys_drop_down_valueService sys_drop_down_valueService,
            Isys_notification_alertService sys_notification_alertService, IStoredProcedureService procedureService,Iadm_itemService adm_itemService)

        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _emr_medicineService = emr_medicineService;
            _service = service; _emr_service_mfService = emr_service_mfService;
            _emr_vitalService = emr_vitalService;
            _procedureService = procedureService;
            _adm_companyService = adm_companyService;
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
        }
        [HttpPost]
        [HttpGet]
        [ActionName("Save")]
        public async Task<ResponseInfo> Save(emr_prescription_mf Model)
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
                decimal UserID = Request.LoginID();

                List<emr_prescription_complaint> emr_prescription_complaint = new List<emr_prescription_complaint>();
                // emr_prescription_complaint.AddRange(Model.emr_prescription_complaint);

                List<emr_prescription_diagnos> emr_prescription_diagnos = new List<emr_prescription_diagnos>();
                //emr_prescription_diagnos.AddRange(Model.emr_prescription_diagnos);

                List<emr_prescription_investigation> emr_prescription_investigation = new List<emr_prescription_investigation>();
                //emr_prescription_investigation.AddRange(Model.emr_prescription_investigation);

                List<emr_prescription_observation> emr_prescription_observation = new List<emr_prescription_observation>();
                //emr_prescription_observation.AddRange(Model.emr_prescription_observation);

                List<emr_prescription_treatment> emr_prescription_treatment = new List<emr_prescription_treatment>();
                List<emr_prescription_treatment_template> emr_prescription_treatment_template = new List<emr_prescription_treatment_template>();

                // emr_prescription_treatment.AddRange(Model.emr_prescription_treatment);

                DataAccessManager dataAccessManager = new DataAccessManager();
                var ht = new Hashtable();
                ht.Add("@CompanyID", CompanyID);
                ht.Add("@PatientId", Model.PatientId);
                var AllData = dataAccessManager.GetDataSet("SP_GetPrescriptionMaxIds", ht);

                var findPatient = AllData.Tables[0];//_emr_appointment_mfService.Queryable().Where(a => a.CompanyId == CompanyID && a.PatientId == Model.PatientId).FirstOrDefault();

                decimal ID = Convert.ToDecimal(AllData.Tables[1].Rows[0]["ID"].ToString());
                //if (_service.Queryable().Count() > 0)
                //    ID = _service.Queryable().Max(e => e.ID) + 1;
                Model.ID = ID;
                Model.CompanyID = Request.CompanyID();
                Model.CreatedBy = Request.LoginID();
                Model.CreatedDate = Request.DateTimes();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                if (findPatient == null)
                {
                    if (Model.FollowUpTime == null)
                        Model.FollowUpTime = DateTime.Now.TimeOfDay;
                }
                decimal complaintID = Convert.ToDecimal(AllData.Tables[2].Rows[0]["complaintID"].ToString());

                foreach (emr_prescription_complaint item in Model.emr_prescription_complaint)
                {
                    emr_prescription_complaint obj = new emr_prescription_complaint();
                    obj.ID = complaintID;
                    obj.PrescriptionId = ID;
                    obj.PatientId = Model.PatientId;
                    obj.CompanyID = CompanyID;
                    obj.Complaint = item.Complaint;
                    obj.ComplaintId = item.ComplaintId;
                    obj.CreatedBy = Request.LoginID();
                    obj.CreatedDate = Request.DateTimes();
                    obj.ModifiedBy = Request.LoginID();
                    obj.ModifiedDate = Request.DateTimes();
                    emr_prescription_complaint.Add(obj);
                    complaintID++;
                }
                Model.emr_prescription_complaint = null;
                Model.emr_prescription_complaint = emr_prescription_complaint;
                //insert diagnos
                decimal diagnosID = Convert.ToDecimal(AllData.Tables[3].Rows[0]["diagnosID"].ToString());

                foreach (emr_prescription_diagnos item in Model.emr_prescription_diagnos)
                {
                    item.ID = diagnosID;
                    item.PrescriptionId = ID;
                    item.PatientId = Model.PatientId;
                    item.CompanyID = CompanyID;
                    item.Diagnos = item.Diagnos;
                    item.DiagnosId = item.DiagnosId;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    emr_prescription_diagnos.Add(item);
                    diagnosID++;
                }
                Model.emr_prescription_diagnos = null;
                Model.emr_prescription_diagnos = emr_prescription_diagnos;
                //insert investigation
                decimal investigationID = Convert.ToDecimal(AllData.Tables[4].Rows[0]["investigationID"].ToString());
                foreach (emr_prescription_investigation item in Model.emr_prescription_investigation)
                {
                    item.ID = investigationID;
                    item.PrescriptionId = ID;
                    item.PatientId = Model.PatientId;
                    item.CompanyID = CompanyID;
                    item.Investigation = item.Investigation;
                    item.InvestigationId = item.InvestigationId;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    emr_prescription_investigation.Add(item);
                    investigationID++;
                }
                Model.emr_prescription_investigation = null;
                Model.emr_prescription_investigation = emr_prescription_investigation;
                //insert Observation
                decimal observationID = Convert.ToDecimal(AllData.Tables[5].Rows[0]["observationID"].ToString());
                foreach (emr_prescription_observation item in Model.emr_prescription_observation)
                {
                    item.ID = observationID;
                    item.PrescriptionId = ID;
                    item.PatientId = Model.PatientId;
                    item.CompanyID = CompanyID;
                    item.Observation = item.Observation;
                    item.ObservationId = item.ObservationId;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    emr_prescription_observation.Add(item);
                    observationID++;
                }
                Model.emr_prescription_observation = null;
                Model.emr_prescription_observation = emr_prescription_observation;
                //insert treatment
                decimal treatmentID = Convert.ToDecimal(AllData.Tables[6].Rows[0]["treatmentID"].ToString());

                foreach (emr_prescription_treatment item in Model.emr_prescription_treatment)
                {
                    item.ID = treatmentID;
                    item.PrescriptionId = ID;
                    item.PatientId = Model.PatientId;
                    item.CompanyID = CompanyID;
                    item.MedicineId = item.MedicineId;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    emr_prescription_treatment.Add(item);
                    treatmentID++;
                }
                Model.emr_prescription_treatment = null;
                Model.emr_prescription_treatment_template = null;
                Model.emr_prescription_treatment = emr_prescription_treatment;
                if (Model.IsTemplate)
                {
                    decimal TemplateID = Convert.ToDecimal(AllData.Tables[7].Rows[0]["TemplateID"].ToString());
                    emr_prescription_treatment_template obj = new emr_prescription_treatment_template();
                    obj.ID = TemplateID;
                    obj.TemplateName = Model.TemplateName;
                    obj.PrescriptionId = ID;
                    obj.CompanyID = Request.CompanyID();
                    obj.CreatedBy = Request.LoginID();
                    obj.CreatedDate = Request.DateTimes();
                    obj.ModifiedBy = Request.LoginID();
                    obj.ModifiedDate = Request.DateTimes();
                    emr_prescription_treatment_template.Add(obj);
                    Model.emr_prescription_treatment_template = emr_prescription_treatment_template;
                }
                emr_appointment_mf appObj = new emr_appointment_mf();
                if (Model.IsCreateAppointment || findPatient == null)
                {
                    decimal AppID = Convert.ToDecimal(AllData.Tables[8].Rows[0]["AppID"].ToString());
                    appObj.ID = AppID;
                    appObj.StatusId = (int)sys_dropdown_mfEnum.StatusId;
                    appObj.CompanyId = Request.CompanyID();
                    appObj.PatientId = Model.PatientId;
                    appObj.DoctorId = Model.DoctorId;
                    appObj.AppointmentDate = Model.AppointmentDate;

                    appObj.AppointmentTime = (TimeSpan)Model.FollowUpTime;
                    appObj.Notes = Model.Notes;
                    appObj.CreatedBy = Request.LoginID();
                    appObj.CreatedDate = Request.DateTimes();
                    appObj.ModifiedBy = Request.LoginID();
                    appObj.ModifiedDate = Request.DateTimes();
                }


                string appointment_mfData = JsonConvert.SerializeObject(appObj, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });

                string JsonData = JsonConvert.SerializeObject(Model, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });

                try
                {
                    Hashtable ht1 = new Hashtable();
                    ht1.Add("@json", JsonData);
                    ht1.Add("@AppmfData", appointment_mfData);
                    dataAccessManager.ExecuteNonQuery("SP_InsertPrescription", ht1);

                    //await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Save;
                    objResponse.IsSuccess = true;

                    var patientInfo = _emr_patientService.Queryable().Where(a => a.CompanyId == CompanyID && a.ID == Model.PatientId)
                        .Select(z => new
                        {
                            z.PatientName,
                            z.Mobile,
                            z.Age,
                            ClinicIogo = z.adm_company.CompanyLogo,
                        }).FirstOrDefault();

                    var doctorInfo = _emr_patientService.DoctorList(CompanyID, UserID);
                    var doctor = doctorInfo.DoctList.AsEnumerable().Where(a => a.ID == Model.PatientId).FirstOrDefault();


                    objResponse.ResultSet = new
                    {
                        doctor = doctor,
                        Model = Model,
                        patientInfo = patientInfo,
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

                var appinfo = _emr_appointment_mfService.Queryable().Where(a => a.ID == Model.AppointmentId).FirstOrDefault();
                if (Model.IsCreateAppointment && appinfo == null)
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
                    appObj.AppointmentDate = Model.AppointmentDate;
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
                        }).FirstOrDefault();
                    var doctorInfo = _emr_patientService.DoctorList(CompanyID, userid);
                    var doctor = doctorInfo.DoctList.AsEnumerable().Where(a => a.ID == Model.DoctorId).FirstOrDefault();
                    var vitallist = _emr_vitalService.Queryable().Where(a => a.CompanyID == CompanyID && a.PatientId == Model.PatientId).Select(a => new
                    {
                        Measure = a.Measure,
                        Unit = a.sys_drop_down_value.Unit,
                        Name = a.sys_drop_down_value.Value
                    }).ToList();
                    objResponse.ResultSet = new
                    {
                        Model = Model,
                        patientInfo = patientInfo,
                        doctor = doctor,
                        vitallist = vitallist,
                    };
                    //if (Model.Email != null && Model.Email != "")
                    //{
                    //    string path = @"~\Templates\Prescription.txt";
                    //    path = HttpContext.Current.Server.MapPath(path);
                    //    string template = File.ReadAllText(path);
                    //    if (template.Contains("[[DoctorName]]"))
                    //    {
                    //        template = template.Replace("[[DoctorName]]", doctor.Name);
                    //    }
                    //    if (template.Contains("[[DoctorDesignationQualification]]"))
                    //    {
                    //        string DesignationQualification = doctor.Designation + " " + doctor.Qualification;
                    //        template = template.Replace("[[DoctorDesignationQualification]]", DesignationQualification);
                    //    }
                    //    if (template.Contains("[[DoctorDesignation]]"))
                    //    {
                    //        template = template.Replace("[[DoctorDesignation]]", doctor.Designation);
                    //    }
                    //    if (template.Contains("[[DoctorPhoneNo]]"))
                    //    {
                    //        template = template.Replace("[[DoctorPhoneNo]]", doctor.PhoneNo);
                    //    }

                    //    if (template.Contains("[[ClinicIogo]]"))
                    //    {
                    //        string ApiTempUrl = System.Configuration.ConfigurationManager.AppSettings["ApiTempUrl"];
                    //        string image = "";
                    //        if (patientInfo.ClinicIogo != null && patientInfo.ClinicIogo != "")
                    //        {
                    //            image = ApiTempUrl + "" + patientInfo.ClinicIogo;
                    //        }
                    //        else
                    //        {
                    //            image = "../assets/app/media/img/icons/doctor-logo.png";
                    //        }

                    //        template = template.Replace("[[ClinicIogo]]", image);
                    //    }

                    //    if (template.Contains("[[PatientName]]"))
                    //    {
                    //        template = template.Replace("[[PatientName]]", patientInfo.PatientName);
                    //    }
                    //    if (template.Contains("[[PatientAge]]"))
                    //    {
                    //        template = template.Replace("[[PatientAge]]", patientInfo.Age.ToString());
                    //    }

                    //    if (template.Contains("[[AppointmentDate]]"))
                    //    {
                    //        template = template.Replace("[[AppointmentDate]]", Model.AppointmentDate.ToString());
                    //    }


                    //    if (template.Contains("[[VitalList]]"))
                    //    {
                    //        string vital = "";
                    //        foreach (var item in vitallist)
                    //        {
                    //            vital += item.Name + " " + item.Measure;
                    //        }
                    //        template = template.Replace("[[VitalList]]", vital);
                    //    }
                    //    if (template.Contains("[[diagnos]]"))
                    //    {
                    //        string diagnos = "";
                    //        if (Model.emr_prescription_diagnos != null)
                    //        {
                    //            foreach (var item in Model.emr_prescription_diagnos)
                    //            {
                    //                diagnos += item.Diagnos;
                    //            }
                    //        }
                    //        template = template.Replace("[[diagnos]]", diagnos);
                    //    }
                    //    if (template.Contains("[[Complaint]]"))
                    //    {
                    //        string Complaint = "";
                    //        if (Model.emr_prescription_complaint != null)
                    //        {
                    //            foreach (var item in Model.emr_prescription_complaint)
                    //            {
                    //                Complaint += item.Complaint;
                    //            }
                    //        }
                    //        template = template.Replace("[[Complaint]]", Complaint);
                    //    }
                    //    if (template.Contains("[[Investigation]]"))
                    //    {
                    //        string Investigation = "";
                    //        if (Model.emr_prescription_investigation != null)
                    //        {
                    //            foreach (var item in Model.emr_prescription_investigation)
                    //            {
                    //                Investigation += item.Investigation;
                    //            }
                    //        }
                    //        template = template.Replace("[[Investigation]]", Investigation);
                    //    }

                    //    if (template.Contains("[[Observation]]"))
                    //    {
                    //        string Observation = "";
                    //        if (Model.emr_prescription_observation != null)
                    //        {
                    //            foreach (var item in Model.emr_prescription_observation)
                    //            {
                    //                Observation += item.Observation;
                    //            }
                    //        }
                    //        template = template.Replace("[[Observation]]", Observation);
                    //    }



                    //    if (template.Contains("[[TreatTable]]"))
                    //    {
                    //        string table = "";

                    //        if (Model.emr_prescription_treatment != null)
                    //        {
                    //            table += "<table>";
                    //            table += "<thead>";
                    //            table += " <th>";
                    //            table += " Medicine";
                    //            table += "</th>";
                    //            table += " <th>";
                    //            table += "   Days";
                    //            table += "</th>";
                    //            table += " <th>";
                    //            table += "Measure";
                    //            table += "</th>";
                    //            table += " <th>";
                    //            table += " Instruction";
                    //            table += " </th>";
                    //            table += "</thead>";
                    //            table += " <tbody>";
                    //            foreach (var item in Model.emr_prescription_treatment)
                    //            {
                    //                table += "<tr>";
                    //                table += "<td>" + item.MedicineName + "</td>";
                    //                table += "<td>" + item.Duration + "</td>";
                    //                table += "<td>" + item.Measure + "</td>";
                    //                table += "<td>" + item.Instructions + "</td>";
                    //                table += "</tr>";
                    //            }
                    //            table += "</tbody>";
                    //            table += "</table>";
                    //        }
                    //        template = template.Replace("[[TreatTable]]", table);
                    //    }

                    //    if (template.Contains("[[Instructions]]"))
                    //    {
                    //        string Instructions = "";
                    //        if (Model.emr_prescription_treatment != null)
                    //        {
                    //            foreach (var item in Model.emr_prescription_treatment)
                    //            {
                    //                Instructions += item.Instructions;
                    //            }
                    //        }
                    //        template = template.Replace("[[Instructions]]", Instructions);
                    //    }



                    //    EmailService obj = new EmailService();
                    //    Task.Run(() => obj.SendEmailPrescription(template, Model.Email));
                    //}
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
        [ActionName("GetById")]
        public ResponseInfo GetById(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var result = _service.Queryable().Where(e => e.ID.ToString() == Id && e.CompanyID == CompanyID).Include(a => a.emr_prescription_treatment_template).Include(a => a.emr_prescription_complaint).Include(a => a.emr_prescription_diagnos).Include(a => a.emr_prescription_investigation).Include(a => a.emr_prescription_observation).Include(a => a.emr_prescription_treatment).FirstOrDefault();
                decimal Doctorid = Convert.ToDecimal(result.DoctorId);
                decimal patientid = Convert.ToDecimal(result.PatientId);
                int userid = Convert.ToInt32(Request.LoginID());
                DateTime date = Convert.ToDateTime(DateTime.Now);

                DataAccessManager dataAccessManager = new DataAccessManager();
                var ht = new Hashtable();
                ht.Add("@CompanyID", CompanyID);
                ht.Add("@PatientId", patientid);
                ht.Add("@Doctorid", Doctorid);
                ht.Add("@Date", date);
                ht.Add("@UserId", userid);

                var AllData = dataAccessManager.GetDataSet("SP_GetPrescriptionByPatientIdAndDoctorId", ht);
                var PatientInfo = AllData.Tables[0];
                var UnitList = AllData.Tables[15];
                var MadicineTypeList = AllData.Tables[16];
                var DoesList = AllData.Tables[17];

                var MedicineList = AllData.Tables[14];
                var CurntPrevDateList = AllData.Tables[1];


                var ComplaintList = AllData.Tables[2];
                var ObservationList = AllData.Tables[3];
                var InvestigationsList = AllData.Tables[4];

                var DiagnosisList = AllData.Tables[5];
                var ClinicList = _adm_user_companyService.Queryable()
                            .Where(e => e.UserID == userid).Include(x => x.adm_company).Select(x => new { x.CompanyID, x.adm_company.CompanyName }).ToList();
                var BloodList = AllData.Tables[19];
                var GenderType = AllData.Tables[18];
                var DoctorList = AllData.Tables[20];
                var DoctorCalander = AllData.Tables[22];
                decimal[] IdList = null;
                var isShowIds = AllData.Tables[21].Rows[0]["IsShowDoctorIds"].ToString();
                if (isShowIds != "")
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
        [ActionName("TemplateLoadById")]
        public ResponseInfo TemplateLoadById(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var result = _service.Queryable().Where(e => e.ID.ToString() == Id && e.CompanyID == CompanyID).Include(a => a.emr_prescription_complaint).Include(a => a.emr_prescription_diagnos).Include(a => a.emr_prescription_investigation).Include(a => a.emr_prescription_observation).Include(a => a.emr_prescription_treatment).Include(a => a.emr_prescription_treatment_template).Include(a => a.emr_patient_mf).FirstOrDefault();

                var MedicineList = _emr_medicineService.Queryable().Where(a => a.CompanyID == CompanyID).Select(a => new { Id = a.ID, Medicine = a.Medicine }).ToList();
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
                        PatientName = z.emr_patient_mf.PatientName,
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
                var doctorInfo = _emr_patientService.DoctorList(CompanyID, userid);
                var doctor = doctorInfo.DoctList.AsEnumerable().Where(a => a.ID == result.DoctorId)
                    .Select(z => new
                    {
                        z.Name,
                        z.Qualification,
                        z.Designation,
                        z.PhoneNo,
                        z.Footer,
                        z.TemplateId,
                        z.HeaderDescription,
                        z.NameUrdu,
                        z.DesignationUrdu,
                        z.QualificationUrdu
                    }).FirstOrDefault();
                var vitallist = _emr_vitalService.Queryable().Where(a => a.CompanyID == CompanyID && a.PatientId == result.PatientId).Select(a => new
                {
                    Measure = a.Measure,
                    Unit = a.sys_drop_down_value.Unit,
                    Name = a.sys_drop_down_value.Value
                }).ToList();
                var MedicineList = _adm_itemService.Queryable().Where(a => a.CompanyId == CompanyID).Select(a => new { Id = a.ID, Medicine = a.Name, TypeId = a.CategoryID, Type = a.sys_drop_down_value1.Value }).ToList();
                objResponse.ResultSet = new
                {
                    doctor = doctor,
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
        public ResponseInfo LoadDropdown()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                int userid = Convert.ToInt32(Request.LoginID());
                DataAccessManager dataAccessManager = new DataAccessManager();
                var ht = new Hashtable();
                ht.Add("@CompanyID", CompanyID);
                ht.Add("@UserId", userid);
                var AllData = dataAccessManager.GetDataSet("SP_LoadDropdown", ht);
                var GenderType = AllData.Tables[0];
                var DoctorList = AllData.Tables[1];
                decimal[] IdList = null;
                var isShowIds = AllData.Tables[2].Rows[0]["IsShowDoctorIds"].ToString();
                if (isShowIds != "")
                    IdList = isShowIds.Split(',').Select(decimal.Parse).ToArray();
                var DoctorCalander = AllData.Tables[3];
                var MedicineList = AllData.Tables[4];
                var ClinicList = AllData.Tables[5];
                var BloodList = AllData.Tables[6];
                var ServiceType = AllData.Tables[7];
                var token = AllData.Tables[8].Rows[0]["TokenNo"].ToString();
                decimal TokenNo = Convert.ToDecimal(token);
                var StatusList = AllData.Tables[10];
                objResponse.ResultSet = new
                {
                    GenderList = GenderType,
                    DoctorList = DoctorList,
                    DoctorCalander = DoctorCalander,
                    IsShowDoctorIds = IdList,
                    ClinicList = ClinicList,
                    BloodList = BloodList,
                    MedicineList = MedicineList,
                    ServiceType = ServiceType,
                    TokenNo = TokenNo,
                    StatusList = StatusList,
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
        [ActionName("PrescriptionLoad")]
        public ResponseInfo PrescriptionLoad(string id, string DocId, string Date)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal Doctorid = Convert.ToDecimal(DocId);
                decimal CompanyID = Request.CompanyID();
                decimal patientid = Convert.ToDecimal(id);
                int userid = Convert.ToInt32(Request.LoginID());
                DateTime date = Convert.ToDateTime(Date);

                DataAccessManager dataAccessManager = new DataAccessManager();
                var ht = new Hashtable();
                ht.Add("@CompanyID", CompanyID);
                ht.Add("@PatientId", patientid);
                ht.Add("@Doctorid", Doctorid);
                ht.Add("@Date", date);
                ht.Add("@UserId", userid);
                var AllData = dataAccessManager.GetDataSet("SP_GetPrescriptionByPatientIdAndDoctorId", ht);



                var patientInfo = AllData.Tables[0];// _emr_patientService.Queryable().Where(a => a.CompanyId == CompanyID && a.ID == patientid).Include(a => a.emr_appointment_mf)
                                                    //.Include(a => a.emr_prescription_mf)
                                                    //.Select(z => new
                                                    //{
                                                    //    patientInfo = z,
                                                    //    Doctorid = z.emr_appointment_mf.OrderByDescending(a => a.AppointmentDate).Where(a => a.PatientId == z.ID).FirstOrDefault().DoctorId,
                                                    //    prescription_mf = z.emr_prescription_mf.Where(a => (a.PatientId == z.ID && DbFunctions.TruncateTime(a.AppointmentDate) == DbFunctions.TruncateTime(date))),
                                                    //    prescription_treatment = z.emr_prescription_mf.Where(a => (a.PatientId == z.ID && DbFunctions.TruncateTime(a.AppointmentDate) == DbFunctions.TruncateTime(date))).SelectMany(a => a.emr_prescription_treatment),
                                                    //    prescription_complaint = z.emr_prescription_mf.Where(a => (a.PatientId == z.ID && DbFunctions.TruncateTime(a.AppointmentDate) == DbFunctions.TruncateTime(date))).SelectMany(a => a.emr_prescription_complaint),
                                                    //    prescription_diagnos = z.emr_prescription_mf.Where(a => (a.PatientId == z.ID && DbFunctions.TruncateTime(a.AppointmentDate) == DbFunctions.TruncateTime(date))).SelectMany(a => a.emr_prescription_diagnos),
                                                    //    prescription_investigation = z.emr_prescription_mf.Where(a => (a.PatientId == z.ID && DbFunctions.TruncateTime(a.AppointmentDate) == DbFunctions.TruncateTime(date))).SelectMany(a => a.emr_prescription_investigation),
                                                    //    prescription_observation = z.emr_prescription_mf.Where(a => (a.PatientId == z.ID && DbFunctions.TruncateTime(a.AppointmentDate) == DbFunctions.TruncateTime(date))).SelectMany(a => a.emr_prescription_observation)
                                                    //}).FirstOrDefault();

                var CurntPrevDate = AllData.Tables[1];// _emr_appointment_mfService.Queryable().Where(a => a.CompanyId == CompanyID && a.PatientId == patientid && a.DoctorId.ToString() == DocId).Select(z => new
                //{
                //    PatientId = z.PatientId,
                //    AppointmentDate = z.AppointmentDate,
                //}).ToList();

                //var CurntPrevDateList = CurntPrevDate.AsEnumerable().Select(z => new
                //{
                //    AppointmentDate = z.AppointmentDate.Value.ToString("yyyy-MM-dd"),
                //    PatientId = z.PatientId,
                //}).OrderByDescending(a => a.AppointmentDate).ToList();

                var AppointmentList = AllData.Tables[7];// AppointmentInfo(CompanyID);

                //AppointmentList = AppointmentList.AsEnumerable().Where(a => a.DoctorId.ToString() == DocId).ToList();

                // DataAccessManager dataAccessManager = new DataAccessManager();
                //var Cquery = "SELECT  distinct top 10  cmp.Id as id,case when dt.Complaint is null then cmp.Complaint else dt.Complaint end as Complaint,ft.id as favoriteId,(case when ft.ReferenceId is null then 0 else 1 end) as Isfavorite FROM emr_complaint cmp inner join emr_prescription_complaint dt on dt.ComplaintId = cmp.Id and dt.CompanyID='" + CompanyID + "' left outer join emr_notes_favorite ft on ft.ReferenceId = cmp.Id and ft.ReferenceType = 'C' and ft.DoctorId = '" + Doctorid + "' where ft.CompanyID='" + CompanyID + "'";
                var ComplaintList = AllData.Tables[2];//  dataAccessManager.GetDataTable(Cquery);

                //var Oquery = "SELECT  distinct top 10  ob.Id as id,case when dt.Observation is null then ob.Observation else dt.Observation end as Observation ,ft.id as favoriteId,(case when ft.ReferenceId is null then 0 else 1 end) as Isfavorite FROM emr_observation ob inner join emr_prescription_observation dt on dt.ObservationId = ob.Id and dt.CompanyID='" + CompanyID + "' left outer join emr_notes_favorite ft on ft.ReferenceId = ob.Id and ft.ReferenceType = 'O' and ft.DoctorId = '" + Doctorid + "' where ft.CompanyID='" + CompanyID + "'";
                var ObservationList = AllData.Tables[3]; //dataAccessManager.GetDataTable(Oquery);

                //var Iquery = "SELECT  distinct top 10  inv.Id as id,case when dt.Investigation is null then inv.Investigation else dt.Investigation end as Investigation ,ft.id as favoriteId,(case when ft.ReferenceId is null then 0 else 1 end) as Isfavorite FROM emr_investigation inv inner join emr_prescription_investigation dt on dt.InvestigationId = inv.Id and dt.CompanyID='" + CompanyID + "' left outer join emr_notes_favorite ft on ft.ReferenceId = inv.Id and ft.ReferenceType = 'I' and ft.DoctorId = '" + Doctorid + "' where ft.CompanyID='" + CompanyID + "'";
                var InvestigationsList = AllData.Tables[4]; //dataAccessManager.GetDataTable(Iquery);

                //var Dquery = "SELECT  distinct top 10  dia.Id as id,case when dt.Diagnos is null then dia.Diagnos else dt.Diagnos end as Diagnos ,ft.id as favoriteId,(case when ft.ReferenceId is null then 0 else 1 end) as Isfavorite FROM emr_diagnos dia inner join emr_prescription_diagnos dt on dt.DiagnosId = dia.Id and dt.CompanyID='" + CompanyID + "' left outer join emr_notes_favorite ft on ft.ReferenceId = dia.Id and ft.ReferenceType = 'D' and ft.DoctorId ='" + Doctorid + "' where ft.CompanyID='" + CompanyID + "'";
                var DiagnosisList = AllData.Tables[5]; //dataAccessManager.GetDataTable(Dquery);
                var IsBackDated = AllData.Tables[6].Rows[0]["IsBackDatedAppointment"];
                bool IsBackDatedAppointment = Convert.ToBoolean(IsBackDated); //_adm_companyService.Queryable().Where(a => a.ID == CompanyID).FirstOrDefault().IsBackDatedAppointment;
                objResponse.ResultSet = new
                {
                    patientInfo = patientInfo,
                    Doctorid = Doctorid,
                    prescription_mf = AllData.Tables[8],
                    prescription_treatment = AllData.Tables[9],
                    prescription_complaint = AllData.Tables[10],
                    prescription_diagnos = AllData.Tables[11],
                    prescription_investigation = AllData.Tables[12],
                    prescription_observation = AllData.Tables[13],
                    CurntPrevDateList = CurntPrevDate,
                    AppointmentList = AppointmentList,
                    ComplaintList = ComplaintList,
                    ObservationList = ObservationList,
                    InvestigationsList = InvestigationsList,
                    DiagnosisList = DiagnosisList,
                    IsBackDatedAppointment = IsBackDatedAppointment,
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
    }
}
