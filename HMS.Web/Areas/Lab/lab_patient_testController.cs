using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
using HMS.Service;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Appointment;
using HMS.Service.Services.Items;
using HMS.Service.Services.Lab;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using HMS.Web.API.Interface;
using MathNet.Numerics.Distributions;
using NPOI.SS.Formula.Functions;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.UI.WebControls;
using static iTextSharp.text.pdf.AcroFields;
using static NPOI.HSSF.Util.HSSFColor;

namespace HMS.Web.API.Areas.Lab.Controllers
{
    [JwtAuthentication]
    public class lab_patient_testController : ApiController, IERPAPIInterface<lab_patient_test>, IDisposable
    {
        private readonly Ilab_patient_testService _service;
        private readonly Iinv_stockService _inv_stockService;
        private readonly Ilab_result_mfService _lab_result_mfService;
        private readonly Ilab_result_dtService _lab_result_dtService;
        private readonly Iemr_investigation_ItemService _emr_investigation_ItemService;
        private readonly Iemr_patientService _emr_patientService;
        private readonly Iemr_prescription_investigationService _emr_prescription_investigationService;
        private readonly Iemr_investigationService _emr_investigationService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly IStoredProcedureService _procedureService;
        private readonly Iadm_role_dtService _adm_role_dtService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iadm_user_companyService _adm_user_companyService;
        private readonly ISmsService _smsService;
        private readonly Iadm_itemService _adm_itemService;
        private readonly Iadm_item_logService _adm_item_logService;
        private readonly Iadm_integrationService _adm_integrationService;
        private readonly Random _random = new Random();
        public lab_patient_testController(IUnitOfWorkAsync unitOfWorkAsync, Ilab_patient_testService Service,
            Isys_drop_down_valueService sys_drop_down_valueService, Iadm_user_companyService adm_user_companyService,
            IStoredProcedureService ProcedureService, Iadm_role_dtService adm_role_dtService, Iemr_patientService emr_patientService, Iemr_investigationService emr_investigationService, Ilab_result_mfService lab_result_mfService, Ilab_result_dtService lab_result_dtService, ISmsService smsService, Iemr_prescription_investigationService emr_prescription_investigationService, Iadm_integrationService adm_integrationService, Iemr_investigation_ItemService emr_investigation_ItemService, Iinv_stockService inv_stockService, Iadm_item_logService adm_item_logService, Iadm_itemService adm_itemService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _service = Service;
            _adm_user_companyService = adm_user_companyService;
            _procedureService = ProcedureService;
            _adm_role_dtService = adm_role_dtService;
            _emr_patientService = emr_patientService;
            _emr_investigationService = emr_investigationService;
            _lab_result_mfService = lab_result_mfService;
            _lab_result_dtService = lab_result_dtService;
            _smsService = smsService;
            _emr_prescription_investigationService = emr_prescription_investigationService;
            _adm_integrationService = adm_integrationService;
            _emr_investigation_ItemService = emr_investigation_ItemService;
            _inv_stockService = inv_stockService;
            _adm_item_logService = adm_item_logService;
            _adm_itemService = adm_itemService;
        }
        public async Task<ResponseInfo> Save(lab_patient_test Model)
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

                var catalogIds = Model.CatalogIds.Split(',')
                            .Select(x => Convert.ToDecimal(x))
                            .ToList();

                var catalogs = _emr_investigationService.Queryable()
                      .Where(x => catalogIds.Contains(x.ID) && x.CompanyID == CompanyID)
                      .ToList();

                var invoiceNo = await GenerateNumbersAsync(CompanyID);

                string prefix = $"SMP-{DateTime.Now:yy}-";
                var existingSerials = _service.Queryable()
          .Where(x => x.CompanyId == CompanyID && x.SerialNo.StartsWith(prefix))
          .Select(x => x.SerialNo)
          .ToList();
                var lastNumber = existingSerials
          .Select(x =>
          {
              var parts = x.Split('-');
              return int.TryParse(parts.Last(), out int n) ? n : 0;
          })
          .DefaultIfEmpty(0)
          .Max();


                var sampleSerialMap = catalogs
           .GroupBy(x => x.SampleId)
           .ToDictionary(
               g => g.Key,
               g => $"SMP-{DateTime.Now:yy}-{(++lastNumber):D3}"
           );
                var accesscode = GenerateUniqueAccessCode(CompanyID);
                var entities = new List<lab_patient_test>();

                var invItemlist = _emr_investigation_ItemService.Queryable()
                   .Where(x => catalogIds.Contains(x.InvestigationId) && x.CompanyId == CompanyID)
                   .ToList();
                var itemIds = invItemlist
    .Select(i => i.ItemId)
    .ToList();

                var itemlist = _adm_itemService.Queryable()
      .Where(x => itemIds.Contains(x.ID)
               && x.CompanyId == CompanyID)
      .ToList();
                adm_item_log adm_item_log_obj = new adm_item_log();
                decimal LogID = 1;
                if (_adm_item_logService.Queryable().Count() > 0)
                    LogID = _adm_item_logService.Queryable().Max(e => e.ID) + 1;

                foreach (var item in invItemlist)
                {
                    var itemObj = itemlist.Where(a => a.CompanyId == CompanyID && a.ID == item.ItemId).FirstOrDefault();
                    if (itemObj != null)
                    {
                        inv_stock inv_stockModel = _inv_stockService.Queryable().
                        Where(e => e.ItemID == item.ItemId && e.CompanyId == CompanyID).FirstOrDefault();
                        if (inv_stockModel != null)
                        {
                            inv_stockModel.Quantity -= Convert.ToDecimal(item.Quantity);
                            _inv_stockService.Update(inv_stockModel);
                            //log maintain                               
                            adm_item_log_obj.ID = LogID;
                            adm_item_log_obj.CompanyId = CompanyID;
                            adm_item_log_obj.ItemId = item.ItemId;
                            adm_item_log_obj.Quantity = "-" + item.Quantity;
                            adm_item_log_obj.Type = "Lab Quantity";
                            adm_item_log_obj.CreatedBy = Request.LoginID();
                            adm_item_log_obj.CreatedDate = Request.DateTimes();
                            adm_item_log_obj.ModifiedBy = Request.LoginID();
                            adm_item_log_obj.ModifiedDate = Request.DateTimes();
                            adm_item_log_obj.ObjectState = ObjectState.Added;
                            _adm_item_logService.Insert(adm_item_log_obj);
                            LogID++;
                        }
                    }
                }
                foreach (var catalog in catalogs)
                {
                    entities.Add(new lab_patient_test
                    {
                        ID = ID++,
                        SerialNo = sampleSerialMap[catalog.SampleId],
                        PriorityDropdownId = Model.PriorityId != 0
                   ? (int)sys_dropdown_mfEnum.LabPriorityDropdownId
                   : 0,
                        lab_result_mf = null,
                        emr_investigation = null,
                        PriorityId = Model.PriorityId,
                        StatusId = 1,
                        InvoiceNo = invoiceNo,
                        AccessCode = accesscode,
                        ReferringPhysician = Model.ReferringPhysician,
                        Discount = Model.Discount,
                        DiscountType = Model.DiscountType,
                        DiscountAmount = Model.DiscountAmount,
                        Total = Model.Total,
                        StatusDropdownId = (int)sys_dropdown_mfEnum.LabStatusDropdownId,
                        PatientId = Model.PatientId,
                        Age = Model.Age,
                        ClinicalNotes = Model.ClinicalNotes,
                        CatalogId = catalog.ID,
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
                    var patient = _emr_patientService.Queryable().Include(a => a.adm_company)
                        .FirstOrDefault(x => x.ID == Model.PatientId);
                    if (patient != null)
                    {
                        var company = patient.adm_company;
                        _ = Task.Run(() => SendRemindersAsync(patient, entities.FirstOrDefault(), company));
                    }
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

        public string GenerateUniqueAccessCode(decimal CompanyID)
        {
            string accessCode;
            do
            {
                accessCode = _random.Next(100000, 10000000).ToString(); // 6-7 digits

            } while (_service.Queryable().Any(x => x.AccessCode == accessCode && x.CompanyId == CompanyID));
            return accessCode;
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
        [HttpPost]
        [HttpGet]
        [ActionName("PrioritySearch")]
        public async Task<ResponseInfo> PrioritySearch(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var result = _sys_drop_down_valueService.Queryable().AsNoTracking().

                    Where(a =>
                     (a.DropDownID == 70) &&
        (
            a.CompanyID == CompanyID ||
            a.CompanyID == null
        ) && a.Value.StartsWith(term))

                    .Select(z => new
                    {
                        value = z.ID,
                        label = z.Value,

                    }).Take(20).ToList();
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
        [ActionName("TestSearch")]
        public async Task<ResponseInfo> TestSearch(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var result = _service.Queryable().AsNoTracking().
                    Where(a => a.CompanyId == CompanyID && a.SerialNo.StartsWith(term))
                    .Select(z => new
                    {
                        value = z.ID,
                        label = z.SerialNo,
                    }).Take(20).ToList();
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
        [ActionName("SearchPatient")]
        public async Task<ResponseInfo> SearchPatient(string term)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var PatientInfo = await _emr_patientService.Queryable()
            .AsNoTracking()
            .Where(a => a.CompanyId == CompanyID && (a.Mobile.Contains(term)))
            .Select(z => new
            {
                value = z.ID,
                label = z.PatientName,
                PatientName = z.PatientName,
                Phone = z.Mobile,
                CNIC = z.CNIC,
                Age = z.Age,
                Gender = z.Gender == 1 ? "Male" : z.Gender == 2 ? "Female" : "Other",
                MRNO = z.MRNO
            }).Take(10)
            .ToListAsync();
                objResponse.ResultSet = new
                {
                    PatientInfo = PatientInfo
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
        [ActionName("Load")]
        public ResponseInfo Load()
        {

            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var result = _sys_drop_down_valueService.Queryable().Where(e => (e.CompanyID == null || e.CompanyID == CompanyID) && e.DropDownID == 70).Select(a => new { a.ID, a.Value, DropDownID = a.DropDownID }).ToList();
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
        [ActionName("GetBillByInvoiceNo")]
        public ResponseInfo GetBillByInvoiceNo(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var result = _service.Queryable().Where(e => e.InvoiceNo == Id && e.CompanyId == CompanyID)
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
                        requestedDate = z.CreatedDate,
                        SampleDate = z.SampleDate,
                        CompletedDate = z.CompletedDate,
                        CompanyLogo = z.adm_company.CompanyLogo,
                        invoiceNo = z.InvoiceNo,
                        MRNo = z.emr_patient_mf.MRNO,
                        DOB = z.emr_patient_mf.DOB,
                        gander = z.emr_patient_mf.Gender == 1 ? "Male" : z.emr_patient_mf.Gender == 2 ? "Female" : "Other",
                        Age = z.emr_patient_mf.Age,
                        Physician = z.ReferringPhysician,
                        DiscountAmount = z.DiscountAmount,
                        Discount = z.Discount,
                        DiscountType = z.DiscountType,
                        Total = z.Total,
                        emr_investigation = z.emr_investigation
                    }).ToList();
                var results = result.GroupBy(l => l.requestedDate)
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
                            invoiceNo = cl.First().invoiceNo,
                            Physician = cl.First().Physician,
                            RequestedDate = cl.First().requestedDate,
                            DOB = cl.First().DOB,
                            Gender = cl.First().gander,
                            Age = cl.First().Age,
                            SampleDate = cl.First().SampleDate,
                            CompletedDate = cl.First().CompletedDate,
                            Discount = cl.First().Discount,
                            DiscountType = cl.First().DiscountType,
                            DiscountAmount = cl.First().DiscountAmount,
                            MRNo = cl.First().MRNo,
                            Total = cl.First().Total,
                            CompanyLogo = cl.First().CompanyLogo,
                            InvoiceModel = cl.Select(z => z.emr_investigation),
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
        [ActionName("GenerateReport")]
        public ResponseInfo GenerateReport(string id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal companyId = Request.CompanyID();

                var invoice = _service.Queryable().Where(x => x.ID.ToString() == id && x.CompanyId == companyId)
                    .Select(z => new
                    {
                        z.ID,
                        z.CompanyId,
                        ReportNo = z.InvoiceNo,
                        ReportDate = z.CompletedDate,
                        DepartmentName = z.emr_investigation.sys_drop_down_value.Value,
                        SampleName = z.emr_investigation.sys_drop_down_value1.Value,
                        MRNO = z.emr_patient_mf.MRNO,
                        PatientName = z.emr_patient_mf.PatientName,
                        DOB = z.emr_patient_mf.DOB,
                        Age = z.emr_patient_mf.Age.ToString(),
                        Gender = z.emr_patient_mf.Gender == 1 ? "Male" : z.emr_patient_mf.Gender == 2 ? "Female" : "Other",
                        Phone = z.emr_patient_mf.Mobile,
                        Address = z.emr_patient_mf.Address,
                        OrderedBy = z.adm_user_mf.Name,
                        ReferringPhysician = z.ReferringPhysician,
                        CollectionCenter = z.adm_company.CompanyName,
                        CollectionCenterAddress = z.adm_company.CompanyAddress1,
                        CollectionCenterAddress2 = z.adm_company.CompanyAddress2,
                        CollectionFax = z.adm_company.Fax,
                        CollectionEmail = z.adm_company.Email,
                        CollectionCenterPhone = z.adm_company.Phone,
                        CollectionCenterWebsite = z.adm_company.Website,
                        CollectionCenterImage = z.adm_company.CompanyLogo,
                        RequestedDate = z.CreatedDate,
                        CollectedDate = z.SampleDate,
                        ReceivedDate = z.SampleDate,
                        ReportedDate = z.CompletedDate,
                        InvoiceModel = z.emr_investigation.lab_parameter.Select(p => new
                        {
                            TestName = p.ParameterName,
                            NormalRange = p.ReferenceRange,
                            Unit = p.sys_drop_down_value.Value,
                            CurrentResult = z.lab_result_mf
                         .SelectMany(a => a.lab_result_dt)
                         .Where(r => r.ParameterId == p.ID)
                         .Select(r => r.Result)
                         .FirstOrDefault(),
                            Status = z.lab_result_mf
                         .SelectMany(a => a.lab_result_dt)
                         .Where(r => r.ParameterId == p.ID)
                         .Select(r => r.Status.ToString())
                         .FirstOrDefault() ?? string.Empty
                        }).ToList()
                    }).FirstOrDefault();
                if (invoice == null)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = "Report not found.";
                    return objResponse;
                }
                objResponse.IsSuccess = true;
                objResponse.ResultSet = invoice;
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
                decimal id = Convert.ToDecimal(Id);
                object result = null;
                var existResult = _lab_result_mfService.Queryable().Where(x => x.CompanyId == CompanyID && x.PatientTestId == id)
                     .Select(x => new
                     {
                         testId = x.lab_patient_test.ID,
                         ID = x.ID,
                         serialNo = x.lab_patient_test.SerialNo,
                         patientid = x.PatientId,
                         catalogId = x.CatalogId,
                         catalogName = x.emr_investigation.Investigation,
                         department = x.emr_investigation.sys_drop_down_value.Value,
                         patientName = x.emr_patient_mf.PatientName,
                         patientMrNo = x.emr_patient_mf.MRNO,
                         sampleType = x.emr_investigation.sys_drop_down_value1.Value,
                         priority = x.lab_patient_test.sys_drop_down_value.Value,
                         comment = x.Comment,
                         Parameter = x.lab_result_dt.Select(z => new
                         {
                             parmId = z.ParameterId,
                             name = z.lab_parameter.ParameterName,
                             unit = z.lab_parameter.sys_drop_down_value.Value,
                             refernce = z.lab_parameter.ReferenceRange,
                             description = z.lab_parameter.Description,
                             normalMin = z.lab_parameter.NormalMinimum,
                             normalMax = z.lab_parameter.NormalMaximum,
                             criticalLow = z.lab_parameter.CriticalLow,
                             criticalHigh = z.lab_parameter.CriticalHigh,
                             result = z.Result,
                             status = z.Status,
                             comment = z.Comment
                         }).ToList(),
                     }).FirstOrDefault();

                if (existResult == null)
                {
                    result = _service.Queryable()
                .Where(x => x.CompanyId == CompanyID && x.ID == id)
                .Select(x => new
                {
                    ID = 0,
                    testId = x.ID,
                    serialNo = x.SerialNo,
                    patientid = x.PatientId,
                    catalogId = x.CatalogId,
                    catalogName = x.emr_investigation.Investigation,
                    department = x.emr_investigation.sys_drop_down_value.Value,
                    patientName = x.emr_patient_mf.PatientName,
                    patientMrNo = x.emr_patient_mf.MRNO,
                    sampleType = x.emr_investigation.sys_drop_down_value1.Value,
                    priority = x.sys_drop_down_value.Value,
                    comment = string.Empty,
                    Parameter = x.emr_investigation.lab_parameter.Select(z => new
                    {
                        parmId = z.ID,
                        name = z.ParameterName,
                        unit = z.sys_drop_down_value.Value,
                        refernce = z.ReferenceRange,
                        description = z.Description,
                        normalMin = z.NormalMinimum,
                        normalMax = z.NormalMaximum,
                        criticalLow = z.CriticalLow,
                        criticalHigh = z.CriticalHigh,
                        result = string.Empty,
                        status = string.Empty,
                        comment = string.Empty,
                    }).ToList(),

                })
                .FirstOrDefault();
                }
                else
                {
                    result = existResult;
                }

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
        [ActionName("SelectedPatientLoadTest")]
        public ResponseInfo SelectedPatientLoadTest(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                decimal id = Convert.ToDecimal(Id);
                var result = _emr_prescription_investigationService.Queryable().Where(x => x.CompanyID == CompanyID && x.PatientId == id && DbFunctions.TruncateTime(x.CreatedDate) == DbFunctions.TruncateTime(DateTime.Now))
                     .Select(x => new
                     {
                         x.ID,
                         x.InvestigationId,
                     }).ToList();
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
        [ActionName("GetPrintById")]
        public ResponseInfo GetPrintById(string Id, string serialNo = null)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal parsedId = 0;
                decimal.TryParse(Id, out parsedId);

                var CompanyID = Request.CompanyID();
                decimal id = Convert.ToDecimal(Id);
                var result = _service.Queryable()
            .Where(x => x.CompanyId == CompanyID)
            .Where(x => x.ID == parsedId || x.SerialNo == serialNo)
            .GroupBy(x => new
            {
                x.SerialNo,
                x.emr_patient_mf.PatientName,
                x.emr_patient_mf.MRNO,
                x.emr_investigation.sys_drop_down_value1.Value,
                priority = x.sys_drop_down_value.Value,
                x.CreatedDate,
            })
            .Select(g => new
            {
                Header = new
                {
                    serialNo = g.Key.SerialNo,
                    patientName = g.Key.PatientName,
                    patientCode = g.Key.MRNO,
                    sampleType = g.Key.Value,
                    priority = g.Key.priority,
                    orderedDate = g.Key.CreatedDate,
                    Details = g.Select(d => new
                    {
                        testName = d.emr_investigation.Investigation
                    }).ToList()
                }
            })
            .FirstOrDefault();

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
                var ids = new int[] { 68, 71, 72 };
                objResult.OtherData3Model = _sys_drop_down_valueService.Queryable().Where(e => (e.CompanyID == null || e.CompanyID == CompanyID) && ids.Contains(e.DropDownID)).Select(a => new { a.ID, a.Value, DropDownID = a.DropDownID }).ToList();

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
        public async Task<ResponseInfo> Update(lab_patient_test Model)
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
                if (Model.PriorityId != 0)
                {
                    Model.PriorityDropdownId = (int)sys_dropdown_mfEnum.LabPriorityDropdownId;
                }
                if (Model.StatusId != 0)
                {
                    Model.StatusDropdownId = (int)sys_dropdown_mfEnum.LabStatusDropdownId;
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
        [HttpPut]
        [HttpGet]
        [ActionName("UpdateStatus")]
        public async Task<ResponseInfo> UpdateStatus(lab_patient_test model)
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
                var CompanyID = Request.CompanyID();
                var Model = _service.Queryable().Where(e => e.ID == model.ID && e.CompanyId == CompanyID).FirstOrDefault();
                if (Model == null)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
                }
                var oldStatusId = Model.StatusId;
                var newStatusId = model.StatusId;

                if (oldStatusId == 4 && newStatusId < 4)
                {
                    Model.CompletedDate = null;
                }
                if (newStatusId < 2)
                {
                    Model.SampleDate = null;
                    Model.CompletedDate = null;
                    Model.InProgressDate = null;
                }
                else if (newStatusId == 2)
                {
                    Model.SampleDate = Request.DateTimes();
                    Model.InProgressDate = null;
                    Model.CompletedDate = null;
                }
                else if (newStatusId == 3)
                {
                    Model.InProgressDate = Request.DateTimes();
                    Model.CompletedDate = null;
                }
                else if (newStatusId == 4)
                {
                    Model.CompletedDate = Request.DateTimes();
                }
                Model.StatusId = newStatusId;
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

        public async void SendRemindersAsync(emr_patient_mf model, lab_patient_test testModel, adm_company company)
        {
            if (model == null || testModel == null || company == null)
                return;
            string webUrl = System.Configuration.ConfigurationManager.AppSettings["WebUrl"];
            string reportUrl = webUrl + "/#/your-lab-results";

            DataAccessManager dataAccessManager = new DataAccessManager();
            var ht = new Hashtable();
            ht.Add("@CompanyID", company.ID);
            ht.Add("@ReminderId", 0);
            var allData = dataAccessManager.GetDataSet("SP_GetReminderData", ht);
            var integrationTable = allData.Tables[1];
            bool isUnicode = false;
            var smsRow = integrationTable.AsEnumerable()
        .FirstOrDefault(r => r["Type"].ToString() == "T" && Convert.ToBoolean(r["IsActive"]));
            var totalAmount = testModel.Total - testModel.DiscountAmount;
            string messageBody = $@"Dear {model.PatientName}, ({model.MRNO}).
            Thank you for visiting {company.CompanyName}. Please note that PKR {totalAmount:N0}/- has been received against your laboratory test(s) booking.
            You can view/download your report(s) online at:
            {reportUrl}
            Invoice #: {testModel.InvoiceNo}
            Access Code: {testModel.AccessCode}
            Thank you for choosing {company.CompanyName}.".Trim();
            if (smsRow != null)
            {
                await _smsService.AuthenticateAsync(
                                smsRow["UserName"].ToString(),
                                smsRow["Password"].ToString(),
                                model.Mobile,
                                messageBody,
                                smsRow["Masking"].ToString(),
                                isUnicode
                            );
            }
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

                List<lab_patient_test> Models = _service.Queryable().Where(e => e.CompanyId == CompanyID && IdList.Contains(e.ID)).ToList();

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
        [ActionName("PaginationWithParm")]
        public PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResult = _service.PaginationWithParm(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, FilterID, IgnorePaging);

                var ids = new int[] { 68, 71, 72, 2 };
                objResult.OtherData3Model = _sys_drop_down_valueService.Queryable().Where(e => (e.CompanyID == null || e.CompanyID == CompanyID) && ids.Contains(e.DropDownID)).Select(a => new { a.ID, a.Value, DropDownID = a.DropDownID }).ToList();

            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }
    }
}
