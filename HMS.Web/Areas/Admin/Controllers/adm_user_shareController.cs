using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Service;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Appointment;
using HMS.Service.Services.Employee;
using HMS.Service.Services.Items;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using HMS.Web.API.Interface;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using static iTextSharp.tool.xml.html.HTML;

namespace HMS.Web.API.Areas.Admin.Controllers
{
    [JwtAuthentication]
    public class adm_user_shareController : ApiController, IERPAPIInterface<adm_user_share>, IDisposable
    {
        private readonly Iadm_user_shareService _service;
        private readonly Iadm_userService _adm_userService;
        private readonly Iemr_investigationService _investigationService;
        private readonly Iemr_service_mfService _emr_service_mfService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly IStoredProcedureService _procedureService;
        private readonly Iadm_role_dtService _adm_role_dtService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iadm_user_companyService _adm_user_companyService;
        private readonly Ipr_employee_mfService _pr_employee_mfService;
        private readonly Iadm_itemService _adm_itemService;
        public adm_user_shareController(IUnitOfWorkAsync unitOfWorkAsync, Iadm_user_shareService Service,
            Isys_drop_down_valueService sys_drop_down_valueService, Iadm_user_companyService adm_user_companyService,
            IStoredProcedureService ProcedureService, Iadm_role_dtService adm_role_dtService, Ipr_employee_mfService pr_employee_mfService, Iadm_userService adm_userService, Iemr_service_mfService emr_service_mfService, Iadm_itemService adm_itemService, Iemr_investigationService investigationService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _service = Service;
            _adm_user_companyService = adm_user_companyService;
            _procedureService = ProcedureService;
            _adm_role_dtService = adm_role_dtService;
            _pr_employee_mfService = pr_employee_mfService;
            _adm_userService = adm_userService;
            _emr_service_mfService = emr_service_mfService;
            _adm_itemService = adm_itemService;
            _investigationService = investigationService;
        }
        [HttpPut]
        [HttpGet]
        [ActionName("SaveExpense")]
        public async Task<ResponseInfo> SaveExpense(adm_user_share Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var user = _adm_userService.Queryable().Where(x => x.ID == Model.DoctorId).FirstOrDefault();
                if (user != null)
                {
                    user.DeductOTCharges = Model.DeductOTCharges;
                    user.DeductMedicineCost = Model.DeductMedicineCost;
                    user.DeductConsumables = Model.DeductConsumables;
                    user.DeductImplantCost = Model.DeductImplantCost;
                    user.DeductLabConsumables = Model.DeductLabConsumables;
                    user.DeductRoomCharges = Model.DeductRoomCharges;
                    Model.ObjectState = ObjectState.Modified;
                    _adm_userService.Update(user);
                }
                else
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
                }
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Save;
                    objResponse.IsSuccess = true;
                    var master = _adm_userService.Queryable().Where(e => e.ID == user.ID
                    ).Select(z => new
                    {
                        DoctorId = z.ID,
                        Name = z.Name,
                        DisDoctorShare = z.DoctorShare,
                        DisOwnerShare = z.OwnerShare,
                        DeductOTCharges = z.DeductOTCharges,
                        DeductMedicineCost = z.DeductMedicineCost,
                        DeductConsumables = z.DeductConsumables,
                        DeductImplantCost = z.DeductImplantCost,
                        DeductLabConsumables = z.DeductLabConsumables,
                        DeductRoomCharges = z.DeductRoomCharges
                    }).FirstOrDefault();

                    var details = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.DoctorId == Model.DoctorId)
                 .Select(s => new
                 {
                     ID = s.ID,
                     DoctorId = s.DoctorId,
                     CategoryName = s.Category,
                     CategoryId = s.Category == "Procedure" ? 1 : s.Category == "Laboratory" ? 2 : 3,
                     ServiceId = s.ServiceId,
                     LabId = s.LabId,
                     ItemId = s.ItemId,
                     ServiceName = !string.IsNullOrEmpty(s.emr_service_mf.ServiceName)
                 ? s.emr_service_mf.ServiceName
                 : !string.IsNullOrEmpty(s.emr_investigation.Investigation)
                     ? s.emr_investigation.Investigation
                     : !string.IsNullOrEmpty(s.adm_item.Name)
                         ? s.adm_item.Name
                         : "",
                     SharedMethod = s.SharedMethod,
                     DoctorShare = s.DoctorShare,
                     OwnerShare = s.OwnerShare,
                     s.CreatedBy,
                     s.CreatedDate
                 })
                 .ToList();
                    objResponse.ResultSet = new
                    {
                        Details = details,
                        Master = master
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
        [ActionName("SaveDiscount")]
        public async Task<ResponseInfo> SaveDiscount(adm_user_share Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
               
                decimal CompanyID = Request.CompanyID();
                var user = _adm_userService.Queryable().Where(x => x.ID == Model.DoctorId).FirstOrDefault();
                if (user != null)
                {
                    user.DoctorShare = Model.DisDoctorShare;
                    user.OwnerShare = Model.DisOwnerShare;
                    Model.ObjectState = ObjectState.Modified;
                    _adm_userService.Update(user);
                }
                else
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
                }
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Save;
                    objResponse.IsSuccess = true;
                    var master = _adm_userService.Queryable().Where(e => e.ID == user.ID).Select(z => new
                    {
                        DoctorId = z.ID,
                        Name = z.Name,
                        DisDoctorShare = z.DoctorShare,
                        DisOwnerShare = z.OwnerShare,
                        DeductOTCharges = z.DeductOTCharges,
                        DeductMedicineCost = z.DeductMedicineCost,
                        DeductConsumables = z.DeductConsumables,
                        DeductImplantCost = z.DeductImplantCost,
                        DeductLabConsumables = z.DeductLabConsumables,
                        DeductRoomCharges = z.DeductRoomCharges
                    }).FirstOrDefault();

                    var details = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.DoctorId == Model.DoctorId)
                 .Select(s => new
                 {
                     ID = s.ID,
                     DoctorId = s.DoctorId,
                     CategoryName = s.Category,
                     CategoryId = s.Category == "Procedure" ? 1 : s.Category == "Laboratory" ? 2 : 3,
                     ServiceId = s.ServiceId,
                     LabId = s.LabId,
                     ItemId = s.ItemId,
                     ServiceName = !string.IsNullOrEmpty(s.emr_service_mf.ServiceName)
                 ? s.emr_service_mf.ServiceName
                 : !string.IsNullOrEmpty(s.emr_investigation.Investigation)
                     ? s.emr_investigation.Investigation
                     : !string.IsNullOrEmpty(s.adm_item.Name)
                         ? s.adm_item.Name
                         : "",
                     SharedMethod = s.SharedMethod,
                     DoctorShare = s.DoctorShare,
                     OwnerShare = s.OwnerShare,
                     s.CreatedBy,
                     s.CreatedDate
                 })
                 .ToList();
                    objResponse.ResultSet = new
                    {
                        Details = details,
                        Master = master
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
        public async Task<ResponseInfo> Save(adm_user_share Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
               
                decimal CompanyID = Request.CompanyID();
                decimal ID = 1;
                if (_service.Queryable().Count() > 0)
                    ID = (_service.Queryable().Where(e => e.CompanyId == CompanyID).Max(x => (decimal?)x.ID) ?? 0) + 1;
                Model.ID = ID;
                Model.CompanyId = CompanyID;
                Model.DoctorId = Model.DoctorId;
                Model.ServiceId = Model.ServiceId;
                Model.LabId = Model.LabId;
                Model.ItemId = Model.ItemId;
                Model.Category = Model.Category;
                Model.SharedMethod = Model.SharedMethod;
                Model.DoctorShare = Model.DoctorShare;
                Model.OwnerShare = Model.OwnerShare;
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
                    var details = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.DoctorId == Model.DoctorId)
                  .Select(s => new
                  {
                      ID = s.ID,
                      DoctorId = s.DoctorId,
                      CategoryName = s.Category,
                      CategoryId = s.Category == "Procedure" ? 1 : s.Category == "Laboratory" ? 2 : 3,
                      ServiceId = s.ServiceId,
                      LabId = s.LabId,
                      ItemId = s.ItemId,
                      ServiceName = !string.IsNullOrEmpty(s.emr_service_mf.ServiceName)
                  ? s.emr_service_mf.ServiceName
                  : !string.IsNullOrEmpty(s.emr_investigation.Investigation)
                      ? s.emr_investigation.Investigation
                      : !string.IsNullOrEmpty(s.adm_item.Name)
                          ? s.adm_item.Name
                          : "",
                      SharedMethod = s.SharedMethod,
                      DoctorShare = s.DoctorShare,
                      OwnerShare = s.OwnerShare,
                      s.CreatedBy,
                      s.CreatedDate
                  })
                  .ToList();
                    objResponse.ResultSet = new
                    {
                        Details = details
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
                decimal companyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var role = _adm_user_companyService.Queryable()
                    .Where(u => u.UserID == UserID && u.CompanyID == companyID)
                    .Select(u => u.adm_role_mf.RoleName)
                    .FirstOrDefault();

                var doctorUsers = _adm_user_companyService.Queryable()
           .Where(u => u.CompanyID == companyID && u.adm_role_mf.RoleName == "Doctor")
           .Select(u => u.adm_user_mf)
           .OrderBy(u => u.ID)
           .ToList();
                var doctors = doctorUsers.Select(mf =>
                {
                    return (object)new
                    {
                        ID = mf.ID,
                        Name = mf.Name,
                    };
                }).ToList();

                List<object> result;
                if (role == "Administrator")
                {
                    result = doctors.ToList<object>();
                }
                else if (role == "Doctor")
                {
                    result = doctors.Where(d => ((dynamic)d).ID == UserID).ToList<object>();
                }
                else
                {
                    result = doctors;
                }
                objResponse.IsSuccess = true;
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
                var result = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID.ToString() == Id).Select(x => new
                {
                    ID = x.ID,
                    DoctorId = x.DoctorId,
                    CategoryName = x.Category,
                    CategoryId = x.Category == "Procedure" ? 1 : x.Category == "Laboratory" ? 2 : 3,
                    ServiceId = x.ServiceId,
                    LabId = x.LabId,
                    ItemId = x.ItemId,
                    ServiceName = !string.IsNullOrEmpty(x.emr_service_mf.ServiceName)
                        ? x.emr_service_mf.ServiceName
                        : !string.IsNullOrEmpty(x.emr_investigation.Investigation)
                            ? x.emr_investigation.Investigation
                            : !string.IsNullOrEmpty(x.adm_item.Name)
                                ? x.adm_item.Name
                                : "",
                    SharedMethod = x.SharedMethod,
                    DoctorShare = x.DoctorShare,
                    OwnerShare = x.OwnerShare
                }).FirstOrDefault();
                objResponse.IsSuccess = true;
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
        [ActionName("GetByDoctorId")]
        public ResponseInfo GetByDoctorId(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var master = _adm_userService.Queryable().Where(e => e.ID.ToString() == Id).Select(z => new
                {
                    DoctorId = z.ID,
                    Name = z.Name,
                    DisDoctorShare = z.DoctorShare,
                    DisOwnerShare = z.OwnerShare,
                    DeductOTCharges = z.DeductOTCharges,
                    DeductMedicineCost = z.DeductMedicineCost,
                    DeductConsumables = z.DeductConsumables,
                    DeductImplantCost = z.DeductImplantCost,
                    DeductLabConsumables = z.DeductLabConsumables,
                    DeductRoomCharges = z.DeductRoomCharges
                }).FirstOrDefault();
                var details = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.DoctorId.ToString() == Id)
                    .Select(s => new
                    {
                        ID = s.ID,
                        s.CreatedBy,
                        s.CreatedDate,
                        CategoryName = s.Category,
                        CategoryId = s.Category == "Procedure" ? 1 : s.Category == "Laboratory" ? 2 : 3,
                        ServiceId = s.ServiceId,
                        LabId = s.LabId,
                        ItemId = s.ItemId,
                        ServiceName = !string.IsNullOrEmpty(s.emr_service_mf.ServiceName)
                    ? s.emr_service_mf.ServiceName
                    : !string.IsNullOrEmpty(s.emr_investigation.Investigation)
                        ? s.emr_investigation.Investigation
                        : !string.IsNullOrEmpty(s.adm_item.Name)
                            ? s.adm_item.Name
                            : "",
                        SharedMethod = s.SharedMethod,
                        DoctorShare = s.DoctorShare,
                        OwnerShare = s.OwnerShare
                    })
                    .ToList();
                objResponse.IsSuccess = true;
                objResponse.ResultSet = new
                {
                    Master = master,
                    Details = details
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
        [ActionName("PaginationWithParm")]
        public PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            var objResponse = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResponse = _service.PaginationWithParm(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, FilterID, IgnorePaging);
            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResponse;
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
        public async Task<ResponseInfo> Update(adm_user_share Model)
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
                Model.CompanyId = CompanyID;
                Model.CreatedBy = Request.LoginID();
                Model.CreatedDate = Request.DateTimes();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Modified;
                _service.Update(Model);
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;

                    objResponse.IsSuccess = true;
                    var details = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.DoctorId == Model.DoctorId)
                  .Select(s => new
                  {
                      ID = s.ID,
                      DoctorId = s.DoctorId,
                      CategoryName = s.Category,
                      CategoryId = s.Category == "Procedure" ? 1 : s.Category == "Laboratory" ? 2 : 3,
                      ServiceId = s.ServiceId,
                      LabId = s.LabId,
                      ItemId = s.ItemId,
                      ServiceName = !string.IsNullOrEmpty(s.emr_service_mf.ServiceName)
                  ? s.emr_service_mf.ServiceName
                  : !string.IsNullOrEmpty(s.emr_investigation.Investigation)
                      ? s.emr_investigation.Investigation
                      : !string.IsNullOrEmpty(s.adm_item.Name)
                          ? s.adm_item.Name
                          : "",
                      SharedMethod = s.SharedMethod,
                      DoctorShare = s.DoctorShare,
                      OwnerShare = s.OwnerShare,
                      s.CreatedBy,
                      s.CreatedDate
                  })
                  .ToList();
                    objResponse.ResultSet = new
                    {
                        Details = details
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
                List<adm_user_share> Models = _service.Queryable().Where(e => e.CompanyId == CompanyID && IdList.Contains(e.ID)).ToList();

                if (Models.Count() == 0)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
                }
                decimal DoctorId = Models.FirstOrDefault().DoctorId;
                foreach (var Model in Models)
                {

                    Model.ObjectState = ObjectState.Deleted;
                    _service.Delete(Model);
                }

                await _unitOfWorkAsync.SaveChangesAsync();
                objResponse.Message = MessageStatement.Delete;
                objResponse.IsSuccess = true;
                var details = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.DoctorId == DoctorId)
                  .Select(s => new
                  {
                      ID = s.ID,
                      DoctorId = s.DoctorId,
                      CategoryName = s.Category,
                      CategoryId = s.Category == "Procedure" ? 1 : s.Category == "Laboratory" ? 2 : 3,
                      ServiceId = s.ServiceId,
                      LabId = s.LabId,
                      ItemId = s.ItemId,
                      ServiceName = !string.IsNullOrEmpty(s.emr_service_mf.ServiceName)
                  ? s.emr_service_mf.ServiceName
                  : !string.IsNullOrEmpty(s.emr_investigation.Investigation)
                      ? s.emr_investigation.Investigation
                      : !string.IsNullOrEmpty(s.adm_item.Name)
                          ? s.adm_item.Name
                          : "",
                      SharedMethod = s.SharedMethod,
                      DoctorShare = s.DoctorShare,
                      OwnerShare = s.OwnerShare,
                      s.CreatedBy,
                      s.CreatedDate
                  })
                  .ToList();
                objResponse.ResultSet = new
                {
                    Details = details
                };
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
        [HttpPost]
        [HttpGet]
        [ActionName("searchServices")]
        public async Task<ResponseInfo> searchServices(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var ServiceInfo = _emr_service_mfService.Queryable().Where(a => a.CompanyId == CompanyID && (a.ServiceName.Contains(term))).Select(z => new
                {
                    value = z.ID,
                    label = z.ServiceName
                }).ToList();
                objResponse.ResultSet = new
                {
                    ServiceInfo = ServiceInfo
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
        [HttpPost]
        [HttpGet]
        [ActionName("SearchItemByName")]
        public async Task<ResponseInfo> SearchItemByName(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var ItemInfo = _adm_itemService.Queryable().Where(a => a.CompanyId == CompanyID && (a.Name.Contains(term))).Select(z => new
                {
                    value = z.ID,
                    label = z.Name
                }).ToList();
                ItemInfo.Insert(0, new
                {
                    value = 0m,
                    label = "All"
                });
                objResponse.ResultSet = new
                {
                    ItemInfo = ItemInfo
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
        [HttpPost]
        [HttpGet]
        [ActionName("SearchLabTest")]
        public async Task<ResponseInfo> SearchLabTest(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var investigation = _investigationService.Queryable().Where(a => a.CompanyID == CompanyID && (a.Investigation.Contains(term))).Select(z => new
                {
                    value = z.ID,
                    label = z.Investigation
                }).ToList();
                investigation.Insert(0, new
                {
                    value = 0m,
                    label = "All"
                });
                objResponse.ResultSet = new
                {
                    investigation = investigation
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
        public PaginationResult Pagination(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            throw new NotImplementedException();
        }
    }
}
