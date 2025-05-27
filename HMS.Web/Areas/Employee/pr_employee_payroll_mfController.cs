using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
using HMS.Service;
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
    public class pr_employee_payroll_mfController : ApiController, IERPAPIInterface<pr_employee_payroll_mf>, IDisposable
    {
        private readonly Ipr_pay_scheduleService _pr_pay_scheduleService;
        private readonly Ipr_employee_payroll_mfService _service;
        private readonly Ipr_employee_payroll_dtService _pr_employee_payroll_dtService;
        private readonly Ipr_allowanceService _pr_allowanceService;
        private readonly Ipr_deduction_contributionService _pr_deduction_contributionService;
        private readonly IStoredProcedureService _StoredProcedureService;
        private readonly Ipr_employee_mfService _pr_employee_mfService;
        private readonly Ipr_employee_allowanceService _pr_employee_allowanceService;
        private readonly Ipr_employee_ded_contributionService _pr_employee_ded_contributionService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        public pr_employee_payroll_mfController(IUnitOfWorkAsync unitOfWorkAsync,
            Ipr_employee_payroll_mfService Service, Ipr_employee_payroll_dtService pr_employee_payroll_dtService, Ipr_pay_scheduleService pr_pay_scheduleService,
           Ipr_employee_mfService Ipr_employee_mfService, IStoredProcedureService StoredProcedureService
            , Ipr_allowanceService pr_allowanceService, Ipr_deduction_contributionService pr_deduction_contributionService
            , Isys_drop_down_valueService sys_drop_down_valueService, Ipr_employee_allowanceService pr_employee_allowanceService
            , Ipr_employee_ded_contributionService pr_employee_ded_contributionService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _pr_employee_mfService = Ipr_employee_mfService;
            _service = Service;
            _pr_employee_payroll_dtService = pr_employee_payroll_dtService;
            _pr_pay_scheduleService = pr_pay_scheduleService;
            _pr_allowanceService = pr_allowanceService;
            _pr_deduction_contributionService = pr_deduction_contributionService;
            _StoredProcedureService = StoredProcedureService;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _pr_employee_allowanceService = pr_employee_allowanceService;
            _pr_employee_ded_contributionService = pr_employee_ded_contributionService;
        }
        public ResponseInfo GetList()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResponse.ResultSet = _service.Queryable().Where(x => x.CompanyId == CompanyID).ToList();
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
        [ActionName("RunPayRoll")]
        public ResponseInfo RunPayRoll(SalaryModel Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                double LoginID = Request.LoginID();
                _StoredProcedureService
                       .SP_PR_CalculateSalary(CompanyID, Model.PayScheduleId, Model.EmployeeIds, LoginID);
                pr_pay_schedule pay_Schedule = _pr_pay_scheduleService.Queryable()
                      .Where(x => x.CompanyID == CompanyID && x.ID == Model.PayScheduleId).FirstOrDefault();
                pay_Schedule.Lock = true;
                pay_Schedule.ModifiedBy = Request.LoginID();
                pay_Schedule.ModifiedDate = Request.DateTimes();
                pay_Schedule.ObjectState = ObjectState.Modified;
                _pr_pay_scheduleService.Update(pay_Schedule);
                try
                {
                    _unitOfWorkAsync.SaveChanges();
                    objResponse.Message = MessageStatement.Update;
                    objResponse.Message = "Success";
                    objResponse.ResultSet = new
                    {
                        UniqueId = pay_Schedule.ID + "#" + pay_Schedule.PayDate,
                    };
                }
                catch (DbUpdateException)
                {
                    //if (!ModelExists(pay_Schedule.ID.ToString()))
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
        [ActionName("PublishPayroll")]
        public async Task<ResponseInfo> PublishPayroll(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal loginID = Request.LoginID();
                decimal CompanyID = Request.CompanyID();
                DateTime modifiedDate = Request.DateTimes();
                var item = Id.Split('#');

                decimal ScheduleId = 0;
                DateTime PayDate = new DateTime();
                if (item.Length > 0)
                {
                    ScheduleId = Convert.ToInt32(item[0]);
                    PayDate = Convert.ToDateTime(item[1]);
                }


                List<pr_employee_payroll_mf> models = _service.Queryable()
                    .Where(e => e.CompanyId == CompanyID && e.PayScheduleID == ScheduleId && e.PayDate == PayDate.Date)
                    .ToList();
                foreach (var payrollMf in models)
                {
                    payrollMf.Status = "P";
                    payrollMf.ModifiedBy = loginID;
                    payrollMf.ModifiedDate = modifiedDate;
                    payrollMf.ObjectState = ObjectState.Modified;
                    _service.Update(payrollMf);
                }


                pr_pay_schedule pay_Schedule = _pr_pay_scheduleService.Queryable()
                      .Where(x => x.CompanyID == CompanyID && x.ID == ScheduleId).FirstOrDefault();
                pay_Schedule.Lock = true;

                pay_Schedule.PeriodStartDate = pay_Schedule.PeriodStartDate.AddMonths(1);
                //var firstDayOfMonth = new DateTime(pay_Schedule.PeriodStartDate.Year, pay_Schedule.PeriodStartDate.Month, 1);
                //var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                pay_Schedule.PeriodEndDate = pay_Schedule.PeriodStartDate.AddMonths(1).AddDays(-1); ;
                pay_Schedule.PayDate = pay_Schedule.PayDate.AddMonths(1);
                pay_Schedule.ModifiedBy = loginID;
                pay_Schedule.ModifiedDate = modifiedDate;
                pay_Schedule.ObjectState = ObjectState.Modified;
                _pr_pay_scheduleService.Update(pay_Schedule);
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;
                    objResponse.Message = "Success";
                }
                catch (DbUpdateException)
                {
                    //if (!ModelExists(pay_Schedule.ID.ToString()))
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
        private bool ModelExists(string key)
        {
            return _service.Query(e => e.ID.ToString() == key).Select().Any();
        }
        [HttpGet]
        [ActionName("GetPaySchedules")]
        public ResponseInfo GetPaySchedules()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var PayschedueIds = _StoredProcedureService.SP_GetOpenPayrollPayScheduleIds(CompanyID).ToList().ToArray();
                //var PayschedueIds = _service.GetPayschedueIds(CompanyID);
                var PayScheduleList = _pr_pay_scheduleService.Queryable()
                    .Where(e => e.CompanyID == CompanyID && !PayschedueIds.Contains(e.ID))
                    .Include(x => x.pr_employee_mf).Include(y => y.pr_employee_payroll_mf)
               .Select(s => new { s.ID, ScheduleName = s.ScheduleName + " - (" + s.PeriodStartDate + " - " + s.PeriodEndDate + ")", PayPeriodStartDate = s.PeriodStartDate, PayPeriodEndDate = s.PeriodEndDate, NoOfEmps = s.pr_employee_mf.Select(x => x.ID).Count(), IsLock = s.Lock, Status = s.pr_employee_payroll_mf.Select(x => x.Status).FirstOrDefault() }).ToList();
                objResponse.ResultSet = new
                {
                    PayScheduleList = PayScheduleList,
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
        [ActionName("FormPayrollListLoad")]
        public ResponseInfo FormPayrollListLoad()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResponse.ResultSet = new
                {
                    AllowancesList = _pr_allowanceService.Queryable().Where(e => e.CompanyID == CompanyID).ToList(),
                    ContributionList = _pr_deduction_contributionService.Queryable().Where(e => e.CompanyID == CompanyID && e.Category == "C").ToList(),
                    DeductionList = _pr_deduction_contributionService.Queryable().Where(e => e.CompanyID == CompanyID && e.Category == "D").ToList(),
                    AllowanceCategoryTypes = _sys_drop_down_valueService.Queryable().Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.AllowanceCategory).ToList(),
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
        [ActionName("GetEmployeesByPayScheduleID")]
        public ResponseInfo GetEmployeesByPayScheduleID(decimal id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var EmployeeList = _pr_employee_mfService.Queryable().Where(e => e.CompanyID == CompanyID && e.StatusID == 1 && e.PayScheduleID == id)
                 .Select(s => new { id = s.ID, name = s.FirstName + " " + s.LastName }).ToList();
                objResponse.ResultSet = new
                {
                    EmployeeList = EmployeeList,
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
        [ActionName("GetPayStubOfEmployeeByPayrollMfID")]
        public ResponseInfo GetPayStubOfEmployeeByPayrollMfID(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal payrollmfID = Convert.ToInt32(Id);


                var payroll = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID == payrollmfID)
                    .Include(y => y.pr_employee_payroll_dt)
                    .Include(y => y.pr_employee_mf).Include(y => y.pr_employee_mf.pr_employee_allowance).Include(y => y.pr_employee_mf.pr_employee_ded_contribution)
                    .Select(x => new
                    {
                        pr_employee_payroll_mf = new
                        {
                            x.ID,
                            x.CompanyId,
                            x.PayScheduleID,
                            x.PayDate,
                            x.EmployeeID,
                            x.PayScheduleFromDate,
                            x.PayScheduleToDate,
                            x.FromDate,
                            x.ToDate,
                            x.BasicSalary,
                            x.Status,
                            x.AdjustmentDate,
                            x.AdjustmentType,
                            x.AdjustmentAmount,
                            x.AdjustmentComments,
                            x.AdjustmentBy,
                            x.CreatedBy,
                            x.CreatedDate,
                            x.ModifiedBy,
                            x.ModifiedDate,
                            pr_employee_payroll_dt = x.pr_employee_payroll_dt.Select(s => new { s.ID, s.PayrollID, s.CompanyId, s.PayScheduleID, s.PayDate, s.EmployeeID, s.Type, s.AllowDedID, s.Amount, s.Taxable, s.AdjustmentDate, s.AdjustmentType, s.AdjustmentAmount, s.AdjustmentComments, s.AdjustmentBy, s.RefID }).ToList()
                        ,
                            pr_employee_mf = new
                            {
                                x.pr_employee_mf.ID,
                                x.pr_employee_mf.FirstName,
                                x.pr_employee_mf.LastName,
                                x.pr_employee_mf.BasicSalary
                            },
                        },
                        //EmpAllowances = x.pr_employee_mf.pr_employee_allowance.Select(d => new { d.ID, d.CompanyID, d.EmployeeID, d.EffectiveFrom, d.EffectiveTo, d.PayScheduleID, d.AllowanceID, d.Percentage, d.Amount, d.Taxable, d.IsHouseOrTransAllow }).ToList(),
                        //EmpDedCon = x.pr_employee_mf.pr_employee_ded_contribution.Select(t => new { t.ID, t.CompanyID, t.EmployeeID, t.EffectiveFrom, t.EffectiveTo, t.PayScheduleID, t.Category, t.DeductionContributionID, t.Percentage, t.Amount, t.StartingBalance, t.Taxable }).ToList()
                    })
                    .FirstOrDefault();






                //var payroll = _service.Queryable().Where(e => e.CompanyID == CompanyID && e.ID == payrollmfID)
                //    .Include(y => y.pr_employee_payroll_dt)
                //    .Include(y => y.pr_employee_mf).Include(y => y.pr_employee_mf.pr_employee_allowance).Include(y => y.pr_employee_mf.pr_employee_ded_contribution)
                //    .Select(x => new
                //    {
                //        pr_employee_payroll_mf = new
                //        {
                //            x.ID,
                //            x.CompanyID,
                //            x.PayScheduleID,
                //            x.PayDate,
                //            x.EmployeeID,
                //            x.PayScheduleFromDate,
                //            x.PayScheduleToDate,
                //            x.FromDate,
                //            x.ToDate,
                //            x.BasicSalary,
                //            x.Status,
                //            x.AdjustmentDate,
                //            x.AdjustmentType,
                //            x.AdjustmentAmount,
                //            x.AdjustmentComments,
                //            x.AdjustmentBy,
                //            x.CreatedBy,
                //            x.CreatedDate,
                //            x.ModifiedBy,
                //            x.ModifiedDate,
                //            pr_employee_payroll_dt = x.pr_employee_payroll_dt.Select(s => new { s.ID, s.PayrollID, s.CompanyID, s.PayScheduleID, s.PayDate, s.EmployeeID, s.Type, s.AllowDedID, s.Amount, s.Taxable, s.AdjustmentDate, s.AdjustmentType, s.AdjustmentAmount, s.AdjustmentComments, s.AdjustmentBy }).ToList()
                //        ,
                //            pr_employee_mf = new
                //            {
                //                x.pr_employee_mf.ID,
                //               x.pr_employee_mf.FirstName,
                //               x.pr_employee_mf.LastName,
                //               x.pr_employee_mf.BasicSalary
                //            },
                //        },
                //        EmpAllowances = x.pr_employee_mf.pr_employee_allowance.Select(d => new { d.ID, d.CompanyID, d.EmployeeID, d.EffectiveFrom, d.EffectiveTo, d.PayScheduleID, d.AllowanceID, d.Percentage, d.Amount, d.Taxable, d.IsHouseOrTransAllow }).ToList(),
                //        EmpDedCon = x.pr_employee_mf.pr_employee_ded_contribution.Select(t => new { t.ID, t.CompanyID, t.EmployeeID, t.EffectiveFrom, t.EffectiveTo, t.PayScheduleID, t.Category, t.DeductionContributionID, t.Percentage, t.Amount, t.StartingBalance, t.Taxable }).ToList()
                //    })
                //    .FirstOrDefault();

                //payroll.pr_employee_payroll_mf.pr_employee_payroll_dt = payroll.pr_employee_payroll_dt.ToList<object>();


                //var payroll_mf = payroll.pr_employee_payroll_mf;
                //payroll_mf.pr_employee_payroll_dt = payroll.pr_employee_payroll_dt;
                //payroll_mf.pr_employee_mf = payroll.pr_employee_mf;


                //var EmpAllowances = _pr_employee_allowanceService.Queryable().Where(e => e.CompanyID == CompanyID && e.EmployeeID == payroll.EmployeeID).ToList();
                //var EmpDedCon = _pr_employee_ded_contributionService.Queryable().Where(e => e.CompanyID == CompanyID && e.EmployeeID == payroll.EmployeeID).ToList();
                //objResponse.ResultSet = new
                //{
                //    PayrollModal = payroll,
                //    EmpAllowances = payroll.EmpAllowances,
                //    EmpDedCon = payroll.EmpDedCon,
                //};


                objResponse.ResultSet = payroll.pr_employee_payroll_mf;

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
        [ActionName("SavePayStubOfSingleEmployee")]
        public async Task<ResponseInfo> SavePayStubOfSingleEmployee(pr_employee_payroll_mf Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                List<pr_employee_payroll_dt> PayrollDetList = new List<pr_employee_payroll_dt>();
                PayrollDetList.AddRange(Model.pr_employee_payroll_dt);
                Model.pr_employee_payroll_dt = null;
                Model.pr_employee_mf = null;
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                _service.Update(Model);
                var Ids = PayrollDetList.Where(e => e.ID != 0).Select(s => s.ID).ToArray();
                var DelModels = _pr_employee_payroll_dtService.Queryable()
                    .Where(e => e.CompanyId == CompanyID && e.PayrollID == Model.ID && e.EmployeeID == Model.EmployeeID && !Ids.Contains(e.ID)).ToList();

                foreach (var obj in DelModels)
                {
                    obj.ObjectState = ObjectState.Deleted;
                    _pr_employee_payroll_dtService.Delete(obj);
                }

                pr_employee_payroll_mf payrollmf = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID == Model.ID).Include(x => x.pr_employee_payroll_dt).FirstOrDefault();

                decimal payrolldetID = 1;
                if (_pr_employee_payroll_dtService.Queryable().Count() > 0)
                    payrolldetID = _pr_employee_payroll_dtService.Queryable().Max(e => e.ID) + 1;

                foreach (pr_employee_payroll_dt item in PayrollDetList.ToList())
                {
                    if (item.ID > 0)
                    {
                        var OldPayrolldtModel = payrollmf.pr_employee_payroll_dt.Where(x => x.ID == item.ID).FirstOrDefault();
                        OldPayrolldtModel.Taxable = item.Taxable;
                        OldPayrolldtModel.Amount = item.Amount;
                        //item.AdjustmentComments = item.AdjustmentComments.Trim();
                        if (!string.IsNullOrEmpty(item.AdjustmentComments))
                        {
                            OldPayrolldtModel.AdjustmentDate = item.AdjustmentDate;
                            OldPayrolldtModel.AdjustmentType = item.AdjustmentType;
                            OldPayrolldtModel.AdjustmentAmount = item.AdjustmentAmount;
                            OldPayrolldtModel.AdjustmentComments = item.AdjustmentComments;
                            OldPayrolldtModel.AdjustmentBy = Request.LoginID();
                        }
                        //else
                        //{
                        //    OldPayrolldtModel.AdjustmentDate = null;
                        //    OldPayrolldtModel.AdjustmentType = null;
                        //    OldPayrolldtModel.AdjustmentAmount = null;
                        //    OldPayrolldtModel.AdjustmentComments = null;
                        //    OldPayrolldtModel.AdjustmentBy = null;
                        //}
                        OldPayrolldtModel.pr_employee_mf = null;
                        OldPayrolldtModel.pr_employee_payroll_mf = null;
                        OldPayrolldtModel.ObjectState = ObjectState.Modified;
                        _pr_employee_payroll_dtService.Update(OldPayrolldtModel);
                    }
                    else
                    {
                        item.ID = payrolldetID;
                        item.PayrollID = Model.ID;
                        item.PayScheduleID = Model.PayScheduleID;
                        item.PayDate = Model.PayDate;
                        item.CompanyId = CompanyID;
                        item.EmployeeID = Model.EmployeeID;
                        item.ObjectState = ObjectState.Added;
                        _pr_employee_payroll_dtService.Insert(item);
                        payrolldetID++;
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
        [ActionName("SaveAdjustmentModel")]
        public async Task<ResponseInfo> SaveAdjustmentModel(AdjustmentModel Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                if (Model.AllDedContType == "A" || Model.AllDedContType == "C" || Model.AllDedContType == "D")
                {
                    pr_employee_payroll_dt obj = _pr_employee_payroll_dtService.Queryable().Where(e => e.CompanyId == CompanyID && e.PayrollID == Model.PayrollMfID && e.ID == Model.PayrollDtID).FirstOrDefault();
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
                    pr_employee_payroll_mf obj = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID == Model.PayrollMfID).FirstOrDefault();
                    obj.AdjustmentAmount = Model.AdjustmentAmount;
                    obj.AdjustmentBy = Request.LoginID();
                    obj.AdjustmentComments = Model.AdjustmentComments;
                    obj.AdjustmentDate = Model.AdjustmentDate;
                    obj.AdjustmentType = Model.AdjustmentType;
                    obj.ObjectState = ObjectState.Modified;
                    _service.Update(obj);
                }
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;
                }
                catch (DbUpdateException)
                {
                    if (!ModelExists(Model.PayrollMfID.ToString()))
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
        [ActionName("DeleteEmployeeFromPayrollDetList")]
        public async Task<ResponseInfo> DeleteEmployeeFromPayrollDetList(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal ID = Convert.ToInt32(Id);
                List<pr_employee_payroll_mf> Models = _service.Queryable()
                    .Where(e => e.CompanyId == CompanyID && e.ID == ID)
                    .Include(e => e.pr_employee_payroll_dt).ToList();

                if (Models.Count() == 0)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
                }

                foreach (var Model in Models)
                {
                    foreach (pr_employee_payroll_dt detail in Model.pr_employee_payroll_dt.ToList())
                    {
                        detail.ObjectState = ObjectState.Deleted;
                        _pr_employee_payroll_dtService.Delete(detail);
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

        [HttpGet]
        [ActionName("GetById")]
        public ResponseInfo GetById(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                string UniqueId = Id;
                //objResult = _service.PaginationDetail(CompanyID, UniqueId, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }

        [HttpGet]
        [ActionName("DeleteAdjustment")]
        public async Task<ResponseInfo> DeleteAdjustment(string idd, string AllDedConBasicSalAmntType)
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

                decimal id = Convert.ToDecimal(idd);
                if (AllDedConBasicSalAmntType == "A" || AllDedConBasicSalAmntType == "D" || AllDedConBasicSalAmntType == "C")
                {
                    pr_employee_payroll_dt prDtmodel = _pr_employee_payroll_dtService.Queryable().Where(x => x.CompanyId == CompanyID && x.ID == id).FirstOrDefault();
                    if (prDtmodel != null)
                    {
                        prDtmodel.pr_employee_mf = null;
                        prDtmodel.AdjustmentAmount = null;
                        prDtmodel.AdjustmentBy = null;
                        prDtmodel.AdjustmentComments = null;
                        prDtmodel.AdjustmentDate = null;
                        prDtmodel.AdjustmentType = null;
                        prDtmodel.ObjectState = ObjectState.Modified;
                        _pr_employee_payroll_dtService.Update(prDtmodel);
                    }
                }
                else
                {
                    pr_employee_payroll_mf prmodel = _service.Queryable().Where(x => x.CompanyId == CompanyID && x.ID == id).FirstOrDefault();
                    if (prmodel != null)
                    {
                        prmodel.pr_employee_mf = null;
                        prmodel.AdjustmentAmount = null;
                        prmodel.AdjustmentBy = null;
                        prmodel.AdjustmentComments = null;
                        prmodel.AdjustmentDate = null;
                        prmodel.AdjustmentType = null;
                        prmodel.ObjectState = ObjectState.Modified;
                        _service.Update(prmodel);
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
        [ActionName("PaginationDetail")]
        public PaginationResult PaginationDetail(string UniqueId, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal PayScheduleID = 0;
                DateTime PayDate = new DateTime();
                var item = UniqueId.Split('#');
                if (item.Length > 0)
                {
                    PayScheduleID = Convert.ToDecimal(item[0]);
                    if (item.Length == 2)
                        PayDate = Convert.ToDateTime(item[1]);
                    else
                    {
                        PayDate = _pr_pay_scheduleService.Queryable().Where(x => x.CompanyID == CompanyID && x.ID == PayScheduleID).FirstOrDefault().PayDate;
                        UniqueId = PayScheduleID + "#" + PayDate;
                    }
                }

                objResult = _service.PaginationDetail(CompanyID, UniqueId, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }
        [HttpGet]
        [ActionName("FilterPaginationDetail")]
        public PaginationResult FilterPaginationDetail(string FilterParams, string UniqueId, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal PayScheduleID = 0;
                DateTime PayDate = new DateTime();
                var item = UniqueId.Split('#');
                if (item.Length > 0)
                {
                    PayScheduleID = Convert.ToDecimal(item[0]);
                    if (item.Length == 2)
                        PayDate = Convert.ToDateTime(item[1]);
                    else
                    {
                        PayDate = _pr_pay_scheduleService.Queryable().Where(x => x.CompanyID == CompanyID && x.ID == PayScheduleID).FirstOrDefault().PayDate;
                        UniqueId = PayScheduleID + "#" + PayDate;
                    }
                }

                objResult = _service.FilterPaginationDetail(FilterParams, CompanyID, UniqueId, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
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
        public async Task<ResponseInfo> Delete(string payScheduleId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var item = payScheduleId.Split('#');

                decimal ScheduleId = 0;
                DateTime PayDate = new DateTime();
                if (item.Length > 0)
                {
                    ScheduleId = Convert.ToDecimal(item[0]);
                    PayDate = Convert.ToDateTime(item[1]);
                }

                //decimal[] IdList = Id.Split(',').Select(decimal.Parse).ToArray();
                //int[] IdList = Id.Split(',').Select(int.Parse).ToArray();

                List<pr_employee_payroll_mf> Models = _service.Queryable()
                    .Where(e => e.CompanyId == CompanyID && e.PayScheduleID == ScheduleId && e.PayDate == PayDate.Date)
                    .Include(e => e.pr_employee_payroll_dt).ToList();

                if (Models.Count() == 0)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
                }

                foreach (var Model in Models)
                {
                    foreach (pr_employee_payroll_dt detail in Model.pr_employee_payroll_dt.ToList())
                    {
                        detail.ObjectState = ObjectState.Deleted;
                        _pr_employee_payroll_dtService.Delete(detail);
                    }

                    Model.ObjectState = ObjectState.Deleted;
                    _service.Delete(Model);
                }

                pr_pay_schedule pay_Schedule = _pr_pay_scheduleService.Queryable()
                    .Where(x => x.CompanyID == CompanyID && x.ID == ScheduleId).FirstOrDefault();
                pay_Schedule.Lock = false;
                pay_Schedule.ObjectState = ObjectState.Modified;
                _pr_pay_scheduleService.Update(pay_Schedule);

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
        public Task<ResponseInfo> Save(pr_employee_payroll_mf Model)
        {
            throw new NotImplementedException();
        }
        public ResponseInfo Load()
        {
            throw new NotImplementedException();
        }
        public Task<ResponseInfo> Update(pr_employee_payroll_mf Model)
        {
            throw new NotImplementedException();
        }

        public PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            throw new NotImplementedException();
        }
    }
}
