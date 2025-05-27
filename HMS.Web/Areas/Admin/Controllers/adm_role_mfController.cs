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
    public class adm_role_mfController : ApiController, IERPAPIInterface<adm_role_mf>, IDisposable
    {
        private readonly Iadm_role_mfService _service;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly IStoredProcedureService _procedureService;
        private readonly Iadm_role_dtService _adm_role_dtService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iadm_user_companyService _adm_user_companyService;
        public adm_role_mfController(IUnitOfWorkAsync unitOfWorkAsync, Iadm_role_mfService Service,
            Isys_drop_down_valueService sys_drop_down_valueService, Iadm_user_companyService adm_user_companyService,
            IStoredProcedureService ProcedureService, Iadm_role_dtService adm_role_dtService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _service = Service;
            _adm_user_companyService = adm_user_companyService;
            _procedureService = ProcedureService;
            _adm_role_dtService = adm_role_dtService;
        }

        public async Task<ResponseInfo> Save(adm_role_mf Model)
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
                adm_role_mf role_mf = _service.Queryable().Where(x => x.CompanyID == CompanyID && x.RoleName.ToLower() == Model.RoleName.ToLower()).FirstOrDefault();
                if (role_mf == null)
                {
                    decimal ID = 1;
                    if (_service.Queryable().Count() > 0)
                        ID = _service.Queryable().Max(e => e.ID) + 1;
                    List<adm_role_dt> roledtList = new List<adm_role_dt>();
                    roledtList.AddRange(Model.adm_role_dt);


                    Model.ID = ID;
                    Model.CompanyID = CompanyID;
                    Model.CreatedBy = Request.LoginID();
                    Model.CreatedDate = Request.DateTimes();
                    Model.ModifiedBy = Request.LoginID();
                    Model.ModifiedDate = Request.DateTimes();
                    Model.ObjectState = ObjectState.Added;

                    Model.adm_role_dt = null;
                    _service.Insert(Model);

                    decimal roledtID = 1;
                    if (_adm_role_dtService.Queryable().Count() > 0)
                        roledtID = _adm_role_dtService.Queryable().Max(e => e.ID) + 1;
                    foreach (adm_role_dt item in roledtList)
                    {
                        item.ID = roledtID;
                        item.RoleID = Model.ID;
                        item.CompanyID = CompanyID;
                        item.CreatedBy = Request.LoginID();
                        item.CreatedDate = Request.DateTimes();
                        item.ModifiedBy = Request.LoginID();
                        item.ModifiedDate = Request.DateTimes();
                        item.ObjectState = ObjectState.Added;
                        _adm_role_dtService.Insert(item);
                        roledtID++;
                    }

                    try
                    {
                        await _unitOfWorkAsync.SaveChangesAsync();
                        objResponse.Message = MessageStatement.Save;
                        objResponse.IsSuccess = true;
                        var ControlLevelRights = _adm_role_dtService.Queryable().Where(e => e.CompanyID == CompanyID && e.RoleID == Model.ID)
                              .Select(s => new { s.ScreenID, s.ViewRights, s.DeleteRights, s.EditRights, s.CreateRights }).Distinct().ToList();
                        objResponse.ResultSet = new
                        {
                            ControlLevelRights = ControlLevelRights,
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
                    objResponse.ErrorMessage = Model.RoleName + " role name already exist";
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
                var AllScreen = _procedureService.GetAllScreen();
               var rolelist = _service.Queryable().Where(e => e.CompanyID == CompanyID && e.ID.ToString() == Id).Include(x => x.adm_role_dt).Include(y => y.adm_user_company).FirstOrDefault();
                objResponse.ResultSet = new
                {
                    AllScreen = AllScreen,
                    rolelist= rolelist
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
        public async Task<ResponseInfo> Update(adm_role_mf Model)
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
                adm_role_mf role_mf = _service.Queryable().Where(x => x.CompanyID == CompanyID && x.ID != Model.ID && x.RoleName.ToLower() == Model.RoleName.ToLower()).FirstOrDefault();
                if (role_mf == null)
                {
                    List<adm_role_dt> roledtList = new List<adm_role_dt>();
                    roledtList.AddRange(Model.adm_role_dt);
                    Model.adm_role_dt = null;
                    Model.ModifiedBy = Request.LoginID();
                    Model.ModifiedDate = Request.DateTimes();
                    Model.ObjectState = ObjectState.Modified;
                    _service.Update(Model);



                    var Ids = roledtList.Where(e => e.ID != 0).Select(s => s.ID).ToArray();
                    var DelModels = _adm_role_dtService.Queryable().Where(e => e.RoleID == Model.ID && !Ids.Contains(e.ID)).ToList();

                    foreach (var obj in DelModels)
                    {
                        obj.ObjectState = ObjectState.Deleted;
                        _adm_role_dtService.Delete(obj);
                    }

                    decimal roledtID = 1;
                    if (_adm_role_dtService.Queryable().Count() > 0)
                        roledtID = _adm_role_dtService.Queryable().Max(e => e.ID) + 1;

                    foreach (adm_role_dt item in roledtList)
                    {
                        item.RoleID = Model.ID;
                        item.CompanyID = Request.CompanyID();

                        if (item.ID > 0)
                        {
                            item.ModifiedBy = Request.LoginID();
                            item.ModifiedDate = Request.DateTimes();
                            item.ObjectState = ObjectState.Modified;
                            _adm_role_dtService.Update(item);
                        }
                        else
                        {
                            item.ID = roledtID;
                            item.CreatedBy = Request.LoginID();
                            item.CreatedDate = Request.DateTimes();
                            item.ModifiedBy = Request.LoginID();
                            item.ModifiedDate = Request.DateTimes();
                            item.ObjectState = ObjectState.Added;
                            _adm_role_dtService.Insert(item);
                            roledtID++;
                        }
                    }

                    try
                    {
                        await _unitOfWorkAsync.SaveChangesAsync();
                        objResponse.Message = MessageStatement.Update;
                        int userid= Request.LoginID();
                        var RoleIds = _adm_user_companyService.Queryable().Where(e => e.CompanyID == CompanyID && e.UserID == userid)
                           .Select(s => s.RoleID).ToArray();

                        var ControlLevelRights = _adm_role_dtService.Queryable().Where(e => e.CompanyID == CompanyID && e.ViewRights && RoleIds.Contains(e.RoleID))
                                .Select(s => new { s.ScreenID, s.ViewRights, s.DeleteRights, s.EditRights, s.CreateRights }).Distinct().ToList();
                        objResponse.ResultSet = new
                        {
                            ControlLevelRights = ControlLevelRights,
                        };
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
                    objResponse.ErrorMessage = Model.RoleName + " role name already exist";
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
                //int[] IdList = Id.Split(',').Select(int.Parse).ToArray();

                List<adm_role_mf> Models = _service.Queryable().Where(e => e.CompanyID == CompanyID && IdList.Contains(e.ID)).Include(x => x.adm_role_dt).ToList();

                if (Models.Count() == 0)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
                }

                foreach (var Model in Models)
                {
                    List<adm_role_dt> roledtModels = Model.adm_role_dt.ToList();
                    foreach (var item in roledtModels)
                    {
                        item.ObjectState = ObjectState.Deleted;
                        _adm_role_dtService.Delete(item);
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
    }
}
