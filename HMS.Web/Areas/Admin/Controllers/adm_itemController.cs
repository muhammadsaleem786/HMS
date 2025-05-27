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
    public class adm_itemController : ApiController, IERPAPIInterface<adm_item>, IDisposable
    {
        private readonly IStoredProcedureService _procedureService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly Iadm_itemService _service;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iadm_companyService _adm_companyService;
        private readonly Iinv_stockService _inv_stockService;
        private readonly Iadm_item_logService _adm_item_logService;
        private readonly Iemr_instructionService _emr_instructionService;

        public adm_itemController(IUnitOfWorkAsync unitOfWorkAsync, Iadm_itemService Service,
        Isys_drop_down_valueService sys_drop_down_valueService,
  Iadm_companyService iadm_companyService, Iinv_stockService inv_stockService,
  Iadm_item_logService adm_item_logService,
        IStoredProcedureService ProcedureService, Iemr_instructionService emr_instructionService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _service = Service;
            _procedureService = ProcedureService;
            _adm_companyService = iadm_companyService;
            _inv_stockService = inv_stockService;
            _adm_item_logService = adm_item_logService;
            _emr_instructionService = emr_instructionService;
        }
        public async Task<ResponseInfo> Save(adm_item Model)
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
                Model.Name = Model.Name;
                decimal CompanyID = Request.CompanyID();

                var admCompany = _adm_companyService.Queryable().Where(a => a.ID == CompanyID).FirstOrDefault();
                var existName = _service.Queryable().Where(a => a.CompanyId == CompanyID && a.Name == Model.Name).FirstOrDefault();
                if (existName != null)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = Model.Name + " " + "already exist.";
                }
                decimal ID = 1;
                if (_service.Queryable().Count() > 0)
                    ID = _service.Queryable().Max(e => e.ID) + 1;
                Model.ID = ID;
                Model.CompanyId = CompanyID;
                Model.UnitDropDownID = (int)sys_dropdown_mfEnum.ItemUnitDropDownID;
                Model.CategoryDropDownID = (int)sys_dropdown_mfEnum.CategoryDropDownID;
                Model.ItemTypeDropDownID = (int)sys_dropdown_mfEnum.ItemTypeDropDownID;
                Model.GroupDropDownId = (int)sys_dropdown_mfEnum.GroupDropDownId;
                Model.CreatedBy = Request.LoginID();
                Model.CreatedDate = Request.DateTimes();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Added;
                Model.sys_drop_down_value = null;
                Model.sys_drop_down_value1 = null;
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
        public async Task<ResponseInfo> Update(adm_item Model)
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
                Model.Name = Model.Name;
                Model.sys_drop_down_value = null;
                Model.sys_drop_down_value1 = null;
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
        [HttpPut]
        [HttpGet]
        [ActionName("ItemPublish")]
        public async Task<ResponseInfo> ItemPublish(adm_item Model)
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
                Model.Name = Model.Name;
                Model.sys_drop_down_value = null;
                Model.sys_drop_down_value1 = null;
                Model.adm_user_mf = null;
                Model.adm_user_mf1 = null;
                Model.adm_company = null;
                decimal CompanyID = Request.CompanyID();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Modified;
                _service.Update(Model);
                if (Model.SaveStatus == 2 && Model.TrackInventory)
                {
                    inv_stock inv_Stock = new inv_stock();
                    decimal ID = 1;
                    if (_inv_stockService.Queryable().Count() > 0)
                        ID = _inv_stockService.Queryable().Max(e => e.ID) + 1;
                    inv_Stock.ID = ID;
                    inv_Stock.CompanyId = CompanyID;
                    inv_Stock.ItemID = Model.ID;
                    inv_Stock.Quantity = Convert.ToDecimal(Model.InventoryOpeningStock);
                    inv_Stock.CreatedBy = Request.LoginID();
                    inv_Stock.CreatedDate = Request.DateTimes();
                    inv_Stock.ModifiedBy = Request.LoginID();
                    inv_Stock.ModifiedDate = Request.DateTimes();
                    inv_Stock.ObjectState = ObjectState.Added;
                    _inv_stockService.Insert(inv_Stock);
                    //log maintain
                    adm_item_log adm_item_log_obj = new adm_item_log();
                    decimal LogID = 1;
                    if (_adm_item_logService.Queryable().Count() > 0)
                        LogID = _adm_item_logService.Queryable().Max(e => e.ID) + 1;
                    adm_item_log_obj.ID = LogID;
                    adm_item_log_obj.CompanyId = CompanyID;
                    adm_item_log_obj.ItemId = Model.ID;
                    adm_item_log_obj.Quantity = Model.InventoryOpeningStock.ToString();
                    adm_item_log_obj.Type = "Item Publish";
                    adm_item_log_obj.CreatedBy = Request.LoginID();
                    adm_item_log_obj.CreatedDate = Request.DateTimes();
                    adm_item_log_obj.ModifiedBy = Request.LoginID();
                    adm_item_log_obj.ModifiedDate = Request.DateTimes();
                    adm_item_log_obj.ObjectState = ObjectState.Added;
                    _adm_item_logService.Insert(adm_item_log_obj);
                }
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
                adm_item Model = _service.Queryable().Where(e => e.ID.ToString() == Id && e.CompanyId == CompanyID).FirstOrDefault();

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
                objResponse.ResultSet = _service.Queryable().Where(x => x.CompanyId == companyID).Select(s => new
                {
                    s.ID,
                    Name = s.Name,
                    sku = s.SKU,
                    Rate = s.SalePrice,
                    Stock = s.InventoryOpeningStock,
                    Unit = s.sys_drop_down_value.Value,
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
                var InstructionList = _emr_instructionService.Queryable().Where(a => a.CompanyID == CompanyID).Select(a => new { ID = a.ID, Name = a.Instructions }).ToList();

                if (dropdownValues != null)
                {

                    objResponse.ResultSet = new
                    {
                        dropdownValues = dropdownValues,
                        InstructionList = InstructionList,
                    };
                    objResponse.IsSuccess = true;
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
                var result = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID.ToString() == Id).Include(a => a.adm_company).Include(a => a.adm_user_mf).Include(x => x.sys_drop_down_value).FirstOrDefault();
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
        [HttpGet]
        [ActionName("UpdateStatus")]
        public ResponseInfo UpdateStatus(string Id, bool IsActive)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var obj = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID.ToString() == Id).FirstOrDefault();
                if (obj != null)
                {
                    obj.IsActive = IsActive;
                    obj.ObjectState = ObjectState.Modified;
                    _service.Update(obj);


                    try
                    {
                        _unitOfWorkAsync.SaveChanges();
                        objResponse.IsSuccess = true;
                        var result = GetById(obj.ID.ToString());
                        objResponse.ResultSet = new
                        {
                            result = result.ResultSet,
                        };
                    }
                    catch (DbUpdateException)
                    {
                        throw;
                    }
                }
                else
                {

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
        public ResponseInfo ExportData(int ExportType, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText)
        {
            throw new NotImplementedException();
        }
        public ResponseInfo Load()
        {
            throw new NotImplementedException();
        }
        [HttpGet]
        [ActionName("PaginationWithParm")]
        public PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResult = _inv_stockService.PaginationWithParm(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, FilterID, IgnorePaging);
            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }

        [HttpGet]
        [ActionName("PaginationWithGroupParm")]
        public PaginationResult PaginationWithGroupParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResult = _service.PaginationWithGroupParm(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, FilterID, IgnorePaging);
                var categoryList = _sys_drop_down_valueService.Queryable().Where(e => e.DropDownID == 64).Select(z => new
                {
                    z.ID,
                    z.Value,
                }).ToList();
                objResult.OtherDataModel = categoryList;
            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }
        [HttpGet]
        [ActionName("ExpirePagination")]
        public PaginationResult ExpirePagination(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResult = _inv_stockService.ExpirePagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, FilterID, IgnorePaging);

            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }
        [HttpGet]
        [ActionName("RestockPagination")]
        public PaginationResult RestockPagination(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResult = _inv_stockService.RestockPagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, FilterID, IgnorePaging);

            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }
        [HttpGet]
        [ActionName("GetItemStockList")]
        public PaginationResult GetItemStockList(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResult = _inv_stockService.GetItemStockList(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, FilterID, IgnorePaging);

            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }

        [HttpPost]
        [ActionName("IsImportDataExistInDB")]
        public ResponseInfo IsImportDataExistInDB(List<ImportDataCheckingInDBModel> ImportEmpList)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var Namelist = ImportEmpList.Select(x => (x.FirstName.ToLower().Trim() + " " + x.LastName.ToLower().Trim())).ToArray();
                var Emaillist = ImportEmpList.Where(x => x.Email != null).Select(x => x.Email).ToArray();

                var EmpList = _service.Queryable()
                    .Where(x => x.CompanyId == CompanyID).Select(x => new { x.CompanyId, x.IsActive, x.Name }).ToList();

                var AlreadyExistEmployees = new List<object>();
                if (Emaillist.Count() > 0)
                {
                    AlreadyExistEmployees = EmpList
                  .Where(x => Emaillist.Contains(x.Name))
                                  .Select(t => new
                                  {
                                      t.Name,
                                      t.CompanyId
                                  }).ToList<object>();
                    objResponse.Message = "Name already exist";
                }


                //if (AlreadyExistEmployees.Count() == 0)
                //{
                //    AlreadyExistEmployees = EmpList
                // .Where(x => Namelist.Contains((x.FirstName.ToLower().Trim() + " " + x.LastName.ToLower().Trim())))
                //                 .Select(t => new
                //                 {
                //                     t.FirstName,
                //                     t.LastName,
                //                     t.Email
                //                 }).ToList<object>();
                //    objResponse.Message = "Name already exist";
                //}


                if (AlreadyExistEmployees.Count > 0)
                {
                    objResponse.ResultSet = AlreadyExistEmployees;
                }
                else
                {
                    objResponse.ResultSet = null;
                    objResponse.Message = "NotExist";
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
        [ActionName("ImportEmpData")]
        public async Task<ResponseInfo> ImportEmpData(List<adm_item> ExcelDataList)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var currentLoginId = Request.LoginID();
                var currentDateTime = Request.DateTimes();
                decimal nextStockId = 1;
                decimal nextItemLogId = 1;

                if (_adm_item_logService.Queryable().Count() > 0)
                    nextItemLogId = _adm_item_logService.Queryable().Max(e => e.ID) + 1;
                if (_inv_stockService.Queryable().Count() > 0)
                    nextStockId = _inv_stockService.Queryable().Max(e => e.ID) + 1;

                decimal ItemID = 1;
                if (_service.Queryable().Count() > 0)
                    ItemID = _service.Queryable().Max(e => e.ID) + 1;

                ExcelDataList.ForEach(item =>
                {
                    item.CreatedBy = currentLoginId;
                    item.CreatedDate = currentDateTime;
                    item.ModifiedBy = currentLoginId;
                    item.ModifiedDate = currentDateTime;
                    item.CompanyId = CompanyID;
                    item.ID = ItemID;
                    item.SaveStatus = 2;
                    item.UnitDropDownID = (int)sys_dropdown_mfEnum.ItemUnitDropDownID;
                    item.CategoryDropDownID = (int)sys_dropdown_mfEnum.CategoryDropDownID;
                    item.ItemTypeDropDownID = (int)sys_dropdown_mfEnum.ItemTypeDropDownID;
                    item.GroupDropDownId = (int)sys_dropdown_mfEnum.GroupDropDownId;
                    item.sys_drop_down_value = null;
                    item.sys_drop_down_value1 = null;
                    item.emr_service_item = null;
                    ItemID++;
                });
                var stockEntries = ExcelDataList
           .Where(item => item.TrackInventory)
           .Select(item => new inv_stock
           {
               ID = nextStockId++,
               CompanyId = CompanyID,
               ItemID = item.ID,
               Quantity = Convert.ToDecimal(item.InventoryOpeningStock),
               ExpiredWarrantyDate = DateTime.Now,
               CreatedBy = currentLoginId,
               CreatedDate = currentDateTime,
               ModifiedBy = currentLoginId,
               ModifiedDate = currentDateTime,
               ObjectState = ObjectState.Added
           }).ToList();

                var LogEntries = ExcelDataList
                           .Where(item => item.TrackInventory)
                           .Select(item => new adm_item_log
                           {
                               ID = nextItemLogId++,
                               CompanyId = CompanyID,
                               ItemId = item.ID,
                               Quantity = item.InventoryOpeningStock.ToString(),
                               Type = "Item Import",
                               CreatedBy = currentLoginId,
                               CreatedDate = currentDateTime,
                               ModifiedBy = currentLoginId,
                               ModifiedDate = currentDateTime,
                               ObjectState = ObjectState.Added
                           }).ToList();


                _service.InsertRange(ExcelDataList);
                if (stockEntries.Any())
                {
                    _inv_stockService.InsertRange(stockEntries);
                }
                if (LogEntries.Any())
                {
                    _adm_item_logService.InsertRange(LogEntries);
                }
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Improted;
                }
                catch (DbUpdateException)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
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


    }
}
