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
using NPOI.XSSF.UserModel;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static iTextSharp.text.pdf.AcroFields;

namespace HMS.Web.API.Areas.Admin.Controllers
{
    [JwtAuthentication]
    public class pur_sale_mfController : ApiController, IERPAPIInterface<pur_sale_mf>, IDisposable
    {
        private readonly IStoredProcedureService _procedureService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Ipur_sale_mfService _service;
        private readonly Ipur_sale_hold_mfService _pur_sale_hold_mfService;
        private readonly Ipur_sale_hold_dtService _pur_sale_hold_dtService;
        private readonly Iemr_incomeService _emr_incomeService;
        private readonly Ipur_sale_dtService _pur_sale_dtService;
        private readonly Iadm_companyService _adm_companyService;
        private readonly Ipur_vendorService _pur_vendorService;
        private readonly Iadm_itemService _adm_itemService;
        private readonly Iadm_userService _adm_userService;
        private readonly Iinv_stockService _inv_stockService;
        private readonly Iemr_patientService _emr_patientService;
        private readonly Iadm_user_companyService _adm_user_companyService;
        private readonly Iadm_item_logService _adm_item_logService;
        public pur_sale_mfController(IUnitOfWorkAsync unitOfWorkAsync,
         Ipur_sale_mfService Service,
         Ipur_sale_dtService pur_sale_dtService,
         Isys_drop_down_valueService sys_drop_down_valueService,
         Iadm_itemService adm_itemService,
         Iadm_userService adm_userService,
         Iemr_incomeService emr_incomeService,
         Ipur_vendorService pur_vendorService,
         Iadm_user_companyService adm_user_companyService,
         Iinv_stockService inv_stockService,
         Iemr_patientService emr_patientService,
         Iadm_item_logService adm_item_logService,
         Ipur_sale_hold_mfService pur_sale_hold_mfService,
        Ipur_sale_hold_dtService pur_sale_hold_dtService,

        IStoredProcedureService ProcedureService, Iadm_role_dtService adm_role_dtService, Iadm_companyService adm_companyService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _procedureService = ProcedureService;
            _pur_vendorService = pur_vendorService;
            _service = Service;
            _pur_sale_hold_mfService = pur_sale_hold_mfService;
            _pur_sale_hold_dtService = pur_sale_hold_dtService;
            _emr_incomeService = emr_incomeService;
            _pur_sale_dtService = pur_sale_dtService;
            _adm_companyService = adm_companyService;
            _adm_itemService = adm_itemService;
            _adm_userService = adm_userService;
            _adm_user_companyService = adm_user_companyService;
            _inv_stockService = inv_stockService;
            _emr_patientService = emr_patientService;
            _adm_item_logService = adm_item_logService;
        }

        [HttpGet]
        [ActionName("Load")]
        public ResponseInfo Load()
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();

                var Companylist = _adm_companyService.Queryable().Where(a => a.ID == CompanyID).Select(a => new { a.Email, a.ID }).ToList();

                objResponse.ResultSet = new
                {
                    Companylist = Companylist,
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
                var PatientInfo = _emr_patientService.Queryable().Where(a => a.CompanyId == CompanyID && (a.PatientName.Contains("Walk In")))
                .Select(z => new
                {
                    value = z.ID,
                    label = z.PatientName + " " + (z.Mobile),
                    Phone = z.Mobile,
                    CNIC = z.CNIC,
                    Age = z.Age,
                    Gender = z.Gender,
                }).FirstOrDefault();
                var companyInfo = _adm_companyService.Queryable().Where(a => a.ID == CompanyID).Select(z => new
                {
                    CompanyAddress1 = z.CompanyAddress1,
                    CompanyAddress2 = z.CompanyAddress2,
                    CompanyName = z.CompanyName,
                    Province = z.Province,
                    PostalCode = z.PostalCode,
                    FaxFax = z.Fax,
                    Phone = z.Phone,
                    Email = z.Email,
                    Website = z.Website,
                    CompanyLogo = z.CompanyLogo,
                }).FirstOrDefault();
                if (dropdownValues != null)
                {
                    objResponse.ResultSet = new
                    {
                        dropdownValues = dropdownValues,
                        PatientInfo = PatientInfo,
                        companyInfo = companyInfo,
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
        [HttpPost]
        [HttpGet]
        [ActionName("searchByName")]
        public async Task<ResponseInfo> searchByName(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var ItemInfo = _adm_itemService.Queryable().Where(a => a.CompanyId == CompanyID && (a.Name.Contains(term))).Include(a => a.inv_stock).Select(z => new
                {
                    value = z.ID,
                    label = z.Name,
                    SalePrice = z.SalePrice,
                    CostPrice = z.CostPrice,
                    Type = z.ItemTypeId,
                    TypeValue = z.sys_drop_down_value2.Value,
                    CategoryId = z.CategoryID,
                    stock = z.inv_stock.Any() ? z.inv_stock.Sum(a => a.Quantity) : 0,
                    Instructions = z.emr_instruction.Instructions,
                    InstructionId = z.InstructionId,
                }).ToList();
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
        [ActionName("searchByNamePrescription")]
        public async Task<ResponseInfo> searchByNamePrescription(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var ItemInfo = _adm_itemService.Queryable().Where(a => a.CompanyId == CompanyID && (a.Name.Contains(term))).Select(z => new
                {
                    value = z.ID,
                    label = z.Name,
                    Instructions = z.emr_instruction.Instructions,
                    InstructionId = z.InstructionId,
                }).ToList();
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
        [ActionName("SaleItemByName")]
        public async Task<ResponseInfo> SaleItemByName(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var ItemInfo = _adm_itemService.Queryable().Where(a => a.CompanyId == CompanyID && a.POSItem == true && (a.Name.Contains(term))).Include(a => a.inv_stock).Select(z => new
                {
                    value = z.ID,
                    label = z.Name,
                    SalePrice = z.SalePrice,
                    CostPrice = z.CostPrice,
                    Type = z.ItemTypeId,
                    TypeValue = z.sys_drop_down_value2.Value,
                    CategoryId = z.CategoryID,
                    stock = z.inv_stock.Any() ? z.inv_stock.Sum(a => a.Quantity) : 0,
                    GroupId = z.GroupId,
                    TrackInventory = z.TrackInventory,
                }).ToList();
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
        [ActionName("ServiceItemByName")]
        public async Task<ResponseInfo> ServiceItemByName(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var ItemInfo = _adm_itemService.Queryable().Where(a => a.CompanyId == CompanyID && a.TrackInventory == true && (a.Name.Contains(term))).Include(a => a.inv_stock).Select(z => new
                {
                    value = z.ID,
                    label = z.Name,
                    SalePrice = z.SalePrice,
                    CostPrice = z.CostPrice,
                    Type = z.ItemTypeId,
                    TypeValue = z.sys_drop_down_value2.Value,
                    CategoryId = z.CategoryID,
                    stock = z.inv_stock.Any() ? z.inv_stock.Sum(a => a.Quantity) : 0
                }).ToList();
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
        [ActionName("SearchAllItemByName")]
        public async Task<ResponseInfo> SearchAllItemByName(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();

                var ItemInfo = _adm_itemService.Queryable().Where(a => a.CompanyId == CompanyID && (a.Name.Contains(term))).Include(a => a.inv_stock).Select(z => new
                {
                    value = z.ID,
                    label = z.Name,
                    SalePrice = z.SalePrice,
                    PurchasePrice = z.CostPrice,
                    Type = z.ItemTypeId,
                    TypeValue = z.sys_drop_down_value2.Value,
                    CategoryId = z.CategoryID,
                    stock = z.inv_stock.Any() ? z.inv_stock.Sum(a => a.Quantity) : 0,
                    GroupId = z.GroupId
                }).ToList();
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
        [ActionName("searchBatchNo")]
        public async Task<ResponseInfo> searchBatchNo(string itemId, string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var BatchInfo = _inv_stockService.Queryable().Where(a => a.ItemID.ToString() == itemId && a.CompanyId == CompanyID && (a.BatchSarialNumber.ToString().Contains(term))).Select(z => new
                {
                    value = z.ID,
                    label = z.BatchSarialNumber
                }).ToList();
                objResponse.ResultSet = new
                {
                    BatchInfo = BatchInfo
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
        [ActionName("GetList")]
        public ResponseInfo GetList()
        {
            var objResponse = new ResponseInfo();

            return objResponse;
        }
        [HttpGet]
        [ActionName("GetByIdParam")]
        public ResponseInfo GetByIdParam(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var result = _service.Queryable().Where(e => e.CompanyID == CompanyID && e.ID.ToString() == Id).Include(x => x.pur_sale_dt).FirstOrDefault();
                var IsRefund = _service.Queryable().Any(e => e.CompanyID == CompanyID && e.ReturnInvoiceId.ToString() == Id);
                var itemids = result.pur_sale_dt.Select(a => a.ItemID).ToList();
                var ItemList = _adm_itemService.Queryable().Where(a => a.CompanyId == CompanyID && (itemids.Contains(a.ID))).Include(a => a.inv_stock).Select(z => new
                {
                    value = z.ID,
                    label = z.Name,
                    SalePrice = z.SalePrice,
                    CostPrice = z.CostPrice,
                    Type = z.ItemTypeId,
                    TypeValue = z.sys_drop_down_value2.Value,
                    CategoryId = z.CategoryID,
                    stock = z.inv_stock.Any() ? z.inv_stock.Sum(a => a.Quantity) : 0,
                    GroupId = z.GroupId,
                }).ToList();
                objResponse.ResultSet = new
                {
                    result = result,
                    ItemList = ItemList,
                    IsRefund = IsRefund,
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
        [ActionName("GetById")]
        public ResponseInfo GetById(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var companyInfo = _adm_companyService.Queryable().Where(a => a.ID == CompanyID).Select(z => new
                {
                    CompanyAddress1 = z.CompanyAddress1,
                    CompanyAddress2 = z.CompanyAddress2,
                    CompanyName = z.CompanyName,
                    Province = z.Province,
                    PostalCode = z.PostalCode,
                    FaxFax = z.Fax,
                    Phone = z.Phone,
                    Email = z.Email,
                    Website = z.Website,
                    CompanyLogo = z.CompanyLogo,
                }).FirstOrDefault();
                var Customer = _pur_vendorService.Queryable().Where(a => a.CompanyID == CompanyID).FirstOrDefault();

                objResponse.ResultSet = new
                {
                    Customer = Customer,
                    companyInfo = companyInfo
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
            throw new NotImplementedException();
        }
        public async Task<ResponseInfo> Save(pur_sale_mf Model)
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
                List<pur_sale_dt> purorderitem = new List<pur_sale_dt>();
                purorderitem.AddRange(Model.pur_sale_dt);
                var itemIds = purorderitem.Select(x => x.ItemID).ToList();
                // Fetch item groups using the list of item IDs
                var itemGroups = _adm_itemService.Queryable()
                    .Where(i => itemIds.Contains(i.ID))
                    .ToDictionary(i => i.ID, i => i.GroupId);
                //insert pur_sale_mf
                decimal ID = 1;
                if (_service.Queryable().Count() > 0)
                    ID = _service.Queryable().Max(e => e.ID) + 1;
                Model.ID = ID;
                Model.CompanyID = CompanyID;
                Model.CreatedBy = Request.LoginID();
                Model.CreatedDate = Request.DateTimes();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.SaleTypeDropDownId = (int)sys_dropdown_mfEnum.SaleType;
                Model.Date = DateTime.Now;
                Model.ObjectState = ObjectState.Added;
                Model.pur_sale_dt = null;
                Model.adm_user_mf = null;
                Model.adm_company = null;
                Model.adm_user_mf1 = null;
                _service.Insert(Model);
                //insert pur_sale_dt
                decimal saleItemID = 1;
                if (_pur_sale_dtService.Queryable().Count() > 0)
                    saleItemID = _pur_sale_dtService.Queryable().Max(e => e.ID) + 1;
                adm_item_log adm_item_log_obj = new adm_item_log();
                decimal LogID = 1;
                if (_adm_item_logService.Queryable().Count() > 0)
                    LogID = _adm_item_logService.Queryable().Max(e => e.ID) + 1;

                foreach (pur_sale_dt item in purorderitem)
                {
                    item.ID = saleItemID;
                    item.SaleID = Model.ID;
                    item.CompanyID = CompanyID;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.ObjectState = ObjectState.Added;
                    item.adm_company = null;
                    _pur_sale_dtService.Insert(item);

                    saleItemID++;
                    inv_stock inv_stockModel = _inv_stockService.Queryable().Where(e => e.ItemID == item.ItemID
                    && (e.BatchSarialNumber == null && item.BatchSarialNumber == null
                    || e.BatchSarialNumber.ToString() == item.BatchSarialNumber.ToString())
                    && e.CompanyId == CompanyID).FirstOrDefault();
                    if (inv_stockModel != null)
                    {
                        inv_stockModel.Quantity -= Convert.ToDecimal(item.Quantity);
                        _inv_stockService.Update(inv_stockModel);
                        //log maintain                               
                        adm_item_log_obj.ID = LogID;
                        adm_item_log_obj.CompanyId = CompanyID;
                        adm_item_log_obj.ItemId = item.ItemID;
                        adm_item_log_obj.Quantity = "-" + item.Quantity;
                        adm_item_log_obj.Type = "Sale";
                        adm_item_log_obj.CreatedBy = Request.LoginID();
                        adm_item_log_obj.CreatedDate = Request.DateTimes();
                        adm_item_log_obj.ModifiedBy = Request.LoginID();
                        adm_item_log_obj.ModifiedDate = Request.DateTimes();
                        adm_item_log_obj.ObjectState = ObjectState.Added;
                        _adm_item_logService.Insert(adm_item_log_obj);
                        LogID++;
                    }
                }
                if (purorderitem.Count > 0)
                {
                    emr_income model = new emr_income();
                    decimal IncomeID = 1;
                    if (_emr_incomeService.Queryable().Count() > 0)
                        IncomeID = _emr_incomeService.Queryable().Max(e => e.ID) + 1;
                    model.ID = IncomeID;
                    model.CategoryDropdownId = (int)sys_dropdown_mfEnum.IncomeCategoryDropdownId;
                    // Set CategoryId based on item group
                    if (purorderitem.Any(i => itemGroups[i.ItemID] == 1))//Pharmacy
                        model.CategoryId = 67;
                    else if (purorderitem.Any(i => itemGroups[i.ItemID] == 3))//Lab
                        model.CategoryId = 65;
                    model.Date = Request.DateTimes();
                    model.Remark = "Invoice # " + ID.ToString();
                    model.DueAmount = 0;
                    model.ReceivedAmount = Model.Total;
                    model.CompanyId = Request.CompanyID();
                    model.CreatedBy = Request.LoginID();
                    model.CreatedDate = Request.DateTimes();
                    model.ModifiedBy = Request.LoginID();
                    model.ModifiedDate = Request.DateTimes();
                    model.ObjectState = ObjectState.Added;
                    _emr_incomeService.Insert(model);
                }
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
        private bool ModelExists(string key)
        {
            return _service.Query(e => e.ID.ToString() == key).Select().Any();
        }
        [HttpPut]
        [HttpGet]
        [ActionName("Update")]
        public async Task<ResponseInfo> Update(pur_sale_mf Model)
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
                pur_sale_mf saleobj = _service.Queryable().Where(a => a.ID == Model.ID).FirstOrDefault();
                if (saleobj != null && saleobj.Total < Model.Total)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.Return;
                    return objResponse;
                }
                decimal ReturnInvoiceId = Model.ID;
                decimal CompanyID = Request.CompanyID();
                List<pur_sale_dt> purorderitem = new List<pur_sale_dt>();
                purorderitem.AddRange(Model.pur_sale_dt);
                var itemIds = purorderitem.Select(x => x.ItemID).ToList();
                // Fetch item groups using the list of item IDs
                var itemGroups = _adm_itemService.Queryable()
                    .Where(i => itemIds.Contains(i.ID))
                    .ToDictionary(i => i.ID, i => i.GroupId);

                decimal ID = 1;
                if (_service.Queryable().Count() > 0)
                    ID = _service.Queryable().Max(e => e.ID) + 1;
                Model.ID = ID;
                Model.CompanyID = CompanyID;
                Model.CreatedBy = Request.LoginID();
                Model.CreatedDate = Request.DateTimes();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ReturnInvoiceId = ReturnInvoiceId;
                Model.SaleTypeDropDownId = (int)sys_dropdown_mfEnum.SaleType;
                Model.Date = DateTime.Now;
                Model.ObjectState = ObjectState.Added;
                Model.pur_sale_dt = null;
                Model.adm_user_mf = null;
                Model.adm_company = null;
                Model.adm_user_mf1 = null;
                _service.Insert(Model);
                //insert pur_sale_dt
                decimal saleItemID = 1;
                if (_pur_sale_dtService.Queryable().Count() > 0)
                    saleItemID = _pur_sale_dtService.Queryable().Max(e => e.ID) + 1;
                decimal LogID = 1;
                if (_adm_item_logService.Queryable().Count() > 0)
                    LogID = _adm_item_logService.Queryable().Max(e => e.ID) + 1;
                foreach (pur_sale_dt item in purorderitem)
                {
                    item.ID = saleItemID;
                    item.SaleID = Model.ID;
                    item.CompanyID = CompanyID;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.ObjectState = ObjectState.Added;
                    item.adm_company = null;
                    _pur_sale_dtService.Insert(item);
                    adm_item_log adm_item_log_obj = new adm_item_log();


                    saleItemID++;
                    inv_stock inv_stockModel = _inv_stockService.Queryable().Where(e => e.ItemID == item.ItemID
                    && (e.BatchSarialNumber == null && item.BatchSarialNumber == null
                        || e.BatchSarialNumber.ToString() == item.BatchSarialNumber.ToString())
                    && e.CompanyId == CompanyID).FirstOrDefault();
                    if (inv_stockModel != null)
                    {
                        inv_stockModel.Quantity += Convert.ToDecimal(item.Quantity);
                        _inv_stockService.Update(inv_stockModel);

                        //log maintain                               
                        adm_item_log_obj.ID = LogID;
                        adm_item_log_obj.CompanyId = CompanyID;
                        adm_item_log_obj.ItemId = item.ItemID;
                        adm_item_log_obj.Quantity = "+" + item.Quantity;
                        adm_item_log_obj.Type = "Sale Update";
                        adm_item_log_obj.CreatedBy = Request.LoginID();
                        adm_item_log_obj.CreatedDate = Request.DateTimes();
                        adm_item_log_obj.ModifiedBy = Request.LoginID();
                        adm_item_log_obj.ModifiedDate = Request.DateTimes();
                        adm_item_log_obj.ObjectState = ObjectState.Added;
                        _adm_item_logService.Insert(adm_item_log_obj);
                        LogID++;
                    }
                }
                if (purorderitem.Count > 0)
                {
                    emr_income model = new emr_income();
                    decimal IncomeID = 1;
                    if (_emr_incomeService.Queryable().Count() > 0)
                        IncomeID = _emr_incomeService.Queryable().Max(e => e.ID) + 1;
                    model.ID = IncomeID;
                    model.CategoryDropdownId = (int)sys_dropdown_mfEnum.IncomeCategoryDropdownId;
                    // Set CategoryId based on item group
                    if (purorderitem.Any(i => itemGroups[i.ItemID] == 1))//Pharmacy
                        model.CategoryId = 67;
                    else if (purorderitem.Any(i => itemGroups[i.ItemID] == 3))//Lab
                        model.CategoryId = 65;
                    model.Date = Request.DateTimes();
                    model.Remark = "Invoice # " + ID.ToString();
                    model.DueAmount = 0;
                    model.ReceivedAmount = -(Model.Total);
                    model.CompanyId = Request.CompanyID();
                    model.CreatedBy = Request.LoginID();
                    model.CreatedDate = Request.DateTimes();
                    model.ModifiedBy = Request.LoginID();
                    model.ModifiedDate = Request.DateTimes();
                    model.ObjectState = ObjectState.Added;
                    _emr_incomeService.Insert(model);
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
                decimal id = Convert.ToInt64(Id);

                pur_sale_mf Model = _service.Queryable().Where(e => e.ID == id && e.CompanyID == CompanyID).FirstOrDefault();

                if (Model == null)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
                }

                var DelAllsale_sale_item = _pur_sale_dtService.Queryable().Where(e => e.SaleID.ToString() == Id && e.CompanyID == CompanyID).ToList();

                foreach (var obj in DelAllsale_sale_item)
                {
                    obj.ObjectState = ObjectState.Deleted;
                    _pur_sale_dtService.Delete(obj);
                }



                Model.ObjectState = ObjectState.Deleted;
                _service.Delete(Model);



                await _unitOfWorkAsync.SaveChangesAsync();
                objResponse.Message = MessageStatement.Delete;
                objResponse.IsSuccess = true;
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
        [HttpGet]
        [ActionName("Pagination")]
        public PaginationResult Pagination(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResponse = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResponse = _service.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }

        public ResponseInfo ExportData(int ExportType, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText)
        {
            throw new NotImplementedException();
        }

        public PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            throw new NotImplementedException();
        }
    }
}
