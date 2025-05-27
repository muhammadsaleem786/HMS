using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Appointment;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using HMS.Web.API.Interface;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HMS.Web.API.Areas.Appointment.Controllers
{
    [JwtAuthentication]
    public class emr_patient_billController : ApiController, IERPAPIInterface<emr_patient_bill>, IDisposable
    {
        private readonly Iemr_patient_billService _service;
        private readonly Iemr_appointment_mfService _emr_appointment_mfService;
        private readonly Iadm_user_companyService _adm_user_companyService;
        private readonly Iadm_role_mfService _adm_role_mfService;
        private readonly Iadm_user_tokenService _adm_user_tokenService;
        private readonly Isys_notification_alertService _sys_notification_alertService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly Iadm_userService _adm_userService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iemr_patientService _emr_patientService;
        private readonly Iemr_service_mfService _emr_service_mfService;
        private readonly Iemr_patient_bill_paymentService _emr_patient_bill_paymentService;

        public emr_patient_billController(IUnitOfWorkAsync unitOfWorkAsync, Iemr_service_mfService emr_service_mfService, Iemr_patient_bill_paymentService emr_patient_bill_paymentService, Iemr_patientService emr_patientService, Iemr_patient_billService service, Iadm_userService adm_userService, Iemr_appointment_mfService emr_appointment_mfService, Iadm_user_companyService adm_user_companyService,
            Iadm_role_mfService adm_role_mfService, Iadm_user_tokenService adm_user_tokenService, Isys_drop_down_valueService sys_drop_down_valueService,
            Isys_notification_alertService sys_notification_alertService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = service;
            _emr_patientService = emr_patientService;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _adm_user_companyService = adm_user_companyService;
            _adm_userService = adm_userService;
            _emr_appointment_mfService = emr_appointment_mfService;
            _adm_role_mfService = adm_role_mfService;
            _emr_service_mfService = emr_service_mfService;
            _adm_user_tokenService = adm_user_tokenService;
            _sys_notification_alertService = sys_notification_alertService;
            _emr_patient_bill_paymentService = emr_patient_bill_paymentService;
        }
        [HttpPost]
        [HttpGet]
        [ActionName("Save")]
        public async Task<ResponseInfo> Save(emr_patient_bill Model)
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
                decimal UserID = Request.LoginID();

                decimal ID = 1;
                if (_service.Queryable().Count() > 0)
                    ID = _service.Queryable().Max(e => e.ID) + 1;
                Model.ID = ID;
                Model.CompanyId = Request.CompanyID();
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
                    objResponse.ResultSet = new
                    {
                        ID = Model.ID,
                    };

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
        [HttpPut]
        [HttpGet]
        [ActionName("Update")]
        public async Task<ResponseInfo> Update(emr_patient_bill Model)
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
                Model.CompanyId = Request.CompanyID();
                Model.CreatedBy = Request.LoginID();
                Model.CreatedDate = Request.DateTimes();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Modified;
                _service.Update(Model);

                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        Model = Model
                    };
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
        [HttpPut]
        [HttpGet]
        [ActionName("UpdateBill")]
        public async Task<ResponseInfo> UpdateBill(emr_patient_bill Model)
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
                Model.CompanyId = Request.CompanyID();
                Model.CreatedBy = Request.LoginID();
                Model.CreatedDate = Request.DateTimes();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Modified;
                _service.Update(Model);

                decimal billdtID = 1;
                if (_emr_patient_bill_paymentService.Queryable().Count() > 0)
                    billdtID = _emr_patient_bill_paymentService.Queryable().Max(e => e.ID) + 1;
                emr_patient_bill_payment billdtObj = new emr_patient_bill_payment();
                billdtObj.ID = billdtID;
                billdtObj.BillId = Model.ID;
                billdtObj.Amount = Model.PartialAmount;
                billdtObj.PaymentDate = Model.PaymentDate;
                billdtObj.Remarks = Model.Remarks;
                billdtObj.CompanyId = Request.CompanyID();
                billdtObj.CreatedBy = Request.LoginID();
                billdtObj.CreatedDate = Request.DateTimes();
                billdtObj.ModifiedBy = Request.LoginID();
                billdtObj.ModifiedDate = Request.DateTimes();
                billdtObj.ObjectState = ObjectState.Added;
                _emr_patient_bill_paymentService.Insert(billdtObj);
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;
                    objResponse.IsSuccess = true;
                    objResponse.ResultSet = new
                    {
                        Model = Model
                    };
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
        [ActionName("GetList")]
        public ResponseInfo GetList()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var GenderType = _sys_drop_down_valueService.Queryable().Where(e => e.DropDownID == 2).ToList();
                objResponse.ResultSet = new
                {
                    GenderList = GenderType
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
                decimal UserID = Request.LoginID();
                decimal CompanyID = Request.CompanyID();
                var DoctorList = _emr_patientService.DoctorList(CompanyID, UserID);
                var result1 = _service.Queryable().Where(e => e.ID.ToString() == Id && e.CompanyId == CompanyID).Include(a => a.emr_patient_bill_payment)
                    .Select(z => new
                    {
                        ID = z.ID,
                        PatientId = z.PatientId,
                        ServiceId = z.ServiceId,
                        DoctorId = z.DoctorId,
                        Price = z.Price,
                        PaidAmount = z.PaidAmount,
                        Discount = z.Discount,
                        BillDate = z.BillDate,

                        OutstandingBalance = z.OutstandingBalance,
                        ServiceName = z.emr_service_mf.ServiceName,
                        PatientName = z.emr_patient_mf.PatientName,
                        Remarks = z.Remarks,
                        AppointmentId = z.AppointmentId,
                        AdmissionId = z.AdmissionId,
                        RefundAmount = z.RefundAmount,
                        RefundDate = z.RefundDate,
                        emr_patient_payment = z.emr_patient_bill_payment,
                    }).ToList();
                var result = result1.AsEnumerable()
                    .Select(z => new
                    {
                        ID = z.ID,
                        PatientId = z.PatientId,
                        ServiceId = z.ServiceId,
                        DoctorId = z.DoctorId,
                        Price = z.Price,
                        PaidAmount = z.PaidAmount,
                        BillDate = z.BillDate,
                        Discount = z.Discount,
                        ServiceName = z.ServiceName,
                        PatientName = z.PatientName,
                        AppointmentId = z.AppointmentId,
                        AdmissionId = z.AdmissionId,
                        RefundAmount = z.RefundAmount,
                        RefundDate = z.RefundDate,
                        OutstandingBalance = z.OutstandingBalance,
                        DoctorName = z.DoctorId == 0 ? "" : DoctorList.DoctList.Where(a => a.ID == z.DoctorId).FirstOrDefault().Name,
                        emr_patient_payment = z.emr_patient_payment,
                    }).FirstOrDefault();


                objResponse.ResultSet = new
                {
                    result = result,
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
        [ActionName("GetBillByPatient")]
        public ResponseInfo GetBillByPatient(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var result = _service.Queryable().Where(e => e.ID.ToString() == Id && e.CompanyId == CompanyID).Include(x => x.emr_patient_bill_payment)
                    .Select(z => new
                    {
                        ID = z.ID,
                        CompanyName = z.adm_company.CompanyName,
                        CompanyAddress = z.adm_company.CompanyAddress1,
                        CompanyPhone = z.adm_company.Phone,
                        CompanyEmail = z.adm_company.Email,
                        PatientName = z.emr_patient_mf.PatientName,
                        PatientAddress = z.emr_patient_mf.Address,
                        PatientEmail = z.emr_patient_mf.Email,
                        PatientMobile = z.emr_patient_mf.Mobile,
                        BillDate = z.BillDate,
                        Discount = z.Discount,
                        Price = z.Price,
                        PaidAmount = z.PaidAmount,
                        OutstandingBalance = z.OutstandingBalance,
                        ServiceName = z.emr_service_mf.ServiceName,
                        CompanyLogo = z.adm_company.CompanyLogo,
                        patient_bill_payment = z.emr_patient_bill_payment,
                    }).ToList();
                var results = result.GroupBy(l => l.BillDate)
                        .Select(cl => new
                        {
                            ID = cl.First().ID,
                            CompanyName = cl.First().CompanyName,
                            CompanyAddress = cl.First().CompanyAddress,
                            CompanyPhone = cl.First().CompanyPhone,
                            CompanyEmail = cl.First().CompanyEmail,
                            PatientName = cl.First().PatientName,
                            PatientAddress = cl.First().PatientAddress,
                            PatientEmail = cl.First().PatientEmail,
                            PatientMobile = cl.First().PatientMobile,
                            BillDate = cl.First().BillDate,
                            Discount = cl.Sum(z => z.Discount),
                            Price = cl.First().Price,
                            PaidAmount = cl.Sum(z => z.PaidAmount),
                            OutstandingBalance = cl.Sum(z => z.OutstandingBalance),
                            ServiceName = cl.First().ServiceName,
                            CompanyLogo = cl.First().CompanyLogo,
                            patient_bill_payment = cl.Select(z => z.patient_bill_payment),
                        }).ToList();

                objResponse.ResultSet = new
                {
                    result = results,
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
        [ActionName("GetBillByPayment")]
        public ResponseInfo GetBillByPayment(string AdmissionId, string AppointmentId, string PatientId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();

                DataAccessManager dataAccessManager = new DataAccessManager();
                var ht = new Hashtable();
                ht.Add("@CompanyId", CompanyID);
                ht.Add("@AdmissionId", AdmissionId);
                ht.Add("@AppointmentId", AppointmentId);
                ht.Add("@PatientId", PatientId);

                var AllData = dataAccessManager.GetDataSet("SP_ProvisionalBillRpt", ht);
                var masterdata = AllData.Tables[0];
                var visitdata = AllData.Tables[1];
                var admitdata = AllData.Tables[2];
                var TotalBill = AllData.Tables[3].Rows[0]["Total"];
                var AdvanceAmt = AllData.Tables[4].Rows[0]["AdvanceAmount"];

                objResponse.ResultSet = new
                {
                    masterdata = masterdata,
                    visitdata = visitdata,
                    admitdata = admitdata,
                    TotalBill = TotalBill,
                    AdvanceAmt = AdvanceAmt,
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
        [ActionName("GetBillByAdmissionId")]
        public ResponseInfo GetBillByAdmissionId(string Id, string patientid)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var result = _service.Queryable().Where(e => e.AdmissionId.ToString() == Id && e.PatientId.ToString() == patientid && e.CompanyId == CompanyID)
                    .Select(z => new
                    {
                        ID = z.ID,
                        CompanyName = z.adm_company.CompanyName,
                        CompanyAddress = z.adm_company.CompanyAddress1,
                        CompanyPhone = z.adm_company.Phone,
                        CompanyEmail = z.adm_company.Email,
                        PatientName = z.emr_patient_mf.PatientName,
                        PatientAddress = z.emr_patient_mf.Address,
                        PatientEmail = z.emr_patient_mf.Email,
                        PatientMobile = z.emr_patient_mf.Mobile,
                        BillDate = z.BillDate,
                        Discount = z.Discount,
                        Price = z.Price,
                        PaidAmount = z.PaidAmount,
                        OutstandingBalance = z.OutstandingBalance,
                        ServiceName = z.emr_service_mf.ServiceName,
                        CompanyLogo = z.adm_company.CompanyLogo,
                    }).ToList();

                var results = result.GroupBy(l => l.BillDate)
    .Select(cl => new
    {
        ID = cl.First().ID,
        CompanyName = cl.First().CompanyName,
        CompanyAddress = cl.First().CompanyAddress,
        CompanyPhone = cl.First().CompanyPhone,
        CompanyEmail = cl.First().CompanyEmail,
        PatientName = cl.First().PatientName,
        PatientAddress = cl.First().PatientAddress,
        PatientEmail = cl.First().PatientEmail,
        PatientMobile = cl.First().PatientMobile,
        BillDate = cl.First().BillDate,
        Discount = cl.Sum(z => z.Discount),
        Price = cl.First().Price,
        PaidAmount = cl.Sum(z => z.PaidAmount),
        OutstandingBalance = cl.Sum(z => z.OutstandingBalance),
        ServiceName = cl.First().ServiceName,
        CompanyLogo = cl.First().CompanyLogo,
    }).ToList();

                objResponse.ResultSet = new
                {
                    result = results,
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
        [ActionName("GetPaymentByPatient")]
        public ResponseInfo GetPaymentByPatient(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var DoctorList = _emr_patientService.DoctorList(CompanyID, UserID);
                var result = _service.Queryable().Where(e => e.ID.ToString() == Id && e.CompanyId == CompanyID).Include(z => z.emr_patient_bill_payment).Include(z => z.ipd_admission)
                    .Select(z => new BillModel
                    {
                        ID = z.ID,
                        CompanyName = z.adm_company.CompanyName,
                        CompanyAddress = z.adm_company.CompanyAddress1,
                        CompanyPhone = z.adm_company.Phone,
                        CompanyEmail = z.adm_company.Email,
                        PatientName = z.emr_patient_mf.PatientName,
                        AdmisDate = z.ipd_admission == null ? "" : z.ipd_admission.AdmissionDate.ToString(),
                        Room = z.ipd_admission == null ? "" : z.ipd_admission.sys_drop_down_value3.Value,
                        Ward = z.ipd_admission == null ? "" : z.ipd_admission.sys_drop_down_value2.Value,
                        MRNo = z.emr_patient_mf.MRNO,
                        PatientAddress = z.emr_patient_mf.Address,
                        PatientEmail = z.emr_patient_mf.Email,
                        PatientMobile = z.emr_patient_mf.Mobile,
                        BillDate = z.BillDate,
                        Amount = z.emr_patient_bill_payment.Sum(a => a.Amount),
                        CompanyLogo = z.adm_company.CompanyLogo,
                        DoctorId = z.DoctorId,
                    }).ToList();
                var results = result.GroupBy(l => l.BillDate)
  .Select(cl => new
  {
      ID = cl.First().ID,
      CompanyName = cl.First().CompanyName,
      CompanyAddress = cl.First().CompanyAddress,
      CompanyPhone = cl.First().CompanyPhone,
      CompanyEmail = cl.First().CompanyEmail,
      PatientName = cl.First().PatientName,
      PatientAddress = cl.First().PatientAddress,
      PatientEmail = cl.First().PatientEmail,
      PatientMobile = cl.First().PatientMobile,
      BillDate = cl.First().BillDate,
      AdmissionDate = cl.First().AdmisDate,
      Room = cl.First().Room,
      Ward = cl.First().Ward,
      MRNo = cl.First().MRNo,
      Amount = cl.Sum(z => z.Amount),
      CompanyLogo = cl.First().CompanyLogo,
      DoctorName = DoctorList.DoctList.Where(a => a.ID == cl.First().DoctorId).FirstOrDefault().Name,
  }).ToList();
                objResponse.ResultSet = new
                {
                    result = results,
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
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                objResponse.ResultSet = _service.Queryable().Where(e => e.ID.ToString() == Id).FirstOrDefault();
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
        [ActionName("BillingPagination")]
        public PaginationResult BillingPagination(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string PatientId, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResult = _service.BillingList(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, PatientId, IgnorePaging);
            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }
        [HttpGet]
        [ActionName("GetIPDBillingList")]
        public ResponseInfo GetIPDBillingList(string PatientId, string AdmitId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                DataAccessManager dataAccessManager = new DataAccessManager();
                var ht = new Hashtable();
                ht.Add("@CompanyId", CompanyID);
                ht.Add("@AdmissionId", AdmitId);
                ht.Add("@PatientId", PatientId);
                var AllData = dataAccessManager.GetDataSet("SP_IPDBillSummary", ht);
                var result = AllData.Tables;
                objResponse.ResultSet = new
                {
                    result = result,
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
        public async Task<ResponseInfo> Delete(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal ID = Convert.ToDecimal(Id);
                emr_patient_bill model = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID == ID).FirstOrDefault();

                if (model == null)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
                }


                model.ObjectState = ObjectState.Deleted;
                _service.Delete(model);
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
        [HttpGet]
        [ActionName("Load")]
        public ResponseInfo Load()
        {

            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
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
        [HttpPost]
        [HttpGet]
        [ActionName("PatientSearch")]
        public async Task<ResponseInfo> PatientSearch(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var result = _emr_patientService.Queryable().Where(a => (a.CompanyId == CompanyID) && (a.PatientName.Contains(term))).Select(z => new
                {
                    value = z.ID,
                    label = z.PatientName,

                }).ToList();
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
        [HttpPost]
        [HttpGet]
        [ActionName("ServiceSearch")]
        public async Task<ResponseInfo> ServiceSearch(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var result = _emr_service_mfService.Queryable().Where(a => (a.CompanyId == CompanyID) && (a.ServiceName.Contains(term))).Select(z => new
                {
                    value = z.ID,
                    label = z.ServiceName,
                    Price = z.Price
                }).ToList();
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

        public PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            throw new NotImplementedException();
        }
    }
}
