using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
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
using System.Web.Http.Results;

namespace HMS.Web.API.Areas.Appointment.Controllers
{
    [JwtAuthentication]
    public class emr_service_mfController : ApiController, IERPAPIInterface<emr_service_mf>, IDisposable
    {
        private readonly Iemr_service_mfService _service;
        private readonly Iemr_service_itemService _emr_service_item;
        private readonly Iemr_appointment_mfService _emr_appointment_mfService;
        private readonly Iadm_user_companyService _adm_user_companyService;
        private readonly Iadm_role_mfService _adm_role_mfService;
        private readonly Iadm_user_tokenService _adm_user_tokenService;
        private readonly Isys_notification_alertService _sys_notification_alertService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly Iadm_userService _adm_userService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iadm_itemService _adm_itemService;

        public emr_service_mfController(IUnitOfWorkAsync unitOfWorkAsync, Iemr_service_mfService service,
            Iemr_service_itemService emr_service_item, Iadm_itemService adm_itemService,
            Iadm_userService adm_userService, Iemr_appointment_mfService emr_appointment_mfService, Iadm_user_companyService adm_user_companyService,
            Iadm_role_mfService adm_role_mfService, Iadm_user_tokenService adm_user_tokenService, Isys_drop_down_valueService sys_drop_down_valueService,
            Isys_notification_alertService sys_notification_alertService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = service;
            _emr_service_item = emr_service_item;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _adm_user_companyService = adm_user_companyService;
            _adm_userService = adm_userService;
            _emr_appointment_mfService = emr_appointment_mfService;
            _adm_role_mfService = adm_role_mfService;
            _adm_itemService = adm_itemService;
            _adm_user_tokenService = adm_user_tokenService;
            _sys_notification_alertService = sys_notification_alertService;
        }
        [HttpPost]
        [HttpGet]
        [ActionName("Save")]
        public async Task<ResponseInfo> Save(emr_service_mf Model)
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
                List<emr_service_item> Objemr_service_item = new List<emr_service_item>();
                Objemr_service_item.AddRange(Model.emr_service_item);

                decimal ID = 1;
                if (_service.Queryable().Count() > 0)
                    ID = _service.Queryable().Max(e => e.ID) + 1;
                Model.ID = ID;
                Model.CompanyId = Request.CompanyID();
                Model.CreatedBy = Request.LoginID();
                Model.IsSystemGenerated = false;
                Model.SpecialityDropdownId = (int)sys_dropdown_mfEnum.SpecialtyDropdownId;
                Model.CreatedDate = Request.DateTimes();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Added;
                Model.emr_service_item = null;
                Model.emr_patient_bill = null;
                _service.Insert(Model);
                //insert item
                decimal seviceID = 1;
                if (_emr_service_item.Queryable().Count() > 0)
                    seviceID = _emr_service_item.Queryable().Max(e => e.ID) + 1;

                foreach (emr_service_item item in Objemr_service_item)
                {
                    item.ID = seviceID;
                    item.ServiceId = Model.ID;
                    item.CompanyId = CompanyID;
                    item.ItemId = item.ItemId;
                    item.Quantity = item.Quantity;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.ObjectState = ObjectState.Added;
                    item.emr_service_mf = null;
                    item.adm_user_mf = null;
                    item.adm_user_mf1 = null;
                    _emr_service_item.Insert(item);
                    seviceID++;
                }
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Save;
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        ID = Model.ID,
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
        [HttpPut]
        [HttpGet]
        [ActionName("Update")]
        public async Task<ResponseInfo> Update(emr_service_mf Model)
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

                List<emr_service_item> Objemr_service_item = new List<emr_service_item>();
                Objemr_service_item.AddRange(Model.emr_service_item);

                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Modified;
                Model.emr_service_item = null;
                Model.adm_user_mf = null;
                Model.adm_user_mf1 = null;
                _service.Update(Model);
                //delete charged
                var DelAlLItem = _emr_service_item.Queryable().Where(e => e.ServiceId == Model.ID && e.CompanyId == CompanyID).ToList();
                foreach (var obj in DelAlLItem)
                {
                    obj.ObjectState = ObjectState.Deleted;
                    _emr_service_item.Delete(obj);
                }
                decimal seviceID = 1;
                if (_emr_service_item.Queryable().Count() > 0)
                    seviceID = _emr_service_item.Queryable().Max(e => e.ID) + 1;

                foreach (emr_service_item item in Objemr_service_item)
                {
                    item.ID = seviceID;
                    item.ServiceId = Model.ID;
                    item.CompanyId = CompanyID;
                    item.ItemId = item.ItemId;
                    item.Quantity = item.Quantity;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.ObjectState = ObjectState.Added;
                    item.emr_service_mf = null;
                    item.adm_user_mf = null;
                    item.adm_user_mf1 = null;
                    _emr_service_item.Insert(item);
                    seviceID++;
                }
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        Model = Model
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
        public ResponseInfo GetById(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var Result = _service.Queryable().Where(e => e.ID.ToString() == Id && e.CompanyId == CompanyID).Include(a => a.emr_service_item).FirstOrDefault();
                var itemids = Result.emr_service_item.Select(a => a.ItemId).ToList();
                var itemList = _adm_itemService.Queryable().Where(a => itemids.Contains(a.ID) && a.CompanyId == CompanyID)
                                 .Select(z => new
                                 {
                                     z.ID,
                                     z.Name,
                                     z.ItemTypeId
                                 }).ToList();
                objResponse.ResultSet = new
                {
                    Result = Result,
                    itemList = itemList
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
                emr_service_mf model = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID == ID).FirstOrDefault();

                if (model == null)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
                }
                var DelAlLitem = _emr_service_item.Queryable().Where(e => e.ServiceId == model.ID && e.CompanyId == CompanyID).ToList();

                foreach (var obj in DelAlLitem)
                {
                    obj.ObjectState = ObjectState.Deleted;
                    _emr_service_item.Delete(obj);
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
        [HttpPost]
        [HttpGet]
        [ActionName("searchService")]
        public async Task<ResponseInfo> searchService(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var serviceInfo = _service.Queryable().Where(a => a.CompanyId == CompanyID && (a.ServiceName.Contains(term))).Select(z => new
                {
                    value = z.ID,
                    label = z.ServiceName,
                    price = z.Price
                }).ToList();
                objResponse.ResultSet = new
                {
                    serviceInfo = serviceInfo
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
                var Speciality = _sys_drop_down_valueService.Queryable().Where(a => a.DropDownID == 24).Select(z => new
                {
                    z.ID,
                    z.Value,
                    z.DropDownID
                }).ToList();

                objResponse.ResultSet = new
                {
                    Speciality = Speciality
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

        public PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            throw new NotImplementedException();
        }
    }
}
