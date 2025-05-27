using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
using HMS.Service;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Admission;
using HMS.Service.Services.Appointment;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using HMS.Web.API.Interface;
using NPOI.POIFS.Properties;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Cms;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

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
        private readonly Iemr_service_mfService _emr_service_mfService;
        private readonly Iemr_notes_favoriteService _emr_notes_favoriteService;
        private readonly Iadm_companyService _adm_companyService;
        private readonly Iipd_diagnosisService _ipd_diagnosisService;
        private readonly Iadm_reminder_mfService _adm_reminder_mf;
        private readonly Iadm_integrationService _adm_integrationService;
        private readonly ISmsService _smsService;
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
            Iadm_integrationService adm_integrationService
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
                    else if (emailRow != null && !string.IsNullOrEmpty(email))
                    {
                        await SendEmailNotify(
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

        private async Task SendEmailNotify(string EmailFrom, string EmailPassword, string EmailSMTP, int? EmailPort, string toemail, string message)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpC = new SmtpClient(EmailSMTP);
            smtpC.EnableSsl = true;
            smtpC.Port = Convert.ToInt32(EmailPort);
            smtpC.Credentials = new System.Net.NetworkCredential(EmailFrom, EmailPassword);
            mail.From = new MailAddress(EmailFrom);
            mail.To.Add(toemail);
            mail.Subject = "";
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

        [HttpPost]
        [HttpGet]
        [ActionName("UserSave")]
        public async Task<ResponseInfo> UserSave(adm_user_mf Model)
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

                var user = _adm_userService.Queryable().Where(y => y.Email.ToLower() == Model.Email.ToLower()).FirstOrDefault();
                if (user == null)
                {
                    adm_user_mf admUser = new adm_user_mf();
                    decimal adm_userID = 1;
                    if (_adm_userService.Queryable().Count() > 0)
                        adm_userID = _adm_userService.Queryable().Max(e => e.ID) + 1;

                    List<adm_user_company> adm_userCompany = new List<adm_user_company>();
                    adm_userCompany.AddRange(Model.adm_user_company);

                    var guid = Guid.NewGuid().ToString();
                    var token = "GUID=" + guid + "&type=ApplicationUser";
                    string encrytoken = Cryptography.Encrypt(token);
                    encrytoken = HttpServerUtility.UrlTokenEncode(Encoding.ASCII.GetBytes(encrytoken));
                    if (Model.AppStartTime != null)
                    {
                        TimeSpan starttime = TimeSpan.Parse(Model.AppStartTime);
                        Model.StartTime = starttime;
                    }
                    if (Model.AppEndTime != null)
                    {
                        TimeSpan endtime = TimeSpan.Parse(Model.AppEndTime);
                        Model.EndTime = endtime;
                    }
                    if (Model.DocWorkingDay != null)
                    {
                        string result = string.Join(",", Model.DocWorkingDay);
                        Model.OffDay = result;
                    }
                    if (Model.SlotTime != null)
                    {
                        Model.SlotTime = Model.SlotTime;
                    }
                    Model.adm_user_company = null;
                    Model.user_payment = null;
                    Model.user_payment1 = null;
                    Model.user_payment2 = null;
                    Model.ID = adm_userID;
                    Model.AccountType = "A";
                    Model.ForgotToken = guid;
                    Model.ForgotTokenDate = Request.DateTimes();
                    Model.MultilingualId = Model.MultilingualId;
                    Model.ObjectState = ObjectState.Added;
                    _adm_userService.Insert(Model);

                    adm_user_token user_token = new adm_user_token();
                    decimal userTokenID = 1;
                    if (_adm_user_tokenService.Queryable().Count() > 0)
                        userTokenID = _adm_user_tokenService.Queryable().Max(e => e.ID) + 1;
                    user_token.ID = userTokenID;
                    user_token.UserID = Model.ID;
                    DateTime date = Request.DateTimes();
                    date = date.AddDays(3);
                    user_token.ExpiryDate = date;
                    user_token.TokenKey = guid;
                    user_token.IsExpired = false;
                    user_token.DeviceType = "web";
                    int deviceID = -1;
                    user_token.DeviceID = deviceID.ToString();
                    user_token.ObjectState = ObjectState.Added;
                    _adm_user_tokenService.Insert(user_token);

                    decimal adm_user_ComID = 1;
                    if (_adm_user_companyService.Queryable().Count() > 0)
                        adm_user_ComID = _adm_user_companyService.Queryable().Max(e => e.ID) + 1;
                    foreach (adm_user_company item in adm_userCompany)
                    {
                        adm_user_company admUserCompanyModel = new adm_user_company();
                        admUserCompanyModel.ID = adm_user_ComID;
                        admUserCompanyModel.UserID = Model.ID;
                        admUserCompanyModel.CompanyID = CompanyID;
                        admUserCompanyModel.RoleID = item.RoleID;
                        admUserCompanyModel.AdminID = _adm_user_companyService.Queryable()
                            .Where(e => e.CompanyID == CompanyID && e.UserID == e.AdminID)
                            .Select(s => s.UserID).FirstOrDefault();

                        admUserCompanyModel.ObjectState = ObjectState.Added;
                        _adm_user_companyService.Insert(admUserCompanyModel);
                        adm_user_ComID++;
                    }


                    //string path = @"~\Templates\EmailConfirm.txt";
                    //SendEmailforResetPassword(Model, "PayPeople | Verify your account", path, encrytoken);

                    //string path = @"~\Templates\EmailConfirm.txt";
                    //path = HttpContext.Current.Server.MapPath(path);
                    //Task.Run(() => SendEmailConfirmNotify(_unitOfWorkAsync, userobj, "PayPeople | Verify your account", path, "WelcomeEmail", encrytoken));

                    try
                    {
                        await _unitOfWorkAsync.SaveChangesAsync();
                        objResponse.Message = MessageStatement.Save;
                        objResponse.IsSuccess = true;
                        var DoctorList = _emr_patientService.DoctorList(CompanyID, UserID);
                        var userObj = _adm_userService.Queryable().Where(a => a.ID == UserID).FirstOrDefault();
                        decimal[] IdList = null;
                        List<DoctorList> DoctorCalander = new List<DoctorList>();
                        if (userObj.IsShowDoctor != null)
                        {
                            IdList = userObj.IsShowDoctor.Split(',').Select(decimal.Parse).ToArray();
                            DoctorCalander = DoctorList.DoctList.Where(a => !IdList.Contains(a.ID)).ToList();
                        }
                        else
                        {
                            DoctorCalander = DoctorList.DoctList.ToList();
                        }
                        objResponse.ResultSet = new
                        {
                            DoctorList = DoctorList,
                            DoctorCalander = DoctorCalander,
                        };

                        //EmailService objService = new EmailService();
                        //string PayrollRegion = System.Configuration.ConfigurationManager.AppSettings["PayrollRegion"].ToString();
                        //string path = "";
                        //    path = @"~\Templates\EmailConfirm.txt";                      

                        //path = HttpContext.Current.Server.MapPath(path);
                        //objService.SendEmailCofirm(path, Model.Email, Model.Name, CompanyID, encrytoken, Request.LoginID(), "Erpisto | Verify your account");


                        //EmailService objService = new EmailService();

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
                else
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = "This " + Model.Email + " already register please use another email.";
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
        public ResponseInfo PatientInfoLoad(string id, string Date, string AppId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal patientid = Convert.ToDecimal(id);
                int userid = Convert.ToInt32(Request.LoginID());
                DateTime date = Convert.ToDateTime(Date);
                decimal AppontId = Convert.ToDecimal(AppId);

                DataAccessManager dataAccessManager = new DataAccessManager();
                var ht = new Hashtable();
                ht.Add("@CompanyID", CompanyID);
                ht.Add("@PatientId", patientid);
                ht.Add("@AppontId", AppontId);
                ht.Add("@Date", date);
                var AllData = dataAccessManager.GetDataSet("SP_GetPrescriptionByPatientId", ht);
                var CurntPrevDateList = AllData.Tables[2];
                var ComplaintList = AllData.Tables[3];
                var ObservationList = AllData.Tables[4];
                var InvestigationsList = AllData.Tables[5];
                var DiagnosisList = AllData.Tables[6];
                var BillTypeList = AllData.Tables[7];
                var TittleList = AllData.Tables[8];
                var AppointmentList = AllData.Tables[9];
                objResponse.ResultSet = new
                {
                    patientInfo = AllData.Tables[1],
                    Doctorid = AllData.Tables[0].Rows[0]["Doctorid"],
                    prescription_mf = AllData.Tables[10],
                    prescription_treatment = AllData.Tables[11],
                    prescription_complaint = AllData.Tables[12],
                    prescription_diagnos = AllData.Tables[13],
                    prescription_investigation = AllData.Tables[14],
                    prescription_observation = AllData.Tables[15],
                    CurntPrevDateList = CurntPrevDateList,
                    ComplaintList = ComplaintList,
                    ObservationList = ObservationList,
                    InvestigationsList = InvestigationsList,
                    DiagnosisList = DiagnosisList,
                    AppointmentList = AppointmentList,
                    BillTypeList = BillTypeList,
                    TittleList = TittleList,
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
                decimal CompanyID = Request.CompanyID();
                decimal patientid = Convert.ToDecimal(id);
                int userid = Convert.ToInt32(Request.LoginID());
                DateTime todayDate = DateTime.Now;
                int? stid = Convert.ToInt32(0);

                DataAccessManager dataAccessManager = new DataAccessManager();
                var ht = new Hashtable();
                ht.Add("@CompanyID", CompanyID);
                ht.Add("@PatientId", patientid);

                var AllData = dataAccessManager.GetDataSet("SP_GetPatientSummaryByPatientId", ht);

                var patientInfo = AllData.Tables[0];
                var GenderType = AllData.Tables[1];
                var MedicineList = AllData.Tables[2];
                var BloodList = AllData.Tables[3];

                var BillTypeList = AllData.Tables[4];
                var TittleList = AllData.Tables[5];

                var DoctorList = AllData.Tables[4];

                var prescriptionresult = _emr_prescription_mfService.Queryable().Where(e => e.PatientId == patientid && e.CompanyID == CompanyID).Include(a => a.emr_prescription_complaint).Include(a => a.emr_prescription_diagnos).Include(a => a.emr_prescription_investigation).Include(a => a.emr_prescription_observation).Include(a => a.emr_prescription_treatment)
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
                    }).ToList();
                string AdmissionNo = "0";
                var PaidAndOutamount = AllData.Tables[6].Rows[0]["PaidAndOutamount"];
                if (AllData.Tables[7].Rows.Count > 0)
                {
                    AdmissionNo = AllData.Tables[7].Rows[0]["AdmissionNo"].ToString();
                }
                objResponse.ResultSet = new
                {
                    patientInfo = patientInfo,
                    GenderList = GenderType,
                    PaidAndOutamount = PaidAndOutamount,
                    BillTypeList = BillTypeList,
                    TittleList = TittleList,
                    BloodList = BloodList,
                    MedicineList = MedicineList,
                    prescriptionresult = prescriptionresult,
                    AdmissionNo = AdmissionNo,
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
        [ActionName("AdmitPatientLoadById")]
        public ResponseInfo AdmitPatientLoadById(string id, string AdmitId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal patientid = Convert.ToDecimal(id);
                decimal AdmissionId = Convert.ToDecimal(AdmitId);
                int userid = Convert.ToInt32(Request.LoginID());
                DateTime todayDate = DateTime.Now;
                int? stid = Convert.ToInt32(0);
                DataAccessManager dataAccessManager = new DataAccessManager();
                var ht = new Hashtable();
                ht.Add("@CompanyID", CompanyID);
                ht.Add("@PatientId", patientid);
                ht.Add("@AdmissionId", AdmissionId);
                var AllData = dataAccessManager.GetDataSet("SP_GetAdmitPatientSummaryByPatientId", ht);
                var patientInfo = AllData.Tables[0];
                var GenderType = AllData.Tables[1];
                var MedicineList = AllData.Tables[2];
                var BloodList = AllData.Tables[3];
                var BillTypeList = AllData.Tables[4];
                var TittleList = AllData.Tables[5];
                var NotesList = AllData.Tables[9];
                var ChartData = AllData.Tables[10];
                var InputOutputList = AllData.Tables[11];
                var MedicationLogList = AllData.Tables[12];
                var DocumentList = AllData.Tables[13];
                var prescriptionresult = _emr_prescription_mfService.Queryable().Where(e => e.PatientId == patientid && e.CompanyID == CompanyID).Include(a => a.emr_prescription_complaint).Include(a => a.emr_prescription_diagnos).Include(a => a.emr_prescription_investigation).Include(a => a.emr_prescription_observation).Include(a => a.emr_prescription_treatment)
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
                    }).ToList();
                var PaidAndOutamount = AllData.Tables[6].Rows[0]["PaidAndOutamount"];
                var AdmissionNo = AllData.Tables[7].Rows[0]["AdmissionNo"];
                var DischargeDate = AllData.Tables[7].Rows[0]["DischargeDate"];
                var Diagnosislist = AllData.Tables[8];
                objResponse.ResultSet = new
                {
                    patientInfo = patientInfo,
                    GenderList = GenderType,
                    PaidAndOutamount = PaidAndOutamount,
                    BillTypeList = BillTypeList,
                    TittleList = TittleList,
                    BloodList = BloodList,
                    MedicineList = MedicineList,
                    prescriptionresult = prescriptionresult,
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
        public PaginationResult AppointList(string date, string StatusId, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                int? stid = Convert.ToInt32(StatusId);
                int userid = Convert.ToInt32(Request.LoginID());
                DateTime todayDate = Convert.ToDateTime(date);
                string TotalRecord = "0";
                if (SearchText == null)
                    SearchText = "";
                DataAccessManager dataAccessManager = new DataAccessManager();
                var ht = new Hashtable();
                ht.Add("@CompanyID", CompanyID);
                ht.Add("@UserId", userid);
                ht.Add("@StatusId", stid);
                ht.Add("@CurrentDate", todayDate);
                ht.Add("@CurrentPageNo", CurrentPageNo);
                ht.Add("@RecordPerPage", RecordPerPage);
                ht.Add("@SearchText", SearchText.ToString());

                var AllData = dataAccessManager.GetDataSet("SP_CalendarData", ht);

                var LIST = AllData.Tables[0];
                if (LIST.Rows.Count > 0)
                    TotalRecord = LIST.Rows[0]["TotalRecord"].ToString();
                var GenderType = AllData.Tables[1];
                var StatusList = AllData.Tables[2];
                var AllStatusList = AllData.Tables[3];
                var DoctorList = AllData.Tables[4];
                var DoctorCalander = AllData.Tables[6];
                var Reminderlist = AllData.Tables[14];
                var Followuplist = AllData.Tables[15];
                decimal[] IdList = null;
                var isShowIds = AllData.Tables[5].Rows[0]["IsShowDoctorIds"].ToString();
                if (isShowIds != "")
                    IdList = isShowIds.Split(',').Select(decimal.Parse).ToArray();
                var PatientInfo = AllData.Tables[7];
                var MedicineList = AllData.Tables[8];
                var ClinicList = AllData.Tables[9];
                var BloodList = AllData.Tables[10];
                var ServiceType = AllData.Tables[11];
                var token = AllData.Tables[12].Rows[0]["TokenNo"].ToString();
                decimal TokenNo = Convert.ToDecimal(token);
                var IsBackDated = AllData.Tables[13].Rows[0]["IsBackDatedAppointment"].ToString();
                bool IsBackDatedAppointment = Convert.ToBoolean(IsBackDated);

                objResult.GenderList = GenderType;
                objResult.DoctorList = DoctorList;
                objResult.AppointmentList = LIST;
                objResult.AllStatusList = AllStatusList;
                objResult.PatientInfo = PatientInfo;
                objResult.DoctorCalander = DoctorCalander;
                objResult.IsShowDoctorIds = IdList;
                objResult.ClinicList = ClinicList;
                objResult.BloodList = BloodList;
                objResult.MedicineList = MedicineList;
                objResult.ServiceType = ServiceType;
                objResult.TokenNo = TokenNo.ToString();
                objResult.IsBackDatedAppointment = IsBackDatedAppointment;
                objResult.TotalRecord = Convert.ToInt32(TotalRecord);
                objResult.Reminderlist = Reminderlist;
                objResult.Followuplist = Followuplist;
            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }
        [HttpGet]
        [ActionName("UpdateAppointList")]
        public PaginationResult UpdateAppointList(string date, string StatusId, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                int? stid = Convert.ToInt32(StatusId);
                int userid = Convert.ToInt32(Request.LoginID());
                DateTime todayDate = Convert.ToDateTime(date);
                string TotalRecord = "0";
                if (SearchText == null)
                    SearchText = "";
                var userobj = _adm_userService.Queryable().Where(a => a.ID == userid).FirstOrDefault();
                userobj.AppointmentStatusId = stid;
                userobj.ObjectState = ObjectState.Modified;
                _adm_userService.Update(userobj);
                _unitOfWorkAsync.SaveChanges();
                DataAccessManager dataAccessManager = new DataAccessManager();
                var ht = new Hashtable();
                ht.Add("@CompanyID", CompanyID);
                ht.Add("@UserId", userid);
                ht.Add("@StatusId", stid);
                ht.Add("@CurrentDate", todayDate);
                ht.Add("@CurrentPageNo", CurrentPageNo);
                ht.Add("@RecordPerPage", RecordPerPage);
                ht.Add("@SearchText", SearchText.ToString());
                var AllData = dataAccessManager.GetDataSet("SP_CalendarData", ht);
                var LIST = AllData.Tables[0];
                if (LIST.Rows.Count > 0)
                    TotalRecord = LIST.Rows[0]["TotalRecord"].ToString();
                var GenderType = AllData.Tables[1];
                var StatusList = AllData.Tables[2];
                var AllStatusList = AllData.Tables[3];
                var DoctorList = AllData.Tables[4];
                var DoctorCalander = AllData.Tables[6];
                decimal[] IdList = null;
                var isShowIds = AllData.Tables[5].Rows[0]["IsShowDoctorIds"].ToString();
                if (isShowIds != "")
                    IdList = isShowIds.Split(',').Select(decimal.Parse).ToArray();
                var PatientInfo = AllData.Tables[7];
                var MedicineList = AllData.Tables[8];
                var ClinicList = AllData.Tables[9];
                var BloodList = AllData.Tables[10];
                var ServiceType = AllData.Tables[11];
                var token = AllData.Tables[12].Rows[0]["TokenNo"].ToString();
                decimal TokenNo = Convert.ToDecimal(token);
                var IsBackDated = AllData.Tables[13].Rows[0]["IsBackDatedAppointment"].ToString();
                bool IsBackDatedAppointment = Convert.ToBoolean(IsBackDated);
                var Reminderlist = AllData.Tables[14];
                var Followuplist = AllData.Tables[15];
                objResult.GenderList = GenderType;
                objResult.DoctorList = DoctorList;
                objResult.AppointmentList = LIST;
                objResult.AllStatusList = AllStatusList;
                objResult.PatientInfo = PatientInfo;
                objResult.DoctorCalander = DoctorCalander;
                objResult.IsShowDoctorIds = IdList;
                objResult.ClinicList = ClinicList;
                objResult.BloodList = BloodList;
                objResult.MedicineList = MedicineList;
                objResult.ServiceType = ServiceType;
                objResult.TokenNo = TokenNo.ToString();
                objResult.IsBackDatedAppointment = IsBackDatedAppointment;
                objResult.TotalRecord = Convert.ToInt32(TotalRecord);
                objResult.Reminderlist = Reminderlist;
                objResult.Followuplist = Followuplist;
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
                var DoctorList = _emr_patientService.DoctorList(CompanyID, userid);
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
                objResponse.IsSuccess = true;
                objResponse.ResultSet = new
                {
                    DoctorList = DoctorList.DoctList,
                    ServiceType = ServiceType,
                    TokenNo = TokenNo,
                    StatusList = StatusList,
                    PatientObj = PatientObj,
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
                objResult = _service.AdmitAppointmentLoad(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, PatientId, AdmissionId, IgnorePaging);
            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }
        [HttpGet]
        [ActionName("GetAdmitAppointmentDropdown")]
        public ResponseInfo GetAdmitAppointmentDropdown(string PatientId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();

                DataAccessManager dataAccessManager = new DataAccessManager();
                var ht = new Hashtable();
                ht.Add("@CompanyID", CompanyID);
                ht.Add("@UserId", UserID);
                var AllData = dataAccessManager.GetDataSet("SP_LoadDropdown", ht);

                var DoctorList = AllData.Tables[1];
                decimal[] IdList = null;
                var isShowIds = AllData.Tables[2].Rows[0]["IsShowDoctorIds"].ToString();
                if (isShowIds != "")
                    IdList = isShowIds.Split(',').Select(decimal.Parse).ToArray();
                var DoctorCalander = AllData.Tables[3];

                var VisitList = AllData.Tables[16];
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
