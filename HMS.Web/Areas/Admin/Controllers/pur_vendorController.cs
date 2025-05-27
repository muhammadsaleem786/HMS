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
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HMS.Web.API.Areas.Admin.Controllers
{
    [JwtAuthentication]
    public class pur_vendorController : ApiController, IERPAPIInterface<pur_vendor>, IDisposable
    {
        private readonly IStoredProcedureService _procedureService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly Ipur_vendorService _service;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iadm_companyService _adm_companyService;
        public pur_vendorController(IUnitOfWorkAsync unitOfWorkAsync, Ipur_vendorService Service,
        Isys_drop_down_valueService sys_drop_down_valueService,
        Iadm_companyService iadm_companyService,
        IStoredProcedureService ProcedureService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _service = Service;
            _procedureService = ProcedureService;
            _adm_companyService = iadm_companyService;
        }
        public async Task<ResponseInfo> Save(pur_vendor Model)
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
                decimal ID = 1;
                if (_service.Queryable().Count() > 0)
                    ID = _service.Queryable().Max(e => e.ID) + 1;
                Model.ID = ID;
                Model.CompanyID = Request.CompanyID();
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
        public async Task<ResponseInfo> Update(pur_vendor Model)
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
                Model.adm_user_mf = null;
                Model.adm_user_mf1 = null;
                Model.adm_company = null;
                decimal CompanyID = Request.CompanyID();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Modified;
                _service.Update(Model);
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;
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
        [ActionName("Delete")]
        public async Task<ResponseInfo> Delete(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal[] IdList = Id.Split(',').Select(decimal.Parse).ToArray();
                pur_vendor Model = _service.Queryable().Where(e => e.ID.ToString() == Id && e.CompanyID == CompanyID).FirstOrDefault();

                if (Model == null)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
                }
                Model.ObjectState = ObjectState.Deleted;
                _service.Delete(Model);
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
        public ResponseInfo GetList()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal companyID = Request.CompanyID();
                objResponse.ResultSet = _service.Queryable().Where(x => x.CompanyID == companyID).Select(s => new
                {
                    s.ID,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    CompanyName = s.CompanyName,
                    VendorEmail = s.VendorEmail,
                    VendorPhone = s.VendorPhone,
                }).ToList();
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
        public ResponseInfo LoadDropdown(string DropdownIds)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();

                List<sys_drop_down_value> dropdownValues = new List<sys_drop_down_value>();
                var Ids = DropdownIds.Split(',').Select(s => int.Parse(s)).ToArray();
                dropdownValues = _sys_drop_down_valueService.Queryable().Where(e => (e.CompanyID == null || e.CompanyID == CompanyID) && Ids.Contains(e.DropDownID)).ToList();
                if (dropdownValues != null)
                {

                    objResponse.ResultSet = new
                    {
                        dropdownValues = dropdownValues,

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
                int userid = Convert.ToInt32(Request.LoginID());
                var result = _service.Queryable().Where(e => e.CompanyID == CompanyID && e.ID.ToString() == Id).Include(a => a.adm_company).Include(a => a.adm_user_mf).FirstOrDefault();
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
        public ResponseInfo GetById(string Id, int NextPreviousIndex)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                objResponse.ResultSet = _service.Queryable().Where(e => e.CompanyID == CompanyID && e.ID.ToString() == Id).FirstOrDefault();
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
        public ResponseInfo ExportData(int ExportType, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText)
        {
            throw new NotImplementedException();
        }
        public ResponseInfo Load()
        {
            throw new NotImplementedException();
        }

        public PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            throw new NotImplementedException();
        }
    }
}
