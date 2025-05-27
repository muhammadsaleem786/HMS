using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Employee;
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
using System.Threading.Tasks;
using System.Web.Http;

namespace HMS.Web.API.Areas.Employee
{
    [JwtAuthentication]
    public class pr_loanController : ApiController, IERPAPIInterface<pr_loan>, IDisposable
    {
        private readonly Ipr_loanService _service;
        private readonly Ipr_loan_payment_dtService _pr_loan_payment_dtService;
        private readonly Ipr_employee_mfService _pr_employee_mfService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly Ipr_employee_payroll_dtService _pr_employee_payroll_dtService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        public pr_loanController(IUnitOfWorkAsync unitOfWorkAsync, Ipr_loanService Service, Ipr_employee_mfService pr_employee_mfService
            , Isys_drop_down_valueService sys_drop_down_valueService, Ipr_loan_payment_dtService pr_loan_payment_dtService,
            Ipr_employee_payroll_dtService pr_employee_payroll_dtService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = Service;
            _pr_loan_payment_dtService = pr_loan_payment_dtService;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _pr_employee_mfService = pr_employee_mfService;
            _pr_employee_payroll_dtService = pr_employee_payroll_dtService;
        }

        [HttpPost]
        public async Task<ResponseInfo> Save(pr_loan Model)
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

                decimal ID = 1;
                if (_service.Queryable().Count() > 0)
                    ID = _service.Queryable().Max(e => e.ID) + 1;

                Model.ID = ID;
                Model.CompanyID = Request.CompanyID();
                Model.PaymentMethodDropdownID = (int)sys_dropdown_mfEnum.LoanPaymentMethod;
                Model.LoanTypeDropdownID = (int)sys_dropdown_mfEnum.LoanType;
                Model.CreatedBy = Request.LoginID();
                Model.CreatedDate = Request.DateTimes();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();

                Model.pr_loan_payment_dt = null;
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

        [HttpPost]
        public async Task<ResponseInfo> AddPayment(pr_loan_payment_dt Model)
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

                decimal ID = 1;
                if (_pr_loan_payment_dtService.Queryable().Count() > 0)
                    ID = _pr_loan_payment_dtService.Queryable().Max(e => e.ID) + 1;

                Model.ID = ID;
                Model.CompanyID = Request.CompanyID();
                Model.ObjectState = ObjectState.Added;
                _pr_loan_payment_dtService.Insert(Model);

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


        [HttpGet]
        [ActionName("GetFilterEmployees")]
        public ResponseInfo GetFilterEmployees(string Keyword)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                //active status id = 1
                objResponse.ResultSet = _pr_employee_mfService.Queryable().Where(x => x.CompanyID == CompanyID && x.StatusID == 1 && string.Concat(x.FirstName.ToLower(), " ", x.LastName.ToLower()).Contains(Keyword.ToLower()))
                    .Select(x => new
                    {
                        x.ID,
                        x.FirstName,
                        x.LastName,
                        x.BasicSalary
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


        public ResponseInfo Load()
        {
            throw new NotImplementedException();
        }

        private bool ModelExists(string key)
        {
            return _service.Query(e => e.ID.ToString() == key).Select().Any();
        }




        [HttpGet]
        [ActionName("GetLoanDetail")]
        public ResponseInfo GetLoanDetail(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var Result = _service.Queryable()
                    .Where(e => e.CompanyID == CompanyID && e.ID.ToString() == Id)
                    .Include(x => x.pr_loan_payment_dt).Include(x => x.pr_employee_mf)
                    .AsEnumerable().Select(x => new
                    {
                        ScreenType = "LMF",
                        ID = x.ID,
                        EmpName = x.pr_employee_mf.FirstName + " " + x.pr_employee_mf.LastName,
                        Description = x.Description,
                        PaymentMethodID = x.PaymentMethodID,
                        DeductionType = x.DeductionType,
                        DeductionValue = x.DeductionType == "F" ? x.DeductionValue : Math.Round(((x.DeductionValue / 100) * x.pr_employee_mf.BasicSalary) ?? 0),
                        InstallmentByBaseSalary = x.InstallmentByBaseSalary,
                        AdjustmentAmount = x.AdjustmentAmount,
                        AdjustmentBy = x.AdjustmentBy,
                        AdjustmentComments = x.AdjustmentComments,
                        AdjustmentDate = x.AdjustmentDate,
                        AdjustmentType = x.AdjustmentType,
                        Transaction = "Loan",
                        TotalBalance = (x.LoanAmount + (double)(x.AdjustmentType == "C" ? (x.AdjustmentAmount ?? 0) : ((x.AdjustmentAmount ?? 0) * -1))) - (x.pr_loan_payment_dt.Sum(z => z.Amount + (double)(z.AdjustmentType == "C" ? (z.AdjustmentAmount ?? 0) : ((z.AdjustmentAmount ?? 0) * -1)))),
                        LoanAmount = x.LoanAmount,
                        LoanDate = x.LoanDate,
                        Payment = 0,
                        Balance = x.LoanAmount + (double)(x.AdjustmentType == "C" ? (x.AdjustmentAmount ?? 0) : ((x.AdjustmentAmount ?? 0) * -1)),
                        TotalInstallmentAmount = x.pr_loan_payment_dt.Sum(z => z.Amount),
                        LoanDetail = x.pr_loan_payment_dt.OrderBy(o => o.PaymentDate).Select((y, index) => new LoanDetailModel
                        {
                            ScreenType = "LDT",
                            ID = y.ID,
                            EmpName = x.pr_employee_mf.FirstName + " " + x.pr_employee_mf.LastName,
                            Description = "",
                            DeductionType = "",
                            DeductionValue = 0,
                            InstallmentByBaseSalary = 0,
                            AdjustmentAmount = y.AdjustmentAmount ?? 0,
                            AdjustmentBy = y.AdjustmentBy,
                            AdjustmentComments = y.AdjustmentComments,
                            AdjustmentDate = y.AdjustmentDate,
                            AdjustmentType = y.AdjustmentType,
                            Transaction = "Loan Payment" + " - Cash",
                            LoanAmount = 0,
                            LoanDate = y.PaymentDate,
                            Payment = y.Amount,
                            Balance = (x.LoanAmount + (double)(x.AdjustmentType == "C" ? (x.AdjustmentAmount ?? 0) : ((x.AdjustmentAmount ?? 0) * -1))) - x.pr_loan_payment_dt.OrderBy(z => z.PaymentDate).Select((s, i) => new { s.PaymentDate, s.ID, s.Amount, RowNo = i, s.AdjustmentAmount, s.AdjustmentType }).Where(d => d.PaymentDate.Date <= y.PaymentDate.Date && d.RowNo <= index).Sum(m => (m.AdjustmentType == "C" ? (m.Amount + (double)(m.AdjustmentAmount ?? 0)) : ((m.Amount + (double)((m.AdjustmentAmount ?? 0) * -1))))),
                        }).OrderBy(z => z.LoanDate).ToList()
                    }).OrderBy(z => z.LoanDate)
                    .FirstOrDefault();

                List<LoanDetailModel> DetModel = new List<LoanDetailModel>();
                var LoanModel = new LoanDetailModel
                {
                    ScreenType = Result.ScreenType,
                    ID = Result.ID,
                    Transaction = Result.Transaction,
                    LoanAmount = Result.LoanAmount,
                    LoanDate = Result.LoanDate,
                    Payment = Result.Payment,
                    Balance = Result.Balance,
                    EmpName = Result.EmpName,
                    Description = Result.Description,
                    TotalBalance = Result.TotalBalance,
                    DeductionType = Result.DeductionType,
                    DeductionValue = Result.DeductionValue,
                    InstallmentByBaseSalary = Result.InstallmentByBaseSalary ?? 0,
                    TotalInstallmentAmount = Result.TotalInstallmentAmount,
                    AdjustmentAmount = Result.AdjustmentAmount ?? 0,
                    AdjustmentBy = Result.AdjustmentBy,
                    AdjustmentComments = Result.AdjustmentComments,
                    AdjustmentDate = Result.AdjustmentDate,
                    AdjustmentType = Result.AdjustmentType ?? "C",
                };

                DetModel.Add(LoanModel);
                DetModel.AddRange(Result.LoanDetail);

                if (Result.PaymentMethodID == 1)
                {
                    var SalaryDeductionList = _pr_employee_payroll_dtService.Queryable().Where(e => e.RefID.ToString() == Id && e.pr_employee_payroll_mf.Status == "P")
                         .Include(i => i.pr_employee_payroll_mf).AsEnumerable().Select(s => new LoanDetailModel
                         {
                             ScreenType = "SLDT",
                             ID = s.ID,
                             EmpName = "",
                             Description = "",
                             DeductionType = "",
                             DeductionValue = 0,
                             InstallmentByBaseSalary = 0,
                             AdjustmentAmount = s.AdjustmentAmount ?? 0,
                             AdjustmentBy = s.AdjustmentBy,
                             AdjustmentComments = s.AdjustmentComments,
                             AdjustmentDate = s.AdjustmentDate,
                             AdjustmentType = s.AdjustmentType,
                             Transaction = "Loan Payment By Salary of " + s.PayDate.ToString("dd/MM/yyyy"),
                             LoanAmount = 0,
                             LoanDate = s.PayDate,
                             Payment = Convert.ToDouble(s.Amount),
                             Balance = 0,
                         }).ToList();

                    DetModel.AddRange(SalaryDeductionList);
                }



                objResponse.ResultSet = DetModel;
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
        [ActionName("AddAdjustment")]
        public async Task<ResponseInfo> AddAdjustment(LoanAdjustmentModel Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                if (Model.LoanScreenType == "LMF")
                {
                    pr_loan obj = _service.Queryable().Where(e => e.CompanyID == CompanyID && e.ID == Model.LoanMfID).FirstOrDefault();
                    obj.AdjustmentAmount = Model.AdjustmentAmount;
                    obj.AdjustmentBy = Request.LoginID();
                    obj.AdjustmentComments = Model.AdjustmentComments;
                    obj.AdjustmentDate = Model.AdjustmentDate;
                    obj.AdjustmentType = Model.AdjustmentType;
                    obj.ObjectState = ObjectState.Modified;
                    _service.Update(obj);
                }
                else if (Model.LoanScreenType == "SLDT")
                {
                    pr_employee_payroll_dt obj = _pr_employee_payroll_dtService.Queryable().Where(e => e.CompanyId == CompanyID && e.ID == Model.LoanDtID).FirstOrDefault();
                    obj.AdjustmentAmount = Model.AdjustmentAmount;
                    obj.AdjustmentBy = Request.LoginID();
                    obj.AdjustmentComments = Model.AdjustmentComments;
                    obj.AdjustmentDate = Model.AdjustmentDate;
                    obj.AdjustmentType = Model.AdjustmentType;
                    obj.ObjectState = ObjectState.Modified;
                    _pr_employee_payroll_dtService.Update(obj);
                }
                else
                {
                    pr_loan_payment_dt obj = _pr_loan_payment_dtService.Queryable().Where(e => e.CompanyID == CompanyID && e.ID == Model.LoanDtID && e.LoanID == Model.LoanMfID).FirstOrDefault();
                    obj.AdjustmentAmount = Model.AdjustmentAmount;
                    obj.AdjustmentBy = Request.LoginID();
                    obj.AdjustmentComments = Model.AdjustmentComments;
                    obj.AdjustmentDate = Model.AdjustmentDate;
                    obj.AdjustmentType = Model.AdjustmentType;
                    obj.ObjectState = ObjectState.Modified;
                    _pr_loan_payment_dtService.Update(obj);
                }


                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;
                }
                catch (DbUpdateException)
                {
                    //if (!ModelExists(Model.PayrollMfID.ToString()))
                    //{
                    //    objResponse.IsSuccess = false;
                    //    objResponse.ErrorMessage = MessageStatement.NotFound;
                    //    return objResponse;
                    //}
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
        [ActionName("DeleteAdjustment")]
        public async Task<ResponseInfo> DeleteAdjustment(string Id, string LoanScreenName)
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

                decimal id = Convert.ToDecimal(Id);
                if (LoanScreenName == "LMF")
                {
                    pr_loan Loanmodel = _service.Queryable().Where(x => x.CompanyID == CompanyID && x.ID == id).FirstOrDefault();
                    if (Loanmodel != null)
                    {
                        Loanmodel.pr_employee_mf = null;
                        Loanmodel.AdjustmentAmount = null;
                        Loanmodel.AdjustmentBy = null;
                        Loanmodel.AdjustmentComments = null;
                        Loanmodel.AdjustmentDate = null;
                        Loanmodel.AdjustmentType = null;
                        Loanmodel.ObjectState = ObjectState.Modified;
                        _service.Update(Loanmodel);
                    }
                }
                else if (LoanScreenName == "SLDT")
                {
                    pr_employee_payroll_dt LoanDTmodel = _pr_employee_payroll_dtService.Queryable().Where(x => x.CompanyId == CompanyID && x.ID == id).FirstOrDefault();
                    if (LoanDTmodel != null)
                    {
                        LoanDTmodel.AdjustmentAmount = null;
                        LoanDTmodel.AdjustmentBy = null;
                        LoanDTmodel.AdjustmentComments = null;
                        LoanDTmodel.AdjustmentDate = null;
                        LoanDTmodel.AdjustmentType = null;
                        LoanDTmodel.ObjectState = ObjectState.Modified;
                        _pr_employee_payroll_dtService.Update(LoanDTmodel);
                    }
                }
                else
                {
                    pr_loan_payment_dt LoanDTmodel = _pr_loan_payment_dtService.Queryable().Where(x => x.CompanyID == CompanyID && x.ID == id).FirstOrDefault();
                    if (LoanDTmodel != null)
                    {
                        LoanDTmodel.AdjustmentAmount = null;
                        LoanDTmodel.AdjustmentBy = null;
                        LoanDTmodel.AdjustmentComments = null;
                        LoanDTmodel.AdjustmentDate = null;
                        LoanDTmodel.AdjustmentType = null;
                        LoanDTmodel.ObjectState = ObjectState.Modified;
                        _pr_loan_payment_dtService.Update(LoanDTmodel);
                    }

                }

                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;
                }
                catch (DbUpdateException)
                {
                    //if (!ModelExists(prModel.ID.ToString()))
                    //{
                    //    objResponse.IsSuccess = false;
                    //    objResponse.ErrorMessage = MessageStatement.NotFound;
                    //    return objResponse;
                    //}
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
        [ActionName("GetById")]
        public ResponseInfo GetById(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                objResponse.ResultSet = _service.Queryable()
                    .Where(e => e.CompanyID == CompanyID && e.ID.ToString() == Id).FirstOrDefault();
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


                var PaginationList = objResult.DataList.Cast<LoanPaginationModel>().ToList();
                var LoanIds = PaginationList.Select(s => s.ID).Distinct().ToArray();
                var SalaryDetail = _pr_employee_payroll_dtService.Queryable()
                       .Where(e => e.CompanyId == CompanyID & e.pr_employee_payroll_mf.Status == "P" && LoanIds.Contains(e.RefID))
                       .AsEnumerable().Select(s => new { s.RefID, s.Amount, s.AdjustmentType, AdjustmentAmount = s.AdjustmentAmount ?? 0 }).ToList();

                double PaymentAmount = 0;
                foreach (var obj in PaginationList)
                {
                    PaymentAmount = Convert.ToDouble(SalaryDetail.Where(e => e.RefID == obj.ID).Select(s => s.Amount + (s.AdjustmentType == "" ? 0 : (s.AdjustmentType == "C" ? s.AdjustmentAmount : s.AdjustmentAmount * -1))).Sum());
                    obj.PaymentAmount += PaymentAmount;
                    obj.Balance -= PaymentAmount;
                }
                objResult.DataList = PaginationList.Cast<object>().ToList();

            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }

        [HttpGet]
        [ActionName("PaginationDetail")]
        public PaginationResult PaginationDetail(decimal LoanID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResult = _service.PaginationDetail(LoanID, CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
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
        public async Task<ResponseInfo> Update(pr_loan Model)
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


                Model.pr_employee_mf = null;
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
                //int[] IdList = Id.Split(',').Select(int.Parse).ToArray();

                List<pr_loan> Models = _service.Queryable().Where(e => e.CompanyID == CompanyID && IdList.Contains(e.ID)).ToList();

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
