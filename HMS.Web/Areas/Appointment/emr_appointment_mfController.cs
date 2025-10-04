using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
using HMS.Service;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Admission;
using HMS.Service.Services.Appointment;
using HMS.Service.Services.Employee;
using HMS.Service.Services.Items;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using HMS.Web.API.Interface;
using NPOI.POIFS.Properties;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.IsisMtt.X509;
using Org.BouncyCastle.Cms;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Results;

namespace HMS.Web.API.Areas.Appointment.Controllers
{

    [JwtAuthentication]
    public class emr_appointment_mfController : ApiController, IERPAPIInterface<emr_appointment_mf>, IDisposable
    {
        private readonly Iemr_appointment_mfService _service;
        private readonly Iemr_medicineService _emr_medicineService;
        private readonly Iadm_user_companyService _adm_user_companyService;
        private readonly Iadm_role_mfService _adm_role_mfService;
        private readonly Iadm_user_tokenService _adm_user_tokenService;
        private readonly Isys_notification_alertService _sys_notification_alertService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iemr_patientService _emr_patientService;
        private readonly Iemr_complaintService _emr_complaintService;
        private readonly Iemr_diagnosService _emr_diagnosService;
        private readonly Iemr_investigationService _emr_investigationService;
        private readonly Iemr_observationService _emr_observationService;
        private readonly Iadm_userService _adm_userService;
        private readonly Iemr_patient_billService _emr_patient_billService;
        private readonly Iemr_prescription_mfService _emr_prescription_mfService;
        private readonly Iemr_prescription_complaintService _emr_prescription_complaintService;
        private readonly Iemr_prescription_diagnosService _emr_prescription_diagnosService;
        private readonly Iemr_prescription_investigationService _emr_prescription_investigationService;
        private readonly Iemr_prescription_observationService _emr_prescription_observationService;
        private readonly Iemr_service_mfService _emr_service_mfService;
        private readonly Iemr_notes_favoriteService _emr_notes_favoriteService;
        private readonly Iadm_companyService _adm_companyService;
        private readonly Iipd_diagnosisService _ipd_diagnosisService;
        private readonly Iipd_admissionService _ipd_admissionService;
        private readonly Iipd_admission_notesService _ipd_admission_notesService;
        private readonly Iipd_input_outputService _ipd_input_outputService;
        private readonly Iipd_medication_logService _ipd_medication_logService;
        private readonly Iipd_admission_vitalService _ipd_admission_vitalService;
        private readonly Iemr_documentService _emr_documentService;
        private readonly Iadm_reminder_mfService _adm_reminder_mf;
        private readonly Iadm_integrationService _adm_integrationService;
        private readonly Iadm_itemService _adm_itemService;
        private readonly ISmsService _smsService;
        private readonly Iemr_vitalService _emr_vitalService;
        private readonly Ipr_employee_mfService _pr_employee_mfService;

        public emr_appointment_mfController(IUnitOfWorkAsync unitOfWorkAsync, Iemr_service_mfService emr_service_mfService, Iemr_prescription_mfService emr_prescription_mfService, Iadm_userService adm_userService, Iemr_medicineService emr_medicineService, Iemr_patientService emr_patientService, Iemr_appointment_mfService service, Iadm_user_companyService adm_user_companyService,
            Iadm_role_mfService adm_role_mfService, Iadm_user_tokenService adm_user_tokenService, Isys_drop_down_valueService sys_drop_down_valueService,
            Isys_notification_alertService sys_notification_alertService, Iipd_diagnosisService ipd_diagnosisService, Iemr_patient_billService emr_patient_billService,
            Iemr_complaintService emr_complaintService,
            ISmsService smsService,
            Iemr_diagnosService emr_diagnosService,
            Iadm_companyService adm_companyService,
            Iemr_notes_favoriteService emr_notes_favoriteService,
            Iemr_investigationService emr_investigationService,
            Iemr_observationService emr_observationService,
            Iadm_reminder_mfService adm_reminder_mf,
            Iadm_integrationService adm_integrationService,
             Iemr_prescription_complaintService emr_prescription_complaintService,
            Iemr_prescription_diagnosService emr_prescription_diagnosService,
            Iemr_prescription_investigationService emr_prescription_investigationService,
            Iemr_prescription_observationService emr_prescription_observationService,
            Iipd_admissionService ipd_admissionService,
            Iipd_admission_notesService ipd_admission_notesService,
            Iipd_input_outputService ipd_input_outputService,
            Iipd_medication_logService ipd_medication_logService,
            Iipd_admission_vitalService ipd_admission_vitalService,
            Iemr_documentService emr_documentService,
            Iadm_itemService adm_itemService,
            Iemr_vitalService emr_vitalService,
            Ipr_employee_mfService pr_employee_mfService
            )
        {
            _ipd_diagnosisService = ipd_diagnosisService;
            _emr_service_mfService = emr_service_mfService;
            _emr_patient_billService = emr_patient_billService;
            _unitOfWorkAsync = unitOfWorkAsync;
            _smsService = smsService;
            _service = service;
            _adm_companyService = adm_companyService;
            _emr_notes_favoriteService = emr_notes_favoriteService;
            _emr_medicineService = emr_medicineService;
            _adm_userService = adm_userService;
            _emr_patientService = emr_patientService;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _adm_user_companyService = adm_user_companyService;
            _adm_role_mfService = adm_role_mfService;
            _adm_user_tokenService = adm_user_tokenService;
            _emr_complaintService = emr_complaintService;
            _emr_diagnosService = emr_diagnosService;
            _emr_investigationService = emr_investigationService;
            _emr_observationService = emr_observationService;
            _emr_prescription_mfService = emr_prescription_mfService;
            _sys_notification_alertService = sys_notification_alertService;
            _adm_reminder_mf = adm_reminder_mf;
            _adm_integrationService = adm_integrationService;
            _emr_prescription_complaintService = emr_prescription_complaintService;
            _emr_prescription_diagnosService = emr_prescription_diagnosService;
            _emr_prescription_investigationService = emr_prescription_investigationService;
            _emr_prescription_observationService = emr_prescription_observationService;
            _ipd_admissionService = ipd_admissionService;
            _ipd_admission_notesService = ipd_admission_notesService;
            _ipd_input_outputService = ipd_input_outputService;
            _ipd_medication_logService = ipd_medication_logService;
            _emr_documentService = emr_documentService;
            _adm_itemService = adm_itemService;
            _ipd_admission_vitalService = ipd_admission_vitalService;
            _emr_vitalService = emr_vitalService;
            _pr_employee_mfService = pr_employee_mfService;
        }

        [HttpPost]
        [HttpGet]
        [ActionName("Save")]
        public async Task<ResponseInfo> Save(emr_appointment_mf Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                decimal ID = 1;
                if (_service.Queryable().Count() > 0)
                    ID = _service.Queryable().Max(e => e.ID) + 1;
                Model.ID = ID;
                Model.CompanyId = Request.CompanyID();
                Model.CreatedBy = Request.LoginID();
                Model.CreatedDate = Request.DateTimes();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Added;
                _service.Insert(Model);
                if (Model.AdmissionId != null)
                {
                    decimal diagnosisID = 1;
                    if (_ipd_diagnosisService.Queryable().Count() > 0)
                        diagnosisID = _ipd_diagnosisService.Queryable().Max(e => e.ID) + 1;
                    if (Model.PrimaryDescription != null)
                    {
                        ipd_diagnosis obj = new ipd_diagnosis();
                        obj.ID = diagnosisID;
                        obj.CompanyID = CompanyID;
                        obj.Description = Model.PrimaryDescription;
                        obj.IsType = "P";
                        obj.Date = Model.AppointmentDate;
                        obj.AdmissionId = Convert.ToDecimal(Model.AdmissionId);
                        obj.IsVisitType = true;
                        obj.CreatedBy = Request.LoginID();
                        obj.CreatedDate = Request.DateTimes();
                        obj.ModifiedBy = Request.LoginID();
                        obj.ModifiedDate = Request.DateTimes();
                        obj.ObjectState = ObjectState.Added;
                        _ipd_diagnosisService.Insert(obj);
                        diagnosisID++;
                    }
                    if (Model.SecondaryDescription != null)
                    {
                        ipd_diagnosis obj = new ipd_diagnosis();
                        obj.ID = diagnosisID;
                        obj.CompanyID = CompanyID;
                        obj.Description = Model.SecondaryDescription;
                        obj.IsType = "S";
                        obj.Date = Model.AppointmentDate;
                        obj.AdmissionId = Convert.ToDecimal(Model.AdmissionId);
                        obj.IsVisitType = true;
                        obj.CreatedBy = Request.LoginID();
                        obj.CreatedDate = Request.DateTimes();
                        obj.ModifiedBy = Request.LoginID();
                        obj.ModifiedDate = Request.DateTimes();
                        obj.ObjectState = ObjectState.Added;
                        _ipd_diagnosisService.Insert(obj);
                    }
                }
                if (Model.ServiceId != 0)
                {
                    emr_patient_bill obj = new emr_patient_bill();
                    decimal BillID = 1;
                    if (_emr_patient_billService.Queryable().Count() > 0)
                        BillID = _emr_patient_billService.Queryable().Max(e => e.ID) + 1;
                    obj.ID = BillID;
                    obj.AppointmentId = Model.ID;
                    obj.DoctorId = Model.DoctorId;
                    obj.PatientId = Model.PatientId;
                    obj.ServiceId = Model.ServiceId;
                    obj.PaidAmount = Model.PaidAmount;
                    obj.OutstandingBalance = Model.OutstandingBalance;
                    obj.Price = Model.Price;
                    obj.Discount = Model.Discount;
                    obj.BillDate = Model.BillDate;
                    obj.Remarks = Model.Remarks;
                    obj.CompanyId = Request.CompanyID();
                    obj.CreatedBy = Request.LoginID();
                    obj.CreatedDate = Request.DateTimes();
                    obj.ModifiedBy = Request.LoginID();
                    obj.ModifiedDate = Request.DateTimes();
                    obj.ObjectState = ObjectState.Added;
                    _emr_patient_billService.Insert(obj);
                }
                emr_patient_mf patientObj = new emr_patient_mf();
                if (Model.PatientId != null)
                {
                    patientObj = _emr_patientService.Queryable().Where(a => a.ID == Model.PatientId && a.CompanyId == CompanyID).FirstOrDefault();
                    patientObj.ReminderId = Model.ReminderId;
                    _emr_patientService.Update(patientObj);
                }
                if (Model.IsExistFollowUp)
                {
                    var prescriptionObj = _emr_prescription_mfService.Queryable()
                        .Where(a => a.CompanyID == CompanyID
                        && a.PatientId == Model.PatientId
                       && DbFunctions.TruncateTime(a.FollowUpDate) >= DbFunctions.TruncateTime(DateTime.Now)
                        && a.FollowUpDate != null).OrderByDescending(a => a.FollowUpDate).FirstOrDefault();
                    if (prescriptionObj != null)
                    {
                        prescriptionObj.FollowUpDate = Model.AppointmentDate;
                        prescriptionObj.ModifiedDate = DateTime.Now;
                        _emr_prescription_mfService.Update(prescriptionObj);
                    }
                }
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Save;
                    objResponse.IsSuccess = true;
                    if (Model.ReminderId != null)
                    {
                        Task.Run(() => SendRemindersAsync(Model.ReminderId, patientObj.Mobile, patientObj.Email, CompanyID));
                    }
                    var CurntPrevDate = _service.Queryable().Where(a => a.CompanyId == CompanyID && a.PatientId == Model.PatientId).Select(z => new
                    {
                        AppId = z.ID,
                        AppointmentDate = z.AppointmentDate,
                        PatientId = z.PatientId
                    }).ToList();
                    var CurntPrevDateList = CurntPrevDate.AsEnumerable().Select(z => new
                    {
                        AppointmentDate = z.AppointmentDate.ToString("yyyy-MM-dd"),
                        ID = z.AppId,
                        PatientId = z.PatientId,
                    }).ToList();
                    objResponse.ResultSet = new
                    {
                        CurntPrevDateList = CurntPrevDateList,
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
        public async void SendRemindersAsync(decimal? ReminderId, string mobileno, string email, decimal CompanyID)
        {
            DataAccessManager dataAccessManager = new DataAccessManager();
            var ht = new Hashtable();
            ht.Add("@CompanyID", CompanyID);
            ht.Add("@ReminderId", ReminderId);
            var allData = dataAccessManager.GetDataSet("SP_GetReminderData", ht);
            var reminderTable = allData.Tables[0];
            var integrationTable = allData.Tables[1];
            bool isUnicode = false;
            if (reminderTable.Rows.Count > 0)
            {
                var smsRow = integrationTable.AsEnumerable()
           .FirstOrDefault(r => r["Type"].ToString() == "T" && Convert.ToBoolean(r["IsActive"]));
                var emailRow = integrationTable.AsEnumerable()
                    .FirstOrDefault(r => r["Type"].ToString() == "E" && Convert.ToBoolean(r["IsActive"]));
                foreach (DataRow row in reminderTable.Rows)
                {
                    int smsTypeId = Convert.ToInt32(row["SMSTypeId"]);
                    string messageBody = row["MessageBody"].ToString();
                    if (Convert.ToBoolean(row["IsUrdu"]))
                        isUnicode = true;
                    if (smsTypeId == 1 && smsRow != null)
                    {
                        await _smsService.AuthenticateAsync(
                             smsRow["UserName"].ToString(),
                             smsRow["Password"].ToString(),
                             mobileno,
                             messageBody,
                             smsRow["Masking"].ToString(),
                             isUnicode
                         );
                    }
                    if (emailRow != null && !string.IsNullOrEmpty(email))
                    {
                        await SendEmailNotify(
                            emailRow["EmailFrom"].ToString(),
                             emailRow["UserName"].ToString(),
                             emailRow["Password"].ToString(),
                             emailRow["SMTP"].ToString(),
                             Convert.ToInt32(emailRow["PortNo"]),
                             email,
                             messageBody
                         );
                    }
                }
            }
        }
        private async Task SendEmailNotify(string EmailFrom, string UserName, string EmailPassword, string EmailSMTP, int? EmailPort, string toemail, string message)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            MailMessage mail = new MailMessage();
            SmtpClient smtpC = new SmtpClient(EmailSMTP);
            smtpC.EnableSsl = true;
            smtpC.Port = Convert.ToInt32(EmailPort);
            if (string.IsNullOrEmpty(EmailFrom))
                EmailFrom = UserName;
            smtpC.Credentials = new System.Net.NetworkCredential(UserName, EmailPassword);
            mail.From = new MailAddress(EmailFrom);
            mail.To.Add(toemail);
            mail.Subject = "Appointment";
            mail.Body = message;
            try
            {
                //Send Email
                smtpC.Send(mail);
            }
            catch (Exception ex)
            {
            }
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
        public async Task<ResponseInfo> Update(emr_appointment_mf Model)
        {
            var objResponse = new ResponseInfo();
            try
            {

                decimal CompanyID = Request.CompanyID();
                Model.CompanyId = CompanyID;
                Model.CreatedDate = Request.DateTimes();
                Model.CreatedBy = Request.LoginID();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Modified;
                _service.Update(Model);
                if (Model.ServiceId != 0)
                {
                    var obj = _emr_patient_billService.Queryable().Where(a => a.CompanyId == CompanyID && a.AppointmentId == Model.ID && a.DoctorId == Model.DoctorId).FirstOrDefault();
                    obj.DoctorId = Model.DoctorId;
                    obj.PatientId = Model.PatientId;
                    obj.ServiceId = Model.ServiceId;
                    obj.PaidAmount = Model.PaidAmount;
                    obj.OutstandingBalance = Model.OutstandingBalance;
                    obj.Price = Model.Price;
                    obj.Discount = Model.Discount;
                    obj.BillDate = Model.BillDate;
                    obj.Remarks = Model.Remarks;
                    obj.CompanyId = Request.CompanyID();
                    obj.CreatedBy = Request.LoginID();
                    obj.CreatedDate = Request.DateTimes();
                    obj.ModifiedBy = Request.LoginID();
                    obj.ModifiedDate = Request.DateTimes();
                    obj.ObjectState = ObjectState.Modified;
                    _emr_patient_billService.Update(obj);
                }
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;
                    objResponse.IsSuccess = true;
                    var CurntPrevDate = _service.Queryable().Where(a => a.CompanyId == CompanyID && a.PatientId == Model.PatientId).Select(z => new
                    {
                        PatientId = z.PatientId,
                        AppointmentDate = z.AppointmentDate,
                    }).ToList();

                    var CurntPrevDateList = CurntPrevDate.AsEnumerable().Select(z => new
                    {
                        AppointmentDate = z.AppointmentDate.ToString("yyyy-MM-dd"),
                        PatientId = z.PatientId,
                    }).ToList();


                    objResponse.ResultSet = new
                    {
                        CurntPrevDateList = CurntPrevDateList,
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
        [HttpPut]
        [HttpGet]
        [ActionName("UpdateAppointment")]
        public async Task<ResponseInfo> UpdateAppointment(emr_appointment_mf Model)
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
                var findAppointment = _service.Queryable().Where(a => a.ID == Model.ID && a.CompanyId == CompanyID).FirstOrDefault();
                if (Model.StatusId != null)
                    findAppointment.StatusId = Model.StatusId;
                if (Model.AppointmentTime != null && Model.AppointmentTime != TimeSpan.Zero)
                    findAppointment.AppointmentTime = Model.AppointmentTime;

                if (Model.DoctorId != null)
                    findAppointment.DoctorId = Model.DoctorId;
                findAppointment.ModifiedBy = Request.LoginID();
                findAppointment.ModifiedDate = Request.DateTimes();
                findAppointment.ObjectState = ObjectState.Modified;
                findAppointment.adm_user_mf2 = null;
                _service.Update(findAppointment);
                var bill = _emr_patient_billService.Queryable().Where(a =>
                    a.AppointmentId == findAppointment.ID && a.PatientId == findAppointment.PatientId).FirstOrDefault();
                if (bill != null)
                {
                    bill.DoctorId = Model.DoctorId;
                    _emr_patient_billService.Update(bill);
                }
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;
                    objResponse.IsSuccess = true;


                    objResponse.ResultSet = new
                    {
                        //DoctorList = DoctorList,
                        //AppointmentList = LIST,
                        //AllStatusList = AllStatusList,
                        //DoctorCalander= DoctorCalander,
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
        public List<AppointmentInfo> AppointmentInfo(decimal CompanyID)
        {
            var AppointmentList = _service.Queryable().Where(a => a.CompanyId == CompanyID && DbFunctions.TruncateTime(a.AppointmentDate) == DbFunctions.TruncateTime(DateTime.Now))
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
                Color = m.StatusId == 1 ? "#dddddd" : m.StatusId == 2 ? "#fd8d64" : m.StatusId == 5 ? "#fdca66" : m.StatusId == 6 ? "#dd2626" : m.StatusId == 7 ? "#65d3fd" : m.StatusId == 8 ? "#6597ff" : m.StatusId == 9 ? "#0bb8ab" : m.StatusId == 10 ? "#fb64a7" : m.StatusId == 25 ? "#6265fd" : "",
                StartDate = m.AppointmentDate.ToString("yyyy-MM-dd") + "T" + m.AppointmentTime.ToString("hh\\:mm\\:ss"),
                EndDate = m.AppointmentDate.ToString("yyyy-MM-dd") + "T" + WorkingHours(Convert.ToDateTime(m.AppointmentDate + m.AppointmentTime), m.DoctorId),
            }).OrderBy(a => a.DoctorId).ToList();
            return LIST.ToList();
        }
        [HttpGet]
        [ActionName("PatientInfoLoad")]
        public async Task<ResponseInfo> PatientInfoLoad(string id, string Date, string AppId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal companyId = Request.CompanyID();
                decimal patientid = Convert.ToDecimal(id);
                int userid = Convert.ToInt32(Request.LoginID());
                DateTime date = Convert.ToDateTime(Date);
                decimal AppontId = Convert.ToDecimal(AppId);
                var doctorId = await _service.Queryable()
    .Where(a => a.CompanyId == companyId && a.ID == AppontId)
    .Select(a => a.DoctorId)
    .FirstOrDefaultAsync();
                var prescription = _emr_prescription_mfService.Queryable().Where(e => e.PatientId == patientid && e.CompanyID == companyId && e.DoctorId == doctorId && DbFunctions.TruncateTime(e.AppointmentDate) == DbFunctions.TruncateTime(date)).Include(a => a.emr_prescription_treatment_template).Include(a => a.emr_prescription_complaint).Include(a => a.emr_prescription_diagnos).Include(a => a.emr_prescription_investigation).Include(a => a.emr_prescription_observation).Include(a => a.emr_prescription_treatment).OrderByDescending(a => a.ID).FirstOrDefault();

                var reslt = await _service.Queryable()
     .Where(a => a.PatientId == patientid
              && a.CompanyId == companyId)
     .GroupBy(a => new { a.ID, a.PatientId, Date = DbFunctions.TruncateTime(a.AppointmentDate) }) // group by patient + date only
     .Select(g => new
     {
         g.Key.ID,
         g.Key.PatientId,
         AppointmentDate = g.Key.Date
     })
     .OrderByDescending(x => x.AppointmentDate)
     .ToListAsync();

                // if you really need string format "yyyy-MM-dd"
                var appointmentDates = reslt.Select(a => new
                {
                    a.ID,
                    a.PatientId,
                    AppointmentDate = a.AppointmentDate.Value.ToString("yyyy-MM-dd")
                }).ToList();
                var complaintList = await GetTopComplaintsAsync(companyId, doctorId, patientid);
                var observationList = await GetTopObservationsAsync(companyId, doctorId, patientid);
                var investigationList = await GetTopInvestigationsAsync(companyId, doctorId, patientid);
                var diagnosisList = await GetTopDiagnosisAsync(companyId, doctorId, patientid);
                var dropDownIds = new[] { 22, 23 };
                var allDropDownValues = await _sys_drop_down_valueService.Queryable()
    .Where(x => dropDownIds.Contains(x.DropDownID)
                && (x.CompanyID == companyId || x.CompanyID == null)).Select(z => new
                {
                    z.ID,
                    z.Value,
                    z.DropDownID,
                })
    .ToListAsync();
                var billTypeList = allDropDownValues
      .Where(x => x.DropDownID == 22).ToList();
                var titleList = allDropDownValues
     .Where(x => x.DropDownID == 23).ToList();
                var rawEventsQuery = await _service.Queryable()
   .Where(mf => mf.CompanyId == companyId
             && DbFunctions.TruncateTime(mf.AppointmentDate) == DbFunctions.TruncateTime(date)
             && mf.DoctorId == doctorId && mf.PatientId == patientid)
   .Select(mf => new
   {
       mf.ID,
       DoctorName = mf.Doctor.Name,
       PatientName = mf.emr_patient_mf.PatientName,
       mf.AppointmentDate,
       mf.AppointmentTime, // keep raw
       mf.DoctorId,
       mf.StatusId,
       mf.PatientId,
       CNIC = mf.emr_patient_mf.CNIC,
       mf.Notes,
       CreatedBy = mf.adm_user_mf.Name,
       SlotTime = mf.Doctor.SlotTime, // keep raw
       Color = mf.StatusId == 1 ? "#dddddd"
            : mf.StatusId == 2 ? "#fd8d64"
            : mf.StatusId == 5 ? "#fdca66"
            : mf.StatusId == 6 ? "#dd2626"
            : mf.StatusId == 7 ? "#65d3fd"
            : mf.StatusId == 8 ? "#6597ff"
            : mf.StatusId == 9 ? "#0bb8ab"
            : mf.StatusId == 10 ? "#fb64a7"
            : mf.StatusId == 25 ? "#6265fd"
            : ""
   }).ToListAsync();

                var pagedEvents = rawEventsQuery?.Select(mf => new
                {
                    mf.ID,
                    mf.DoctorName,
                    mf.PatientName,
                    mf.DoctorId,
                    mf.StatusId,
                    mf.CNIC,
                    mf.PatientId,
                    mf.CreatedBy,
                    mf.Notes,
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
                var patientinfo = _emr_patientService.Queryable().Where(a => a.ID == patientid && a.CompanyId == companyId).FirstOrDefault();

                var CurntPrevDateList = appointmentDates;
                var ComplaintList = complaintList;
                var ObservationList = observationList;
                var InvestigationsList = investigationList;
                var DiagnosisList = diagnosisList;
                var BillTypeList = billTypeList;
                var TittleList = titleList;
                var AppointmentList = pagedEvents;
                var patientInfo = patientinfo;
                var Doctorid = doctorId;
                var prescription_mf = prescription;
                var prescription_treatment = prescription?.emr_prescription_treatment ?? new List<emr_prescription_treatment>();
                var prescription_complaint = prescription?.emr_prescription_complaint ?? new List<emr_prescription_complaint>();
                var prescription_diagnos = prescription?.emr_prescription_diagnos ?? new List<emr_prescription_diagnos>();
                var prescription_investigation = prescription?.emr_prescription_investigation ?? new List<emr_prescription_investigation>();
                var prescription_observation = prescription?.emr_prescription_observation ?? new List<emr_prescription_observation>();

                var ids = prescription?.emr_prescription_treatment?.Select(a => a.MedicineId).ToList() ?? new List<decimal?>();
                var medicineList = _emr_medicineService.Queryable().Where(a => a.CompanyID == companyId && ids.Contains(a.ID)).Select(a => new { Id = a.ID, Medicine = a.Medicine });
                var itemList = _adm_itemService.Queryable().Where(a => a.CompanyId == companyId && ids.Contains(a.ID)).Select(a => new { Id = a.ID, Medicine = a.Name });
                var MedicineList = medicineList.Union(itemList).ToList();
                var vitalObj = new List<object>();
                if (prescription != null)
                {
                    vitalObj = _emr_vitalService.Queryable()
                        .Where(a => a.CompanyID == companyId
                            && a.PatientId == patientid
                            && DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(prescription.AppointmentDate))
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

                objResponse.ResultSet = new
                {
                    patientInfo = patientinfo,
                    Doctorid = doctorId,
                    prescription_mf = prescription_mf,
                    prescription_treatment = prescription_treatment,
                    prescription_complaint = prescription_complaint,
                    prescription_diagnos = prescription_diagnos,
                    prescription_investigation = prescription_investigation,
                    prescription_observation = prescription_observation,
                    CurntPrevDateList = CurntPrevDateList,
                    ComplaintList = ComplaintList,
                    ObservationList = ObservationList,
                    InvestigationsList = InvestigationsList,
                    DiagnosisList = DiagnosisList,
                    AppointmentList = AppointmentList,
                    BillTypeList = BillTypeList,
                    TittleList = TittleList,
                    MedicineList = MedicineList,
                    vitallist = vitalObj,
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
        [ActionName("PatientLoadById")]
        public ResponseInfo PatientLoadById(string id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal companyId = Request.CompanyID();
                decimal patientid = Convert.ToDecimal(id);
                int userid = Convert.ToInt32(Request.LoginID());
                DateTime todayDate = DateTime.Now;
                int? stid = Convert.ToInt32(0);
                var patientinfo = _emr_patientService.Queryable().Where(a => a.ID == patientid && a.CompanyId == companyId).FirstOrDefault();
                var dropDownIds = new[] { 2, 17, 22, 23 };
                var allDropDownValues = _sys_drop_down_valueService.Queryable()
    .Where(x => dropDownIds.Contains(x.DropDownID)
                && (x.CompanyID == companyId || x.CompanyID == null)).Select(z => new
                {
                    z.ID,
                    z.Value,
                    z.DropDownID,
                })
    .ToList();
                var genderList = allDropDownValues
    .Where(d => d.DropDownID == 2)
    .Select(d => new { d.ID, d.Value })
    .ToList();

                var bloodList = allDropDownValues
    .Where(d => d.DropDownID == 17)
    .ToList();
                var billTypeList = allDropDownValues
   .Where(d => d.DropDownID == 22)
   .ToList();
                var tittleList = allDropDownValues
   .Where(d => d.DropDownID == 23)
   .ToList();
                var paidAndOutAmount = _emr_prescription_mfService.Queryable()
    .Where(p => p.PatientId == patientid && p.CompanyID == companyId)
    .SelectMany(p => p.emr_patient_mf.emr_patient_bill)
    .Sum(b => (decimal?)b.OutstandingBalance) ?? 0;
                var prescriptionresult = _emr_prescription_mfService.Queryable().Where(e => e.PatientId == patientid && e.CompanyID == companyId).Include(a => a.emr_prescription_complaint).Include(a => a.emr_prescription_diagnos).Include(a => a.emr_prescription_investigation).Include(a => a.emr_prescription_observation).Include(a => a.emr_prescription_treatment)
                    .Select(z => new
                    {
                        z.ID,
                        z.AppointmentDate,
                        z.emr_prescription_complaint,
                        z.emr_prescription_diagnos,
                        z.emr_prescription_investigation,
                        z.emr_prescription_observation,
                        z.emr_prescription_treatment,
                        PaidAmount = z.emr_patient_mf.emr_patient_bill.Where(c => c.PatientId == z.emr_patient_mf.ID && (decimal?)c.DoctorId == (decimal?)z.DoctorId).Sum(c => (decimal?)c.PaidAmount),
                        OutAmount = z.emr_patient_mf.emr_patient_bill.Where(c => c.PatientId == z.emr_patient_mf.ID && (decimal?)c.DoctorId == (decimal?)z.DoctorId).Sum(c => (decimal?)c.OutstandingBalance),
                    }).OrderByDescending(a => a.AppointmentDate).ToList();
                var ids = prescriptionresult.SelectMany(p => p.emr_prescription_treatment).Select(t => t.MedicineId).Distinct().ToList();
                var medicineList = _emr_medicineService.Queryable().Where(a => a.CompanyID == companyId && ids.Contains(a.ID)).Select(a => new { Id = a.ID, Medicine = a.Medicine, Type = a.sys_drop_down_value1.Value });
                var itemList = _adm_itemService.Queryable().Where(a => a.CompanyId == companyId && ids.Contains(a.ID)).Select(a => new { Id = a.ID, Medicine = a.Name, Type = a.sys_drop_down_value1.Value });
                var MedicineList = itemList.Union(medicineList).ToList();

                var ipdAdmissions = _ipd_admissionService.Queryable()
                                 .Where(a => a.PatientId == patientid && a.CompanyId == companyId)
                                 .Select(a => new
                                 {
                                     AdmissionNo = a.AdmissionNo,
                                     DischargeDate = a.DischargeDate,
                                     DischargeTime = a.DischargeTime
                                 }).FirstOrDefault();
                objResponse.ResultSet = new
                {
                    patientInfo = patientinfo,
                    GenderList = genderList,
                    PaidAndOutamount = paidAndOutAmount,
                    BillTypeList = billTypeList,
                    TittleList = tittleList,
                    BloodList = bloodList,
                    MedicineList = MedicineList,
                    prescriptionresult = prescriptionresult,
                    AdmissionNo = ipdAdmissions == null ? 0 : Convert.ToInt32(ipdAdmissions.AdmissionNo),
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
        [ActionName("AdmitPatientLoadById")]
        public async Task<ResponseInfo> AdmitPatientLoadById(string id, string AdmitId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal companyId = Request.CompanyID();
                decimal patientid = Convert.ToDecimal(id);
                decimal AdmissionId = Convert.ToDecimal(AdmitId);
                int userid = Convert.ToInt32(Request.LoginID());
                DateTime todayDate = DateTime.Now;
                int? stid = Convert.ToInt32(0);
                var patientinfo = await _emr_patientService.Queryable().Where(a => a.ID == patientid && a.CompanyId == companyId).FirstOrDefaultAsync();
                var dropDownIds = new[] { 2, 17, 22, 23 };
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
    .Where(d => d.DropDownID == 2)
    .Select(d => new { d.ID, d.Value })
    .ToList();
                var medicines = _emr_medicineService.Queryable()
                    .Where(m => m.CompanyID == companyId)
                    .Select(m => new { Id = m.ID, Medicine = m.Medicine })
                    .ToList();
                var bloodList = allDropDownValues
    .Where(d => d.DropDownID == 17)
    .ToList();
                var billTypeList = allDropDownValues
   .Where(d => d.DropDownID == 22)
   .ToList();
                var tittleList = allDropDownValues
   .Where(d => d.DropDownID == 23)
   .ToList();
                var paidAndOutAmount = _emr_prescription_mfService.Queryable()
    .Where(p => p.PatientId == patientid && p.CompanyID == companyId)
    .SelectMany(p => p.emr_patient_mf.emr_patient_bill)
    .Sum(b => (decimal?)b.OutstandingBalance) ?? 0;
                var ipdAdmissions = _ipd_admissionService.Queryable()
                              .Where(a => a.PatientId == patientid && a.CompanyId == companyId
                                    && a.ID == AdmissionId)
                              .Select(a => new
                              {
                                  AdmissionNo = a.AdmissionNo,
                                  DischargeDate = a.DischargeDate,
                                  DischargeTime = a.DischargeTime
                              }).FirstOrDefault();
                var ipddiagnosis = _ipd_diagnosisService.Queryable().Where(a => a.CompanyID == companyId && a.AdmissionId == AdmissionId).ToList();
                var notes = _ipd_admission_notesService.Queryable().Where(n => n.CompanyId == companyId
            && n.PatientId == patientid
            && n.AdmissionId == AdmissionId).AsEnumerable()
   .Select(n => new
   {
       CreatedDate = n.CreatedDate.ToString("yyyy-MM-dd hh:mm tt"),
       n.Note,
       n.OnBehalfOf
   })
   .ToList();

                var inputOutput = _ipd_input_outputService.Queryable()
    .Where(io => io.CompanyId == companyId
              && io.PatientId == patientid
              && io.AdmissionId == AdmissionId)
    .Select(io => new
    {
        Medicine = io.IntakeValue ?? io.Output,
        io.Time,
        io.Value,
        io.Type
    })
    .OrderBy(io => io.Medicine)
    .ThenBy(io => io.Time)
    .ThenBy(io => io.Type)
    .ToList();

                var medicationLog = _ipd_medication_logService.Queryable()
    .Where(md => md.CompanyId == companyId
              && md.PatientId == patientid
              && md.AdmissionId == AdmissionId)
    .Select(md => new
    {
        md.Time,
        Value = md.Dose,
        md.Date,
        Medicine = md.adm_item.Name,
        md.RouteDropdownId,
        md.RouteId
    })
    .ToList();
                var routeDropdownIds = medicationLog.Select(m => m.RouteDropdownId).Distinct().ToList();
                var routeIds = medicationLog.Select(m => m.RouteId).Distinct().ToList();
                var sysValues = _sys_drop_down_valueService.Queryable()
    .Where(sys => routeDropdownIds.Contains(sys.DropDownID)
               && routeIds.Contains(sys.ID))
    .ToList();


                var medicationLogList = medicationLog
    .Select(md =>
    {
        var sys = sysValues.FirstOrDefault(s => s.DropDownID == md.RouteDropdownId
                                             && s.ID == md.RouteId);

        return new
        {
            Medicine = md.Medicine,
            RouteVal = sys?.Value,
            md.Time,
            Value = md.Value,
            md.Date
        };
    })
    .OrderBy(x => x.Medicine)
    .ThenBy(x => x.RouteVal)
    .ThenBy(x => x.Time)
    .ThenBy(x => x.Value)
    .ToList();
                var documentsList = _emr_documentService.Queryable()
    .Where(doc => doc.DocumentTypeId == 4
               && doc.CompanyID == companyId
               && doc.ReferenceNo == AdmissionId.ToString())
    .Select(a => new
    {
        DocumentUpload = a.DocumentUpload,
        Date = a.Date,
        Uploaddate = a.CreatedDate,
        Remarks = a.Remarks,
        Tag = a.sys_drop_down_value.Value
    })
    .ToList();

                var vitals = _ipd_admission_vitalService.Queryable()
    .Where(v => v.CompanyId == companyId
             && v.PatientId == patientid
             && v.AdmissionId == AdmissionId)
    .ToList();
                var fever = vitals.Select(v => new object[]
{
    v.DateRecorded.ToString("yyyy-MM-dd") + "T" +
    (DateTime.TryParse(v.TimeRecorded, out var t) ? t.ToString("HH:mm:ss") : "00:00:00"),
    v.Temperature
}).ToList();

                var bp = vitals.Select(v => new object[]
                {
    v.DateRecorded.ToString("yyyy-MM-dd") + "T" +
    (DateTime.TryParse(v.TimeRecorded, out var t) ? t.ToString("HH:mm:ss") : "00:00:00"),
    v.BP ?? "0"
                }).ToList();

                var heartRate = vitals.Select(v => new object[]
                {
    v.DateRecorded.ToString("yyyy-MM-dd") + "T" +
    (DateTime.TryParse(v.TimeRecorded, out var t) ? t.ToString("HH:mm:ss") : "00:00:00"),
    v.HeartRate ?? "0"
                }).ToList();

                var SPO2 = vitals.Select(v => new object[]
                {
    v.DateRecorded.ToString("yyyy-MM-dd") + "T" +
    (DateTime.TryParse(v.TimeRecorded, out var t) ? t.ToString("HH:mm:ss") : "00:00:00"),
    v.SPO2 ?? "0"
                }).ToList();

                // Step 3: Build final ChartData object
                var chartData = new
                {
                    fever,
                    bp,
                    heartRate,
                    SPO2
                };

                // Step 4: Serialize to JSON (if needed)
                var json = JsonSerializer.Serialize(chartData);
                var patientInfo = patientinfo;
                var GenderType = genderList;
                var MedicineList = medicines;
                var BloodList = bloodList;
                var BillTypeList = billTypeList;
                var TittleList = tittleList;
                var NotesList = notes;
                var ChartData = json;
                var InputOutputList = inputOutput;
                var MedicationLogList = medicationLogList;
                var DocumentList = documentsList;
                var PaidAndOutamount = paidAndOutAmount;
                var AdmissionNo = ipdAdmissions.AdmissionNo;
                var DischargeDate = ipdAdmissions.DischargeDate;
                var Diagnosislist = ipddiagnosis;
                objResponse.ResultSet = new
                {
                    patientInfo = patientInfo,
                    GenderList = GenderType,
                    PaidAndOutamount = PaidAndOutamount,
                    BillTypeList = BillTypeList,
                    TittleList = TittleList,
                    BloodList = BloodList,
                    MedicineList = MedicineList,
                    AdmissionNo = AdmissionNo,
                    DischargeDate = DischargeDate,
                    Diagnosislist = Diagnosislist,
                    NotesList = NotesList,
                    ChartData = ChartData,
                    InputOutputList = InputOutputList,
                    MedicationLogList = MedicationLogList,
                    DocumentList = DocumentList,
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
        [ActionName("PatientAppointmentLoad")]
        public ResponseInfo PatientAppointmentLoad(string id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal patientid = Convert.ToDecimal(id);
                int userid = Convert.ToInt32(Request.LoginID());
                var AppointmentList = _service.Queryable().Where(a => a.CompanyId == CompanyID && a.PatientId == patientid)
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
                    PatientId = m.PatientId,
                    CreatedBy = m.CreatedBy,
                    AppointmentDate = m.AppointmentDate,
                    Status = _sys_drop_down_valueService.Queryable().Where(a => a.DropDownID == 1 && a.ID == m.StatusId).FirstOrDefault().Value,
                    StartDate = m.AppointmentDate.ToString("yyyy-MM-dd") + " " + m.AppointmentTime.ToString("hh\\:mm\\:ss"),
                    EndDate = m.AppointmentDate.ToString(),//.Value.ToString("yyyy-MM-dd") + "T" + WorkingHours(Convert.ToDateTime(m.AppointmentDate + m.AppointmentTime), m.DoctorId),
                }).OrderBy(a => a.DoctorId).ToList();

                var FuturePatientList = LIST.Where(x => x.AppointmentDate.Value.Date >= DateTime.Now.Date).ToList();
                var PreviousPatientList = LIST.Where(x => x.AppointmentDate.Value.Date <= DateTime.Now.Date).ToList();


                objResponse.ResultSet = new
                {
                    FuturePatientList = FuturePatientList,
                    PreviousPatientList = PreviousPatientList,
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
        [ActionName("PatientInfo")]
        public async Task<ResponseInfo> PatientInfo(emr_appointment_mf Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var PatientInfo = _service.Queryable().Where(e => e.CompanyId == CompanyID).FirstOrDefault();
                objResponse.ResultSet = new
                {
                    PatientInfo = PatientInfo
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
        [ActionName("GetList")]
        public ResponseInfo GetList()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var GenderType = _sys_drop_down_valueService.Queryable().Where(e => e.DropDownID == 1).ToList();
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
        public ResponseInfo Load()
        {
            throw new NotImplementedException();
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
                var CompanyID = Request.CompanyID();
                //objResponse.ResultSet = _service.Queryable().Where(e => e.ID.ToString() == Id).Include(x=>x.adm_user_company.Select(s=> s.adm_role_mf)).FirstOrDefault();
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
        [ActionName("AppointList")]
        public async Task<PaginationResult> AppointListAsync(string date, string StatusId, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                int statusId = Convert.ToInt32(StatusId);
                int userId = Convert.ToInt32(Request.LoginID());
                DateTime todayDate = Convert.ToDateTime(date);
                int stId = (statusId == 0) ? 1 : statusId;
                if (SearchText == null)
                    SearchText = "";
                int? appointmentStatusId = await _adm_userService.Queryable()
.Where(u => u.ID == userId)
.Select(u => (int?)u.AppointmentStatusId)
.FirstOrDefaultAsync();
                int finalAppointmentStatusId = (appointmentStatusId == null || appointmentStatusId == 0) ? 1 : appointmentStatusId.Value;
                //Roleid
                var roleId = _adm_user_companyService.Queryable().Where(u => u.UserID == userId)
                             .Select(u => u.RoleID).FirstOrDefault();
                // Step 2: Get RoleName from adm_role_mf
                var roleName = _adm_role_mfService.Queryable().Where(r => r.ID == roleId)
                    .Select(r => r.RoleName).FirstOrDefault();
                decimal? doctorFilterId = null;
                if (string.Equals(roleName, "Doctor", StringComparison.OrdinalIgnoreCase))
                {
                    doctorFilterId = Convert.ToDecimal(userId);
                }
                var patientNewCounts = _emr_patientService.Queryable()
    .Where(p => p.CompanyId == CompanyID)
    .Select(p => new
    {
        PatientId = p.ID,
        NewPatientCount = p.emr_appointment_mf
            .Where(a => a.CompanyId == p.CompanyId && DbFunctions.TruncateTime(a.AppointmentDate) <= DbFunctions.TruncateTime(todayDate))
            .Count()
    });


                var rawEventsQuery = _service.Queryable()
     .Where(mf => mf.CompanyId == CompanyID
               && DbFunctions.TruncateTime(mf.AppointmentDate) == DbFunctions.TruncateTime(todayDate)
               && (finalAppointmentStatusId == 1 || mf.StatusId == finalAppointmentStatusId)
               && (doctorFilterId == null || mf.DoctorId == doctorFilterId)
               && (string.IsNullOrEmpty(SearchText) || mf.emr_patient_mf.PatientName.Contains(SearchText)))
     .Select(mf => new
     {
         mf.ID,
         DoctorName = mf.Doctor.Name,
         PatientName = mf.emr_patient_mf.PatientName,

         NewPatients = patientNewCounts
             .Where(nc => nc.PatientId == mf.PatientId)
             .Select(nc => nc.NewPatientCount)
             .FirstOrDefault(),

         mf.AppointmentDate,
         mf.AppointmentTime, // keep raw
         SlotTime = mf.Doctor.SlotTime, // keep raw

         mf.PatientProblem,
         ServiceId = mf.emr_patient_mf.emr_patient_bill.Where(a => a.AppointmentId == mf.ID && a.PatientId == mf.PatientId && a.DoctorId == mf.DoctorId).Select(b => b.ServiceId).FirstOrDefault(),
         PaidAmount = mf.emr_patient_mf.emr_patient_bill.Where(a => a.AppointmentId == mf.ID && a.PatientId == mf.PatientId && a.DoctorId == mf.DoctorId).Select(b => (decimal?)b.PaidAmount).FirstOrDefault(),
         Discount = mf.emr_patient_mf.emr_patient_bill.Where(a => a.AppointmentId == mf.ID && a.PatientId == mf.PatientId && a.DoctorId == mf.DoctorId).Select(b => (decimal?)b.Discount).FirstOrDefault(),
         BillDate = mf.emr_patient_mf.emr_patient_bill.Where(a => a.AppointmentId == mf.ID && a.PatientId == mf.PatientId && a.DoctorId == mf.DoctorId).Select(b => (DateTime?)b.BillDate).FirstOrDefault(),
         Price = mf.emr_patient_mf.emr_patient_bill.Where(a => a.AppointmentId == mf.ID && a.PatientId == mf.PatientId && a.DoctorId == mf.DoctorId).Select(b => (decimal?)b.Price).FirstOrDefault(),
         ServiceName = mf.emr_patient_mf.emr_patient_bill.Where(a => a.AppointmentId == mf.ID && a.PatientId == mf.PatientId && a.DoctorId == mf.DoctorId)
                         .Select(b => b.emr_service_mf.ServiceName)
                         .FirstOrDefault(),

         mf.TokenNo,
         mf.DoctorId,
         MRNO = mf.emr_patient_mf.MRNO,
         mf.StatusId,
         CNIC = mf.emr_patient_mf.CNIC,
         mf.PatientId,
         CreatedBy = mf.adm_user_mf.Name,
         Gender = mf.emr_patient_mf.Gender,
         Image = mf.emr_patient_mf.Image,
         mf.Notes,
         mf.ReminderId,

         Color = mf.StatusId == 1 ? "#dddddd"
              : mf.StatusId == 2 ? "#fd8d64"
              : mf.StatusId == 5 ? "#fdca66"
              : mf.StatusId == 6 ? "#dd2626"
              : mf.StatusId == 7 ? "#65d3fd"
              : mf.StatusId == 8 ? "#6597ff"
              : mf.StatusId == 9 ? "#0bb8ab"
              : mf.StatusId == 10 ? "#fb64a7"
              : mf.StatusId == 25 ? "#6265fd"
              : ""
     });

                int totalRecords = await rawEventsQuery.CountAsync();

                var pagedEventsRaw = await rawEventsQuery
                    .OrderBy(e => e.ID)
                    .Skip((CurrentPageNo - 1) * RecordPerPage)
                    .Take(RecordPerPage)
                    .ToListAsync();

                // format after ToListAsync
                var pagedEvents = pagedEventsRaw.Select(mf => new
                {
                    mf.ID,
                    mf.DoctorName,
                    mf.PatientName,
                    mf.NewPatients,
                    mf.PatientProblem,
                    mf.ServiceId,
                    mf.PaidAmount,
                    mf.Discount,
                    mf.BillDate,
                    mf.Price,
                    mf.ServiceName,
                    mf.TokenNo,
                    mf.DoctorId,
                    mf.MRNO,
                    mf.StatusId,
                    mf.CNIC,
                    mf.PatientId,
                    mf.CreatedBy,
                    mf.Gender,
                    mf.Image,
                    mf.Notes,
                    mf.ReminderId,
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
                });

                var dropDownIds = new[] { 2, 1, 17 };
                var allDropDownValues = await _sys_drop_down_valueService.Queryable()
    .Where(x => dropDownIds.Contains(x.DropDownID)
                && (x.CompanyID == CompanyID || x.CompanyID == null)).Select(z => new
                {
                    z.ID,
                    z.Value,
                    z.DropDownID,
                })
    .ToListAsync();

                var genderList = allDropDownValues
    .Where(d => d.DropDownID == 2)
    .Select(d => new { d.ID, d.Value })
    .ToList();

                var dropdownValues = allDropDownValues
    .Where(val => val.DropDownID == 1)
    .ToList();

                var appointmentStatusCounts = dropdownValues.Select(val => new
                {
                    val.ID,
                    val.Value,
                    IsActive = val.ID == finalAppointmentStatusId,

                    Count = _service.Queryable()
         .Where(mf => mf.CompanyId == CompanyID
                   && DbFunctions.TruncateTime(mf.AppointmentDate) == DbFunctions.TruncateTime(todayDate)
                   && (
                        (val.ID == 1 &&
                             ((roleName == "Doctor" && mf.DoctorId == userId)
                              || roleName != "Doctor"))
                        ||
                        (val.ID != 1 && mf.StatusId == val.ID &&
                             ((roleName == "Doctor" && mf.DoctorId == userId)
                              || roleName != "Doctor"))
                       )
               )
         .Count()
                }).ToList();

                var isShowDoctorId = await _adm_userService.Queryable()
                .Where(u => u.ID == userId)
                .Select(u => u.IsShowDoctor)
                .FirstOrDefaultAsync();
                //doctor list




                var companyInfo = await _adm_user_companyService.Queryable()
    .Where(uc => uc.UserID == userId)
    .Select(uc => new
    {
        uc.CompanyID,
        CompanyName = uc.adm_company.CompanyName // navigation property
    })
    .ToListAsync();
                var bloodList = allDropDownValues
    .Where(d => d.DropDownID == 17)
    .ToList();

                var consultationService = await _emr_service_mfService.Queryable()
    .Where(s => s.CompanyId == CompanyID && s.ServiceName == "Consultation")
    .Select(s => new { s.ID, s.ServiceName, s.Price })
    .ToListAsync();
                var maxCompanyToken = await _adm_companyService.Queryable()
    .Where(c => c.ID == CompanyID)
    .Select(c => (int?)c.TokenNo) // nullable to handle empty table
    .MaxAsync() ?? 0;
                var nextToken = await _service.Queryable()
    .Where(mf => mf.CompanyId == CompanyID
              && DbFunctions.TruncateTime(mf.AppointmentDate) == DbFunctions.TruncateTime(DateTime.Now))
    .Select(mf => (int?)mf.TokenNo)
    .MaxAsync() ?? maxCompanyToken;

                nextToken += 1;

                var isBackDatedAppointment = await _adm_companyService.Queryable()
    .Where(c => c.ID == CompanyID)
    .Select(c => c.IsBackDatedAppointment)
    .FirstOrDefaultAsync();

                var reminders = await _adm_reminder_mf.Queryable()
    .Where(r => r.CompanyId == CompanyID)
    .Select(r => new { r.ID, Value = r.Name })
    .ToListAsync();


                var followUps = await _emr_prescription_mfService.Queryable()
    .Where(mf => mf.CompanyID == CompanyID
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
                var ht = new Hashtable();
                ht.Add("@CompanyId", CompanyID);
                ht.Add("@UserId", userId);
                DataAccessManager dataAccessManager = new DataAccessManager();
                var doctorList = dataAccessManager.GetDataSet("SP_GetDoctorList", ht);
                objResult.GenderList = genderList;
                objResult.DoctorList = doctorList.Tables[0];
                objResult.AppointmentList = pagedEvents;
                objResult.AllStatusList = appointmentStatusCounts;
                objResult.DoctorCalander = doctorList.Tables[0];
                objResult.IsShowDoctorIds = isShowDoctorId;
                objResult.ClinicList = companyInfo;
                objResult.BloodList = bloodList;
                objResult.ServiceType = consultationService;
                objResult.TokenNo = nextToken.ToString();
                objResult.IsBackDatedAppointment = isBackDatedAppointment;
                objResult.TotalRecord = totalRecords;
                objResult.Reminderlist = reminders;
                objResult.Followuplist = followUpsResult;

            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }
        [HttpGet]
        [ActionName("UpdateAppointList")]
        public async Task<PaginationResult> UpdateAppointList(string date, string StatusId, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                int? stid = Convert.ToInt32(StatusId);
                int userId = Convert.ToInt32(Request.LoginID());
                DateTime todayDate = Convert.ToDateTime(date);
                string TotalRecord = "0";
                if (SearchText == null)
                    SearchText = "";
                var userobj = _adm_userService.Queryable().Where(a => a.ID == userId).FirstOrDefault();
                userobj.AppointmentStatusId = stid;
                userobj.ObjectState = ObjectState.Modified;
                _adm_userService.Update(userobj);
                _unitOfWorkAsync.SaveChanges();
                int? finalAppointmentStatusId = (stid == 0) ? 1 : stid;
                //Roleid
                var roleId = _adm_user_companyService.Queryable().Where(u => u.UserID == userId)
                             .Select(u => u.RoleID).FirstOrDefault();
                // Step 2: Get RoleName from adm_role_mf
                var roleName = _adm_role_mfService.Queryable().Where(r => r.ID == roleId)
                    .Select(r => r.RoleName).FirstOrDefault();
                decimal? doctorFilterId = null;
                if (string.Equals(roleName, "Doctor", StringComparison.OrdinalIgnoreCase))
                {
                    doctorFilterId = Convert.ToDecimal(userId);
                }
                var patientNewCounts = _emr_patientService.Queryable()
    .Where(p => p.CompanyId == CompanyID)
    .Select(p => new
    {
        PatientId = p.ID,
        NewPatientCount = p.emr_appointment_mf
            .Where(a => a.CompanyId == p.CompanyId && DbFunctions.TruncateTime(a.AppointmentDate) <= DbFunctions.TruncateTime(todayDate))
            .Count()
    });


                var rawEventsQuery = _service.Queryable()
     .Where(mf => mf.CompanyId == CompanyID
               && DbFunctions.TruncateTime(mf.AppointmentDate) == DbFunctions.TruncateTime(todayDate)
               && (finalAppointmentStatusId == 1 || mf.StatusId == finalAppointmentStatusId)
               && (doctorFilterId == null || mf.DoctorId == doctorFilterId)
               && (string.IsNullOrEmpty(SearchText) || mf.emr_patient_mf.PatientName.Contains(SearchText)))
     .Select(mf => new
     {
         mf.ID,
         DoctorName = mf.Doctor.Name,
         PatientName = mf.emr_patient_mf.PatientName,

         NewPatients = patientNewCounts
             .Where(nc => nc.PatientId == mf.PatientId)
             .Select(nc => nc.NewPatientCount)
             .FirstOrDefault(),

         mf.AppointmentDate,
         mf.AppointmentTime, // keep raw
         SlotTime = mf.Doctor.SlotTime, // keep raw

         mf.PatientProblem,
         ServiceId = mf.emr_patient_mf.emr_patient_bill.Where(a => a.AppointmentId == mf.ID && a.PatientId == mf.PatientId && a.DoctorId == mf.DoctorId).Select(b => b.ServiceId).FirstOrDefault(),
         PaidAmount = mf.emr_patient_mf.emr_patient_bill.Where(a => a.AppointmentId == mf.ID && a.PatientId == mf.PatientId && a.DoctorId == mf.DoctorId).Select(b => (decimal?)b.PaidAmount).FirstOrDefault(),
         Discount = mf.emr_patient_mf.emr_patient_bill.Where(a => a.AppointmentId == mf.ID && a.PatientId == mf.PatientId && a.DoctorId == mf.DoctorId).Select(b => (decimal?)b.Discount).FirstOrDefault(),
         BillDate = mf.emr_patient_mf.emr_patient_bill.Where(a => a.AppointmentId == mf.ID && a.PatientId == mf.PatientId && a.DoctorId == mf.DoctorId).Select(b => (DateTime?)b.BillDate).FirstOrDefault(),
         Price = mf.emr_patient_mf.emr_patient_bill.Where(a => a.AppointmentId == mf.ID && a.PatientId == mf.PatientId && a.DoctorId == mf.DoctorId).Select(b => (decimal?)b.Price).FirstOrDefault(),
         ServiceName = mf.emr_patient_mf.emr_patient_bill.Where(a => a.AppointmentId == mf.ID && a.PatientId == mf.PatientId && a.DoctorId == mf.DoctorId)
                         .Select(b => b.emr_service_mf.ServiceName)
                         .FirstOrDefault(),

         mf.TokenNo,
         mf.DoctorId,
         MRNO = mf.emr_patient_mf.MRNO,
         mf.StatusId,
         CNIC = mf.emr_patient_mf.CNIC,
         mf.PatientId,
         CreatedBy = mf.adm_user_mf.Name,
         Gender = mf.emr_patient_mf.Gender,
         Image = mf.emr_patient_mf.Image,
         mf.Notes,
         mf.ReminderId,

         Color = mf.StatusId == 1 ? "#dddddd"
              : mf.StatusId == 2 ? "#fd8d64"
              : mf.StatusId == 5 ? "#fdca66"
              : mf.StatusId == 6 ? "#dd2626"
              : mf.StatusId == 7 ? "#65d3fd"
              : mf.StatusId == 8 ? "#6597ff"
              : mf.StatusId == 9 ? "#0bb8ab"
              : mf.StatusId == 10 ? "#fb64a7"
              : mf.StatusId == 25 ? "#6265fd"
              : ""
     });

                int totalRecords = await rawEventsQuery.CountAsync();

                var pagedEventsRaw = await rawEventsQuery
                    .OrderBy(e => e.ID)
                    .Skip((CurrentPageNo - 1) * RecordPerPage)
                    .Take(RecordPerPage)
                    .ToListAsync();

                // format after ToListAsync
                var pagedEvents = pagedEventsRaw.Select(mf => new
                {
                    mf.ID,
                    mf.DoctorName,
                    mf.PatientName,
                    mf.NewPatients,
                    mf.PatientProblem,
                    mf.ServiceId,
                    mf.PaidAmount,
                    mf.Discount,
                    mf.BillDate,
                    mf.Price,
                    mf.ServiceName,
                    mf.TokenNo,
                    mf.DoctorId,
                    mf.MRNO,
                    mf.StatusId,
                    mf.CNIC,
                    mf.PatientId,
                    mf.CreatedBy,
                    mf.Gender,
                    mf.Image,
                    mf.Notes,
                    mf.ReminderId,
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
                });

                var dropDownIds = new[] { 2, 1, 17 };
                var allDropDownValues = await _sys_drop_down_valueService.Queryable()
    .Where(x => dropDownIds.Contains(x.DropDownID)
                && (x.CompanyID == CompanyID || x.CompanyID == null)).Select(z => new
                {
                    z.ID,
                    z.Value,
                    z.DropDownID,
                })
    .ToListAsync();

                var genderList = allDropDownValues
    .Where(d => d.DropDownID == 2)
    .Select(d => new { d.ID, d.Value })
    .ToList();

                var dropdownValues = allDropDownValues
    .Where(val => val.DropDownID == 1)
    .ToList();

                var appointmentStatusCounts = dropdownValues.Select(val => new
                {
                    val.ID,
                    val.Value,
                    IsActive = val.ID == finalAppointmentStatusId,

                    Count = _service.Queryable()
         .Where(mf => mf.CompanyId == CompanyID
                   && DbFunctions.TruncateTime(mf.AppointmentDate) == DbFunctions.TruncateTime(todayDate)
                   && (
                        (val.ID == 1 &&
                             ((roleName == "Doctor" && mf.DoctorId == userId)
                              || roleName != "Doctor"))
                        ||
                        (val.ID != 1 && mf.StatusId == val.ID &&
                             ((roleName == "Doctor" && mf.DoctorId == userId)
                              || roleName != "Doctor"))
                       )
               )
         .Count()
                }).ToList();

                var isShowDoctorId = await _adm_userService.Queryable()
                .Where(u => u.ID == userId)
                .Select(u => u.IsShowDoctor)
                .FirstOrDefaultAsync();
                //doctor list


                var companyInfo = await _adm_user_companyService.Queryable()
    .Where(uc => uc.UserID == userId)
    .Select(uc => new
    {
        uc.CompanyID,
        CompanyName = uc.adm_company.CompanyName // navigation property
    })
    .ToListAsync();
                var bloodList = allDropDownValues
    .Where(d => d.DropDownID == 17)
    .ToList();

                var consultationService = await _emr_service_mfService.Queryable()
    .Where(s => s.CompanyId == CompanyID && s.ServiceName == "Consultation")
    .Select(s => new { s.ID, s.ServiceName, s.Price })
    .ToListAsync();
                var maxCompanyToken = await _adm_companyService.Queryable()
    .Where(c => c.ID == CompanyID)
    .Select(c => (int?)c.TokenNo) // nullable to handle empty table
    .MaxAsync() ?? 0;
                var nextToken = await _service.Queryable()
    .Where(mf => mf.CompanyId == CompanyID
              && DbFunctions.TruncateTime(mf.AppointmentDate) == DbFunctions.TruncateTime(DateTime.Now))
    .Select(mf => (int?)mf.TokenNo)
    .MaxAsync() ?? maxCompanyToken;

                nextToken += 1;

                var isBackDatedAppointment = await _adm_companyService.Queryable()
    .Where(c => c.ID == CompanyID)
    .Select(c => c.IsBackDatedAppointment)
    .FirstOrDefaultAsync();

                var reminders = await _adm_reminder_mf.Queryable()
    .Where(r => r.CompanyId == CompanyID)
    .Select(r => new { r.ID, Value = r.Name })
    .ToListAsync();


                var followUps = await _emr_prescription_mfService.Queryable()
    .Where(mf => mf.CompanyID == CompanyID
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
                var ht = new Hashtable();
                ht.Add("@CompanyId", CompanyID);
                ht.Add("@UserId", userId);
                DataAccessManager dataAccessManager = new DataAccessManager();
                var doctorList = dataAccessManager.GetDataSet("SP_GetDoctorList", ht);
                objResult.GenderList = genderList;
                objResult.DoctorList = doctorList.Tables[0];
                objResult.AppointmentList = pagedEvents;
                objResult.AllStatusList = appointmentStatusCounts;
                objResult.DoctorCalander = doctorList.Tables[0];
                objResult.IsShowDoctorIds = isShowDoctorId;
                objResult.ClinicList = companyInfo;
                objResult.BloodList = bloodList;
                objResult.ServiceType = consultationService;
                objResult.TokenNo = nextToken.ToString();
                objResult.IsBackDatedAppointment = isBackDatedAppointment;
                objResult.TotalRecord = totalRecords;
                objResult.Reminderlist = reminders;
                objResult.Followuplist = followUpsResult;
            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }

        [HttpGet]
        [ActionName("DeshboardLoadDropdown")]
        public ResponseInfo DeshboardLoadDropdown(string PatientId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                int userid = Convert.ToInt32(Request.LoginID());
                DateTime todayDate = Convert.ToDateTime(new DateTime());
                var CompanyID = Request.CompanyID();
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
                        IsOverLap = mf.IsOverLap
                    };
                }).ToList();
                List<object> result;
                if (role == "Administrator")
                {
                    result = doctors;
                }
                else if (role == "Doctor")
                {
                    result = doctors.Where(d => ((dynamic)d).ID == userid).ToList<object>();
                }
                else
                {
                    result = doctors;
                }


                var ServiceType = _emr_service_mfService.Queryable().Where(e => e.CompanyId == CompanyID && e.ServiceName == "Consultation")
                 .Select(a => new { a.ID, a.ServiceName, a.Price }).FirstOrDefault();
                decimal TokenNo = GetNextToken(CompanyID);
                var StatusList = _sys_drop_down_valueService.Queryable().Where(a => a.DropDownID == 1 && (a.CompanyID == CompanyID || a.CompanyID == null)).Select(m => new
                {
                    ID = m.ID,
                    Value = m.Value,
                }).ToList();
                var PatientObj = _emr_patientService.Queryable().Where(a => a.CompanyId == CompanyID && a.ID.ToString() == PatientId).Select(z => new
                {
                    z.ID,
                    z.PatientName,
                    z.CNIC,
                    z.Mobile,
                }).FirstOrDefault();
                var reminderlist = _adm_reminder_mf.Queryable().Where(a => a.CompanyId == CompanyID)
                    .Select(z => new
                    {
                        ID = z.ID,
                        Value = z.Name
                    })
                    .ToList();

                objResponse.IsSuccess = true;
                objResponse.ResultSet = new
                {
                    DoctorList = result,
                    ServiceType = ServiceType,
                    TokenNo = TokenNo,
                    StatusList = StatusList,
                    PatientObj = PatientObj,
                    reminderlist = reminderlist,
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
            bool hasAppointmentsToday = _service.Queryable()
                .Any(mf => mf.CompanyId == companyId && mf.AppointmentDate == DateTime.Today);

            // Step 3: Get the maximum TokenNo from emr_appointment_mf for today
            int maxAppointmentToken = hasAppointmentsToday
                ? _service.Queryable()
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
        [ActionName("AdmitAppointmentLoad")]
        public PaginationResult AdmitAppointmentLoad(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string PatientId, string AdmissionId, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var ipdAdmissions = _ipd_admissionService.Queryable()
              .Where(a => a.PatientId.ToString() == PatientId && a.CompanyId == CompanyID
                    && a.ID.ToString() == AdmissionId)
              .Select(a => new
              {
                  AdmissionNo = a.AdmissionNo,
                  DischargeDate = a.DischargeDate,
                  DischargeTime = a.DischargeTime
              }).FirstOrDefault();
                objResult = _service.AdmitAppointmentLoad(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, PatientId, AdmissionId, IgnorePaging);
                objResult.OtherDataModel = ipdAdmissions;
            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }
        [HttpGet]
        [ActionName("GetAdmitAppointmentDropdown")]
        public async Task<ResponseInfo> GetAdmitAppointmentDropdown(string PatientId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal companyId = Request.CompanyID();
                decimal userid = Request.LoginID();
                var isShowDoctorIds = await _adm_userService.Queryable()
.Where(x => x.ID == userid)
.Select(x => x.IsShowDoctor)
.FirstOrDefaultAsync();
                var visitList = await _sys_drop_down_valueService.Queryable()
   .Where(x => x.DropDownID == 30 && (x.CompanyID == companyId || x.CompanyID == null))
   .ToListAsync();
                var ht = new Hashtable();
                ht.Add("@CompanyId", companyId);
                ht.Add("@UserId", userid);
                DataAccessManager dataAccessManager = new DataAccessManager();
                var doctorData = dataAccessManager.GetDataSet("SP_GetDoctorList", ht);
                var DoctorList = doctorData.Tables[0];
                decimal[] IdList = null;
                var isShowIds = isShowDoctorIds;
                if (!string.IsNullOrWhiteSpace(isShowIds))
                    IdList = isShowIds.Split(',').Select(decimal.Parse).ToArray();
                var DoctorCalander = doctorData.Tables[0];
                var VisitList = visitList;
                objResponse.ResultSet = new
                {
                    VisitList = VisitList,
                    DoctorList = DoctorList,
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
        [ActionName("PreviousAppointmentLoad")]
        public PaginationResult PreviousAppointmentLoad(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string PatientId, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResult = _service.PreviousAppointmentLoad(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, PatientId, IgnorePaging);
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
                emr_appointment_mf model = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID == ID).FirstOrDefault();
                var bill = _emr_patient_billService.Queryable().Where(a => a.CompanyId == CompanyID && a.AppointmentId == model.ID).ToList();
                foreach (var item in bill)
                {
                    item.ObjectState = ObjectState.Deleted;
                    _emr_patient_billService.Delete(item);
                }
                if (model == null)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
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
        public PaginationResult Pagination(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            throw new NotImplementedException();
        }

        public PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            throw new NotImplementedException();
        }
    }
}
