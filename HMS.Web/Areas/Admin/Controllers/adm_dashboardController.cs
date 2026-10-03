using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Service;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Admission;
using HMS.Service.Services.Appointment;
using HMS.Service.Services.Items;
using HMS.Service.Services.Lab;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;

namespace HMS.Web.API.Areas.Admin.Controllers
{
    [JwtAuthentication]
    public class adm_dashboardController : ApiController, IDisposable
    {
        private readonly Iadm_companyService _adm_companyService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IStoredProcedureService _procedureService;
        private readonly Iemr_incomeService _emr_incomeService;
        private readonly Iemr_appointment_mfService _emr_appointment_mfService;
        private readonly Iemr_patient_billService _emr_patient_billService;
        private readonly Iemr_patientService _emr_patientService;
        private readonly Iemr_expenseService _emr_expenseService;
        private readonly Ipur_paymentService _pur_paymentService;
        private readonly Iadm_userService _adm_userService;
        private readonly Iadm_role_mfService _adm_role_mfService;
        private readonly Iadm_user_companyService _adm_User_CompanyService;
        private readonly Iipd_procedure_expenseService _ipd_procedure_expenseService;
        private readonly Iemr_prescription_mfService _emr_prescription_mfService;
        private readonly Ipur_sale_mfService _pur_sale_mfService;
        private readonly Iipd_paymentService _ipd_paymentService;
        private readonly Ilab_patient_testService _lab_patient_testService;



        public adm_dashboardController(IUnitOfWorkAsync unitOfWorkAsync,
             Iadm_companyService adm_companyService,
           IStoredProcedureService ProcedureService, Iemr_incomeService emr_IncomeService,
           Iemr_appointment_mfService emr_appointment_mfService, Iemr_patient_billService emr_patient_billService,
           Iemr_patientService emr_PatientService, Iemr_expenseService emr_ExpenseService, Ipur_paymentService pur_paymentService
            , Iadm_userService adm_userService, Iadm_role_mfService adm_role_mfService,
           Iadm_user_companyService adm_User_CompanyService, Iipd_procedure_expenseService ipd_procedure_expenseService
            , Iemr_prescription_mfService emr_prescription_mfService
            , Ipur_sale_mfService pur_sale_mfService,
           Iipd_paymentService ipd_paymentService,
           Ilab_patient_testService lab_patient_testService
            )
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _adm_companyService = adm_companyService;
            _procedureService = ProcedureService;
            _emr_incomeService = emr_IncomeService;
            _emr_appointment_mfService = emr_appointment_mfService;
            _emr_patient_billService = emr_patient_billService;
            _emr_patientService = emr_PatientService;
            _emr_expenseService = emr_ExpenseService;
            _pur_paymentService = pur_paymentService;
            _adm_userService = adm_userService;
            _adm_role_mfService = adm_role_mfService;
            _adm_User_CompanyService = adm_User_CompanyService;
            _ipd_procedure_expenseService = ipd_procedure_expenseService;
            _emr_prescription_mfService = emr_prescription_mfService;
            _pur_sale_mfService = pur_sale_mfService;
            _ipd_paymentService = ipd_paymentService;
            _lab_patient_testService = lab_patient_testService;
        }

        [HttpGet]
        [ActionName("UpdateNotificationViewedStatus")]
        public async Task<ResponseInfo> UpdateNotificationViewedStatus(string Ids)
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
                var IdsArr = Ids.Split('#').Select(decimal.Parse).ToArray();
                //List<adm_notification_alert> list = _adm_notification_alertService.Queryable().Where(x => x.CompanyID == CompanyID && x.TypeID == 2 && IdsArr.Contains(x.ID)).ToList();
                //foreach (var item in list)
                //{
                //    item.IsRead = true;
                //    item.ObjectState = ObjectState.Modified;
                //    _adm_notification_alertService.Update(item);
                //}

                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;

                }
                catch (DbUpdateException)
                {
                    //if (!ModelExists(Model.ID.ToString()))
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
        [ActionName("GetAdmNotifications")]
        public ResponseInfo GetAdmNotifications()
        {
            var objResponse = new ResponseInfo();
            try
            {
                objResponse.ResultSet = GetNotifications();
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        public List<object> GetNotifications()
        {
            var CompanyID = Request.CompanyID();
            List<object> li = new List<object>();
            //return _adm_notification_alertService.Queryable().Where(x => x.CompanyID == CompanyID && x.TypeID == 2)
            //         .Take(100).Include(x => x.pr_employee_mf)
            //         .Select(x => new { x.ID, x.Subject, x.Body, x.IsRead, x.CreatedDate, EmpID = x.UserID, EmpName = x.pr_employee_mf.FirstName + " " + x.pr_employee_mf.LastName, Dept = x.pr_employee_mf.pr_department.DepartmentName, Desig = x.pr_employee_mf.pr_designation.DesignationName })
            //         .OrderByDescending(x => x.CreatedDate).ToList<object>();
            return li;
        }
        [HttpGet]
        [ActionName("Pagination")]
        public PaginationResult Pagination(string filterParams, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                string PayrollRegion = WebConfigurationManager.AppSettings["PayrollRegion"];
                decimal CompanyID = Request.CompanyID();

                //var leaves = _pr_leave_typeService.Queryable().Where(x => x.CompanyID == CompanyID && (x.Category == "V" || x.Category == "S")).ToList();
                //DashbrdIdsmodel.VacationIds = leaves.Where(x => x.Category == "V").Select(x => x.ID).ToArray();
                //DashbrdIdsmodel.SickIds = leaves.Where(x => x.Category == "S").Select(x => x.ID).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }
        [HttpGet]
        [ActionName("Load")]
        public ResponseInfo Load()
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();





                objResponse.ResultSet = new
                {
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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWorkAsync.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpGet]
        [ActionName("DataLoad")]
        public async Task<ResponseInfo> DataLoad(string FromDate, string ToDate)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var start = Convert.ToDateTime(FromDate).Date;
                var end = Convert.ToDateTime(ToDate).Date.AddDays(1);
                int userId = Request.LoginID();
                var CompanyID = Request.CompanyID();

                var roleId = _adm_User_CompanyService.Queryable().Where(u => u.UserID == userId)
                            .Select(u => u.RoleID).FirstOrDefault();
                // Step 2: Get RoleName from adm_role_mf
                var roleName = _adm_role_mfService.Queryable().Where(r => r.ID == roleId)
                    .Select(r => r.RoleName).FirstOrDefault();

                decimal? doctorFilterId = null;
                if (string.Equals(roleName, "Doctor", StringComparison.OrdinalIgnoreCase))
                {
                    doctorFilterId = Convert.ToDecimal(userId);
                }
                var incomeByCategory = await _emr_incomeService.Queryable()
                .AsNoTracking()
    .Where(i => i.CompanyId == CompanyID && i.Date >= start && i.Date < end)
    .GroupBy(i => i.CategoryId)
    .Select(g => new
    {
        CategoryId = g.Key,
        TotalReceived = g.Sum(x => (decimal?)x.ReceivedAmount) ?? 0m,
        TotalDue = g.Sum(x => (decimal?)x.DueAmount) ?? 0m
    })
    .ToListAsync();
                decimal incomeFor(int catId) =>
    incomeByCategory.FirstOrDefault(x => x.CategoryId == catId)?.TotalReceived ?? 0m;
                var totalReturns = _pur_sale_mfService.Queryable().Where(r => r.CompanyID == CompanyID && r.Date >= start && r.Date < end && r.ReturnInvoiceId != null).Sum(r => (decimal?)r.Total) ?? 0m;
                var totalSales = _pur_sale_mfService.Queryable().Where(i => i.CompanyID == CompanyID && i.Date >= start && i.Date < end && i.ReturnInvoiceId == null).Sum(a => a.Total);
                var pharmacyIncome = totalSales - totalReturns;
                var ipdIncome = _ipd_paymentService.Queryable().Where(i => i.CompanyId == CompanyID && i.PaymentDate >= start && i.PaymentDate < end).Sum(a => (decimal?)a.Amount) ?? 0m;
                var pathologyData = _lab_patient_testService.Queryable()
    .Where(i => i.CompanyId == CompanyID &&
                i.CreatedDate >= start &&
                i.CreatedDate < end)
    .GroupBy(x => x.PatientId)
    .Select(g => new
    {
        TotalAmount = g.Select(x => x.Total).FirstOrDefault(),
        TotalDiscount = g.Select(x => x.DiscountAmount).FirstOrDefault()
    })
    .ToList();
                var pathologyAmount = pathologyData.Sum(x => x.TotalAmount);
                var pathologyDiscount = pathologyData.Sum(x => x.TotalDiscount) ?? 0m;
                var pathologyIncome = pathologyAmount - pathologyDiscount;
                var generalIncome = incomeByCategory.Sum(x => x.TotalReceived);
                var outStandingIncome = incomeByCategory.Sum(x => x.TotalDue);


                var appointments = await _emr_appointment_mfService.Queryable()
    .AsNoTracking()
    .Where(a => a.CompanyId == CompanyID
             && a.AppointmentDate >= start
             && a.AppointmentDate < end)
    .Select(a => new
    {
        a.PatientId,
        a.CompanyId,
        a.StatusId,
        Age = a.emr_patient_mf.Age,
        Gender = a.emr_patient_mf.Gender,
    })
    .ToListAsync();
                var previousAppointments = await _emr_appointment_mfService.Queryable()
    .AsNoTracking()
    .Where(mf => mf.CompanyId == CompanyID && mf.AppointmentDate < start)
    .Select(mf => mf.PatientId)
    .Distinct()
    .ToListAsync();

                var appointmentAgg = new
                {
                    Child = appointments.Count(x => x.Age <= 18),
                    NewPatients = appointments.Count(x => !previousAppointments.Contains(x.PatientId)),
                    Male = appointments.Count(x => x.Gender == 1 && x.Age > 18),
                    Female = appointments.Count(x => x.Gender == 2 && x.Age > 18),
                    Other = appointments.Count(x => x.Gender == 3 && x.Age > 18),
                    PatientVisits = appointments.Count(x => x.StatusId != 1 && x.StatusId != 2 && x.StatusId != 6 && x.StatusId != 25),
                    TotalAppointments = appointments.Count(),
                    MissedAppointments = appointments.Count(x => x.StatusId == 1 || x.StatusId == 2 || x.StatusId == 6)
                };




                IQueryable<emr_patient_bill> billQ = _emr_patient_billService.Queryable()
    .AsNoTracking()
    .Where(b => b.CompanyId == CompanyID && b.BillDate >= start && b.BillDate < end);
                if (doctorFilterId.HasValue)
                {
                    billQ = billQ.Where(b => b.DoctorId == doctorFilterId.Value);
                }
                var billAgg = await billQ.GroupBy(_ => 1)
    .Select(g => new
    {
        PaymentCollection = g.Sum(x => (decimal?)x.PaidAmount) ?? 0m,
        OutstandingAmount = g.Sum(x => (decimal?)x.OutstandingBalance) ?? 0m,
        ProfessionalFees = g.Sum(x => (decimal?)x.Price) ?? 0m
    }).FirstOrDefaultAsync() ?? new { PaymentCollection = 0m, OutstandingAmount = 0m, ProfessionalFees = 0m };

                var expenses = await _emr_expenseService.Queryable()
    .AsNoTracking()
    .Where(e => e.CompanyId == CompanyID && e.Date >= start && e.Date < end)
    .SumAsync(e => (decimal?)e.Amount) ?? 0m;
                var proExpenses = await _ipd_procedure_expenseService.Queryable()
    .AsNoTracking()
    .Where(e => e.CompanyID == CompanyID && e.CreatedDate >= start && e.CreatedDate < end)
    .SumAsync(e => (decimal?)e.Amount) ?? 0m;


                var purPayment = await _pur_paymentService.Queryable()
                    .AsNoTracking()
                    .Where(p => p.CompanyId == CompanyID
                             && p.PaymentMethodID != 1
                             && p.PaymentDate >= start && p.PaymentDate < end)
                    .SumAsync(p => (decimal?)p.Amount) ?? 0m;

                var dueIncomeAllTime = await _emr_incomeService.Queryable()
    .AsNoTracking()
    .Where(i => i.CompanyId == CompanyID)
    .SumAsync(i => (decimal?)i.DueAmount) ?? 0m;

                var today = DateTime.Today;
                var todaysAppointments = _emr_appointment_mfService.Queryable()
.Where(mf => mf.CompanyId == CompanyID && mf.AppointmentDate == today)
.Select(mf => new
{
    mf.AppointmentTime,
    SlotTime = mf.adm_user_mf.SlotTime,
    PatientName = mf.emr_patient_mf.PatientName,
    StatusId = mf.StatusId,
    DoctorName = mf.adm_user_mf.Name
})
.ToList().Select(mf => new AppointmentDto
{
    StartTime = mf.AppointmentTime != null
   ? (DateTime.Today.Add(mf.AppointmentTime)).ToString("hh:mm tt")
   : null,
    EndTime = mf.AppointmentTime != null
   ? (DateTime.Today.Add(mf.AppointmentTime)
       .AddMinutes(mf.SlotTime.HasValue ? mf.SlotTime.Value.TotalMinutes : 0))
       .ToString("hh:mm tt")
   : null,
    PatientName = mf.PatientName,
    ScheduledStatus = mf.StatusId == 2 ? "Missed" :
                 mf.StatusId == 5 ? "Arrival" :
                 mf.StatusId == 6 ? "Cancelled" :
                 mf.StatusId == 7 ? "Delay" :
                 mf.StatusId == 8 ? "Waiting" :
                 mf.StatusId == 9 ? "Checkout" :
                 mf.StatusId == 10 ? "Engaged" :
                 "Scheduled",
    DoctorName = mf.DoctorName
})
.ToList();

                var todaysBirthdays = _emr_patientService.Queryable()
    .Where(mf => mf.CompanyId == CompanyID &&
                 DbFunctions.TruncateTime(mf.DOB) == today)
    .Select(mf => new BirthdayDto
    {
        Id = mf.ID,
        PatientName = mf.PatientName,
        DOB = mf.DOB,
        Image = mf.Image,
        Gender = mf.Gender
    })
    .ToList();
                var todaysFollowUps = _emr_prescription_mfService.Queryable()
      .Where(mf => mf.CompanyID == CompanyID && DbFunctions.TruncateTime(mf.FollowUpDate) == DbFunctions.TruncateTime(DateTime.Today))
      .Select(mf => new
      {
          mf.FollowUpTime,
          mf.emr_patient_mf.PatientName,
          DoctorName = mf.adm_user_mf.Name,
          mf.emr_patient_mf.ID,
          mf.IsCreateAppointment,
          SlotTime = mf.adm_user_mf.SlotTime,
          mf.emr_patient_mf.Mobile,
          mf.FollowUpNotes
      })
      .ToList().Select(mf => new FollowUpDto
      {
          AppointmentTime = mf.FollowUpTime.HasValue
            ? DateTime.Today.Add(mf.FollowUpTime.Value).ToString("hh:mm tt")
            : null,

          PatientName = mf.PatientName,
          DoctorName = mf.DoctorName,
          Id = mf.ID,
          IsCreateAppointment = mf.IsCreateAppointment,

          EndTime = mf.FollowUpTime.HasValue
            ? DateTime.Today.Add(mf.FollowUpTime.Value)
                .AddMinutes(mf.SlotTime.HasValue ? mf.SlotTime.Value.TotalMinutes : 0)
                .ToString("hh:mm tt")
            : null,

          Mobile = mf.Mobile,
          FollowUpNotes = mf.FollowUpNotes
      })
    .Distinct()
    .ToList();
                //graph for income and expense
                var startOfYear = new DateTime(start.Year, 1, 1);
                var currentMonth = DateTime.Now.Month;

                var months = Enumerable.Range(1, currentMonth)
            .Select(m => new DateTime(start.Year, m, 1))
            .ToList();

                var expenselist = await _emr_expenseService.Queryable()
    .Where(e => e.CompanyId == CompanyID)
    .ToListAsync();

                var expensesG = expenselist
    .GroupBy(e => new DateTime(e.Date.Year, e.Date.Month, 1))
    .Select(g => new
    {
        MonthStart = g.Key,
        Amount = g.Sum(x => x.Amount)
    })
    .ToList();
                var incomelist = await _emr_incomeService.Queryable()
           .Where(i => i.CompanyId == CompanyID).ToListAsync();
                var incomeFromEmr = incomelist.GroupBy(e => new DateTime(e.Date.Year, e.Date.Month, 1))
           .Select(g => new
           {
               MonthStart = g.Key,
               Amount = g.Sum(x => (decimal?)x.ReceivedAmount)
           }).ToList();

                var Billlist = await _emr_patient_billService.Queryable()
            .Where(b => b.CompanyId == CompanyID).ToListAsync();
                var incomeFromBills = Billlist.GroupBy(e => new DateTime(e.BillDate.Year, e.BillDate.Month, 1))
            .Select(g => new
            {
                MonthStart = g.Key,
                Amount = g.Sum(x => (decimal?)x.PaidAmount)
            }).ToList();

                var incomes = incomeFromEmr.Concat(incomeFromBills)
                    .GroupBy(x => x.MonthStart)
                    .Select(g => new
                    {
                        MonthStart = g.Key,
                        Amount = g.Sum(x => x.Amount)
                    })
                    .ToList();
                var result = months.Select(m => new IncomeAndExpense
                {
                    MonthNumber = m.Month,
                    MonthName = m.ToString("MMMM"),
                    ExAmount = expensesG.FirstOrDefault(e => e.MonthStart == m)?.Amount ?? 0,
                    Income = incomes.FirstOrDefault(i => i.MonthStart == m)?.Amount ?? 0
                }).OrderBy(r => r.MonthNumber) // Sort by number, not name
.ToList();


                var dto = new DashboardSummaryDto
                {
                    Child = appointmentAgg.Child,
                    NewPatients = appointmentAgg.NewPatients,
                    Male = appointmentAgg.Male,
                    Female = appointmentAgg.Female,
                    Other = appointmentAgg.Other,
                    PatientVisits = appointmentAgg.PatientVisits,
                    TotalAppointments = appointmentAgg.TotalAppointments,
                    MissedAppointments = appointmentAgg.MissedAppointments,
                    PaymentCollection = billAgg.PaymentCollection,
                    OutstandingAmount = billAgg.OutstandingAmount,
                    ProfessionalFees = billAgg.ProfessionalFees,
                    Expenses = expenses,
                    ProExpenses = proExpenses,
                    PurPayment = purPayment,
                    OutStandingIncome = outStandingIncome, // from IncomeCTE aggregation (date-range)
                    DueIncome = dueIncomeAllTime,
                    Income = 0m, // per SP
                    PharmacyIncome = Convert.ToDecimal(pharmacyIncome),
                    IPDIncome = ipdIncome,
                    PathologyIncome = pathologyIncome,
                    GeneralIncome = generalIncome,
                    TodaysAppointments = todaysAppointments,
                    TodaysBirthdays = todaysBirthdays,
                    TodaysFollowUps = todaysFollowUps,
                    IncomeAndExpense = result,
                };


                objResponse.ResultSet = new
                {
                    dto
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
        [ActionName("Refresh")]
        public ResponseInfo Refresh()
        {
            var objResponse = new ResponseInfo();
            return objResponse;
        }


    }
}
