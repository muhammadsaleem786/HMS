using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Employee;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using HMS.Web.API.Interface;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace HMS.Web.API.Areas.Employee
{
    [JwtAuthentication]
    public class sys_holidaysController : ApiController, IERPAPIInterface<sys_holidays>, IDisposable
    {
        private readonly Isys_holidaysService _service;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        public sys_holidaysController(IUnitOfWorkAsync unitOfWorkAsync, Isys_holidaysService Service,
            Isys_drop_down_valueService sys_drop_down_valueService
           , Iadm_role_dtService adm_role_dtService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = Service;
        }
        public async Task<ResponseInfo> Save(sys_holidays Model)
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
                sys_holidays sys_holiday = _service.Queryable().Where(x => x.CompanyID == CompanyID && x.HolidayName.ToLower() == Model.HolidayName.ToLower()).FirstOrDefault();
                if (sys_holiday == null)
                {
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
                else
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = Model.HolidayName + " already exist";
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
                objResponse.ResultSet = _service.Queryable().Where(x => x.CompanyID == companyID).ToList();
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
        public async Task<ResponseInfo> Update(sys_holidays Model)
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
                sys_holidays sys_holiday = _service.Queryable().Where(x => x.CompanyID == CompanyID && x.ID != Model.ID && x.HolidayName.ToLower() == Model.HolidayName.ToLower()).FirstOrDefault();
                if (sys_holiday == null)
                {

                    Model.ModifiedDate = Request.DateTimes();
                    Model.ModifiedBy = Request.LoginID();
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
                else
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = Model.HolidayName + " already exist";
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
                //decimal[] IdList = Id.Split(',').Select(decimal.Parse).ToArray();
                //int[] IdList = Id.Split(',').Select(int.Parse).ToArray();
                decimal id = Convert.ToDecimal(Id);
                sys_holidays Model = _service.Queryable()
                    .Where(e => e.CompanyID == CompanyID && e.ID == id).FirstOrDefault();

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
