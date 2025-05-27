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
using Org.BouncyCastle.Crypto;
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
    public class pur_paymentController : ApiController, IERPAPIInterface<pur_payment>, IDisposable
    {
        private readonly IStoredProcedureService _procedureService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Ipur_paymentService _service;
        private readonly Ipur_sale_dtService _pur_sale_dtService;
        private readonly Iadm_companyService _adm_companyService;
        private readonly Ipur_vendorService _pur_vendorService;
        private readonly Iadm_itemService _adm_itemService;
        private readonly Iadm_userService _adm_userService;
        private readonly Iinv_stockService _inv_stockService;
        private readonly Ipur_invoice_mfService _pur_invoice_mfService;

        private readonly Iadm_user_companyService _adm_user_companyService;
        public pur_paymentController(IUnitOfWorkAsync unitOfWorkAsync,
        Ipur_paymentService Service,
           Ipur_sale_dtService pur_sale_dtService,
         Isys_drop_down_valueService sys_drop_down_valueService,
          Iadm_itemService adm_itemService,
          Iadm_userService adm_userService,
          Ipur_vendorService pur_vendorService,
             Iadm_user_companyService adm_user_companyService,
              Iinv_stockService inv_stockService,
               Ipur_invoice_mfService pur_invoice_mfService,
        IStoredProcedureService ProcedureService, Iadm_role_dtService adm_role_dtService, Iadm_companyService adm_companyService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _procedureService = ProcedureService;
            _pur_vendorService = pur_vendorService;
            _service = Service;
            _pur_invoice_mfService = pur_invoice_mfService;
            _pur_sale_dtService = pur_sale_dtService;
            _adm_companyService = adm_companyService;
            _adm_itemService = adm_itemService;
            _adm_userService = adm_userService;
            _adm_user_companyService = adm_user_companyService;
            _inv_stockService = inv_stockService;
        }

        [HttpGet]
        [ActionName("Load")]
        public ResponseInfo Load()
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();

                var list = _sys_drop_down_valueService.Queryable().Where(e => (e.CompanyID == null || e.CompanyID == CompanyID) && e.DropDownID == 41)
                    .Select(z => new
                    {
                        z.ID,
                        z.Value
                    })
                    .ToList();

                objResponse.ResultSet = new
                {
                    list = list,
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
                if (dropdownValues != null)
                {
                    objResponse.ResultSet = new
                    {
                        dropdownValues = dropdownValues,
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
                var result = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID.ToString() == Id).FirstOrDefault();
                var user = _adm_userService.Queryable().Where(a => a.ID == result.CreatedBy).FirstOrDefault();

                objResponse.ResultSet = new
                {
                    result = result,
                    user = user,
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
                var payment = _service.Queryable().Where(a => a.CompanyId == CompanyID).FirstOrDefault();

                objResponse.ResultSet = new
                {
                    payment = payment
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

        public async Task<ResponseInfo> Save(pur_payment Model)
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
                var Totalamount = _pur_invoice_mfService.Queryable().Where(a => a.ID == Model.InvoiveId).Sum(z => z.Total);
                var alreadyPayment = _service.Queryable()
                    .Where(a => a.CompanyId == CompanyID && a.InvoiveId == Model.InvoiveId)
                    .Select(z => (decimal?)z.Amount)
                    .Sum() ?? 0;
                alreadyPayment += Model.Amount;
                if (alreadyPayment > Totalamount)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = "You cannot exceed the purchase order amount.";
                    return objResponse;
                }
                else
                {
                    decimal ID = 1;
                    if (_service.Queryable().Count() > 0)
                        ID = _service.Queryable().Max(e => e.ID) + 1;
                    Model.ID = ID;
                    Model.CompanyId = CompanyID;
                    Model.PaymentMethodDropdownID = (int)sys_dropdown_mfEnum.SalaryPaymentMethod;
                    Model.CreatedBy = Request.LoginID();
                    Model.CreatedDate = Request.DateTimes();
                    Model.ModifiedBy = Request.LoginID();
                    Model.ModifiedDate = Request.DateTimes();
                    Model.ObjectState = ObjectState.Added;
                    Model.pur_invoice_mf = null;
                    Model.adm_user_mf = null;
                    Model.adm_company = null;
                    Model.adm_user_mf1 = null;
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
        public async Task<ResponseInfo> Update(pur_payment Model)
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
                Model.pur_invoice_mf = null;
                Model.adm_company = null;
                Model.adm_user_mf = null;
                Model.adm_user_mf1 = null;
                Model.adm_company = null;
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

                pur_payment Model = _service.Queryable().Where(e => e.ID == id && e.CompanyId == CompanyID).FirstOrDefault();

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

        public ResponseInfo ExportData(int ExportType, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText)
        {
            throw new NotImplementedException();
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
        [ActionName("Pagination")]
        public PaginationResult Pagination(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            throw new NotImplementedException();
        }
    }
}
