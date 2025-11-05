using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
using HMS.Service;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Admission;
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

namespace HMS.Web.API.Areas.Admission.Controllers
{
    [JwtAuthentication]
    public class ipd_paymentController : ApiController, IERPAPIInterface<ipd_payment>, IDisposable
    {
        private readonly IStoredProcedureService _procedureService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly ipd_paymentService _service;
        private readonly Iadm_companyService _adm_companyService;
        private readonly Iadm_userService _adm_userService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly Iadm_user_companyService _adm_user_companyService;
        public ipd_paymentController(IUnitOfWorkAsync unitOfWorkAsync,
        ipd_paymentService Service,
          Iadm_userService adm_userService,
             Iadm_user_companyService adm_user_companyService, Isys_drop_down_valueService sys_drop_down_valueService,
        IStoredProcedureService ProcedureService, Iadm_companyService adm_companyService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = Service;
            _adm_companyService = adm_companyService;
            _adm_userService = adm_userService;
            _adm_user_companyService = adm_user_companyService;
            _sys_drop_down_valueService = sys_drop_down_valueService;
        }

        [HttpGet]
        [ActionName("Load")]
        public ResponseInfo Load()
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var dropDownIds = new[] {41 };
                var allDropDownValues = _sys_drop_down_valueService.Queryable()
    .Where(x => dropDownIds.Contains(x.DropDownID)
                && (x.CompanyID == CompanyID || x.CompanyID == null)).Select(z => new
                {
                    z.ID,
                    z.Value,
                    z.DropDownID,
                })
    .ToList();
                var paymentList = allDropDownValues.Where(d => d.DropDownID == 41)
    .Select(d => new { d.ID, d.Value })
    .ToList();

                objResponse.ResultSet = new
                {
                    paymentList= paymentList,
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
        [ActionName("GetPaymentList")]
        public ResponseInfo GetPaymentList(string AdmitId, string PatientId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var result = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.AdmissionId.ToString() == AdmitId && e.PatientId.ToString() == PatientId).Select(a => new
                {
                    a.ID,
                    a.PatientId,
                    a.AdmissionId,
                    a.PaymentDate,
                    a.Amount,
                    a.Notes
                });
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
        [ActionName("GetById")]
        public ResponseInfo GetById(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var dropDownIds = new[] { 41 };
                var allDropDownValues = _sys_drop_down_valueService.Queryable()
    .Where(x => dropDownIds.Contains(x.DropDownID)
                && (x.CompanyID == CompanyID || x.CompanyID == null)).Select(z => new
                {
                    z.ID,
                    z.Value,
                    z.DropDownID,
                })
    .ToList();
                var paymentList = allDropDownValues.Where(d => d.DropDownID == 41)
.Select(d => new { d.ID, d.Value })
.ToList();
                var payment = _service.Queryable().Where(a => a.CompanyId == CompanyID && a.ID.ToString() == Id).FirstOrDefault();
                objResponse.IsSuccess = true;
                objResponse.ResultSet = new
                {
                    payment = payment,
                    paymentList= paymentList
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

        public async Task<ResponseInfo> Save(ipd_payment Model)
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
                //var alreadyPayment = _service.Queryable()
                //    .Where(a => a.CompanyId == CompanyID)
                //    .Select(z => (decimal?)z.Amount)
                //    .Sum() ?? 0;
                //alreadyPayment += Model.Amount;
                //if (alreadyPayment > Totalamount)
                //{
                //    objResponse.IsSuccess = false;
                //    objResponse.ErrorMessage = "You cannot exceed the purchase order amount.";
                //    return objResponse;
                //}
                //else
                //{
                decimal ID = 1;
                if (_service.Queryable().Count() > 0)
                    ID = _service.Queryable().Max(e => e.ID) + 1;
                Model.ID = ID;
                Model.CompanyId = CompanyID;
                Model.CreatedBy = Request.LoginID();
                Model.CreatedDate = Request.DateTimes();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Added;
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
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        [HttpPost]
        [ActionName("SaveMultiple")]
        public async Task<ResponseInfo> SaveMultiple(List<ipd_payment> Model)
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
                decimal ID = 1;
                if (_service.Queryable().Count() > 0)
                    ID = _service.Queryable().Max(e => e.ID) + 1;
                foreach (ipd_payment item in Model)
                {
                    var payment = _service.Queryable().Where(x => x.ID == item.ID && x.CompanyId == CompanyID).FirstOrDefault();
                    if (payment != null) {
                        payment.PaymentDate = item.PaymentDate;
                        payment.Amount = item.Amount;
                        payment.Notes = item.Notes;
                        payment.ModifiedBy = Request.LoginID();
                        payment.ModifiedDate = Request.DateTimes();
                        payment.ObjectState = ObjectState.Modified;
                        _service.Update(payment);
                    }
                    else
                    {
                        item.ID = ID;
                        item.CompanyId = CompanyID;
                        item.PaymentMethodDropdownID = (int)sys_dropdown_mfEnum.PaymentMethodType;
                        item.CreatedBy = Request.LoginID();
                        item.CreatedDate = Request.DateTimes();
                        item.ModifiedBy = Request.LoginID();
                        item.ModifiedDate = Request.DateTimes();
                        item.ObjectState = ObjectState.Added;
                        _service.Insert(item);
                        ID++;
                    }                    
                }
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Save;
                    objResponse.IsSuccess = true;
                }
                catch (DbUpdateException)
                {
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
        public async Task<ResponseInfo> Update(ipd_payment Model)
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

                ipd_payment Model = _service.Queryable().Where(e => e.ID == id && e.CompanyId == CompanyID).FirstOrDefault();

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
