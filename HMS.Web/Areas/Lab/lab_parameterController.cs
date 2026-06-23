using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
using HMS.Service;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Lab;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using HMS.Web.API.Interface;
using NPOI.SS.Formula.Functions;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace HMS.Web.API.Areas.Lab.Controllers
{
    [JwtAuthentication]
    public class lab_parameterController : ApiController, IERPAPIInterface<lab_parameter>, IDisposable
    {
        private readonly Ilab_parameterService _service;
        private readonly Ilab_catalogService _lab_catalogService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly IStoredProcedureService _procedureService;
        private readonly Iadm_role_dtService _adm_role_dtService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iadm_user_companyService _adm_user_companyService;
        public lab_parameterController(IUnitOfWorkAsync unitOfWorkAsync, Ilab_parameterService Service,
            Isys_drop_down_valueService sys_drop_down_valueService, Iadm_user_companyService adm_user_companyService,
            IStoredProcedureService ProcedureService, Iadm_role_dtService adm_role_dtService, Ilab_catalogService lab_catalogService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _service = Service;
            _adm_user_companyService = adm_user_companyService;
            _procedureService = ProcedureService;
            _adm_role_dtService = adm_role_dtService;
            _lab_catalogService = lab_catalogService;
        }

        public async Task<ResponseInfo> Save(lab_parameter Model)
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
                if (Model.UnitId != 0)
                {
                    Model.UnitDropdownId = (int)sys_dropdown_mfEnum.LabUnitDropdownId;
                }
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
                    var resultObj = _lab_catalogService.Queryable().Where(e => e.CompanyId == CompanyID)
                        .Select(z => new
                        {
                            code = z.ID,
                            name = z.Name,
                            price = z.Price,
                            category = z.sys_drop_down_value.Value,
                            sample = z.sys_drop_down_value1.Value,
                            tat = z.TAT,
                            parameterCount = z.lab_parameter.Count(),
                            catalogParameter = z.lab_parameter.Select(c => new
                            {
                                Id = c.ID,
                                name = c.ParameterName,
                                unit = c.sys_drop_down_value.Value,
                                range = c.ReferenceRange,
                                code = c.CatalogId
                            }).ToList()
                        }).ToList();

                    objResponse.ResultSet = new
                    {
                        resultObj = resultObj
                    };
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
        [HttpPost]
        [HttpGet]
        [ActionName("UnitSearch")]
        public async Task<ResponseInfo> UnitSearch(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var result = _sys_drop_down_valueService.Queryable().AsNoTracking().

                    Where(a =>
                     (a.DropDownID == 69) &&
        (
            a.CompanyID == CompanyID ||
            a.CompanyID == null
        ) && a.Value.StartsWith(term))

                    .Select(z => new
                    {
                        value = z.ID,
                        label = z.Value,

                    }).Take(20).ToList();
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
        [ActionName("Load")]
        public ResponseInfo Load()
        {

            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var result = _sys_drop_down_valueService.Queryable().Where(e => (e.CompanyID == null || e.CompanyID == CompanyID) && e.DropDownID == 69).Select(a => new { a.ID, a.Value, DropDownID = a.DropDownID }).ToList();
                var resultObj = _service.Queryable().Where(e => e.CompanyId == CompanyID)
                    .Select(z => new
                    {
                        code = z.CatalogId,
                        name = z.ParameterName,
                        unit = z.sys_drop_down_value.Value,
                        range = z.ReferenceRange,
                    }).ToList();

                objResponse.ResultSet = new
                {
                    result = result,
                    resultObj = resultObj
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
        [ActionName("GetByCatalogId")]
        public ResponseInfo GetByCatalogId(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var query = _service.Queryable()
                            .Include(a => a.sys_drop_down_value)
                            .Where(e => e.CompanyId == CompanyID);
                if (Id != "0")
                {
                    query = query.Where(e => e.CatalogId.ToString() == Id);
                }

                var result = query.ToList();

                objResponse.ResultSet = new
                {
                    result = result
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
        [HttpGet]
        [ActionName("GetById")]
        public ResponseInfo GetById(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var result = _service.Queryable().Include(a => a.sys_drop_down_value).Where(e => e.CompanyId == CompanyID && e.ID.ToString() == Id).FirstOrDefault();
                objResponse.ResultSet = new
                {
                    result = result
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
        public async Task<ResponseInfo> Update(lab_parameter Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                if (!ModelState.IsValid)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.BadRequest;
                    return objResponse;
                }
                if (Model.UnitId != 0)
                {
                    Model.UnitDropdownId = (int)sys_dropdown_mfEnum.LabUnitDropdownId;
                }
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Modified;
                Model.sys_drop_down_value = null;
                _service.Update(Model);
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;
                    objResponse.IsSuccess = true;
                    var resultObj = _lab_catalogService.Queryable().Where(e => e.CompanyId == CompanyID)
                        .Select(z => new
                        {
                            code = z.ID,
                            name = z.Name,
                            price = z.Price,
                            category = z.sys_drop_down_value.Value,
                            sample = z.sys_drop_down_value1.Value,
                            tat = z.TAT,
                            parameterCount = z.lab_parameter.Count(),
                            catalogParameter = z.lab_parameter.Select(c => new
                            {
                                Id = c.ID,
                                name = c.ParameterName,
                                unit = c.sys_drop_down_value.Value,
                                range = c.ReferenceRange,
                                code = c.CatalogId
                            }).ToList()
                        }).ToList();

                    objResponse.ResultSet = new
                    {
                        resultObj = resultObj
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

        [HttpGet]
        [ActionName("Delete")]
        public async Task<ResponseInfo> Delete(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal[] IdList = Id.Split(',').Select(decimal.Parse).ToArray();

                List<lab_parameter> Models = _service.Queryable().Where(e => e.CompanyId == CompanyID && IdList.Contains(e.ID)).ToList();

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
