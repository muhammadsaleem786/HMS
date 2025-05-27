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
    public class ipd_admission_labController : ApiController, IERPAPIInterface<ipd_admission_lab>, IDisposable
    {
        private readonly Iipd_admission_labService _service;
        private readonly Iemr_appointment_mfService _emr_appointment_mfService;
        private readonly Iemr_patientService _emr_patientService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        public ipd_admission_labController(IUnitOfWorkAsync unitOfWorkAsync, Iipd_admission_labService service,
            Iemr_patientService emr_patientService,
            Iemr_appointment_mfService emr_appointment_mfService,
            Isys_drop_down_valueService sys_drop_down_valueService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = service;
            _emr_patientService = emr_patientService;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _emr_appointment_mfService = emr_appointment_mfService;
        }
        [HttpPost]
        [HttpGet]
        [ActionName("Save")]
        public async Task<ResponseInfo> Save(ipd_admission_lab Model)
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
                decimal UserID = Request.LoginID();

                decimal ID = 1;
                if (_service.Queryable().Count() > 0)
                    ID = _service.Queryable().Max(e => e.ID) + 1;
                Model.ID = ID;
                Model.LabTypeDropdownId = (int)sys_dropdown_mfEnum.LabTypeDropdownId;
                Model.StatusDropdownId = (int)sys_dropdown_mfEnum.StatusDropdownId;
                Model.ResultDropdownId = (int)sys_dropdown_mfEnum.ResultDropdownId;
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
        [HttpPut]
        [HttpGet]
        [ActionName("Update")]
        public async Task<ResponseInfo> Update(ipd_admission_lab Model)
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
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Modified;
                _service.Update(Model);

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
        [ActionName("GetLabDropDown")]
        public ResponseInfo GetLabDropDown(string PatientId)
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


                var LabTypeList = AllData.Tables[20];
                var StatusList = AllData.Tables[17];
                var ResultList = AllData.Tables[18];

                var patientInfo = _emr_patientService.Queryable().Where(a => a.CompanyId == CompanyID && a.ID.ToString() == PatientId).Select(z => new
                {
                    z.PatientName,
                    z.ID,
                    DOB = z.DOB,
                    Gender = z.Gender == 1 ? "Male" : z.Gender == 2 ? "FeMale" : "Other",
                }).FirstOrDefault();

                objResponse.ResultSet = new
                {
                    LabTypeList = LabTypeList,
                    patientInfo = patientInfo,
                    StatusList = StatusList,
                    ResultList = ResultList,
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
        public ResponseInfo GetById(string Id, string PatientId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                int ID = Convert.ToInt32(Id);
                var Result = _service.Queryable().Where(e => e.ID == ID && e.CompanyId == CompanyID).FirstOrDefault();
               
                DataAccessManager dataAccessManager = new DataAccessManager();
                var ht = new Hashtable();
                ht.Add("@CompanyID", CompanyID);
                ht.Add("@UserId", UserID);
                var AllData = dataAccessManager.GetDataSet("SP_LoadDropdown", ht);
                var LabTypeList = AllData.Tables[20];
                var StatusList = AllData.Tables[17];
                var ResultList = AllData.Tables[18];


                var patientInfo = _emr_patientService.Queryable().Where(a => a.CompanyId == CompanyID && a.ID.ToString() == PatientId).Select(z => new
                {
                    z.PatientName,
                    z.ID,
                    DOB = z.DOB,
                    Gender = z.Gender == 1 ? "Male" : z.Gender == 2 ? "FeMale" : "Other",
                }).FirstOrDefault();
                objResponse.ResultSet = new
                {
                    Result = Result,
                    LabTypeList = LabTypeList,
                    patientInfo= patientInfo,
                    StatusList = StatusList,
                    ResultList = ResultList,
                };
                objResponse.IsSuccess = true;
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
        [ActionName("LabPagination")]
        public PaginationResult LabPagination(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string AdmitId,string PatientId, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResult = _service.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, AdmitId, PatientId, IgnorePaging);
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
                //var ObjList = Pagination(0, 0, VisibleColumnInfo, SortName, SortOrder, SearchText,0, true);
                //objResponse.FilePath = Documents.ExportWithType(ExportType, VisibleColumnInfo, ObjList.DataList);

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
                ipd_admission_lab model = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID == ID).FirstOrDefault();

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

            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }

        public ResponseInfo GetById(string Id)
        {
            throw new NotImplementedException();
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
