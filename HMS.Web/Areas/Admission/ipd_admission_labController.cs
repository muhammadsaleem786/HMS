using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
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
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HMS.Web.API.Areas.Admission.Controllers
{
    [JwtAuthentication]
    public class ipd_admission_labController : ApiController, IERPAPIInterface<ipd_admission_lab>, IDisposable
    {
        private readonly Iipd_admission_labService _service;
        private readonly Iemr_investigationService _emr_investigationService;
        private readonly Iemr_appointment_mfService _emr_appointment_mfService;
        private readonly Iemr_patientService _emr_patientService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iipd_admissionService _ipd_admissionService;

        public ipd_admission_labController(IUnitOfWorkAsync unitOfWorkAsync, Iipd_admission_labService service,
            Iemr_patientService emr_patientService,
            Iemr_appointment_mfService emr_appointment_mfService,
            Isys_drop_down_valueService sys_drop_down_valueService, Iipd_admissionService ipd_admissionService, Iemr_investigationService emr_investigationService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = service;
            _emr_patientService = emr_patientService;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _emr_appointment_mfService = emr_appointment_mfService;
            _ipd_admissionService = ipd_admissionService;
            _emr_investigationService = emr_investigationService;
        }
        [HttpPost]
        [HttpGet]
        [ActionName("Save")]
        public async Task<ResponseInfo> Save(ipd_admission_lab Model)
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
                    ID = (_service.Queryable().Where(e => e.CompanyId == CompanyID).Max(x => (decimal?)x.ID) ?? 0) + 1;
                var catalogIds = Model.CatalogIds.Split(',')
                            .Select(x => Convert.ToDecimal(x))
                            .ToList();
                var entities = new List<ipd_admission_lab>();
                var invoiceNo = await GenerateNumbersAsync(CompanyID);
                foreach (var catalog in catalogIds)
                {
                    entities.Add(new ipd_admission_lab
                    {
                        ID = ID++,
                        PriorityDropdownId = Model.PriorityId != 0
                   ? (int)sys_dropdown_mfEnum.LabPriorityDropdownId
                   : 0,
                        ipd_admission = null,
                        sys_drop_down_value = null,
                        PriorityId = Model.PriorityId,
                        AdmissionId=Model.AdmissionId,
                        InvoiceNo = invoiceNo,
                        ReferringPhysician = Model.ReferringPhysician,
                        PatientId = Model.PatientId,
                        ClinicalNotes = Model.ClinicalNotes,
                        CatalogId = catalog,
                        CompanyId = CompanyID,
                        CreatedBy = UserID,
                        CreatedDate = DateTime.Now,
                        ModifiedBy = UserID,
                        ModifiedDate = DateTime.Now,
                        ObjectState = ObjectState.Added
                    });
                }
                _service.InsertRange(entities);
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
        public async Task<string> GenerateNumbersAsync(decimal companyId)
        {
            string year = DateTime.Now.ToString("yy");
            // Invoice No (260000000001)
            string invoicePrefix = year;

            var lastInvoiceNumber = _service.Queryable()
                .Where(x => x.CompanyId == companyId &&
                            x.InvoiceNo.StartsWith(invoicePrefix))
                .AsEnumerable()
                .Select(x =>
                {
                    var sequencePart = x.InvoiceNo.Length > 2
                        ? x.InvoiceNo.Substring(2)
                        : "0";

                    return long.TryParse(sequencePart, out var n) ? n : 0;
                })
                .DefaultIfEmpty(0)
                .Max();
            string invoiceNo = $"{invoicePrefix}{(lastInvoiceNumber + 1):D8}";
            return invoiceNo;
        }

        [HttpPut]
        [HttpGet]
        [ActionName("Update")]
        public async Task<ResponseInfo> Update(ipd_admission_lab Model)
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
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Modified;
                _service.Update(Model);

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
        [ActionName("GetLabDropDown")]
        public ResponseInfo GetLabDropDown(string PatientId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();

                var result = _sys_drop_down_valueService.Queryable().Where(e => (e.CompanyID == null || e.CompanyID == CompanyID) && e.DropDownID == 68).Select(a => new { a.ID, a.Value, DropDownID = a.DropDownID }).ToList();
                var resultObj = _emr_investigationService.Queryable().Where(e => e.CompanyID == CompanyID)
                   .Select(z => new
                   {
                       ID = z.ID,
                       name = z.Investigation,
                       price = z.Price,
                       category = z.sys_drop_down_value.Value,
                       sample = z.sys_drop_down_value1.Value,
                       tat = z.TAT,
                   }).ToList();
                var patientInfo = _emr_patientService.Queryable().Where(a => a.CompanyId == CompanyID && a.ID.ToString() == PatientId).Select(z => new
                {
                    z.PatientName,
                    z.ID,
                    DOB = z.DOB,
                    Age = z.Age,
                    Gender = z.Gender == 1 ? "Male" : z.Gender == 2 ? "FeMale" : "Other",
                }).FirstOrDefault();

                objResponse.ResultSet = new
                {
                    patientInfo = patientInfo,
                    result = result,
                    resultObj = resultObj
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
            try
            {
                decimal CompanyID = Request.CompanyID();
                var GenderType = _sys_drop_down_valueService.Queryable().Where(e => (e.CompanyID == null || e.CompanyID == CompanyID) && e.DropDownID == 2).ToList();
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
        public ResponseInfo GetById(string Id, string PatientId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal companyId = Request.CompanyID();
                decimal UserID = Request.LoginID();
                int ID = Convert.ToInt32(Id);
                var Result = _service.Queryable().Where(e => e.ID == ID && e.CompanyId == companyId).Include(a => a.sys_drop_down_value).FirstOrDefault();
                var patientInfo = _emr_patientService.Queryable().Where(a => a.CompanyId == companyId && a.ID.ToString() == PatientId).Select(z => new
                {
                    z.PatientName,
                    z.ID,
                    DOB = z.DOB,
                    Age = z.Age,
                    Gender = z.Gender == 1 ? "Male" : z.Gender == 2 ? "FeMale" : "Other",
                }).FirstOrDefault();
                var resultObj = _emr_investigationService.Queryable().Where(e => e.CompanyID == companyId)
                  .Select(z => new
                  {
                      ID = z.ID,
                      name = z.Investigation,
                      price = z.Price,
                      category = z.sys_drop_down_value.Value,
                      sample = z.sys_drop_down_value1.Value,
                      tat = z.TAT,
                  }).ToList();
                objResponse.ResultSet = new
                {
                    Result = Result,
                    patientInfo = patientInfo,
                    resultObj = resultObj
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
        public ResponseInfo GetById(string Id, int NextPreviousIndex)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                objResponse.ResultSet = _service.Queryable().Where(e => e.ID.ToString() == Id && e.CompanyId == CompanyID).FirstOrDefault();
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
        [ActionName("LabPagination")]
        public PaginationResult LabPagination(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string AdmitId, string PatientId, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var ipdAdmissions = _ipd_admissionService.Queryable()
.Where(a => a.PatientId.ToString() == PatientId && a.CompanyId == CompanyID
    && a.ID.ToString() == AdmitId)
.Select(a => new
{
    AdmissionNo = a.AdmissionNo,
    DischargeDate = a.DischargeDate,
    DischargeTime = a.DischargeTime
}).FirstOrDefault();
                objResult = _service.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, AdmitId, PatientId, IgnorePaging);
                objResult.OtherDataModel = ipdAdmissions;
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
                //var ObjList = Pagination(0, 0, VisibleColumnInfo, SortName, SortOrder, SearchText,0, true);
                //objResponse.FilePath = Documents.ExportWithType(ExportType, VisibleColumnInfo, ObjList.DataList);

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
                ipd_admission_lab model = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID == ID).FirstOrDefault();

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

            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }

        public ResponseInfo GetById(string Id)
        {
            throw new NotImplementedException();
        }

        public PaginationResult Pagination(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            throw new NotImplementedException();
        }

        public PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            throw new NotImplementedException();
        }
    }
}
