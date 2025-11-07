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
    public class adm_user_sessionController : ApiController, IERPAPIInterface<adm_user_session>, IDisposable
    {
        private readonly Iadm_user_sessionService _service;
        private readonly IStoredProcedureService _procedureService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iemr_patient_billService _emr_patient_billService;
        private readonly Ipur_sale_mfService _pur_sale_mfService;
        private readonly Iipd_procedure_mfService _ipd_procedure_mfService;
        private readonly Iemr_incomeService _emr_incomeService;
        private readonly Iemr_expenseService _emr_expenseService;
        private readonly ipd_paymentService _ipd_paymentService;
        private readonly Ipur_paymentService _pur_paymentService;

        public adm_user_sessionController(IUnitOfWorkAsync unitOfWorkAsync, Iadm_user_sessionService Service,
            IStoredProcedureService ProcedureService, Iemr_patient_billService emr_patient_billService, Ipur_sale_mfService pur_sale_mfService, Ipur_paymentService pur_paymentService
          , ipd_paymentService ipd_paymentService, Iipd_procedure_mfService ipd_procedure_mfService, Iemr_incomeService emr_incomeService, Iemr_expenseService emr_expenseService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = Service;
            _procedureService = ProcedureService;
            _emr_patient_billService = emr_patient_billService;
            _pur_sale_mfService = pur_sale_mfService;
            _ipd_procedure_mfService = ipd_procedure_mfService;
            _emr_incomeService = emr_incomeService;
            _emr_expenseService = emr_expenseService;
            _ipd_paymentService = ipd_paymentService;
            _pur_paymentService = pur_paymentService;
        }

        public async Task<ResponseInfo> Save(adm_user_session Model)
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

                DateTime fullDateTime = DateTime.Today.Add(DateTime.Now.TimeOfDay);

                Model.ID = ID;
                Model.CompanyId = CompanyID;
                Model.UserId = Request.LoginID();
                Model.ShiftStartTime = fullDateTime;
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
                decimal userid = Request.LoginID();
                objResponse.ResultSet = _service.Queryable().Where(x => x.CompanyId == companyID && x.UserId == userid && x.ShiftEndTime == null).FirstOrDefault();
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
        [ActionName("GetShiftCloseData")]
        public ResponseInfo GetShiftCloseData(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var result = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID.ToString() == Id).FirstOrDefault();
                result.OPDBalance = _emr_patient_billService.Queryable().Where(a => a.CompanyId == CompanyID && a.CreatedDate >= result.ShiftStartTime).Sum(a => (decimal?)a.PaidAmount) ?? 0;
                result.PharmacyBalance = _pur_sale_mfService.Queryable().Where(a => a.CompanyID == CompanyID && a.CreatedDate >= result.ShiftStartTime && a.SaleTypeID == 1).Sum(a => (decimal?)a.Total) ?? 0;
                result.Refunds = _pur_sale_mfService.Queryable().Where(a => a.CompanyID == CompanyID && a.CreatedDate >= result.ShiftStartTime && a.SaleTypeID == 2).Sum(a => (decimal?)a.Total) ?? 0;
                result.IPDBalance = _ipd_paymentService.Queryable().Where(a => a.CompanyId == CompanyID && a.CreatedDate >= result.ShiftStartTime).Sum(a => (decimal?)a.Amount) ?? 0;
                result.ExpensesAmount = _emr_expenseService.Queryable().Where(a => a.CompanyId == CompanyID && a.CreatedDate >= result.ShiftStartTime).Sum(a => (decimal?)a.Amount) ?? 0;
                result.PurchaseExpensesAmount = _pur_paymentService.Queryable().Where(p => p.CompanyId == CompanyID && p.PaymentMethodID != 1 && p.CreatedDate >= result.ShiftStartTime).Sum(p => (decimal?)p.Amount) ?? 0m;
                result.OtherIncome = _emr_incomeService.Queryable().Where(a => a.CompanyId == CompanyID && a.CreatedDate >= result.ShiftStartTime).Sum(a => (decimal?)a.ReceivedAmount) ?? 0; ;
                result.TotalCollection = (result.StartingCash + result.OPDBalance + result.PharmacyBalance + result.IPDBalance + result.OtherIncome) - (result.Refunds + result.ExpensesAmount + result.PurchaseExpensesAmount);
                objResponse.ResultSet = result;
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
        public async Task<ResponseInfo> Update(adm_user_session Model)
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
                var shift = _service.Queryable().Where(a => a.ID == Model.ID).FirstOrDefault();
                if (shift == null)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = "Session not found.";
                    return objResponse;
                }
                if (shift.ShiftEndTime != null)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = "Session is already closed.";
                    return objResponse;
                }
                DateTime fullDateTime = DateTime.Today.Add(DateTime.Now.TimeOfDay);
                shift.ShiftEndTime = fullDateTime;
                shift.ClosingBalance = Model.ClosingBalance;
                shift.ClosingComments = Model.ClosingComments;
                shift.ModifiedBy = Request.LoginID();
                shift.ModifiedDate = Request.DateTimes();
                shift.ObjectState = ObjectState.Modified;
                _service.Update(shift);
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;
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
                List<adm_user_session> Models = _service.Queryable().Where(e => e.CompanyId == CompanyID && IdList.Contains(e.ID)).ToList();

                if (Models.Count() == 0)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
                }

                foreach (var Model in Models)
                {

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
        [HttpGet]
        [ActionName("LoadDropdown")]
        public ResponseInfo LoadDropdown(string DropdownIds)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var Ids = DropdownIds.Split(',').Select(s => int.Parse(s)).ToArray();
                //var result = _sys_drop_down_valueService.Queryable().Where(e => (e.CompanyID == null || e.CompanyID == CompanyID) && Ids.Contains(e.DropDownID))
                //     .Select(z => new
                //     {
                //         z.ID,
                //         z.Value,
                //         z.DropDownID
                //     }).ToList();

                //if (result != null)
                //{

                //    objResponse.ResultSet = new
                //    {
                //        dropdownValues = result,

                //    };
                //    objResponse.IsSuccess = true;
                //}
                //else
                //{
                //    objResponse.IsSuccess = false;
                //    objResponse.ErrorMessage = "Email does not exist";
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

        public PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            throw new NotImplementedException();
        }
    }
}
