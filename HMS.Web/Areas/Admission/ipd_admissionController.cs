using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Admission;
using HMS.Service.Services.Appointment;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using HMS.Web.API.Interface;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HMS.Web.API.Areas.Admission.Controllers
{
    [JwtAuthentication]
    public class ipd_admissionController : ApiController, IERPAPIInterface<ipd_admission>, IDisposable
    {
        private readonly Iipd_admissionService _service;
        private readonly Iemr_appointment_mfService _emr_appointment_mfService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly Iemr_patient_billService _emr_patient_billService;
        private readonly Iemr_service_mfService _emr_service_mfService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iemr_patientService _emr_patientService;
        private readonly Iadm_companyService _adm_companyService;
        private readonly Iadm_userService _adm_userService;

        public ipd_admissionController(IUnitOfWorkAsync unitOfWorkAsync, Iemr_service_mfService emr_service_mfService, Iemr_patient_billService emr_patient_billService, Iipd_admissionService service, Iemr_appointment_mfService emr_appointment_mfService,
           Iadm_userService adm_userService, Isys_drop_down_valueService sys_drop_down_valueService, Iemr_patientService emr_patientService, Iadm_companyService adm_companyService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = service;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _emr_appointment_mfService = emr_appointment_mfService;
            _emr_patient_billService = emr_patient_billService;
            _emr_service_mfService = emr_service_mfService;
            _emr_patientService = emr_patientService;
            _adm_companyService = adm_companyService;
            _adm_userService = adm_userService;
        }
        [HttpPost]
        [HttpGet]
        [ActionName("Save")]
        public async Task<ResponseInfo> Save(ipd_admission Model)
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
                emr_patient_mf patientModel = Model.emr_patient_mf;
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                decimal ID = 1;
                if (_service.Queryable().Count() > 0)
                    ID = (_service.Queryable().Where(e => e.CompanyId == CompanyID).Max(x => (decimal?)x.ID) ?? 0) + 1;
                Model.ID = ID;
                int sequenceNumber = (int)ID;
                var date = DateTime.Now.ToString("yy");
                var admissionNo = $"IP-{date}{sequenceNumber:D4}";
                Model.AdmissionNo = admissionNo;
                Model.AdmissionTypeDropdownId = (int)sys_dropdown_mfEnum.AdmissionTypeDropdownId;
                if (Model.WardTypeId != null)
                {
                    Model.WardTypeDropdownId = (int)sys_dropdown_mfEnum.WardTypeDropdownId;
                }
                if (Model.BedId != null)
                {
                    Model.BedDropdownId = (int)sys_dropdown_mfEnum.BedDropdownId;
                }
                if (Model.RoomId != null)
                {
                    Model.RoomDropdownId = (int)sys_dropdown_mfEnum.RoomDropdownId;
                }
                Model.CompanyId = Request.CompanyID();
                Model.CreatedBy = Request.LoginID();
                Model.CreatedDate = Request.DateTimes();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.emr_patient_mf = null;
                Model.ObjectState = ObjectState.Added;
                _service.Insert(Model);
                decimal AppID = 1;
                var findAppointment = _emr_appointment_mfService.Queryable().Where(a => a.CompanyId == CompanyID && a.PatientId == Model.PatientId && DbFunctions.TruncateTime(a.AppointmentDate) == DbFunctions.TruncateTime(Model.AdmissionDate)).FirstOrDefault();
                if (findAppointment == null && Model.DoctorId != 0 && Model.DoctorId != null)
                {
                    emr_appointment_mf appoint = new emr_appointment_mf();
                    if (_emr_appointment_mfService.Queryable().Count() > 0)
                        AppID = (_emr_appointment_mfService.Queryable().Where(e => e.CompanyId == CompanyID).Max(x => (decimal?)x.ID) ?? 0) + 1;
                    appoint.ID = AppID;
                    appoint.AdmissionId = Model.ID;
                    appoint.PatientId = Model.PatientId;
                    appoint.DoctorId = Model.DoctorId;
                    appoint.AppointmentDate = Model.AdmissionDate;
                    appoint.AppointmentTime = Model.AdmissionTime;
                    appoint.IsAdmit = true;
                    appoint.StatusId = 25;
                    appoint.IsAdmission = true;
                    appoint.TokenNo = GetNextToken(CompanyID);
                    appoint.CompanyId = Request.CompanyID();
                    appoint.CreatedBy = Request.LoginID();
                    appoint.CreatedDate = Request.DateTimes();
                    appoint.ModifiedBy = Request.LoginID();
                    appoint.ModifiedDate = Request.DateTimes();
                    appoint.emr_patient_mf = null;
                    appoint.ObjectState = ObjectState.Added;
                    _emr_appointment_mfService.Insert(appoint);
                }
                //patient update
                patientModel.ModifiedBy = Request.LoginID();
                patientModel.ModifiedDate = Request.DateTimes();
                patientModel.ObjectState = ObjectState.Modified;
                patientModel.emr_appointment_mf = null;
                patientModel.emr_patient_bill = null;
                patientModel.emr_prescription_mf = null;
                patientModel.ipd_admission = null;
                patientModel.pur_sale_mf = null;
                _emr_patientService.Update(patientModel);
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Save;
                    objResponse.IsSuccess = true;
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

        [HttpPut]
        [HttpGet]
        [ActionName("Update")]
        public async Task<ResponseInfo> Update(ipd_admission Model)
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
                emr_patient_mf patientModel = Model.emr_patient_mf;
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                if (Model.WardTypeId != null)
                {
                    Model.WardTypeDropdownId = (int)sys_dropdown_mfEnum.WardTypeDropdownId;
                }
                if (Model.BedId != null)
                {
                    Model.BedDropdownId = (int)sys_dropdown_mfEnum.BedDropdownId;
                }
                if (Model.RoomId != null)
                {
                    Model.RoomDropdownId = (int)sys_dropdown_mfEnum.RoomDropdownId;
                }
                Model.emr_patient_mf = null;
                Model.ObjectState = ObjectState.Modified;
                _service.Update(Model);

                //patient update
                patientModel.ModifiedBy = Request.LoginID();
                patientModel.ModifiedDate = Request.DateTimes();
                patientModel.ObjectState = ObjectState.Modified;
                patientModel.emr_appointment_mf = null;
                patientModel.emr_patient_bill = null;
                patientModel.emr_prescription_mf = null;
                patientModel.ipd_admission = null;
                patientModel.pur_sale_mf = null;
                _emr_patientService.Update(patientModel);
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;
                    objResponse.IsSuccess = true;
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
                var GenderType = _sys_drop_down_valueService.Queryable().Where(e => (e.CompanyID == null || e.CompanyID == CompanyID) && e.DropDownID == 2).ToList();
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
                int ID = Convert.ToInt32(Id);
                var obj = _service.Queryable().Where(a => a.CompanyId == CompanyID && a.ID == ID).Include(a => a.emr_patient_mf).FirstOrDefault();
                objResponse.IsSuccess = true;
                objResponse.ResultSet = new
                {
                    obj = obj,
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
                objResponse.ResultSet = _service.Queryable().Where(e => e.ID.ToString() == Id && e.CompanyId == CompanyID).FirstOrDefault();
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
        public async Task<ResponseInfo> Delete(string Id, string VitalId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal ID = Convert.ToDecimal(Id);
                ipd_admission model = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID == ID).FirstOrDefault();

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
                decimal companyId = Request.CompanyID();
                int userid = Convert.ToInt32(Request.LoginID());
                var dropDownIds = new[] { 2, 17, 1, 22, 23, 25, 30 };
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
   .Where(x => x.DropDownID == 2).ToList();
                var bloodList = allDropDownValues
    .Where(x => x.DropDownID == 17).ToList();
                var statusList = allDropDownValues
    .Where(x => x.DropDownID == 1)
    .ToList();
                var bloodTypeList = allDropDownValues
    .Where(x => x.DropDownID == 22)
    .ToList();
                var titleList = allDropDownValues
   .Where(x => x.DropDownID == 23)
   .ToList();
                var admissionList = allDropDownValues
   .Where(x => x.DropDownID == 25)
   .ToList();
                var locationList = allDropDownValues
   .Where(x => x.DropDownID == 30)
   .ToList();
                var isShowDoctorIds = _adm_userService.Queryable()
.Where(x => x.ID == userid)
.Select(x => x.IsShowDoctor)
.FirstOrDefault();
                var admissionNo = _service.Queryable()
    .Where(a => a.CompanyId == companyId && DbFunctions.TruncateTime(a.AdmissionDate) == DbFunctions.TruncateTime(DateTime.Now))
    .Max(a => (int?)a.ID) ?? 0;

                admissionNo += 1;
                var mrNo = _emr_patientService.Queryable()
    .Where(p => p.CompanyId == companyId)
    .Max(p => (int?)p.ID) ?? 0;
                mrNo += 1;


                var ht = new Hashtable();
                ht.Add("@CompanyId", companyId);
                ht.Add("@UserId", userid);
                DataAccessManager dataAccessManager = new DataAccessManager();
                var doctorData = dataAccessManager.GetDataSet("SP_GetDoctorList", ht);
                var GenderType = genderList;
                var BloodList = bloodList;
                var StatusList = statusList;
                var BillTypeList = bloodTypeList;
                var TittleList = titleList;
                var AdmissionTypeList = admissionList;
                var AdmissionLocation = locationList;
                var DoctorList = doctorData.Tables[0];
                decimal[] IdList = null;
                var isShowIds = isShowDoctorIds;
                if (!string.IsNullOrWhiteSpace(isShowIds))
                    IdList = isShowIds.Split(',').Select(decimal.Parse).ToArray();
                var DoctorCalander = doctorData.Tables[0];
                var AdmissionNo = admissionNo;

                decimal getMaxMRNO = mrNo;
                string MRNO = "";
                MRNO = (getMaxMRNO).ToString("0000000000");

                objResponse.ResultSet = new
                {
                    MRNO = MRNO,
                    GenderList = GenderType,
                    AdmissionNo = AdmissionNo,
                    DoctorList = DoctorList,
                    AllStatusList = StatusList,
                    BloodList = BloodList,
                    DoctorCalander = DoctorCalander,
                    BillTypeList = BillTypeList,
                    TittleList = TittleList,
                    AdmissionTypeList = AdmissionTypeList,
                    AdmissionLocation = AdmissionLocation,
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
        [ActionName("WardDropDown")]
        public ResponseInfo WardDropDown(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal companyId = Request.CompanyID();
                decimal UserID = Request.LoginID();
                int ID = Convert.ToInt32(Id);
                var obj = _service.Queryable().Where(a => a.CompanyId == companyId && a.ID == ID).Include(a => a.emr_patient_mf).FirstOrDefault();

                var dropDownIds = new[] { 33, 34, 35 };
                var allDropDownValues = _sys_drop_down_valueService.Queryable()
    .Where(x => dropDownIds.Contains(x.DropDownID)
                && (x.CompanyID == companyId || x.CompanyID == null)).Select(z => new
                {
                    z.ID,
                    z.Value,
                    z.DropDownID,
                    z.DependedDropDownValueID,
                })
    .ToList();
                var wardList = allDropDownValues
    .Where(x => x.DropDownID == 33).ToList();
                var roomList = allDropDownValues
  .Where(x => x.DropDownID == 34).ToList();
                var bedList = allDropDownValues
  .Where(x => x.DropDownID == 35).ToList();

                var WardList = wardList;
                var RoomList = roomList;
                var BedList = bedList;


                objResponse.ResultSet = new
                {
                    RoomList = RoomList,
                    WardList = WardList,
                    BedList = BedList,
                    obj = obj,
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
        [ActionName("Delete")]
        public async Task<ResponseInfo> Delete(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal ID = Convert.ToDecimal(Id);
                ipd_admission model = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID == ID).FirstOrDefault();

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
        [HttpPut]
        [HttpGet]
        [ActionName("DischargePatient")]
        public async Task<ResponseInfo> DischargePatient(string Id)
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
                decimal ID = Convert.ToDecimal(Id);
                decimal CompanyID = Request.CompanyID();
                var findIPD = _service.Queryable().Where(a => a.ID == ID && a.CompanyId == CompanyID).FirstOrDefault();
                if (findIPD != null)
                {
                    findIPD.DischargeDate = DateTime.Now;
                    findIPD.DischargeTime = DateTime.Now.TimeOfDay;
                    _service.Update(findIPD);
                }
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;
                    objResponse.IsSuccess = true;
                }
                catch (DbUpdateException)
                {
                    if (!ModelExists(findIPD.ID.ToString()))
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
        [ActionName("PaginationWithParm")]
        public PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResult = _service.PaginationWithParm(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, FilterID, IgnorePaging);
                var categoryList = _sys_drop_down_valueService.Queryable().Where(e => (e.CompanyID == null || e.CompanyID == CompanyID) && e.DropDownID == 25).Select(z => new
                {
                    z.ID,
                    z.Value,
                }).ToList();
                objResult.OtherDataModel = categoryList;

            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }

    }
}
