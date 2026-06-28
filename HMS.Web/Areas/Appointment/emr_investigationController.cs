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

namespace HMS.Web.API.Areas.Appointment.Controllers
{
    [JwtAuthentication]
    public class emr_investigationController : ApiController, IERPAPIInterface<emr_investigation>, IDisposable
    {
        private readonly Iemr_investigationService _service;
        private readonly Iemr_appointment_mfService _emr_appointment_mfService;
        private readonly Iadm_user_companyService _adm_user_companyService;
        private readonly Iadm_role_mfService _adm_role_mfService;
        private readonly Iadm_user_tokenService _adm_user_tokenService;
        private readonly Isys_notification_alertService _sys_notification_alertService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly Iadm_userService _adm_userService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        public emr_investigationController(IUnitOfWorkAsync unitOfWorkAsync, Iemr_investigationService service, Iadm_userService adm_userService, Iemr_appointment_mfService emr_appointment_mfService, Iadm_user_companyService adm_user_companyService,
            Iadm_role_mfService adm_role_mfService, Iadm_user_tokenService adm_user_tokenService, Isys_drop_down_valueService sys_drop_down_valueService,
            Isys_notification_alertService sys_notification_alertService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = service; _sys_drop_down_valueService = sys_drop_down_valueService;
            _adm_user_companyService = adm_user_companyService;
            _adm_userService = adm_userService;
            _emr_appointment_mfService = emr_appointment_mfService;
            _adm_role_mfService = adm_role_mfService;
            _adm_user_tokenService = adm_user_tokenService;
            _sys_notification_alertService = sys_notification_alertService;
        }
        [HttpPost]
        [HttpGet]
        [ActionName("Save")]
        public async Task<ResponseInfo> Save(emr_investigation Model)
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
                if (Model.CategoryId != 0)
                {
                    Model.CategoryDropdownId = (int)sys_dropdown_mfEnum.LabCategoryDropdownId;
                }
                if (Model.SampleId != 0)
                {
                    Model.SampleDropdownId = (int)sys_dropdown_mfEnum.LabSampleDropdownId;
                }
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
                    var resultObj = _service.Queryable().Where(e => e.CompanyID == CompanyID)
                  .Select(z => new
                  {
                      ID = z.ID,
                      name = z.Investigation,
                      price = z.Price,
                      category = z.sys_drop_down_value.Value,
                      sample = z.sys_drop_down_value1.Value,
                      tat = z.TAT,
                      parameterCount = z.lab_parameter.Count(),
                      catalogParameter = z.lab_parameter.Select(c => new
                      {
                          Id = z.ID,
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
        [HttpPost]
        [HttpGet]
        [ActionName("CategorySearch")]
        public async Task<ResponseInfo> CategorySearch(string term, int dropdownId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var result = _sys_drop_down_valueService.Queryable().AsNoTracking().

                    Where(a =>
                     (a.DropDownID == dropdownId) &&
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

        [HttpPut]
        [HttpGet]
        [ActionName("Update")]
        public async Task<ResponseInfo> Update(emr_investigation Model)
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
                if (Model.CategoryId != 0)
                {
                    Model.CategoryDropdownId = (int)sys_dropdown_mfEnum.LabCategoryDropdownId;
                }
                if (Model.SampleId != 0)
                {
                    Model.SampleDropdownId = (int)sys_dropdown_mfEnum.LabSampleDropdownId;
                }
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Modified;
                Model.sys_drop_down_value = null;
                Model.sys_drop_down_value1 = null;
                _service.Update(Model);

                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;
                    objResponse.IsSuccess = true;
                    var resultObj = _service.Queryable().Where(e => e.CompanyID == CompanyID)
                  .Select(z => new
                  {
                      ID = z.ID,
                      name = z.Investigation,
                      price = z.Price,
                      category = z.sys_drop_down_value.Value,
                      sample = z.sys_drop_down_value1.Value,
                      tat = z.TAT,
                      parameterCount = z.lab_parameter.Count(),
                      catalogParameter = z.lab_parameter.Select(c => new
                      {
                          Id = z.ID,
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
                var CompanyID = Request.CompanyID();
                var Result = _service.Queryable().Include(a=>a.sys_drop_down_value).Include(a=>a.sys_drop_down_value1).Where(e => e.ID.ToString() == Id && e.CompanyID== CompanyID).FirstOrDefault();
                objResponse.ResultSet = new
                {
                    Result = Result,
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
                emr_investigation model = _service.Queryable().Where(e => e.CompanyID == CompanyID && e.ID == ID).FirstOrDefault();

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
                var result = _sys_drop_down_valueService.Queryable().Where(e => (e.CompanyID == null || e.CompanyID == CompanyID) && e.DropDownID == 68).Select(a => new { a.ID, a.Value, DropDownID = a.DropDownID }).ToList();
                var resultObj = _service.Queryable().Where(e => e.CompanyID == CompanyID)
                    .Select(z => new
                    {
                        ID = z.ID,
                        name = z.Investigation,
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
        [HttpGet]
        [ActionName("GetByCategoryId")]
        public ResponseInfo GetByCategoryId(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var query = _service.Queryable()
                            .Include(a => a.sys_drop_down_value)
                            .Where(e => e.CompanyID == CompanyID);
                if (Id != "0")
                {
                    query = query.Where(e => e.CategoryId.ToString() == Id);
                }

                var result = query.Select(z => new
                {
                    ID = z.ID,
                    name = z.Investigation,
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
        public PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            throw new NotImplementedException();
        }
    }
}
