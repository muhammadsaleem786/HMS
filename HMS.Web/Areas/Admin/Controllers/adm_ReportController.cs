using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Admission;
using HMS.Service.Services.Appointment;
using HMS.Service.Services.Items;
using HMS.Service.Services.Lab;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using HMS.Web.API.Interface;
using NPOI.HSSF.Record.Chart;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
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
    public class adm_ReportController : ApiController, IDisposable
    {
        private readonly Iadm_userService _service;
        private readonly Iemr_patientService _emr_patientService;
        private readonly Ilab_patient_testService _lab_patient_testService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iemr_appointment_mfService _emr_appointment_mfService;
        private readonly Iemr_patient_billService _emr_patient_billService;
        private readonly Iemr_prescription_treatmentService _emr_prescription_treatmentService;
        private readonly Iemr_prescription_mfService _emr_prescription_mfService;
        private readonly Iemr_incomeService _emr_incomeService;
        private readonly Iemr_expenseService _emr_expenseService;
        private readonly Iipd_procedure_mfService _ipd_procedure_mfService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly Iipd_paymentService _ipd_paymentService;
        private readonly Iipd_admissionService _ipd_admissionService;
        private readonly Ipur_sale_mfService _pur_sale_mfService;
        private readonly Iadm_user_shareService _adm_user_shareService;
        public adm_ReportController(IUnitOfWorkAsync unitOfWorkAsync,
            Iemr_appointment_mfService emr_appointment_mfService,
            Iemr_patient_billService emr_patient_billService,
            Iadm_userService service, Iemr_incomeService emr_incomeService, Iemr_expenseService emr_expenseService,
        Iemr_prescription_treatmentService emr_prescription_treatmentService,
            Iemr_prescription_mfService emr_prescription_mfService
         , Iemr_patientService emr_patientService, Iipd_procedure_mfService ipd_procedure_mfService, Isys_drop_down_valueService sys_drop_down_valueService, Iipd_paymentService ipd_paymentService, Iipd_admissionService ipd_admissionService, Ipur_sale_mfService pur_sale_mfService, Iadm_user_shareService adm_user_shareService, Ilab_patient_testService lab_patient_testService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = service;
            _emr_prescription_treatmentService = emr_prescription_treatmentService;
            _emr_prescription_mfService = emr_prescription_mfService;
            _emr_patientService = emr_patientService;
            _emr_appointment_mfService = emr_appointment_mfService;
            _emr_patient_billService = emr_patient_billService;
            _emr_incomeService = emr_incomeService;
            _emr_expenseService = emr_expenseService;
            _ipd_procedure_mfService = ipd_procedure_mfService;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _ipd_paymentService = ipd_paymentService;
            _ipd_admissionService = ipd_admissionService;
            _pur_sale_mfService = pur_sale_mfService;
            _adm_user_shareService = adm_user_shareService;
            _lab_patient_testService = lab_patient_testService;
        }
        [HttpPost]
        [ActionName("AppointmentRpt")]
        public ResponseInfo AppointmentRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();

                    DataAccessManager dataAccessManager = new DataAccessManager();
                    DateTime fDate = Convert.ToDateTime(model.FromDate);
                    DateTime TDate = Convert.ToDateTime(model.ToDate);
                    string query = "";
                    if (model.PatientId != null)
                    {
                        query = "select mf.AppointmentDate,mf.AppointmentTime,comp.CompanyName,u.Name,pat.PatientName,pat.Mobile,pat.MRNO, mf.PatientProblem,val.Value,DATEPART(minute,u.SlotTime)SlotTime,(case when val.Value='Missed' then 1 else 0 end)Missed from emr_appointment_mf mf inner join sys_drop_down_value val on val.ID = mf.StatusId and val.DropDownID=1 inner join adm_company comp on comp.ID = mf.CompanyId inner join adm_user_mf u on u.ID = mf.DoctorId inner join emr_patient_mf pat on pat.ID=mf.PatientId and pat.CompanyId=mf.CompanyId where mf.CompanyId ='" + CompanyID + "' and cast(mf.AppointmentDate as date) > '" + fDate.Date + "' and cast(mf.AppointmentDate as date)< '" + TDate.Date + "'and (mf.PatientId = '" + model.PatientId + "') order by mf.AppointmentDate desc";
                    }
                    else
                    {
                        query = "select mf.AppointmentDate,mf.AppointmentTime,comp.CompanyName,u.Name,pat.PatientName,pat.Mobile,pat.MRNO,mf.PatientProblem,val.Value,DATEPART(minute,u.SlotTime)SlotTime,(case when val.Value='Missed' then 1 else 0 end)Missed from emr_appointment_mf mf inner join sys_drop_down_value val on val.ID = mf.StatusId and val.DropDownID=1 inner join adm_company comp on comp.ID = mf.CompanyId inner join adm_user_mf u on u.ID = mf.DoctorId inner join emr_patient_mf pat on pat.ID=mf.PatientId and pat.CompanyId=mf.CompanyId where mf.CompanyId ='" + CompanyID + "' and cast(mf.AppointmentDate as date) > '" + fDate.Date + "' and cast(mf.AppointmentDate as date)< '" + TDate.Date + "' order by mf.AppointmentDate desc";
                    }
                    DataTable dt = dataAccessManager.GetDataTable(query);

                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        AppointList = dt
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
        }
        [HttpPost]
        [ActionName("FeeRpt")]
        public ResponseInfo FeeRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    var FeeList = _emr_patient_billService.Queryable().Where(a => a.CompanyId == CompanyID &&
                  DbFunctions.TruncateTime(a.BillDate) >= DbFunctions.TruncateTime(model.FromDate) &&
                  DbFunctions.TruncateTime(a.BillDate) <= DbFunctions.TruncateTime(model.ToDate)).Select(a => new
                  {
                      ID = a.ID,
                      DoctorId = a.DoctorId,
                      BillDate = a.BillDate,
                      PaidAmount = a.PaidAmount,
                      PatientId = a.PatientId,
                      PatientName = a.emr_patient_mf.PatientName,
                      MrNo = a.emr_patient_mf.MRNO,
                      Mobile = a.emr_patient_mf.Mobile,
                  }).Where(a => model.PatientId == null ? a.PatientId == a.PatientId : a.PatientId == model.PatientId).OrderByDescending(a => a.BillDate).ToList();

                    var result = FeeList.AsEnumerable().Select(a => new
                    {
                        ID = a.ID,
                        DoctorName = _service.Queryable().Where(z => z.ID == a.DoctorId).Count() > 0 ? _service.Queryable().Where(z => z.ID == a.DoctorId).FirstOrDefault().Name : "",
                        BillDate = a.BillDate,
                        PaidAmount = a.PaidAmount,
                        PatientId = a.PatientId,
                        PatientName = a.PatientName,
                        MrNo = a.MrNo,
                        Mobile = a.Mobile,
                    }).ToList();

                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        FeeList = result,
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
        }
        [HttpPost]
        [ActionName("ProcedureRpt")]
        public ResponseInfo ProcedureRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    int userid = Convert.ToInt32(Request.LoginID());
                    DataAccessManager dataAccessManager = new DataAccessManager();
                    var ht = new Hashtable();
                    if (model.ProcedureId != null && model.ProcedureId != 0)
                    {
                        ht.Add("@ProcedureId", model.ProcedureId);
                    }
                    else
                    {
                        ht.Add("@ProcedureId", DBNull.Value);
                    }
                    if (model.DoctorId != null && model.DoctorId != 0)
                    {
                        ht.Add("@DoctorId", model.DoctorId);
                    }
                    else
                    {
                        ht.Add("@DoctorId", DBNull.Value);
                    }
                    ht.Add("@CompanyId", CompanyID);
                    ht.Add("@FromeDate", Convert.ToDateTime(model.FromDate));
                    ht.Add("@ToDate", Convert.ToDateTime(model.ToDate));
                    ht.Add("@UserId", userid);
                    var result = dataAccessManager.GetDataSet("SP_Procedure_Rpt", ht);
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        FeeList = result,
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
        }
        [HttpPost]
        [ActionName("ItemRpt")]
        public ResponseInfo ItemRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    int userid = Convert.ToInt32(Request.LoginID());
                    DataAccessManager dataAccessManager = new DataAccessManager();
                    var ht = new Hashtable();
                    if (model.PatientId != null && model.PatientId != 0)
                    {
                        ht.Add("@PatientId", model.PatientId);
                    }
                    else
                    {
                        ht.Add("@PatientId", DBNull.Value);
                    }
                    if (model.ItemId != null && model.ItemId != 0)
                    {
                        ht.Add("@ItemId", model.ItemId);
                    }
                    else
                    {
                        ht.Add("@ItemId", DBNull.Value);
                    }
                    ht.Add("@CompanyId", CompanyID);
                    ht.Add("@FromeDate", Convert.ToDateTime(model.FromDate));
                    ht.Add("@ToDate", Convert.ToDateTime(model.ToDate));
                    ht.Add("@UserId", userid);
                    var result = dataAccessManager.GetDataSet("SP_SaleByPatient_Rpt", ht);
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        FeeList = result,
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
        }
        [HttpPost]
        [ActionName("PurchaseRpt")]
        public ResponseInfo PurchaseRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    int userid = Convert.ToInt32(Request.LoginID());
                    DataAccessManager dataAccessManager = new DataAccessManager();
                    var ht = new Hashtable();
                    if (model.ItemId != null && model.ItemId != 0)
                    {
                        ht.Add("@ItemId", model.ItemId);
                    }
                    else
                    {
                        ht.Add("@ItemId", DBNull.Value);
                    }
                    ht.Add("@CompanyId", CompanyID);
                    ht.Add("@FromeDate", Convert.ToDateTime(model.FromDate));
                    ht.Add("@ToDate", Convert.ToDateTime(model.ToDate));
                    var result = dataAccessManager.GetDataSet("SP_Purchase_Rpt", ht);
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        FeeList = result,
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
        }
        [HttpPost]
        [ActionName("CurrentStockRpt")]
        public ResponseInfo CurrentStockRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    int userid = Convert.ToInt32(Request.LoginID());
                    DataAccessManager dataAccessManager = new DataAccessManager();
                    var ht = new Hashtable();
                    if (model.ProcedureId != null && model.ProcedureId != 0)
                    {
                        ht.Add("@GroupId", model.ProcedureId);
                    }
                    else
                    {
                        ht.Add("@GroupId", DBNull.Value);
                    }
                    if (model.ItemId != null && model.ItemId != 0)
                    {
                        ht.Add("@ItemId", model.ItemId);
                    }
                    else
                    {
                        ht.Add("@ItemId", DBNull.Value);
                    }
                    ht.Add("@CompanyId", CompanyID);
                    ht.Add("@UserId", userid);
                    var result = dataAccessManager.GetDataSet("SP_CurrentStock_Rpt", ht);
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        FeeList = result,
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
                var categoryList = _sys_drop_down_valueService.Queryable().Where(e => (e.CompanyID == null || e.CompanyID == CompanyID) && e.DropDownID == 64 && e.Value.Contains(term)).Select(z => new
                {
                    value = z.ID,
                    label = z.Value,
                }).ToList();
                objResponse.ResultSet = new
                {
                    categoryList = categoryList
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
        [ActionName("PatientDetailRpt")]
        public ResponseInfo PatientDetailRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    var patient = _emr_patientService.Queryable().Where(a => a.CompanyId == CompanyID &&
                    DbFunctions.TruncateTime(a.CreatedDate) >= DbFunctions.TruncateTime(model.FromDate) &&
                    DbFunctions.TruncateTime(a.CreatedDate) <= DbFunctions.TruncateTime(model.ToDate)).Select(a => new
                    {
                        ID = a.ID,
                        PatientName = a.PatientName,
                        Mobile = a.Mobile,
                        Email = a.Email,
                        Gender = a.Gender == 1 ? "Male" : a.Gender == 2 ? "Female" : "Other",
                        DOB = a.DOB,
                        Age = a.Age,
                        BloodGroup = "", //a.sys_drop_down_value.Value,
                        Address = a.Address,
                        CreatedDate = a.CreatedDate,
                    }).OrderByDescending(a => a.CreatedDate).ToList();
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        patient = patient,
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

        }
        public decimal GetTotalIncome(decimal companyId, DateTime fromDate, DateTime toDate)
        {
            // =========================
            // OPD INCOME
            // =========================
            decimal opd = _emr_patient_billService.Queryable()
                .Where(a => a.CompanyId == companyId &&
                            DbFunctions.TruncateTime(a.BillDate) >= DbFunctions.TruncateTime(fromDate) &&
                            DbFunctions.TruncateTime(a.BillDate) <= DbFunctions.TruncateTime(toDate))
                .Sum(x => (decimal?)x.PaidAmount) ?? 0m;

            // =========================
            // IPD INCOME
            // =========================
            decimal ipd = _ipd_paymentService.Queryable()
                .Where(a => a.CompanyId == companyId &&
                            DbFunctions.TruncateTime(a.PaymentDate) >= DbFunctions.TruncateTime(fromDate) &&
                            DbFunctions.TruncateTime(a.PaymentDate) <= DbFunctions.TruncateTime(toDate))
                .Sum(x => (decimal?)x.Amount) ?? 0m;

            // =========================
            // GENERAL INCOME
            // =========================
            decimal income = _emr_incomeService.Queryable()
                .Where(a => a.CompanyId == companyId &&
                            DbFunctions.TruncateTime(a.Date) >= DbFunctions.TruncateTime(fromDate) &&
                            DbFunctions.TruncateTime(a.Date) <= DbFunctions.TruncateTime(toDate))
                .Sum(x => (decimal?)x.ReceivedAmount) ?? 0m;

            // =========================
            // PHARMACY INCOME
            // =========================
            decimal sales = _pur_sale_mfService.Queryable()
                .Where(a => a.CompanyID == companyId &&
                            DbFunctions.TruncateTime(a.Date) >= DbFunctions.TruncateTime(fromDate) &&
                            DbFunctions.TruncateTime(a.Date) <= DbFunctions.TruncateTime(toDate) &&
                            a.ReturnInvoiceId == null)
                .Sum(x => (decimal?)x.Total) ?? 0m;

            decimal returns = _pur_sale_mfService.Queryable()
                .Where(a => a.CompanyID == companyId &&
                            DbFunctions.TruncateTime(a.Date) >= DbFunctions.TruncateTime(fromDate) &&
                            DbFunctions.TruncateTime(a.Date) <= DbFunctions.TruncateTime(toDate) &&
                            a.ReturnInvoiceId != null)
                .Sum(x => (decimal?)x.Total) ?? 0m;

            decimal pharmacy = sales - returns;

            // =========================
            // FINAL TOTAL
            // =========================
            return opd + ipd + income + pharmacy;
        }
        [HttpPost]
        [ActionName("ClinicwiseRpt")]
        public ResponseInfo ClinicwiseRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    // =========================
                    // OPD (Clinic-wise)
                    // =========================
                    var Paymentlist = _emr_patient_billService.Queryable().Where(a => a.CompanyId == CompanyID &&
               DbFunctions.TruncateTime(a.BillDate) >= DbFunctions.TruncateTime(model.FromDate) &&
               DbFunctions.TruncateTime(a.BillDate) <= DbFunctions.TruncateTime(model.ToDate)).Select(a => new
               {
                   CompanyId = a.adm_company.ID,
                   CompanyName = a.adm_company.CompanyName,
                   PaidAmount = a.PaidAmount
               }).Where(a => model.PatientId == null ? a.CompanyId == a.CompanyId : a.CompanyId == model.PatientId).ToList();

                    // =========================
                    // PHARMACY (Clinic-wise)
                    // =========================
                    var pharmacyList = _pur_sale_mfService.Queryable().
                        Where(r => r.CompanyID == CompanyID && DbFunctions.TruncateTime(r.Date) >= DbFunctions.TruncateTime(model.FromDate) && DbFunctions.TruncateTime(r.Date) <= DbFunctions.TruncateTime(model.ToDate))
                        .Select(x => new
                        {
                            CompanyId = x.CompanyID,
                            Sales = x.ReturnInvoiceId == null ? x.Total : 0m,
                            Returns = x.ReturnInvoiceId != null ? x.Total : 0m
                        }).ToList();
                    var pharmacyGrouped = pharmacyList
      .GroupBy(x => x.CompanyId)
      .ToDictionary(
          g => g.Key,
          g => g.Sum(x => x.Sales) - g.Sum(x => x.Returns)
      );

                    // =========================
                    // IPD (Clinic-wise)
                    // =========================
                    var ipdGrouped = _ipd_paymentService.Queryable()
          .Where(i => i.CompanyId == CompanyID && DbFunctions.TruncateTime(i.PaymentDate) >= DbFunctions.TruncateTime(model.FromDate) && DbFunctions.TruncateTime(i.PaymentDate) <= DbFunctions.TruncateTime(model.ToDate))
          .GroupBy(x => x.CompanyId)
          .ToDictionary(
              g => g.Key,
              g => g.Sum(x => (decimal?)x.Amount) ?? 0m
          );
                    // =========================
                    // GENERAL INCOME (Clinic-wise)
                    // =========================
                    var incomeGrouped = _emr_incomeService.Queryable()
         .Where(i => i.CompanyId == CompanyID && DbFunctions.TruncateTime(i.Date) >= DbFunctions.TruncateTime(model.FromDate) && DbFunctions.TruncateTime(i.Date) <= DbFunctions.TruncateTime(model.ToDate))
         .GroupBy(x => x.CompanyId)
         .ToDictionary(
             g => g.Key,
             g => g.Sum(x => (decimal?)x.ReceivedAmount) ?? 0m
         );
                    // =========================
                    // FINAL GROUP (OPD + Income + Pharmacy + IPD)
                    // =========================
                    var results = Paymentlist
            .GroupBy(x => new { x.CompanyId, x.CompanyName })
            .Select(g =>
            {
                var cid = g.Key.CompanyId;

                decimal opd = g.Sum(x => x.PaidAmount);
                decimal ipd = ipdGrouped.ContainsKey(cid) ? ipdGrouped[cid] : 0m;
                decimal income = incomeGrouped.ContainsKey(cid) ? incomeGrouped[cid] : 0m;
                decimal? pharmacy = pharmacyGrouped.ContainsKey(cid) ? pharmacyGrouped[cid] : 0m;

                return new
                {
                    CompanyName = g.Key.CompanyName,
                    OpdAmount = opd,
                    IpdIncome = ipd,
                    IncomeAmount = income,
                    PharmacyIncome = pharmacy,
                    TotalIncome = opd + ipd + income + pharmacy
                };
            })
            .OrderByDescending(x => x.TotalIncome)
            .ToList();
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        Paymentlist = results,
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
        }
        [HttpPost]
        [ActionName("DoctorswiseFeeRpt")]
        public ResponseInfo DoctorswiseFeeRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    var feeList = _emr_patient_billService.Queryable().Where(a => a.CompanyId == CompanyID &&
                 DbFunctions.TruncateTime(a.BillDate) >= DbFunctions.TruncateTime(model.FromDate) &&
                 DbFunctions.TruncateTime(a.BillDate) <= DbFunctions.TruncateTime(model.ToDate)).Select(a => new
                 {
                     ID = a.ID,
                     DoctorId = a.DoctorId,
                     Price = a.Price,
                     PaidAmount = a.PaidAmount,
                     BillDate = a.BillDate
                 }).Where(a => model.DoctorId == null ? a.DoctorId == a.DoctorId : a.DoctorId == model.DoctorId).OrderByDescending(a => a.ID).ToList();

                    var doctorIds = feeList
           .Select(x => x.DoctorId)
           .Distinct()
           .ToList();

                    var doctors = _service.Queryable()
           .Where(d => doctorIds.Contains(d.ID))
           .Select(d => new
           {
               d.ID,
               d.Name
           }).GroupBy(d => d.ID).ToDictionary(g => g.Key, g => g.First().Name);

                    var expenses = _emr_expenseService.Queryable()
      .Where(e => e.CompanyId == CompanyID
          && e.DoctorId.HasValue
          && doctorIds.Contains((int)e.DoctorId)
          && DbFunctions.TruncateTime(e.Date) >= DbFunctions.TruncateTime(model.FromDate)
          && DbFunctions.TruncateTime(e.Date) <= DbFunctions.TruncateTime(model.ToDate))
      .GroupBy(e => new
      {
          DoctorId = e.DoctorId,
          Date = DbFunctions.TruncateTime(e.Date)
      })
      .Select(g => new
      {
          DoctorId = g.Key.DoctorId,
          Date = g.Key.Date,
          Expense = g.Sum(x => x.Amount)
      })
      .ToList();


                    var result = feeList.Select(a => new
                    {
                        a.ID,
                        a.DoctorId,
                        DoctorName = doctors.ContainsKey(a.DoctorId)
                           ? doctors[a.DoctorId]
                           : "",
                        Expense = expenses
        .Where(e => e.DoctorId == a.DoctorId
            && e.Date.Value.Date == a.BillDate.Date)
        .Select(e => e.Expense)
        .FirstOrDefault(),
                        a.Price,
                        a.PaidAmount,
                        a.BillDate
                    }).ToList();
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        FeeList = result,
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
        }
        [HttpPost]
        [ActionName("DoctorswisePaymentRpt")]
        public ResponseInfo DoctorswisePaymentRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    // =========================
                    // Doctor List
                    // =========================
                    var doctorList = _service.Queryable()
                        .Select(x => new
                        {
                            x.ID,
                            x.Name
                        }).ToList();
                    // =========================
                    // OPD Income
                    // =========================
                    List<DoctorWisePaymentVM> opdIncome = _emr_patient_billService.Queryable()
           .Where(a => a.CompanyId == CompanyID &&
                  DbFunctions.TruncateTime(a.BillDate) >= DbFunctions.TruncateTime(model.FromDate) &&
                  DbFunctions.TruncateTime(a.BillDate) <= DbFunctions.TruncateTime(model.ToDate))
           .Select(a => new DoctorWisePaymentVM
           {
               ID = a.ID,
               CompanyName = a.adm_company.CompanyName,
               OutstandingBalance = a.OutstandingBalance,
               PaidAmount = a.PaidAmount,
               DoctorId = (decimal)a.DoctorId,
               PatientName = a.emr_patient_mf.PatientName,
               Date = a.BillDate,
               Type = "OPD"
           })
           .Where(a => model.DoctorId == null || a.DoctorId == model.DoctorId)
           .ToList();
                    // =========================
                    // IPD Income
                    // =========================
                    var ipdIncome = _ipd_admissionService.Queryable()
                        .Where(i => i.CompanyId == CompanyID &&
                               DbFunctions.TruncateTime(i.AdmissionDate) >= DbFunctions.TruncateTime(model.FromDate) &&
                               DbFunctions.TruncateTime(i.AdmissionDate) <= DbFunctions.TruncateTime(model.ToDate))
                        .Select(a => new
                        {
                            ID = a.ID,
                            CompanyName = a.adm_company.CompanyName,
                            OutstandingBalance = 0m,
                            DoctorId = a.DoctorId,
                            PatientName = a.emr_patient_mf.PatientName,
                            Date = a.AdmissionDate,
                            Type = "IPD"
                        })
                        .Where(a => model.DoctorId == null || a.DoctorId == model.DoctorId)
                        .ToList();
                    List<DoctorWisePaymentVM> ipdResult = ipdIncome.Select(a => new DoctorWisePaymentVM
                    {
                        ID = a.ID,
                        CompanyName = a.CompanyName,
                        OutstandingBalance = a.OutstandingBalance,

                        PaidAmount = _ipd_paymentService.Queryable()
                            .Where(z => z.AdmissionId == a.ID && z.CompanyId == CompanyID)
                            .Sum(y => (decimal?)y.Amount) ?? 0m,

                        DoctorId = a.DoctorId,
                        PatientName = a.PatientName,
                        Date = a.Date,
                        Type = a.Type
                    }).ToList();
                    // =========================
                    // Other Income
                    // =========================
                    List<DoctorWisePaymentVM> income = _emr_incomeService.Queryable()
                        .AsNoTracking()
                        .Where(i => i.CompanyId == CompanyID &&
                               DbFunctions.TruncateTime(i.Date) >= DbFunctions.TruncateTime(model.FromDate) &&
                               DbFunctions.TruncateTime(i.Date) <= DbFunctions.TruncateTime(model.ToDate))
                        .Select(a => new DoctorWisePaymentVM
                        {
                            ID = a.ID,
                            CompanyName = a.adm_company.CompanyName,
                            OutstandingBalance = 0m,
                            PaidAmount = a.ReceivedAmount,
                            DoctorId = a.DoctorId,
                            PatientName = "",
                            Date = a.Date,
                            Type = "Income"
                        })
                        .Where(a => model.DoctorId == null || a.DoctorId == model.DoctorId)
                        .ToList();
                    // =========================
                    // Combine All Results
                    // =========================
                    var allData = opdIncome
            .Concat(ipdResult)
            .Concat(income)
            .Select(a => new DoctorWisePaymentVM
            {
                ID = a.ID,
                DoctorName = doctorList
                    .Where(z => z.ID == a.DoctorId)
                    .Select(z => z.Name)
                    .FirstOrDefault() ?? "",
                CompanyName = a.CompanyName,
                OutstandingBalance = a.OutstandingBalance,
                PaidAmount = a.PaidAmount,
                DoctorId = a.DoctorId,
                PatientName = a.PatientName,
                Date = a.Date,
                Type = a.Type
            })
            .OrderByDescending(x => x.Date)
            .ToList();
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        FeeList = allData
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
        }
        [HttpPost]
        [ActionName("DatewiseExpenseRpt")]
        public ResponseInfo DatewiseExpenseRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    // =========================
                    // Doctor List
                    // =========================
                    var doctorList = _service.Queryable()
                        .Select(x => new
                        {
                            x.ID,
                            x.Name
                        }).ToList();
                    // =========================
                    // Expense
                    // =========================
                    var xpense = _emr_expenseService.Queryable()
           .Where(a => a.CompanyId == CompanyID &&
                  DbFunctions.TruncateTime(a.Date) >= DbFunctions.TruncateTime(model.FromDate) &&
                  DbFunctions.TruncateTime(a.Date) <= DbFunctions.TruncateTime(model.ToDate))
           .Select(a => new
           {
               ID = a.ID,
               CompanyName = a.adm_company.CompanyName,
               PaidAmount = a.Amount,
               DoctorId = (decimal)a.DoctorId,
               Date = a.Date,
               Category = a.sys_drop_down_value.Value,
               Type = "Expense"
           })
           .Where(a => model.DoctorId == null || a.DoctorId == model.DoctorId)
           .ToList();

                    // =========================
                    // Combine All Results
                    // =========================
                    var allData = xpense
            .Select(a => new
            {
                ID = a.ID,
                DoctorName = doctorList
                    .Where(z => z.ID == a.DoctorId)
                    .Select(z => z.Name)
                    .FirstOrDefault() ?? "",
                CompanyName = a.CompanyName,
                PaidAmount = a.PaidAmount,
                DoctorId = a.DoctorId,
                Date = a.Date,
                Type = a.Type,
                Category = a.Category
            })
            .OrderByDescending(x => x.Date)
            .ToList();
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        FeeList = allData
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
        }
        public class DoctorWisePaymentVM
        {
            public decimal ID { get; set; }
            public string DoctorName { get; set; }
            public string CompanyName { get; set; }
            public decimal OutstandingBalance { get; set; }
            public decimal? PaidAmount { get; set; }
            public decimal? DoctorId { get; set; }
            public string PatientName { get; set; }
            public DateTime Date { get; set; }
            public string Type { get; set; }
        }
        public class DoctorShareWiseVM
        {
            public decimal ID { get; set; }
            public string DoctorName { get; set; }
            public string MRNO { get; set; }
            public decimal? Discount { get; set; }
            public decimal? ServiceId { get; set; }
            public string Category { get; set; }
            public decimal OutstandingBalance { get; set; }
            public decimal? DoctorId { get; set; }
            public string PatientName { get; set; }
            public DateTime Date { get; set; }
            public string Type { get; set; }

            public decimal ShareAmount { get; set; }
            public decimal DoctorShare { get; set; }
            public string OwnerShare { get; set; }
            public decimal DoctorShareRate { get; set; }
            public string OwnerShareRate { get; set; }
            public string SharedMethod { get; set; }
        }
        [HttpPost]
        [ActionName("DoctorswiseSummaryPaymentRpt")]
        public ResponseInfo DoctorswiseSummaryPaymentRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    var doctorId = model.DoctorId;
                    decimal CompanyID = Request.CompanyID();
                    // =========================
                    // DOCTOR LIST (FAST LOOKUP)
                    // =========================
                    var doctorDict = _service.Queryable()
            .ToDictionary(x => (int)x.ID, x => x.Name);

                    // =========================
                    // OPD Income
                    // =========================
                    var opdIncome = _emr_patient_billService.Queryable().Where(a => a.CompanyId == CompanyID &&
               DbFunctions.TruncateTime(a.BillDate) >= DbFunctions.TruncateTime(model.FromDate) &&
               DbFunctions.TruncateTime(a.BillDate) <= DbFunctions.TruncateTime(model.ToDate) && (doctorId == null || a.DoctorId == doctorId)).Select(a => new
               {
                   CompanyName = a.adm_company.CompanyName,
                   PaidAmount = a.PaidAmount,
                   DoctorId = a.DoctorId,
               }).ToList();

                    // =========================
                    // IPD Income
                    // =========================
                    var ipdIncome = _ipd_admissionService.Queryable()
                        .Where(i => i.CompanyId == CompanyID &&
                               DbFunctions.TruncateTime(i.AdmissionDate) >= DbFunctions.TruncateTime(model.FromDate) &&
                               DbFunctions.TruncateTime(i.AdmissionDate) <= DbFunctions.TruncateTime(model.ToDate)
                               && (doctorId == null || i.DoctorId == doctorId))
                        .Select(a => new
                        {
                            a.ID,
                            DoctorId = a.DoctorId,
                        })
                        .Where(a => model.DoctorId == null || a.DoctorId == model.DoctorId)
                        .ToList();
                    var ipdGrouped = ipdIncome
            .Select(x => new
            {
                x.DoctorId,
                Amount = _ipd_paymentService.Queryable()
                    .Where(z => z.AdmissionId == x.ID)
                    .Sum(y => (decimal?)y.Amount) ?? 0m
            })
            .GroupBy(x => x.DoctorId)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(x => x.Amount)
            );
                    // =========================
                    // Other Income
                    // =========================
                    var incomeGrouped = _emr_incomeService.Queryable()
                        .AsNoTracking()
                        .Where(i => i.CompanyId == CompanyID &&
                               DbFunctions.TruncateTime(i.Date) >= DbFunctions.TruncateTime(model.FromDate) &&
                               DbFunctions.TruncateTime(i.Date) <= DbFunctions.TruncateTime(model.ToDate) &&
                                (doctorId == null || i.DoctorId == doctorId))
    .ToList()
    .GroupBy(x => x.DoctorId ?? 0)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(x => (decimal?)x.ReceivedAmount) ?? 0m
            );
                    // =========================
                    // OPD GROUPED
                    // =========================
                    var opdGrouped = opdIncome
.GroupBy(x => new
{
    x.DoctorId
})
.Select(g => new
{
    DoctorId = (int)g.Key.DoctorId,
    CompanyName = g.FirstOrDefault().CompanyName,
    OpdAmount = g.Sum(x => x.PaidAmount)
})
.ToList();
                    // =========================
                    // FINAL RESULT
                    // =========================
                    var results = opdGrouped.Select(x =>
                    {
                        decimal ipd = ipdGrouped.ContainsKey(x.DoctorId)
                            ? ipdGrouped[x.DoctorId]
                            : 0m;
                        decimal income = 0m;

                        if (model.DoctorId != null)
                        {
                            income = incomeGrouped.ContainsKey(x.DoctorId)
                                ? incomeGrouped[x.DoctorId]
                                : 0m;
                        }
                        else
                        {
                            income = incomeGrouped.ContainsKey(0)
                                ? incomeGrouped[0]
                                : 0m;
                        }
                        return new
                        {
                            DoctorId = x.DoctorId,
                            DoctorName = doctorDict.ContainsKey((int)x.DoctorId)
                                ? doctorDict[(int)x.DoctorId]
                                : "",

                            CompanyName = x.CompanyName,

                            OpdAmount = x.OpdAmount,
                            IpdIncome = ipd,
                            IncomeAmount = income,
                            TotalIncome = x.OpdAmount + ipd + income
                        };
                    })
.OrderByDescending(x => x.TotalIncome)
.ToList();

                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        FeeList = results
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
        }

        [HttpPost]
        [ActionName("CashFlowRpt")]
        public ResponseInfo CashFlowRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    DataAccessManager dataAccessManager = new DataAccessManager();
                    var ht = new Hashtable();
                    ht.Add("@CompanyId", CompanyID);
                    ht.Add("@FromeDate", Convert.ToDateTime(model.FromDate));
                    ht.Add("@ToDate", Convert.ToDateTime(model.ToDate));
                    ht.Add("@Type", model.Type);
                    var datalist = dataAccessManager.GetDataSet("SP_Emr_CashFlowRpt", ht);

                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        datalist = datalist,
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
        }

        [HttpPost]
        [ActionName("ProfitLossRpt")]
        public ResponseInfo ProfitLossRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    DataAccessManager dataAccessManager = new DataAccessManager();
                    var ht = new Hashtable();
                    ht.Add("@CompanyId", CompanyID);
                    ht.Add("@FromDate", Convert.ToDateTime(model.FromDate));
                    ht.Add("@ToDate", Convert.ToDateTime(model.ToDate));
                    var datalist = dataAccessManager.GetDataSet("SP_ProfitLoss_Rpt", ht);

                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        datalist = datalist,
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
        }
        [HttpPost]
        [ActionName("MedicinewiseRpt")]
        public ResponseInfo MedicinewiseRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    var MedicineList = _emr_prescription_treatmentService.Queryable().Where(a => a.CompanyID == CompanyID &&
                 DbFunctions.TruncateTime(a.emr_prescription_mf.AppointmentDate) >= DbFunctions.TruncateTime(model.FromDate) &&
                 DbFunctions.TruncateTime(a.emr_prescription_mf.AppointmentDate) <= DbFunctions.TruncateTime(model.ToDate)).Select(a => new
                 {
                     ID = a.ID,
                     MedicineName = a.MedicineName,
                     MedicineId = a.MedicineId,
                 }).OrderByDescending(a => a.ID).ToList();
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        MedicineList = MedicineList,

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
        }
        [HttpPost]
        [ActionName("OutstandingRpt")]
        public ResponseInfo OutstandingRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    var OutstandingList = _emr_incomeService.Queryable().Where(a => a.CompanyId == CompanyID &&
                   DbFunctions.TruncateTime(a.Date) >= DbFunctions.TruncateTime(model.FromDate) &&
                   DbFunctions.TruncateTime(a.Date) <= DbFunctions.TruncateTime(model.ToDate)).Select(a => new
                   {
                       ID = a.ID,
                       Remarks = a.Remark,
                       Date = a.Date,
                       OutstandingBalance = a.DueAmount,
                   }).Where(z => z.OutstandingBalance > 0).OrderByDescending(a => a.Date).ToList();
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        OutstandingList = OutstandingList,
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
        }
        [HttpPost]
        [ActionName("PaymentSummaryRpt")]
        public ResponseInfo PaymentSummaryRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();


                    // =========================
                    // Doctor List
                    // =========================
                    var doctorDict = _service.Queryable()
          .ToDictionary(x => x.ID, x => x.Name);
                    // =========================
                    // OPD DATA
                    // =========================
                    var paymentList = _emr_patient_billService.Queryable()
           .Where(a => a.CompanyId == CompanyID &&
                  DbFunctions.TruncateTime(a.BillDate) >= DbFunctions.TruncateTime(model.FromDate) &&
                  DbFunctions.TruncateTime(a.BillDate) <= DbFunctions.TruncateTime(model.ToDate)).Select(a => new
                  {
                      ID = a.ID,
                      BillDate = a.BillDate,
                      DoctorId = a.DoctorId,
                      PaidAmount = a.PaidAmount
                  }).OrderByDescending(a => a.BillDate).ToList();

                    // =========================
                    // IPD DATA (preload once)
                    // =========================
                    var ipdData = _ipd_paymentService.Queryable()
           .Where(i => i.CompanyId == CompanyID &&
                  DbFunctions.TruncateTime(i.PaymentDate) >= DbFunctions.TruncateTime(model.FromDate) &&
                  DbFunctions.TruncateTime(i.PaymentDate) <= DbFunctions.TruncateTime(model.ToDate))
           .GroupBy(x => DbFunctions.TruncateTime(x.PaymentDate))
           .Select(g => new
           {
               Date = g.Key,
               Total = g.Sum(x => (decimal?)x.Amount) ?? 0m
           })
           .ToList();

                    var ipdDict = ipdData.ToDictionary(x => x.Date, x => x.Total);

                    // =========================
                    // INCOME DATA (preload once)
                    // =========================
                    var incomeDict = _emr_incomeService.Queryable()
                        .Where(z => z.CompanyId == CompanyID &&
                               DbFunctions.TruncateTime(z.Date) >= DbFunctions.TruncateTime(model.FromDate) &&
                               DbFunctions.TruncateTime(z.Date) <= DbFunctions.TruncateTime(model.ToDate))
                        .GroupBy(x => DbFunctions.TruncateTime(x.Date))
                        .Select(g => new
                        {
                            Date = g.Key,
                            Total = g.Sum(x => (decimal?)x.ReceivedAmount) ?? 0m
                        })
                        .ToDictionary(x => x.Date, x => x.Total);

                    // =========================
                    // PHARMACY DATA (pre-calc once)
                    // =========================
                    var sales = _pur_sale_mfService.Queryable()
                        .Where(i => i.CompanyID == CompanyID &&
                               DbFunctions.TruncateTime(i.Date) >= DbFunctions.TruncateTime(model.FromDate) &&
                               DbFunctions.TruncateTime(i.Date) <= DbFunctions.TruncateTime(model.ToDate) &&
                               i.ReturnInvoiceId == null)
                        .GroupBy(x => DbFunctions.TruncateTime(x.Date))
                        .Select(g => new
                        {
                            Date = g.Key,
                            Total = g.Sum(x => (decimal?)x.Total) ?? 0m
                        })
                        .ToDictionary(x => x.Date, x => x.Total);

                    var returns = _pur_sale_mfService.Queryable()
                        .Where(r => r.CompanyID == CompanyID &&
                               DbFunctions.TruncateTime(r.Date) >= DbFunctions.TruncateTime(model.FromDate) &&
                               DbFunctions.TruncateTime(r.Date) <= DbFunctions.TruncateTime(model.ToDate) &&
                               r.ReturnInvoiceId != null)
                        .GroupBy(x => DbFunctions.TruncateTime(x.Date))
                        .Select(g => new
                        {
                            Date = g.Key,
                            Total = g.Sum(x => (decimal?)x.Total) ?? 0m
                        })
                        .ToDictionary(x => x.Date, x => x.Total);
                    // =========================
                    // FINAL RESULT (FAST + CLEAN)
                    // =========================
                    var result = paymentList
                        .Select(a =>
                        {
                            var date = a.BillDate.Date;

                            decimal ipd = ipdDict.ContainsKey(date) ? ipdDict[date] : 0m;
                            decimal income = incomeDict.ContainsKey(date) ? incomeDict[date] : 0m;

                            decimal sale = sales.ContainsKey(date) ? sales[date] : 0m;
                            decimal ret = returns.ContainsKey(date) ? returns[date] : 0m;

                            return new
                            {
                                ID = a.ID,
                                BillDate = a.BillDate,
                                DoctorId = a.DoctorId,

                                DoctorName = doctorDict.ContainsKey(a.DoctorId)
                                    ? doctorDict[a.DoctorId]
                                    : "",

                                OpdAmount = a.PaidAmount,
                                IpdIncome = ipd,
                                IncomeAmount = income,
                                pharmacyIncome = sale - ret
                            };
                        })
                        .ToList();

                    // =========================
                    // GROUP FINAL (Doctor + Date)
                    // =========================
                    var results = result
                        .GroupBy(x => new
                        {
                            x.BillDate,
                            x.DoctorId,
                            x.DoctorName
                        })
                        .Select(g => new
                        {
                            g.Key.DoctorName,
                            g.Key.BillDate,
                            g.Key.DoctorId,

                            OpdAmount = g.Sum(x => x.OpdAmount),
                            IpdIncome = g.FirstOrDefault().IpdIncome,
                            IncomeAmount = g.FirstOrDefault().IncomeAmount,
                            pharmacyIncome = g.FirstOrDefault().pharmacyIncome
                        })
                        .OrderByDescending(x => x.BillDate)
                        .ToList();


                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        paymentList = results,
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
        }
        [HttpPost]
        [ActionName("TreatmentwiseRpt")]
        public ResponseInfo TreatmentwiseRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    int userid = Convert.ToInt32(Request.LoginID());
                    DataAccessManager dataAccessManager = new DataAccessManager();
                    var ht = new Hashtable();
                    ht.Add("@CompanyId", CompanyID);
                    ht.Add("@FromeDate", Convert.ToDateTime(model.FromDate));
                    ht.Add("@ToDate", Convert.ToDateTime(model.ToDate));
                    ht.Add("@UserId", userid);
                    var result = dataAccessManager.GetDataSet("SP_TreatmentWise_Rpt", ht);
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        TreatmentList = result,
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
        }
        [HttpPost]
        [ActionName("BirthdayRpt")]
        public ResponseInfo BirthdayRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    var patient = _emr_patientService.Queryable().Where(a => a.CompanyId == CompanyID &&
                   DbFunctions.TruncateTime(a.CreatedDate) >= DbFunctions.TruncateTime(model.FromDate) &&
                   DbFunctions.TruncateTime(a.CreatedDate) <= DbFunctions.TruncateTime(model.ToDate)).Select(a => new
                   {
                       ID = a.ID,
                       PatientName = a.PatientName,
                       Mobile = a.Mobile,
                       Gender = a.Gender == 1 ? "Male" : a.Gender == 2 ? "Female" : "Other",
                       DOB = a.DOB,
                       Age = a.Age,
                   }).OrderByDescending(a => a.ID).ToList();
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        patient = patient,
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
        }
        [HttpPost]
        [ActionName("DetailedPatientRpt")]
        public ResponseInfo DetailedPatientRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    var patient = _emr_patientService.Queryable().Where(a => a.CompanyId == CompanyID &&
                  DbFunctions.TruncateTime(a.CreatedDate) >= DbFunctions.TruncateTime(model.FromDate) &&
                  DbFunctions.TruncateTime(a.CreatedDate) <= DbFunctions.TruncateTime(model.ToDate)).Select(a => new
                  {
                      ID = a.ID,
                      PatientName = a.PatientName,
                      Mobile = a.Mobile,
                      email = a.Email,
                      Gender = a.Gender == 1 ? "Male" : a.Gender == 2 ? "Female" : "Other",
                      DOB = a.DOB,
                      Age = a.Age,
                      Group = "",
                      Notes = "",
                      TotalFeeMedicine = 0,
                      TotalFeetreatment = 0,
                      TotalFee = 0,
                  }).OrderByDescending(a => a.ID).ToList();
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        patient = patient,
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
        }
        [HttpPost]
        [ActionName("FollowupRpt")]
        public ResponseInfo FollowupRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    var followuplist = _emr_prescription_mfService.Queryable().Where(a => a.CompanyID == CompanyID && a.FollowUpDate != null &&
                 DbFunctions.TruncateTime(a.FollowUpDate) >= DbFunctions.TruncateTime(model.FromDate) &&
                 DbFunctions.TruncateTime(a.FollowUpDate) <= DbFunctions.TruncateTime(model.ToDate)).Select(a => new
                 {
                     ID = a.ID,
                     PatientName = a.emr_patient_mf.PatientName,
                     lastAppointment = a.AppointmentDate,
                     MobileNo = a.emr_patient_mf.Mobile,
                     lastVisit = a.FollowUpDate,
                 }).OrderByDescending(a => a.lastVisit).ToList();
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        followuplist = followuplist,
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
        }
        [HttpPost]
        [ActionName("PatientOutstandingRpt")]
        public ResponseInfo PatientOutstandingRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    var billlist = _emr_patient_billService.Queryable().Where(a => a.CompanyId == CompanyID &&
                    DbFunctions.TruncateTime(a.BillDate) >= DbFunctions.TruncateTime(model.FromDate) &&
                    DbFunctions.TruncateTime(a.BillDate) <= DbFunctions.TruncateTime(model.ToDate)).Select(a => new
                    {
                        ID = a.ID,
                        PatientName = a.emr_patient_mf.PatientName,
                        Mobile = a.emr_patient_mf.Mobile,
                        OutstandingBalance = a.OutstandingBalance,
                        MrNo = a.emr_patient_mf.MRNO,
                    }).Where(z => z.OutstandingBalance > 0).OrderByDescending(a => a.ID).ToList();
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        billlist = billlist,
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
        }
        [HttpPost]
        [ActionName("PaymentRpt")]
        public ResponseInfo PaymentRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    var billlist = _emr_patient_billService.Queryable().Where(a => a.CompanyId == CompanyID &&
                DbFunctions.TruncateTime(a.BillDate) >= DbFunctions.TruncateTime(model.FromDate) &&
                DbFunctions.TruncateTime(a.BillDate) <= DbFunctions.TruncateTime(model.ToDate)).Select(a => new
                {
                    ID = a.ID,
                    BillDate = a.BillDate,
                    DoctorId = a.DoctorId,
                    PaidAmount = a.PaidAmount,
                    CreatedBy = a.adm_user_mf.Name,
                    PatientId = a.PatientId,
                    PatientName = a.emr_patient_mf.PatientName,
                    MrNo = a.emr_patient_mf.MRNO,
                    Mobile = a.emr_patient_mf.Mobile,
                }).Where(a => model.PatientId == null ? a.PatientId == a.PatientId : a.PatientId == model.PatientId).OrderByDescending(a => a.BillDate).ToList();

                    var result = billlist.AsEnumerable().Select(a => new
                    {
                        ID = a.ID,
                        BillDate = a.BillDate,
                        DoctorName = _service.Queryable().Where(z => z.ID == a.DoctorId).Count() > 0 ? _service.Queryable().Where(z => z.ID == a.DoctorId).FirstOrDefault().Name : "",
                        PaidAmount = a.PaidAmount,
                        CreatedBy = a.CreatedBy,
                        PatientId = a.PatientId,
                        PatientName = a.PatientName,
                        MrNo = a.MrNo,
                        Mobile = a.Mobile,
                    }).ToList();

                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        billlist = result,
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
        }
        [HttpPost]
        [ActionName("UserShareRpt")]
        public ResponseInfo UserShareRpt(ReportModel model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                // =========================
                // Doctor List
                // =========================
                var doctorList = _service.Queryable()
                    .Select(x => new
                    {
                        x.ID,
                        x.Name
                    }).ToList();
                var userShares = _adm_user_shareService.Queryable()
    .Where(a => a.CompanyId == CompanyID)
    .Select(z => new AdmUserShareVM
    {
        DoctorId = z.DoctorId,
        ServiceId = z.ServiceId ?? 0,
        DoctorShare = z.DoctorShare,
        OwnerShare = z.OwnerShare,
        Category = z.Category,
        SharedMethod = z.SharedMethod
    })
    .ToList();
                // =========================
                // OPD Income
                // =========================
                List<DoctorShareWiseVM> opdIncome = _emr_patient_billService.Queryable()
       .Where(a => a.CompanyId == CompanyID &&
              DbFunctions.TruncateTime(a.BillDate) >= DbFunctions.TruncateTime(model.FromDate) &&
              DbFunctions.TruncateTime(a.BillDate) <= DbFunctions.TruncateTime(model.ToDate))
       .Select(a => new DoctorShareWiseVM
       {
           ID = a.ID,
           MRNO = a.emr_patient_mf.MRNO,
           OutstandingBalance = a.OutstandingBalance,
           ShareAmount = a.PaidAmount,
           DoctorId = (decimal)a.DoctorId,
           Discount = a.Discount,
           ServiceId = a.ServiceId,
           Category = "Procedure",
           PatientName = a.emr_patient_mf.PatientName,
           Date = a.BillDate,
           Type = "OPD"
       })
       .Where(a => model.DoctorId == null || a.DoctorId == model.DoctorId)
       .ToList();
                foreach (var item in opdIncome)
                {
                    CalculateShare(item, userShares);
                }
                // =========================
                // IPD Income
                // =========================
                var ipdIncome = _ipd_admissionService.Queryable()
                    .Where(i => i.CompanyId == CompanyID &&
                           DbFunctions.TruncateTime(i.AdmissionDate) >= DbFunctions.TruncateTime(model.FromDate) &&
                           DbFunctions.TruncateTime(i.AdmissionDate) <= DbFunctions.TruncateTime(model.ToDate))
                    .Select(a => new
                    {
                        ID = a.ID,
                        MRNO = a.emr_patient_mf.MRNO,
                        OutstandingBalance = 0m,
                        DoctorId = a.DoctorId,
                        PatientName = a.emr_patient_mf.PatientName,
                        Date = a.AdmissionDate,
                        Type = "IPD",
                        Category = "Procedure",
                        Discount = 0
                    })
                    .Where(a => model.DoctorId == null || a.DoctorId == model.DoctorId)
                    .ToList();
                List<DoctorShareWiseVM> ipdResult = ipdIncome.Select(a => new DoctorShareWiseVM
                {
                    ID = a.ID,
                    MRNO = a.MRNO,
                    OutstandingBalance = a.OutstandingBalance,
                    ShareAmount = _ipd_paymentService.Queryable()
                        .Where(z => z.AdmissionId == a.ID && z.CompanyId == CompanyID)
                        .Sum(y => (decimal?)y.Amount) ?? 0m,
                    ServiceId = _ipd_procedure_mfService.Queryable()
                        .Where(z => z.AdmissionId == a.ID && z.CompanyId == CompanyID).FirstOrDefault().ServiceId,
                    Discount = a.Discount,
                    DoctorId = a.DoctorId,
                    PatientName = a.PatientName,
                    Date = a.Date,
                    Type = a.Type,
                    Category = a.Category
                }).ToList();
                foreach (var item in ipdResult)
                {
                    CalculateShare(item, userShares);
                }
                // =========================
                // Pharmacy Income
                // =========================
                List<DoctorShareWiseVM> pharmacy = _pur_sale_mfService.Queryable()
                    .AsNoTracking()
                    .Where(i => i.CompanyID == CompanyID &&
                           DbFunctions.TruncateTime(i.Date) >= DbFunctions.TruncateTime(model.FromDate) &&
                           DbFunctions.TruncateTime(i.Date) <= DbFunctions.TruncateTime(model.ToDate))
                    .Select(a => new DoctorShareWiseVM
                    {
                        ID = a.ID,
                        MRNO = a.emr_patient_mf.MRNO,
                        OutstandingBalance = 0m,
                        ShareAmount = a.Total ?? 0,
                        Discount = a.DiscountAmount,
                        DoctorId = null,
                        PatientName = a.emr_patient_mf.PatientName,
                        Date = a.Date,
                        Type = "Pharmacy",
                        ServiceId = 1,
                        Category = "Pharmacy",
                    })
                    .Where(a => model.DoctorId == null || a.DoctorId == model.DoctorId)
                    .ToList();
                foreach (var item in pharmacy)
                {
                    CalculateShare(item, userShares);
                }
                // =========================
                // Lab Income
                // =========================
                List<DoctorShareWiseVM> lab = _lab_patient_testService.Queryable()
                    .AsNoTracking()
                    .Where(i => i.CompanyId == CompanyID &&
                           DbFunctions.TruncateTime(i.CreatedDate) >= DbFunctions.TruncateTime(model.FromDate) &&
                           DbFunctions.TruncateTime(i.CreatedDate) <= DbFunctions.TruncateTime(model.ToDate))
                    .Select(a => new DoctorShareWiseVM
                    {
                        ID = a.ID,
                        MRNO = a.emr_patient_mf.MRNO,
                        OutstandingBalance = 0m,
                        ShareAmount = a.Total,
                        Discount = a.DiscountAmount,
                        DoctorId = null,
                        PatientName = a.emr_patient_mf.PatientName,
                        Date = a.CreatedDate,
                        Type = "Laboratory",
                        Category = "Laboratory",
                        ServiceId = 1,
                    })
                    .Where(a => model.DoctorId == null || a.DoctorId == model.DoctorId)
                    .ToList();
                foreach (var item in lab)
                {
                    CalculateShare(item, userShares);
                }
                // =========================
                // Combine All Results
                // =========================
                var allData = opdIncome
        .Concat(ipdResult)
        .Concat(pharmacy)
        .Concat(lab)
        .Select(a => new DoctorShareWiseVM
        {
            ID = a.ID,
            DoctorName = doctorList
                .Where(z => z.ID == a.DoctorId)
                .Select(z => z.Name)
                .FirstOrDefault() ?? "",
            MRNO = a.MRNO,
            OutstandingBalance = a.OutstandingBalance,
            ShareAmount = a.ShareAmount,
            Discount = a.Discount,
            DoctorId = a.DoctorId,
            PatientName = a.PatientName,
            Date = a.Date,
            Type = a.Type,
            Category = a.Category,
            ServiceId = a.ServiceId,
            DoctorShare = a.DoctorShare,
            OwnerShare = a.OwnerShare,
            DoctorShareRate = a.DoctorShareRate,
            OwnerShareRate = a.OwnerShareRate,
            SharedMethod = a.SharedMethod
        })
        .OrderByDescending(x => x.Date)
        .ToList();


                objResponse.IsSuccess = true;
                objResponse.ResultSet = new
                {
                    FeeList = allData
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
        private void CalculateShare(DoctorShareWiseVM item, List<AdmUserShareVM> userShares)
        {
            var share = userShares.FirstOrDefault(x =>
                x.DoctorId == item.DoctorId &&
                x.ServiceId == item.ServiceId &&
                x.Category == item.Category);

            if (share == null)
            {
                item.DoctorShare = 0;
                item.OwnerShare = "0";
                item.DoctorShareRate = 0;
                item.OwnerShareRate = "0";
                item.SharedMethod = "";
                return;
            }

            item.SharedMethod = share.SharedMethod;
            item.DoctorShareRate = share.DoctorShare;
            item.OwnerShareRate = share.OwnerShare;

            if (share.SharedMethod == "Percentage")
            {
                item.DoctorShare =
                    item.ShareAmount * share.DoctorShare / 100m;

                item.OwnerShare =
                    (item.ShareAmount * Convert.ToDecimal(share.OwnerShare) / 100m).ToString();
            }
            else if (share.SharedMethod == "Fixed")
            {
                item.DoctorShare = share.DoctorShare;

                item.OwnerShare = (item.ShareAmount - item.DoctorShare).ToString();
            }
        }
        public class AdmUserShareVM
        {
            public decimal DoctorId { get; set; }
            public decimal ServiceId { get; set; }
            public decimal DoctorShare { get; set; }
            public string OwnerShare { get; set; }
            public string Category { get; set; }
            public string SharedMethod { get; set; }
        }
        [HttpPost]
        [ActionName("SendEmail")]
        public ResponseInfo SendEmail(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    var FolderPath = HttpContext.Current.Server.MapPath("~/Files");
                    FolderPath = Path.Combine(FolderPath, "Temp");
                    string sourcePath = Path.Combine(FolderPath, model.FileName);

                    if (model.base64 != null && model.base64 != "")
                        Base64ToImage(model.base64, model.FileName);
                    else
                        model.FileName = "";

                    EmailService obj = new EmailService();
                    obj.SendEmailReport(sourcePath, model.Email);

                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {

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
        }

        public static void Base64ToImage(string base64String, string docName)
        {
            var FolderPath = HttpContext.Current.Server.MapPath("~/Files");
            FolderPath = Path.Combine(FolderPath, "Temp");

            bool exists = System.IO.Directory.Exists(FolderPath);
            if (!exists)
                System.IO.Directory.CreateDirectory(FolderPath);

            string filePath = FolderPath + "/" + docName;

            File.WriteAllBytes(filePath, Convert.FromBase64String(base64String));

            //using (System.IO.FileStream stream = System.IO.File.Create(filePath))
            //{
            //    System.Byte[] byteArray = System.Convert.FromBase64String(base64String);
            //    stream.Write(byteArray, 0, byteArray.Length);
            //}
        }

    }
}
