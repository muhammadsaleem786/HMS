using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Appointment;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using HMS.Web.API.Interface;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HMS.Web.API.Areas.Appointment.Controllers
{
    [JwtAuthentication]
    public class emr_patientController : ApiController, IERPAPIInterface<emr_patient_mf>, IDisposable
    {
        private readonly Iemr_patientService _service;
        private readonly Iemr_medicineService _emr_medicineService;
        private readonly Iemr_appointment_mfService _emr_appointment_mfService;
        private readonly Iadm_user_companyService _adm_user_companyService;
        private readonly Iadm_role_mfService _adm_role_mfService;
        private readonly Iadm_user_tokenService _adm_user_tokenService;
        private readonly Isys_notification_alertService _sys_notification_alertService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly Iadm_userService _adm_userService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iemr_service_mfService _emr_service_mfService;
        private readonly Iemr_patient_billService _emr_patient_billService;
        private readonly Iadm_companyService _adm_companyService;

        public emr_patientController(IUnitOfWorkAsync unitOfWorkAsync, Iemr_service_mfService emr_service_mfService, Iemr_patient_billService emr_patient_billService, Iemr_medicineService emr_medicineService, Iemr_patientService service, Iadm_userService adm_userService, Iemr_appointment_mfService emr_appointment_mfService, Iadm_user_companyService adm_user_companyService,
            Iadm_role_mfService adm_role_mfService, Iadm_user_tokenService adm_user_tokenService, Isys_drop_down_valueService sys_drop_down_valueService,
            Isys_notification_alertService sys_notification_alertService, Iadm_companyService adm_companyService)
        {
            _adm_companyService = adm_companyService;
            _emr_patient_billService = emr_patient_billService;
            _emr_service_mfService = emr_service_mfService;
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = service; _sys_drop_down_valueService = sys_drop_down_valueService;
            _adm_user_companyService = adm_user_companyService;
            _adm_userService = adm_userService;
            _emr_appointment_mfService = emr_appointment_mfService;
            _adm_role_mfService = adm_role_mfService;
            _adm_user_tokenService = adm_user_tokenService;
            _emr_medicineService = emr_medicineService;
            _sys_notification_alertService = sys_notification_alertService;
        }
        [HttpPost]
        [HttpGet]
        [ActionName("Save")]
        public async Task<ResponseInfo> Save(emr_patient_mf Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                if (!ModelState.IsValid)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.BadRequest;
                    return objResponse;
                }

                var CNICExist = new emr_patient_mf();
                if (Model.CNIC != null)
                    CNICExist = _service.Queryable().Where(y => y.CNIC.ToLower() == Model.CNIC.ToLower()).FirstOrDefault();

                //if (Model.Mobile != null)
                //    CNICExist = _service.Queryable().Where(y => y.Mobile == Model.Mobile).FirstOrDefault();

                if (CNICExist == null || CNICExist.CNIC == null)
                {
                    decimal CompanyID = Request.CompanyID();
                    decimal UserID = Request.LoginID();

                    decimal ID = 1;
                    if (_service.Queryable().Count() > 0)
                        ID = _service.Queryable().Max(e => e.ID) + 1;
                    Model.ID = ID;
                    if (Model.BloodGroupId != 0 && Model.BloodGroupId != null)
                        Model.BloodGroupDropDownId = (int)sys_dropdown_mfEnum.BloodDropdownId;
                    Model.BillTypeDropdownId = (int)sys_dropdown_mfEnum.BillTypeDropdownId;
                    Model.PrefixDropdownId = (int)sys_dropdown_mfEnum.PrefixDropdownId;
                    Model.CompanyId = Request.CompanyID();
                    Model.CreatedBy = Request.LoginID();
                    Model.CreatedDate = Request.DateTimes();
                    Model.ModifiedBy = Request.LoginID();
                    Model.ModifiedDate = Request.DateTimes();
                    Model.ObjectState = ObjectState.Added;
                    _service.Insert(Model);
                    try
                    {
                        await _unitOfWorkAsync.SaveChangesAsync();
                        objResponse.Message = MessageStatement.Save;
                        objResponse.IsSuccess = true;
                        objResponse.ResultSet = new
                        {
                            CurrentModel = Model,
                            Model = _service.Queryable().Where(a => a.CompanyId == CompanyID).ToList(),
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
                else
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = "This " + Model.CNIC + " already register please use another CNIC.";
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
        [ActionName("Update")]
        public async Task<ResponseInfo> Update(emr_patient_mf Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                if (!ModelState.IsValid)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.BadRequest;
                    return objResponse;
                }
                decimal CompanyID = Request.CompanyID();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Modified;
                Model.emr_appointment_mf = null;
                Model.emr_patient_bill = null;
                Model.emr_prescription_mf = null;
                Model.ipd_admission = null;
                Model.pur_sale_mf = null;
                _service.Update(Model);

                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        CurrentModel = Model,
                        Model = _service.Queryable().Where(a => a.CompanyId == CompanyID).ToList(),
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
        [AllowAnonymous]
        [HttpGet]
        [ActionName("IsPhoneExist")]
        public ResponseInfo IsPhoneExist(string Phone)
        {
            var objResponse = new ResponseInfo();
            try
            {
                emr_patient_mf obj = new emr_patient_mf();
                try
                {
                    obj = _service.Queryable().Where(e => e.Mobile == Phone).FirstOrDefault();
                    if (obj != null)
                    {
                        objResponse.IsSuccess = true;
                        objResponse.ErrorMessage = "Are you sure want to add another patient against this " + Phone + ".";
                    }
                    else
                    {
                        objResponse.IsSuccess = false;
                    }
                }
                catch (Exception ex)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = ex.Message;
                    Logger.Trace.Error(ex);
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
        [HttpPost]
        [HttpGet]
        [ActionName("searchByName")]
        public async Task<ResponseInfo> searchByName(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();

                var serviceObj = await _emr_service_mfService.Queryable()
            .Where(a => a.CompanyId == CompanyID && a.ServiceName == "Consultation")
            .Select(a => new { a.ID, a.ServiceName, a.Price })
            .FirstOrDefaultAsync();

                var PatientInfo = await _service.Queryable()
            .AsNoTracking()
            .Where(a => a.CompanyId == CompanyID && (a.Mobile.Contains(term)))
            .Select(z => new
            {
                value = z.ID,
                label = z.PatientName + " " + (z.Mobile),
                PatientName = z.PatientName,
                Phone = z.Mobile,
                CNIC = z.CNIC,
                Age = z.Age,
                Gender = z.Gender,
                MRNO = z.MRNO,
                ReminderId = z.ReminderId
            }).Take(10)
            .ToListAsync();
                objResponse.ResultSet = new
                {
                    PatientInfo = PatientInfo.Select(z => new
                    {
                        z.value,
                        z.label,
                        z.PatientName,
                        z.Phone,
                        z.CNIC,
                        z.Age,
                        z.Gender,
                        z.MRNO,
                        z.ReminderId,
                        ServiceName = serviceObj?.ServiceName,
                        ServiceID = serviceObj?.ID,
                        ServicePrice = serviceObj?.Price
                    })
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
        [ActionName("getPatientById")]
        public async Task<ResponseInfo> getPatientById(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();

                var result = await _service.Queryable().AsNoTracking().Where(a => a.CompanyId == CompanyID && a.ID.ToString() == Id).Select(z => new
                {
                    ID = z.ID,
                    model = z
                }).ToListAsync();
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
        private static string RemoveNonNumericCharacters(string s)
        {
            var aaa = Regex.Replace(s, "[^0-9]", "");
            return Regex.Replace(s, "[^0-9]", "");
        }
        [HttpPost]
        [HttpGet]
        [ActionName("searchByClinic")]
        public async Task<ResponseInfo> searchByClinic(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var companyInfo = _adm_companyService.Queryable().Where(a => a.ID == CompanyID && a.CompanyName.Contains(term)).Select(z => new
                {
                    value = z.ID,
                    label = z.CompanyName,
                }).ToList();
                objResponse.ResultSet = new
                {
                    companyInfo = companyInfo
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
        [ActionName("GetRoles")]
        public ResponseInfo GetRoles()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResponse.ResultSet = _adm_role_mfService.Queryable().Where(x => x.CompanyID == CompanyID).ToList();
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
        [HttpGet]
        [ActionName("LoadData")]
        public ResponseInfo LoadData(string date, string StatusId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                DateTime todayDate = Convert.ToDateTime(date);
                int? stid = Convert.ToInt32(StatusId);
                int userid = Convert.ToInt32(Request.LoginID());
                var GenderType = _sys_drop_down_valueService.Queryable().Where(e => e.DropDownID == 2).ToList();

                var AppointmentList = _emr_appointment_mfService.Queryable().Where(a => a.CompanyId == CompanyID && DbFunctions.TruncateTime(a.AppointmentDate) == DbFunctions.TruncateTime(todayDate))
                   .Include(a => a.adm_user_mf2).Include(a => a.emr_patient_mf).Select(z => new
                   {
                       ID = z.ID,
                       DoctorName = z.adm_user_mf2.Name,
                       PatientName = z.emr_patient_mf.PatientName,
                       AppointmentDate = z.AppointmentDate,
                       AppointmentTime = z.AppointmentTime,
                       DoctorId = z.DoctorId,
                       StatusId = z.StatusId,
                       CNIC = z.emr_patient_mf.CNIC,
                       PatientId = z.PatientId,
                       CreatedBy = z.adm_user_mf.Name,
                       Note = z.Notes,
                   }).OrderBy(d => d.ID).ToList();

                var LIST = AppointmentList.AsEnumerable().Select(m => new
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
                }).Where(a => stid == 0 ? a.StatusId == a.StatusId : a.StatusId == stid).OrderBy(a => a.DoctorId).ToList();

                var StatusList = _sys_drop_down_valueService.Queryable().Where(a => a.DropDownID == 1 && (a.CompanyID == CompanyID || a.CompanyID == null)).Select(m => new
                {
                    ID = m.ID,
                    Value = m.Value
                }).ToList();
                var AllStatusList = StatusList.AsEnumerable().Select(z => new
                {
                    ID = z.ID,
                    Value = z.Value,
                    count = AppointmentList.AsEnumerable().Where(a => a.StatusId == z.ID).Count(),
                }).ToList();
                var DoctorList = _service.DoctorList(CompanyID, userid);
                decimal UserID = Request.LoginID();
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

                var PatientInfo = _service.Queryable().Where(a => a.CompanyId == CompanyID).Select(z => new
                {
                    ID = z.ID,
                    CompanyId = z.CompanyId,
                    PatientName = z.PatientName,
                    Gender = z.Gender,
                    DOB = z.DOB,
                    Email = z.Email,
                    Mobile = z.Mobile,
                    CNIC = z.CNIC,
                    Image = z.Image,
                    Notes = z.Notes,
                    MRNO = z.MRNO,
                    BloodGroupId = z.BloodGroupId,
                    BloodGroupDropDownId = z.BloodGroupDropDownId,
                    EmergencyNo = z.EmergencyNo,
                    Address = z.Address,
                    ReferredBy = z.ReferredBy,
                    AnniversaryDate = z.AnniversaryDate,
                    Illness_Diabetes = z.Illness_Diabetes,
                    Illness_Tuberculosis = z.Illness_Tuberculosis,
                    Illness_HeartPatient = z.Illness_HeartPatient,
                    Illness_LungsRelated = z.Illness_LungsRelated,
                    Illness_BloodPressure = z.Illness_BloodPressure,
                    Illness_Migraine = z.Illness_Migraine,
                    Illness_Other = z.Illness_Other,
                    Allergies_Food = z.Allergies_Food,
                    Allergies_Drug = z.Allergies_Drug,
                    Allergies_Other = z.Allergies_Other,
                    Habits_Smoking = z.Habits_Smoking,
                    Habits_Drinking = z.Habits_Drinking,
                    Habits_Tobacco = z.Habits_Tobacco,
                    Habits_Other = z.Habits_Other,
                    MedicalHistory = z.MedicalHistory,
                    CurrentMedication = z.CurrentMedication,
                    HabitsHistory = z.HabitsHistory,
                    CreatedBy = z.CreatedBy,
                    CreatedDate = z.CreatedDate,
                    ModifiedBy = z.ModifiedBy,
                    ModifiedDate = z.ModifiedDate,
                    Age = z.Age,
                }).ToList();

                var MedicineList = _emr_medicineService.Queryable().Where(a => a.CompanyID == CompanyID).Select(a => new { Id = a.ID, Medicine = a.Medicine }).ToList();
                var ClinicList = _adm_user_companyService.Queryable()
                            .Where(e => e.UserID == userid).Include(x => x.adm_company).Select(x => new { x.CompanyID, x.adm_company.CompanyName }).ToList();
                var BloodList = _sys_drop_down_valueService.Queryable().Where(e => e.DropDownID == 17).ToList();
                var ServiceType = _emr_service_mfService.Queryable().Where(e => e.CompanyId == CompanyID && e.ServiceName == "Consultation")
                    .Select(a => new { a.ID, a.ServiceName, a.Price }).FirstOrDefault();


                decimal TokenNo = 1;
                if (_emr_appointment_mfService.Queryable().Where(a => a.CompanyId == CompanyID && DbFunctions.TruncateTime(a.AppointmentDate) == DbFunctions.TruncateTime(todayDate)).Count() > 0)
                    TokenNo = _emr_appointment_mfService.Queryable().Where(a => a.CompanyId == CompanyID && DbFunctions.TruncateTime(a.AppointmentDate) == DbFunctions.TruncateTime(todayDate)).Max(e => e.TokenNo) + 1;
                objResponse.ResultSet = new
                {
                    GenderList = GenderType,
                    DoctorList = DoctorList,
                    AppointmentList = LIST,
                    AllStatusList = AllStatusList,
                    PatientInfo = PatientInfo,
                    DoctorCalander = DoctorCalander,
                    IsShowDoctorIds = IdList,
                    ClinicList = ClinicList,
                    BloodList = BloodList,
                    MedicineList = MedicineList,
                    ServiceType = ServiceType,
                    TokenNo = TokenNo,
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
        [ActionName("DropdownFilterData")]
        public ResponseInfo DropdownFilterData(string date, string StatusId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                DateTime todayDate = Convert.ToDateTime(date);
                int? stid = Convert.ToInt32(StatusId);
                var GenderType = _sys_drop_down_valueService.Queryable().Where(e => e.DropDownID == 2).ToList();

                var AppointmentList = _emr_appointment_mfService.Queryable().Where(a => a.CompanyId == CompanyID && DbFunctions.TruncateTime(a.AppointmentDate) == DbFunctions.TruncateTime(todayDate))
                   .Include(a => a.adm_user_mf2).Include(a => a.emr_patient_mf).Select(z => new
                   {
                       ID = z.ID,
                       DoctorName = z.adm_user_mf2.Name,
                       PatientName = z.emr_patient_mf.PatientName,
                       AppointmentDate = z.AppointmentDate,
                       AppointmentTime = z.AppointmentTime,
                       Gender = z.emr_patient_mf.Gender,
                       Image = z.emr_patient_mf.Image,
                       DoctorId = z.DoctorId,
                       StatusId = z.StatusId,
                       CNIC = z.emr_patient_mf.CNIC,
                       PatientId = z.PatientId,
                       CreatedBy = z.adm_user_mf.Name,
                       Note = z.Notes,
                   }).OrderBy(d => d.ID).ToList();

                var LIST = AppointmentList.AsEnumerable().Select(m => new
                {
                    ID = m.ID,
                    DoctorName = m.DoctorName,
                    PatientName = m.PatientName,
                    DoctorId = m.DoctorId,
                    Note = m.Note,
                    StatusId = m.StatusId,
                    Gender = m.Gender,
                    CNIC = m.CNIC,
                    PatientId = m.PatientId,
                    CreatedBy = m.CreatedBy,
                    Image = m.Image,
                    Color = m.StatusId == 25 ? "#007bff" : m.StatusId == 5 ? "#00FF00" : m.StatusId == 6 ? "#FF0000" : m.StatusId == 8 ? "#808080" : "",
                    StartDate = m.AppointmentDate.ToString("yyyy-MM-dd") + "T" + m.AppointmentTime.ToString("hh\\:mm\\:ss"),
                    EndDate = m.AppointmentDate.ToString("yyyy-MM-dd") + "T" + WorkingHours(Convert.ToDateTime(m.AppointmentDate + m.AppointmentTime), m.DoctorId),
                }).Where(a => stid == 0 ? a.Gender == a.Gender : a.Gender == stid).OrderBy(a => a.DoctorId).ToList();

                var StatusList = _sys_drop_down_valueService.Queryable().Where(a => a.DropDownID == 1 && (a.CompanyID == CompanyID || a.CompanyID == null)).Select(m => new
                {
                    ID = m.ID,
                    Value = m.Value
                }).ToList();
                var AllStatusList = StatusList.AsEnumerable().Select(z => new
                {
                    ID = z.ID,
                    Value = z.Value,
                    count = LIST.AsEnumerable().Where(a => a.StatusId == z.ID).Count(),
                }).ToList();
                decimal UserID = Request.LoginID();
                var DoctorList = _service.DoctorList(CompanyID, UserID);

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

                var PatientInfo = _service.Queryable().Where(a => a.CompanyId == CompanyID).ToList();
                objResponse.ResultSet = new
                {
                    GenderList = GenderType,
                    DoctorList = DoctorList,
                    AppointmentList = LIST,
                    AllStatusList = AllStatusList,
                    PatientInfo = PatientInfo,
                    DoctorCalander = DoctorCalander,
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
        [ActionName("MonthLoadData")]
        public ResponseInfo MonthLoadData(string fdate, string tdate, string StatusId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                DateTime fdayDate = Convert.ToDateTime(fdate);
                DateTime tdayDate = Convert.ToDateTime(tdate);
                int? stid = Convert.ToInt32(StatusId);
                decimal UserID = Request.LoginID();
                var GenderType = _sys_drop_down_valueService.Queryable().Where(e => e.DropDownID == 2).ToList();

                var LIST = AppointmentInfo(CompanyID, stid, fdayDate, tdayDate);

                var StatusList = _sys_drop_down_valueService.Queryable().Where(a => a.DropDownID == 1 && (a.CompanyID == CompanyID || a.CompanyID == null)).Select(m => new
                {
                    ID = m.ID,
                    Value = m.Value
                }).ToList();
                var AllStatusList = StatusList.AsEnumerable().Select(z => new
                {
                    ID = z.ID,
                    Value = z.Value,
                    count = LIST.AsEnumerable().Where(a => a.StatusId == z.ID).Count(),

                }).ToList();
                var DoctorList = _service.DoctorList(CompanyID, UserID);

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
                    GenderList = GenderType,
                    DoctorList = DoctorList,
                    AppointmentList = LIST,
                    AllStatusList = AllStatusList,
                    DoctorCalander = DoctorCalander,
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
        public List<AppointmentInfo> AppointmentInfo(decimal CompanyID, int? stid, DateTime fdayDate, DateTime tdayDate)
        {
            var AppointmentList = _emr_appointment_mfService.Queryable().Where(a => a.CompanyId == CompanyID && DbFunctions.TruncateTime(a.AppointmentDate) >= DbFunctions.TruncateTime(fdayDate) && DbFunctions.TruncateTime(a.AppointmentDate) <= DbFunctions.TruncateTime(tdayDate))
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
                      CreatedBy = z.adm_user_mf.Name,
                      CNIC = z.emr_patient_mf.CNIC,
                      Note = z.Notes,
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
            }).Where(a => stid == 0 ? a.StatusId == a.StatusId : a.StatusId == stid).OrderBy(a => a.DoctorId).ToList();


            return LIST.ToList();
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
        [ActionName("Pagination")]
        public PaginationResult Pagination(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResult = _service.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
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
                emr_patient_mf model = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID == ID).FirstOrDefault();

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
        [HttpGet]
        [ActionName("Load")]
        public ResponseInfo Load()
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


                var GenderType = AllData.Tables[0];//_sys_drop_down_valueService.Queryable().Where(e => e.DropDownID == 2).ToList();
                var BloodList = AllData.Tables[6];//_sys_drop_down_valueService.Queryable().Where(e => e.DropDownID == 17).ToList();

                var StatusList = AllData.Tables[10];//_sys_drop_down_valueService.Queryable().Where(a => a.DropDownID == 1 && (a.CompanyID == CompanyID || a.CompanyID == null)).Select(m => new
                //{
                //    ID = m.ID,
                //    Value = m.Value
                //}).ToList();

                var BillTypeList = AllData.Tables[11];//_sys_drop_down_valueService.Queryable().Where(a => a.DropDownID == 22 && (a.CompanyID == CompanyID || a.CompanyID == null)).Select(m => new
                //{
                //    ID = m.ID,
                //    Value = m.Value
                //}).ToList();

                var TittleList = AllData.Tables[12];//_sys_drop_down_valueService.Queryable().Where(a => a.DropDownID == 23 && (a.CompanyID == CompanyID || a.CompanyID == null)).Select(m => new
                //{
                //    ID = m.ID,
                //    Value = m.Value
                //}).ToList();


                //decimal UserID = Request.LoginID();
                var DoctorList = AllData.Tables[1];//_service.DoctorList(CompanyID, UserID);

                //var userObj = _adm_userService.Queryable().Where(a => a.ID == UserID).FirstOrDefault();

                decimal[] IdList = null;
                var isShowIds = AllData.Tables[2].Rows[0]["IsShowDoctorIds"].ToString();
                if (isShowIds != "")
                    IdList = isShowIds.Split(',').Select(decimal.Parse).ToArray();
                var DoctorCalander = AllData.Tables[3];


                decimal getMaxMRNO = Convert.ToDecimal(AllData.Tables[14].Rows[0]["MRNO"].ToString());
                string MRNO = "";
                MRNO = (getMaxMRNO).ToString("0000000000");

                //string MRNO = CompanyID.ToString();
                //decimal? maxid = _service.Queryable().Where(a => a.CompanyId == CompanyID).Max(a => (decimal?)a.ID);

                //var getMaxVal = _service.Queryable().Where(a => a.CompanyId == CompanyID && a.ID == maxid).Select(a => new { a.ID }).FirstOrDefault();
                //MRNO = ((getMaxVal.ID) + 1).ToString("0000000000");

                objResponse.ResultSet = new
                {
                    MRNO = MRNO,
                    GenderList = GenderType,
                    DoctorList = DoctorList,
                    AllStatusList = StatusList,
                    BloodList = BloodList,
                    DoctorCalander = DoctorCalander,
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
        [ActionName("GetEMRNO")]
        public ResponseInfo GetEMRNO()
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
                decimal getMaxMRNO = Convert.ToDecimal(AllData.Tables[14].Rows[0]["MRNO"].ToString());
                string MRNO = "";
                MRNO = (getMaxMRNO).ToString("0000000000");
                var GenderList = AllData.Tables[0];
                var BillTypeList = AllData.Tables[11];
                var TittleList = AllData.Tables[12];
                objResponse.ResultSet = new
                {
                    BillTypeList = BillTypeList,
                    TittleList = TittleList,
                    MRNO = MRNO,
                    GenderList = GenderList,
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
        [ActionName("DoctorSearch")]
        public async Task<ResponseInfo> DoctorSearch(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var DoctorList = _service.SerachDoctorList(CompanyID, UserID);

                var result = DoctorList.DoctList.AsEnumerable().Where(a => a.DoctorName.ToLower().Contains(term.ToLower())).Select(z => new
                {
                    value = z.DoctorId,
                    label = z.DoctorName,

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
        [HttpGet]
        [ActionName("DoctorList")]
        public ResponseInfo DoctorList()
        {

            var objResponse = new ResponseInfo();
            try
            {
                DateTime todayDate = Convert.ToDateTime(DateTime.Now);
                decimal CompanyID = Request.CompanyID();

                var AppointmentList = AppointmentInfo(CompanyID);

                var GenderType = _sys_drop_down_valueService.Queryable().Where(e => e.DropDownID == 2).ToList();
                var BloodList = _sys_drop_down_valueService.Queryable().Where(e => e.DropDownID == 17).ToList();

                var StatusList = _sys_drop_down_valueService.Queryable().Where(a => a.DropDownID == 1 && (a.CompanyID == CompanyID || a.CompanyID == null)).Select(m => new
                {
                    ID = m.ID,
                    Value = m.Value
                }).ToList();

                var BillTypeList = _sys_drop_down_valueService.Queryable().Where(a => a.DropDownID == 22 && (a.CompanyID == CompanyID || a.CompanyID == null)).Select(m => new
                {
                    ID = m.ID,
                    Value = m.Value
                }).ToList();

                var TittleList = _sys_drop_down_valueService.Queryable().Where(a => a.DropDownID == 23 && (a.CompanyID == CompanyID || a.CompanyID == null)).Select(m => new
                {
                    ID = m.ID,
                    Value = m.Value
                }).ToList();
                decimal UserID = Request.LoginID();
                var DoctorList = _service.DoctorList(CompanyID, UserID);

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
                string MRNO = CompanyID.ToString();
                decimal? maxid = _service.Queryable().Where(a => a.CompanyId == CompanyID).Max(a => (decimal?)a.ID);

                var getMaxVal = _service.Queryable().Where(a => a.CompanyId == CompanyID && a.ID == maxid).Select(a => new { a.ID }).FirstOrDefault();
                MRNO = ((getMaxVal.ID) + 1).ToString("0000000000");

                var ServiceType = _emr_service_mfService.Queryable().Where(e => e.CompanyId == CompanyID && e.ServiceName == "Consultation")
                  .Select(a => new { a.ID, a.ServiceName, a.Price }).FirstOrDefault();
                decimal TokenNo = 1;
                if (_emr_appointment_mfService.Queryable().Where(a => a.CompanyId == CompanyID && DbFunctions.TruncateTime(a.AppointmentDate) == DbFunctions.TruncateTime(todayDate)).Count() > 0)
                    TokenNo = _emr_appointment_mfService.Queryable().Where(a => a.CompanyId == CompanyID && DbFunctions.TruncateTime(a.AppointmentDate) == DbFunctions.TruncateTime(todayDate)).Max(e => e.TokenNo) + 1;


                objResponse.IsSuccess = true;
                objResponse.ResultSet = new
                {
                    MRNO = MRNO,
                    GenderList = GenderType,
                    DoctorList = DoctorList.DoctList,
                    AllStatusList = StatusList,
                    BloodList = BloodList,
                    DoctorCalander = DoctorCalander,
                    BillTypeList = BillTypeList,
                    TittleList = TittleList,
                    ServiceType = ServiceType,
                    TokenNo = TokenNo,
                    AppointmentList = AppointmentList,
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
