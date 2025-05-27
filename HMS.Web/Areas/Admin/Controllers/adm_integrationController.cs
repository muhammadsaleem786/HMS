using HMS.Entities.CustomModel;
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
    public class adm_integrationController : ApiController, IERPAPIInterface<adm_integration>, IDisposable
    {
        private readonly Iadm_integrationService _service;
        private readonly IStoredProcedureService _procedureService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        public adm_integrationController(IUnitOfWorkAsync unitOfWorkAsync, Iadm_integrationService Service,
               IStoredProcedureService ProcedureService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = Service;
            _procedureService = ProcedureService;
        }
        public async Task<ResponseInfo> Save(adm_integration Model)
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
                decimal ID = 1;
                if (_service.Queryable().Count() > 0)
                    ID = _service.Queryable().Max(e => e.ID) + 1;
                Model.ID = ID;
                Model.CompanyId = CompanyID;
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
        [HttpGet]
        [ActionName("Load")]
        public ResponseInfo Load()
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var Integrationlist = _service.Queryable().Where(e => e.CompanyId == CompanyID).ToList();
                objResponse.ResultSet = new
                {
                    Integrationlist = Integrationlist
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
                var CompanyID = Request.CompanyID();
                var AllScreen = _procedureService.GetAllScreen();
                var rolelist = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID.ToString() == Id).FirstOrDefault();
                objResponse.ResultSet = new
                {
                    AllScreen = AllScreen,
                    rolelist = rolelist
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
        public async Task<ResponseInfo> Update(adm_integration Model)
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
                Model.UserName = Model.UserName;
                Model.Password = Model.Password;
                Model.Masking = Model.Masking;
                Model.SMTP = Model.SMTP;
                Model.PortNo = Model.PortNo;
                Model.IsActive = Model.IsActive;
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

                List<adm_integration> Models = _service.Queryable().Where(e => e.CompanyId == CompanyID && IdList.Contains(e.ID)).ToList();

                if (Models.Count() == 0)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
                }

                foreach (var Model in Models)
                {
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
    }
}
