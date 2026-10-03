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
using static iTextSharp.text.pdf.AcroFields;

namespace HMS.Web.API.Areas.Admin.Controllers
{
    [JwtAuthentication]
    public class sale_invoice_mfController : ApiController, IERPAPIInterface<sale_invoice_mf>, IDisposable
    {
        private readonly IStoredProcedureService _procedureService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Isale_invoice_mfService _service;
        private readonly Isale_invoice_dtService _sale_invoice_dtService;
        private readonly Iadm_companyService _adm_companyService;
        private readonly Isale_customerService _sale_customerService;
        private readonly Iadm_itemService _adm_itemService;
        private readonly Iadm_userService _adm_userService;
        private readonly Iinv_stockService _inv_stockService;
        private readonly Iadm_item_logService _adm_item_logService;
        private readonly Iadm_user_companyService _adm_user_companyService;
        private readonly Iadm_user_sessionService _adm_user_sessionService;

        public sale_invoice_mfController(IUnitOfWorkAsync unitOfWorkAsync,
        Isale_invoice_mfService Service,
           Isale_invoice_dtService sale_invoice_dtService,
         Isys_drop_down_valueService sys_drop_down_valueService,
          Iadm_itemService adm_itemService,
          Iadm_userService adm_userService,
          Isale_customerService sale_customerService,
             Iadm_user_companyService adm_user_companyService,
              Iinv_stockService inv_stockService,
              Iadm_item_logService adm_item_logService, Iadm_user_sessionService adm_user_sessionService,
        IStoredProcedureService ProcedureService, Iadm_role_dtService adm_role_dtService, Iadm_companyService adm_companyService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _procedureService = ProcedureService;
            _sale_customerService = sale_customerService;
            _service = Service;
            _sale_invoice_dtService = sale_invoice_dtService;
            _adm_companyService = adm_companyService;
            _adm_itemService = adm_itemService;
            _adm_userService = adm_userService;
            _adm_user_companyService = adm_user_companyService;
            _inv_stockService = inv_stockService;
            _adm_user_sessionService = adm_user_sessionService;
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
                var ItemList = _adm_itemService.Queryable().Where(a => a.CompanyId == CompanyID).Include(a => a.sys_drop_down_value2).Select(z => new
                {
                    ID = z.ID,
                    SalePrice = z.SalePrice,
                    PurchasePrice = z.CostPrice,
                    Type = z.sys_drop_down_value2.Value,
                    Name = z.Name
                });

                dropdownValues = _sys_drop_down_valueService.Queryable().Where(e => (e.CompanyID == null || e.CompanyID == CompanyID) && Ids.Contains(e.DropDownID)).ToList();
                if (dropdownValues != null)
                {
                    objResponse.ResultSet = new
                    {
                        dropdownValues = dropdownValues,
                        ItemList = ItemList,
                        companyInfo = companyInfo
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
        [ActionName("SearchCustomerByName")]
        public async Task<ResponseInfo> SearchCustomerByName(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var CustomerInfo = _sale_customerService.Queryable().Where(a => a.CompanyID == CompanyID && (a.DisplayName.Contains(term))).Select(z => new
                {
                    value = z.ID,
                    label = z.DisplayName

                }).ToList();
                objResponse.ResultSet = new
                {
                    CustomerInfo = CustomerInfo
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

                var result = _service.Queryable().Where(e => e.CompanyID == CompanyID && e.ID.ToString() == Id).Include(x => x.sale_invoice_dt).Include(a => a.sale_customer).FirstOrDefault();
                var itemids = result.sale_invoice_dt.Select(a => a.ItemID).ToList();
                var itemList = _adm_itemService.Queryable().Where(a => itemids.Contains(a.ID) && a.CompanyId == CompanyID)
                                 .Select(z => new
                                 {
                                     z.ID,
                                     z.Name,
                                     z.sys_drop_down_value2.Value
                                 }).ToList();

                objResponse.ResultSet = new
                {
                    result = result,
                    itemList = itemList,
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
        private bool IsUserShiftOpen(decimal userID, decimal companyID, out string errorMessage)
        {
            errorMessage = string.Empty;

            var sessionOpen = _adm_user_sessionService.Queryable()
                .FirstOrDefault(a => a.UserId == userID
                                  && a.CompanyId == companyID
                                  && a.ShiftEndTime == null);

            if (sessionOpen == null)
            {
                errorMessage = "Please start your shift before performing this operation. The system will not work if your shift is not open.";
                return false;
            }

            return true;
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
                var Customer = _sale_customerService.Queryable().Where(a => a.CompanyID == CompanyID).FirstOrDefault();

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
        [HttpGet]
        [ActionName("UpdateStatus")]
        public ResponseInfo UpdateStatus(string Id, string Statusid)
        {
            var objResponse = new ResponseInfo();

            return objResponse;
        }

        public ResponseInfo GetById(string Id, int NextPreviousIndex)
        {
            throw new NotImplementedException();
        }
        public async Task<ResponseInfo> Save(sale_invoice_mf Model)
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
                List<sale_invoice_dt> saleorderitem = new List<sale_invoice_dt>();
                saleorderitem.AddRange(Model.sale_invoice_dt);
                //insert sale_invoice_mf
                decimal ID = 1;
                if (_service.Queryable().Count() > 0)
                    ID = (_service.Queryable().Where(e => e.CompanyID == CompanyID).Max(x => (decimal?)x.ID) ?? 0) + 1;
                Model.ID = ID;
                Model.CompanyID = CompanyID;
                Model.CreatedBy = Request.LoginID();
                Model.CreatedDate = Request.DateTimes();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Added;
                Model.sale_invoice_dt = null;
                Model.adm_user_mf = null;
                Model.adm_company = null;
                Model.sale_customer = null;
                Model.adm_user_mf1 = null;
                _service.Insert(Model);
                //insert sale_order_item
                decimal invoicItemID = 1;
                if (_sale_invoice_dtService.Queryable().Count() > 0)
                    invoicItemID = (_sale_invoice_dtService.Queryable().Where(e => e.CompanyID == CompanyID).Max(x => (decimal?)x.ID) ?? 0) + 1;
                foreach (sale_invoice_dt item in saleorderitem)
                {
                    item.ID = invoicItemID;
                    item.InvoiceID = Model.ID;
                    item.CompanyID = CompanyID;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.ObjectState = ObjectState.Added;
                    item.adm_company = null;
                    _sale_invoice_dtService.Insert(item);
                    invoicItemID++;

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
        public async Task<ResponseInfo> Update(sale_invoice_mf Model)
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
                var itemIds = Model.sale_invoice_dt.Select(a => a.ItemID).ToList();
                var itemList = _adm_itemService.Queryable().Where(a => itemIds.Contains(a.ID) && a.CompanyId == CompanyID).ToList();


                List<sale_invoice_dt> saleorderitem = new List<sale_invoice_dt>();
                saleorderitem.AddRange(Model.sale_invoice_dt);
                Model.sale_invoice_dt = null;
                Model.adm_company = null;
                Model.adm_user_mf = null;
                Model.adm_user_mf1 = null;
                Model.adm_company = null;
                Model.sale_customer = null;
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Modified;
                _service.Update(Model);
                var DelAllsale_sale_item = _sale_invoice_dtService.Queryable().Where(e => e.InvoiceID == Model.ID && e.CompanyID == CompanyID).ToList();
                foreach (var obj in DelAllsale_sale_item)
                {
                    obj.ObjectState = ObjectState.Deleted;
                    _sale_invoice_dtService.Delete(obj);
                }
                decimal invoicItemID = 1;
                if (_sale_invoice_dtService.Queryable().Count() > 0)
                    invoicItemID = (_sale_invoice_dtService.Queryable().Where(e => e.CompanyID == CompanyID).Max(x => (decimal?)x.ID) ?? 0) + 1;
                foreach (sale_invoice_dt item in saleorderitem)
                {
                    item.ID = invoicItemID;
                    item.InvoiceID = Model.ID;
                    item.CompanyID = CompanyID;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.ObjectState = ObjectState.Added;
                    _sale_invoice_dtService.Insert(item);
                    invoicItemID++;
                }
                if (Model.SaveStatus == 2)
                {
                    decimal LogID = 1;
                    if (_adm_item_logService.Queryable().Count() > 0)
                        LogID = (_adm_item_logService.Queryable().Where(e => e.CompanyId == CompanyID).Max(x => (decimal?)x.ID) ?? 0) + 1;
                    decimal InvID = 1;
                    if (_inv_stockService.Queryable().Count() > 0)
                        InvID = (_inv_stockService.Queryable().Where(e => e.CompanyId == CompanyID).Max(x => (decimal?)x.ID) ?? 0) + 1;

                    foreach (sale_invoice_dt item in saleorderitem)
                    {
                        var findItem = itemList.Where(a => a.ID == item.ItemID && a.CompanyId == CompanyID).FirstOrDefault();
                        adm_item_log adm_item_log_obj = new adm_item_log();
                        if (findItem.TrackInventory)
                        {
                            var batchNo = (item.BatchSarialNumber ?? "0").ToString();
                            inv_stock inv_stockModel = _inv_stockService.Queryable()
                        .Where(e => e.ItemID == item.ItemID
                            && e.CompanyId == CompanyID
                            && (e.BatchSarialNumber ?? "0") == batchNo)
                        .FirstOrDefault();
                            if (inv_stockModel != null)
                            {
                                inv_stockModel.Quantity -= Convert.ToDecimal(item.Quantity);
                                _inv_stockService.Update(inv_stockModel);
                                //log maintain                               
                                adm_item_log_obj.ID = LogID;
                                adm_item_log_obj.CompanyId = CompanyID;
                                adm_item_log_obj.ItemId = item.ItemID;
                                adm_item_log_obj.Quantity = "-" + item.Quantity;
                                adm_item_log_obj.Type = "Sale to customer";
                                adm_item_log_obj.CreatedBy = Request.LoginID();
                                adm_item_log_obj.CreatedDate = Request.DateTimes();
                                adm_item_log_obj.ModifiedBy = Request.LoginID();
                                adm_item_log_obj.ModifiedDate = Request.DateTimes();
                                adm_item_log_obj.ObjectState = ObjectState.Added;
                                _adm_item_logService.Insert(adm_item_log_obj);
                                LogID++;
                            }
                            else
                            {                               
                                //log maintain                               
                                adm_item_log_obj.ID = LogID;
                                adm_item_log_obj.CompanyId = CompanyID;
                                adm_item_log_obj.ItemId = item.ItemID;
                                adm_item_log_obj.Quantity = item.Quantity.ToString();
                                adm_item_log_obj.Type = "Sale to customer";
                                adm_item_log_obj.CreatedBy = Request.LoginID();
                                adm_item_log_obj.CreatedDate = Request.DateTimes();
                                adm_item_log_obj.ModifiedBy = Request.LoginID();
                                adm_item_log_obj.ModifiedDate = Request.DateTimes();
                                adm_item_log_obj.ObjectState = ObjectState.Added;
                                _adm_item_logService.Insert(adm_item_log_obj);
                                LogID++;
                            }
                        }
                        else if (findItem != null)
                        {
                            findItem.CostPrice = Convert.ToDecimal(item.Rate);
                            findItem.SalePrice = Convert.ToDecimal(item.SalePrice);
                            _adm_itemService.Update(findItem);
                            //log maintain                               
                            adm_item_log_obj.ID = LogID;
                            adm_item_log_obj.CompanyId = CompanyID;
                            adm_item_log_obj.ItemId = item.ItemID;
                            adm_item_log_obj.Quantity = "+" + item.Quantity;
                            adm_item_log_obj.Type = "Sale item not find";
                            adm_item_log_obj.CreatedBy = Request.LoginID();
                            adm_item_log_obj.CreatedDate = Request.DateTimes();
                            adm_item_log_obj.ModifiedBy = Request.LoginID();
                            adm_item_log_obj.ModifiedDate = Request.DateTimes();
                            adm_item_log_obj.ObjectState = ObjectState.Added;
                            _adm_item_logService.Insert(adm_item_log_obj);
                            LogID++;
                        }
                    }
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
                //}
                //else
                //{

                //    objResponse.IsSuccess = false;
                //    objResponse.ErrorMessage = "A bill with this number has already been created for this vendor.please check and try again.";

                //}

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
        [ActionName("UpdateBatch")]
        public async Task<ResponseInfo> UpdateBatch(inv_stock Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var obj = _inv_stockService.Queryable().Where(e => e.CompanyId == CompanyID && e.ItemID == Model.ItemID).FirstOrDefault();
                if (obj != null)
                {
                    obj.BatchSarialNumber = Model.BatchSarialNumber;
                    obj.ExpiredWarrantyDate = Model.ExpiredWarrantyDate;
                    obj.ModifiedDate = DateTime.Now;
                    obj.ObjectState = ObjectState.Modified;
                    _inv_stockService.Update(obj);
                    try
                    {
                        await _unitOfWorkAsync.SaveChangesAsync();
                        objResponse.Message = MessageStatement.Update;
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
                sale_invoice_mf Model = _service.Queryable().Where(e => e.ID == id && e.CompanyID == CompanyID).FirstOrDefault();
                if (Model == null)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
                }
                var DelAllsale_sale_item = _sale_invoice_dtService.Queryable().Where(e => e.InvoiceID.ToString() == Id && e.CompanyID == CompanyID).ToList();
                foreach (var obj in DelAllsale_sale_item)
                {
                    obj.ObjectState = ObjectState.Deleted;
                    _sale_invoice_dtService.Delete(obj);
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
