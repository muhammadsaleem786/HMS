using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
using HMS.Service;
using HMS.Service.Services.Admin;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using HMS.Web.API.Interface;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace HMS.Web.API.Areas.Admin.Controllers
{
    [JwtAuthentication]
    public class adm_reminder_mfController : ApiController, IERPAPIInterface<adm_reminder_mf>, IDisposable
    {
        private readonly Iadm_reminder_mfService _service;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly IStoredProcedureService _procedureService;
        private readonly Iadm_reminder_dtService _adm_reminder_dtService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iadm_user_companyService _adm_user_companyService;
        public adm_reminder_mfController(IUnitOfWorkAsync unitOfWorkAsync, Iadm_reminder_mfService Service,
            Isys_drop_down_valueService sys_drop_down_valueService, Iadm_user_companyService adm_user_companyService,
            IStoredProcedureService ProcedureService, Iadm_reminder_dtService adm_reminder_dtService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _service = Service;
            _adm_user_companyService = adm_user_companyService;
            _procedureService = ProcedureService;
            _adm_reminder_dtService = adm_reminder_dtService;
        }

        public async Task<ResponseInfo> Save(adm_reminder_mf Model)
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
                adm_reminder_mf reminder_mf = _service.Queryable().Where(x => x.CompanyId == CompanyID && x.Name.ToLower() == Model.Name.ToLower()).FirstOrDefault();
                if (reminder_mf == null)
                {
                    decimal ID = 1;
                    if (_service.Queryable().Count() > 0)
                        ID = _service.Queryable().Max(e => e.ID) + 1;
                    List<adm_reminder_dt> reminderdtList = new List<adm_reminder_dt>();
                    reminderdtList.AddRange(Model.adm_reminder_dt);


                    Model.ID = ID;
                    Model.CompanyId = CompanyID;
                    Model.CreatedBy = Request.LoginID();
                    Model.CreatedDate = Request.DateTimes();
                    Model.ModifiedBy = Request.LoginID();
                    Model.ModifiedDate = Request.DateTimes();
                    Model.ObjectState = ObjectState.Added;
                    Model.adm_reminder_dt = null;
                    _service.Insert(Model);

                    decimal reminderdtID = 1;
                    if (_adm_reminder_dtService.Queryable().Count() > 0)
                        reminderdtID = _adm_reminder_dtService.Queryable().Max(e => e.ID) + 1;
                    foreach (adm_reminder_dt item in reminderdtList)
                    {
                        item.ID = reminderdtID;
                        item.ReminderId = Model.ID;
                        item.CompanyId = CompanyID;
                        item.SMSTypeDropDownId = (int)sys_dropdown_mfEnum.SMSTypeDropDown;
                        item.TimeTypeDropDownId = (int)sys_dropdown_mfEnum.TimeTypeDropDown;
                        item.CreatedBy = Request.LoginID();
                        item.CreatedDate = Request.DateTimes();
                        item.ModifiedBy = Request.LoginID();
                        item.ModifiedDate = Request.DateTimes();
                        item.ObjectState = ObjectState.Added;
                        _adm_reminder_dtService.Insert(item);
                        reminderdtID++;
                    }

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
                else
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = Model.Name + " name already exist";
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
        public ResponseInfo GetList()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal companyID = Request.CompanyID();
                objResponse.ResultSet = _service.Queryable().Where(x => x.CompanyId == companyID).ToList();
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
                objResponse.ResultSet = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID.ToString() == Id).Include(a => a.adm_reminder_dt).FirstOrDefault();

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
                objResponse.ResultSet = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID.ToString() == Id).FirstOrDefault();
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
        [HttpPut]
        [HttpGet]
        [ActionName("Update")]
        public async Task<ResponseInfo> Update(adm_reminder_mf Model)
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
                adm_reminder_mf reminder_mf = _service.Queryable().Where(x => x.CompanyId == CompanyID && x.ID != Model.ID && x.Name.ToLower() == Model.Name.ToLower()).FirstOrDefault();
                if (reminder_mf == null)
                {
                    List<adm_reminder_dt> reminderdtList = new List<adm_reminder_dt>();
                    reminderdtList.AddRange(Model.adm_reminder_dt);
                    Model.adm_reminder_dt = null;
                    Model.ModifiedBy = Request.LoginID();
                    Model.ModifiedDate = Request.DateTimes();
                    Model.ObjectState = ObjectState.Modified;
                    _service.Update(Model);
                    var DelModels = _adm_reminder_dtService.Queryable().Where(e => e.ReminderId == Model.ID && e.CompanyId == CompanyID).ToList();

                    foreach (var obj in DelModels)
                    {
                        obj.ObjectState = ObjectState.Deleted;
                        _adm_reminder_dtService.Delete(obj);
                    }

                    decimal reminderdtID = 1;
                    if (_adm_reminder_dtService.Queryable().Count() > 0)
                        reminderdtID = _adm_reminder_dtService.Queryable().Max(e => e.ID) + 1;

                    foreach (adm_reminder_dt item in reminderdtList)
                    {
                        item.ReminderId = Model.ID;
                        item.CompanyId = Request.CompanyID();
                        item.ID = reminderdtID;
                        item.SMSTypeDropDownId = (int)sys_dropdown_mfEnum.SMSTypeDropDown;
                        item.TimeTypeDropDownId = (int)sys_dropdown_mfEnum.TimeTypeDropDown;
                        item.CreatedBy = Request.LoginID();
                        item.CreatedDate = Request.DateTimes();
                        item.ModifiedBy = Request.LoginID();
                        item.ModifiedDate = Request.DateTimes();
                        item.ObjectState = ObjectState.Added;
                        _adm_reminder_dtService.Insert(item);
                        reminderdtID++;
                    }

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
                else
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = Model.Name + "name already exist";
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
        [ActionName("Delete")]
        public async Task<ResponseInfo> Delete(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal[] IdList = Id.Split(',').Select(decimal.Parse).ToArray();
                List<adm_reminder_mf> Models = _service.Queryable().Where(e => e.CompanyId == CompanyID && IdList.Contains(e.ID)).Include(x => x.adm_reminder_dt).ToList();

                if (Models.Count() == 0)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
                }

                foreach (var Model in Models)
                {
                    List<adm_reminder_dt> reminderdtModels = Model.adm_reminder_dt.ToList();
                    foreach (var item in reminderdtModels)
                    {
                        item.ObjectState = ObjectState.Deleted;
                        _adm_reminder_dtService.Delete(item);
                    }

                    Model.ObjectState = ObjectState.Deleted;
                    _service.Delete(Model);
                }

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

        [AllowAnonymous]
        [HttpGet]
        [ActionName("GetAllScreens")]
        public ResponseInfo GetAllScreens()
        {
            var objResponse = new ResponseInfo();
            try
            {
                var AllScreen = _procedureService.GetAllScreen();
                objResponse.ResultSet = _procedureService.GetAllScreen();
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
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
        public PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            throw new NotImplementedException();
        }
        [HttpGet]
        [ActionName("LoadDropdown")]
        public ResponseInfo LoadDropdown(string DropdownIds)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var Ids = DropdownIds.Split(',').Select(s => int.Parse(s)).ToArray();
                var result = _sys_drop_down_valueService.Queryable().Where(e => (e.CompanyID == null || e.CompanyID == CompanyID) && Ids.Contains(e.DropDownID))
                     .Select(z => new
                     {
                         z.ID,
                         z.Value,
                         z.DropDownID
                     }).ToList();

                if (result != null)
                {

                    objResponse.ResultSet = new
                    {
                        dropdownValues = result,

                    };
                    objResponse.IsSuccess = true;
                }
                else
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = "Email does not exist";
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

    }
}
