using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Admission;
using HMS.Service.Services.Appointment;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using HMS.Web.API.Interface;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections;
using System.Collections.Generic;
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
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iemr_appointment_mfService _emr_appointment_mfService;
        private readonly Iemr_patient_billService _emr_patient_billService;
        private readonly Iemr_prescription_treatmentService _emr_prescription_treatmentService;
        private readonly Iemr_prescription_mfService _emr_prescription_mfService;
        private readonly Iemr_incomeService _emr_incomeService;
        private readonly Iemr_expenseService _emr_expenseService;
        private readonly Iipd_procedure_mfService _ipd_procedure_mfService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;

        public adm_ReportController(IUnitOfWorkAsync unitOfWorkAsync,
            Iemr_appointment_mfService emr_appointment_mfService,
            Iemr_patient_billService emr_patient_billService,
            Iadm_userService service, Iemr_incomeService emr_incomeService, Iemr_expenseService emr_expenseService,
        Iemr_prescription_treatmentService emr_prescription_treatmentService,
            Iemr_prescription_mfService emr_prescription_mfService
         , Iemr_patientService emr_patientService, Iipd_procedure_mfService ipd_procedure_mfService, Isys_drop_down_valueService sys_drop_down_valueService)
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
                var categoryList = _sys_drop_down_valueService.Queryable().Where(e => e.DropDownID == 64 && e.Value.Contains(term)).Select(z => new
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
        [HttpPost]
        [ActionName("ClinicwiseRpt")]
        public ResponseInfo ClinicwiseRpt(ReportModel model)
        {
            {
                var objResponse = new ResponseInfo();
                try
                {
                    decimal CompanyID = Request.CompanyID();
                    var Paymentlist = _emr_patient_billService.Queryable().Where(a => a.CompanyId == CompanyID &&
               DbFunctions.TruncateTime(a.BillDate) >= DbFunctions.TruncateTime(model.FromDate) &&
               DbFunctions.TruncateTime(a.BillDate) <= DbFunctions.TruncateTime(model.ToDate)).Select(a => new
               {
                   ID = a.ID,
                   CompanyName = a.adm_company.CompanyName,
                   OutstandingBalance = a.OutstandingBalance,
                   PaidAmount = a.PaidAmount,
                   CompanyId = a.adm_company.ID,
               }).Where(a => model.PatientId == null ? a.CompanyId == a.CompanyId : a.CompanyId == model.PatientId).OrderByDescending(a => a.ID).ToList();
                    var results = Paymentlist.GroupBy(l => l.CompanyName)
    .Select(cl => new
    {
        CompanyName = cl.First().CompanyName,
        OutstandingBalance = cl.Sum(c => c.OutstandingBalance),
        PaidAmount = cl.Sum(c => c.PaidAmount),
    }).ToList();
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
                    var FeeList = _emr_patient_billService.Queryable().Where(a => a.CompanyId == CompanyID &&
                 DbFunctions.TruncateTime(a.BillDate) >= DbFunctions.TruncateTime(model.FromDate) &&
                 DbFunctions.TruncateTime(a.BillDate) <= DbFunctions.TruncateTime(model.ToDate)).Select(a => new
                 {
                     ID = a.ID,
                     DoctorId = a.DoctorId,
                     Discount = a.Discount,
                     Price = a.Price,
                     OutstandingBalance = a.OutstandingBalance,
                     PaidAmount = a.PaidAmount,
                 }).Where(a => model.DoctorId == null ? a.DoctorId == a.DoctorId : a.DoctorId == model.DoctorId).OrderByDescending(a => a.ID).ToList();

                    var reslt = FeeList.AsEnumerable().Select(a => new
                    {
                        ID = a.ID,
                        DoctorId = a.DoctorId,
                        DoctorName = _service.Queryable().Where(z => z.ID == a.DoctorId).Count() > 0 ? _service.Queryable().Where(z => z.ID == a.DoctorId).FirstOrDefault().Name : "",
                        Discount = a.Discount,
                        Price = a.Price,
                        OutstandingBalance = a.OutstandingBalance,
                        PaidAmount = a.PaidAmount,
                    }).ToList();
                    var results = reslt.GroupBy(l => l.DoctorName)
    .Select(cl => new
    {
        DoctorName = cl.First().DoctorName,
        Discount = cl.Sum(c => c.Discount),
        OutstandingBalance = cl.Sum(c => c.OutstandingBalance),
        Price = cl.Sum(c => c.Price),
        PaidAmount = cl.Sum(c => c.PaidAmount),
    }).ToList();
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        FeeList = results,
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
                    var FeeList = _emr_patient_billService.Queryable().Where(a => a.CompanyId == CompanyID &&
               DbFunctions.TruncateTime(a.BillDate) >= DbFunctions.TruncateTime(model.FromDate) &&
               DbFunctions.TruncateTime(a.BillDate) <= DbFunctions.TruncateTime(model.ToDate)).Select(a => new
               {
                   ID = a.ID,
                   CompanyName = a.adm_company.CompanyName,
                   OutstandingBalance = a.OutstandingBalance,
                   PaidAmount = a.PaidAmount,
                   DoctorId = a.DoctorId,
                   PatientName = a.emr_patient_mf.PatientName,
                   Date = a.BillDate,
               }).Where(a => model.DoctorId == null ? a.DoctorId == a.DoctorId : a.DoctorId == model.DoctorId).OrderByDescending(a => a.ID).ToList();

                    var result = FeeList.AsEnumerable().Select(a => new
                    {
                        ID = a.ID,
                        DoctorName = _service.Queryable().Where(z => z.ID == a.DoctorId).Count() > 0 ? _service.Queryable().Where(z => z.ID == a.DoctorId).FirstOrDefault().Name : "",
                        CompanyName = a.CompanyName,
                        OutstandingBalance = a.OutstandingBalance,
                        PaidAmount = a.PaidAmount,
                        DoctorId = a.DoctorId,
                        PatientName = a.PatientName,
                        Date = a.Date,
                    }).ToList();
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        FeeList = result
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
        [ActionName("DoctorswiseSummaryPaymentRpt")]
        public ResponseInfo DoctorswiseSummaryPaymentRpt(ReportModel model)
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
                   CompanyName = a.adm_company.CompanyName,
                   OutstandingBalance = a.OutstandingBalance,
                   PaidAmount = a.PaidAmount,
                   DoctorId = a.DoctorId,
               }).Where(a => model.DoctorId == null ? a.DoctorId == a.DoctorId : a.DoctorId == model.DoctorId).OrderByDescending(a => a.ID).ToList();
                    var result = FeeList.AsEnumerable().Select(a => new
                    {
                        ID = a.ID,
                        DoctorName = _service.Queryable().Where(z => z.ID == a.DoctorId).Count() > 0 ? _service.Queryable().Where(z => z.ID == a.DoctorId).FirstOrDefault().Name : "",
                        CompanyName = a.CompanyName,
                        OutstandingBalance = a.OutstandingBalance,
                        PaidAmount = a.PaidAmount,
                        DoctorId = a.DoctorId,
                    }).ToList();

                    var results = result.GroupBy(l => l.DoctorName)
    .Select(cl => new
    {
        DoctorName = cl.First().DoctorName,
        CompanyName = cl.First().CompanyName,
        OutstandingBalance = cl.Sum(c => c.OutstandingBalance),
        PaidAmount = cl.Sum(c => c.PaidAmount),
    }).ToList();


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
                    var paymentList = _emr_patient_billService.Queryable().Where(a => a.CompanyId == CompanyID &&
                   DbFunctions.TruncateTime(a.BillDate) >= DbFunctions.TruncateTime(model.FromDate) &&
                   DbFunctions.TruncateTime(a.BillDate) <= DbFunctions.TruncateTime(model.ToDate)).Select(a => new
                   {
                       ID = a.ID,
                       BillDate = a.BillDate,
                       DoctorId = a.DoctorId,
                       PaidAmount = a.PaidAmount
                   }).OrderByDescending(a => a.BillDate).ToList();


                    var result = paymentList.AsEnumerable().Select(a => new
                    {
                        ID = a.ID,
                        BillDate = a.BillDate,
                        DoctorName = _service.Queryable().Where(z => z.ID == a.DoctorId).Count() > 0 ? _service.Queryable().Where(z => z.ID == a.DoctorId).FirstOrDefault().Name : "",
                        IncomeAmount = _emr_incomeService.Queryable().Where(z => z.CompanyId == CompanyID && DbFunctions.TruncateTime(z.Date) == DbFunctions.TruncateTime(a.BillDate)).Sum(x => x.ReceivedAmount),
                        PaidAmount = a.PaidAmount,
                        DoctorId = a.DoctorId,
                    }).ToList();
                    var results = result.GroupBy(l => l.BillDate)
   .Select(cl => new
   {
       DoctorName = cl.First().DoctorName,
       BillDate = cl.First().BillDate,
       PaidAmount = cl.Sum(c => c.PaidAmount),
       IncomeAmount = cl.First().IncomeAmount,
       DoctorId = cl.First().DoctorId,
   }).ToList();
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
