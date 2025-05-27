using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Service.Services.Admin;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace HMS.Web.API.Areas.Admin.Controllers
{
    [JwtAuthentication]
    public class sys_drop_downController : ApiController
    {
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly Isys_drop_down_mfService _service;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iadm_multilingual_mfService _adm_multilingual_mfservice;

        public sys_drop_downController(IUnitOfWorkAsync unitOfWorkAsync, Isys_drop_down_mfService drop_down_mfService, Isys_drop_down_valueService drop_down_valueService
             , Iadm_multilingual_mfService adm_multilingual_mfservice)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = drop_down_mfService;
            _adm_multilingual_mfservice = adm_multilingual_mfservice;
            _sys_drop_down_valueService = drop_down_valueService;
        }

        [AllowAnonymous]
        [HttpGet]
        //[ActionName("LoadDropdown")]
        [Route("api/admin/sys_drop_down/LoadDropdown")]
        public ResponseInfo LoadDropdown(string DropdownIds)
        {
            var objResponse = new ResponseInfo();
            try
            {
                List<sys_drop_down_value> dropdownValues = new List<sys_drop_down_value>();
                var Ids = DropdownIds.Split(',').Select(s => int.Parse(s)).ToArray();
                dropdownValues = _sys_drop_down_valueService.Queryable().Where(e => Ids.Contains(e.DropDownID)).ToList();
                if (dropdownValues != null)
                {
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = dropdownValues;
                    //objResponse.ErrorMessage = "User Name or Password is incorrect.";
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

        [HttpGet]
        [ActionName("Pagination")]
        public PaginationResult Pagination(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResult = _sys_drop_down_valueService.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }

        public async Task<ResponseInfo> Save(sys_drop_down_value Model)
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

                //decimal CompanyID = Request.CompanyID();
                //if (string.IsNullOrEmpty(Model.Value))
                //{
                //    //var DropDownValue = _sys_drop_down_valueService.Queryable().Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.AllowanceCategory && e.ID == Model.CategoryID).FirstOrDefault();
                //    //Model.Value = DropDownValue.Value;
                //}
                sys_drop_down_value Value = _sys_drop_down_valueService.Queryable()
                    .Where(x => x.DropDownID == Model.DropDownID && (Model.DependedDropDownValueID == null || x.DependedDropDownValueID == Model.DependedDropDownValueID) && (x.Value).ToLower() == Model.Value.ToLower()).FirstOrDefault();
                sys_drop_down_value Value1 = _sys_drop_down_valueService.Queryable().Where(a => a.ID == Model.DependedDropDownValueID).FirstOrDefault();
                if (Value == null)
                {
                    int ID = 1;
                    if (_sys_drop_down_valueService.Queryable().Where(x => x.DropDownID == Model.DropDownID).Count() > 0)
                        ID = _sys_drop_down_valueService.Queryable().Where(x => x.DropDownID == Model.DropDownID).Max(e => e.ID) + 1;

                    Model.ID = ID;
                    //Model.SystemGenerated = false;
                    Model.SystemGenerated = false;
                    // Model.DependedDropDownID = Value1.DropDownID;
                    Model.ObjectState = ObjectState.Added;
                    _sys_drop_down_valueService.Insert(Model);

                    //    Model.CompanyID = Request.CompanyID();
                    //    Model.CategoryDropDownID = (int)sys_dropdown_mfEnum.AllowanceCategory;
                    //    Model.CreatedBy = Request.LoginID();
                    //    Model.CreatedDate = Request.DateTimes();
                    //    Model.ModifiedBy = Request.LoginID();
                    //    Model.ModifiedDate = Request.DateTimes();
                    //    Model.ObjectState = ObjectState.Added;
                    //    _service.Insert(Model);

                    try
                    {
                        await _unitOfWorkAsync.SaveChangesAsync();
                        objResponse.Message = MessageStatement.Save;
                        objResponse.IsSuccess = true;

                        objResponse.ResultSet = GetValuesList(Model);
                    }
                    catch (DbUpdateException)
                    {
                        //if (ModelExists(Model.ID.ToString()))
                        //{
                        //    objResponse.IsSuccess = false;
                        //    objResponse.ErrorMessage = MessageStatement.Conflict;
                        //    return objResponse;
                        //}
                        throw;
                    }
                }
                else
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = Model.Value + " already exist. Please enter a different name and try again.";
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
        public async Task<ResponseInfo> Update(sys_drop_down_value Model)
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

                //decimal CompanyID = Request.CompanyID();
                //if (string.IsNullOrEmpty(Model.Value))
                //{
                //    //var DropDownValue = _sys_drop_down_valueService.Queryable().Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.AllowanceCategory && e.ID == Model.CategoryID).FirstOrDefault();
                //    //Model.Value = DropDownValue.Value;
                //}
                //sys_drop_down_value Value = _sys_drop_down_valueService.Queryable().Where(x => x.DropDownID == Model.DropDownID && x.DependedDropDownID == Model.DependedDropDownID && x.ID != Model.ID && (x.Value).ToLower() == Model.Value.ToLower()).FirstOrDefault();
                sys_drop_down_value Value = _sys_drop_down_valueService.Queryable()
                   .Where(x => x.DropDownID == Model.DropDownID && (Model.DependedDropDownValueID == null || x.DependedDropDownValueID == Model.DependedDropDownValueID) && x.ID != Model.ID && (x.Value).ToLower() == Model.Value.ToLower()).FirstOrDefault();
                if (Value == null)
                {
                    Model.ObjectState = ObjectState.Modified;
                    _sys_drop_down_valueService.Update(Model);

                    //    Model.CompanyID = Request.CompanyID();
                    //    Model.CategoryDropDownID = (int)sys_dropdown_mfEnum.AllowanceCategory;
                    //    Model.CreatedBy = Request.LoginID();
                    //    Model.CreatedDate = Request.DateTimes();
                    //    Model.ModifiedBy = Request.LoginID();
                    //    Model.ModifiedDate = Request.DateTimes();
                    //    Model.ObjectState = ObjectState.Added;
                    //    _service.Insert(Model);

                    try
                    {
                        await _unitOfWorkAsync.SaveChangesAsync();
                        objResponse.Message = MessageStatement.Save;
                        objResponse.IsSuccess = true;
                        objResponse.ResultSet = GetValuesList(Model);
                    }
                    catch (DbUpdateException)
                    {
                        //if (ModelExists(Model.ID.ToString()))
                        //{
                        //    objResponse.IsSuccess = false;
                        //    objResponse.ErrorMessage = MessageStatement.Conflict;
                        //    return objResponse;
                        //}
                        throw;
                    }
                }
                else
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = Model.Value + " already exist. Please enter a different name and try again.";
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
        public async Task<ResponseInfo> Delete(sys_drop_down_value Model)
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


                sys_drop_down_value id = _sys_drop_down_valueService.Queryable().Where(x => x.DropDownID == Model.DropDownID && (Model.DependedDropDownValueID == null || x.DependedDropDownValueID == Model.DependedDropDownValueID) && x.ID == Model.ID).FirstOrDefault();
                if (id != null)
                {
                    Model.ObjectState = ObjectState.Deleted;
                    _sys_drop_down_valueService.Delete(id);

                    //    Model.CompanyID = Request.CompanyID();
                    //    Model.CategoryDropDownID = (int)sys_dropdown_mfEnum.AllowanceCategory;
                    //    Model.CreatedBy = Request.LoginID();
                    //    Model.CreatedDate = Request.DateTimes();
                    //    Model.ModifiedBy = Request.LoginID();
                    //    Model.ModifiedDate = Request.DateTimes();
                    //    Model.ObjectState = ObjectState.Added;
                    //    _service.Insert(Model);

                    try
                    {
                        await _unitOfWorkAsync.SaveChangesAsync();
                        objResponse.Message = MessageStatement.Delete;
                        objResponse.IsSuccess = true;
                        objResponse.ResultSet = GetValuesList(Model);
                    }
                    catch (DbUpdateException)
                    {
                        //if (ModelExists(Model.ID.ToString()))
                        //{
                        //    objResponse.IsSuccess = false;
                        //    objResponse.ErrorMessage = MessageStatement.Conflict;
                        //    return objResponse;
                        //}
                        throw;
                    }
                }

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
            //catch (Exception ex)
            //{
            //    objResponse.IsSuccess = false;
            //    objResponse.ErrorMessage = ex.Message;
            //    Logger.Trace.Error(ex);
            //}

            return objResponse;

        }
        [HttpGet]
        [ActionName("Load")]
        public ResponseInfo Load()
        {
            var objResponse = new ResponseInfo();
            try

            {
                sys_drop_down_value model = new sys_drop_down_value();
                var CompanyID = Request.CompanyID();
                var MFDropDownList = _service.Queryable().ToList();
                //var DTDropDownList = new List<sys_drop_down_value>();
                if (MFDropDownList.Count() > 0)
                {
                    model.DropDownID = MFDropDownList[0].ID;
                    //objResponse.ResultSet = GetdropdowValues(model);
                    //DTDropDownList = _sys_drop_down_valueService.Queryable()
                    //   .Where(x => x.DropDownID == MFID).ToList();
                }

                objResponse.ResultSet = new
                {
                    MFDropDownList = MFDropDownList,
                    DTDropDownList = model.DropDownID > 0 ? GetdropdowValues(model) : 0
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

        public List<sys_drop_down_value> GetValuesList(sys_drop_down_value Model)
        {
            //var obj = _sys_drop_down_valueService.Queryable()
            //                .Where(x => x.DropDownID == Model.DropDownID && (x.DependedDropDownValueID == null || x.DependedDropDownValueID == Model.DependedDropDownValueID))
            //                .ToList();
            return _sys_drop_down_valueService.Queryable()
                             .Where(x => x.DropDownID == Model.DropDownID && (Model.DependedDropDownValueID == null || x.DependedDropDownValueID == Model.DependedDropDownValueID))
                             .ToList();
        }

        [HttpPost]
        [ActionName("GetValueByMFID")]
        public ResponseInfo GetValueByMFID(sys_drop_down_value model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                objResponse.ResultSet = GetdropdowValues(model);
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }

        public object GetdropdowValues(sys_drop_down_value model)
        {
            var AllDTList = _sys_drop_down_valueService.Queryable()
                  .Where(x => x.DropDownID == model.DropDownID).Select(z => new { 
                  z.ID,
                  z.Value,
                  SystemGenerated= z.SystemGenerated==true?"Yes":"No",
                  z.DependedDropDownID,
                  z.DependedDropDownValueID                 
                  }).ToList();

            var PaginationList = new List<sys_drop_down_value>();
            var DepList = new List<sys_drop_down_value>();

            var DepListExist = AllDTList.Where(x => x.DependedDropDownID != null).ToList();
            if (DepListExist.Count() > 0)
            {
                decimal DepDDID = DepListExist[0].DependedDropDownID ?? 0;

                DepList = _sys_drop_down_valueService.Queryable()
                    .Where(x => x.DropDownID == DepDDID).ToList();
                if (DepList.Count() > 0)
                {
                    int id = DepList[0].ID;
                    AllDTList = AllDTList.Where(x => x.DependedDropDownValueID == id).ToList();
                }
            }

            var obj = new
            {
                DTList = AllDTList,
                DepList = DepList,
            };
            return obj;
        }


        [HttpPost]
        [ActionName("GetDependentValueByDepID")]
        public ResponseInfo GetDependentValueByDepID(sys_drop_down_value model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                objResponse.ResultSet = _sys_drop_down_valueService.Queryable()
                  .Where(x => x.DropDownID == model.DropDownID && (model.DependedDropDownValueID == null || x.DependedDropDownValueID == model.DependedDropDownValueID))
                  .ToList();
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
