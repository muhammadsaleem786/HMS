using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HMS.Web.API.Areas.Admission.Controllers
{
    [JwtAuthentication]
    public class ipd_procedure_mfController : ApiController, IERPAPIInterface<ipd_procedure_mf>, IDisposable
    {
        private readonly Iipd_procedure_mfService _service;
        private readonly Iipd_procedure_chargedService _ipd_procedure_chargedservice;
        private readonly Iipd_procedure_medicationService _ipd_procedure_medicationservice;
        private readonly Iinv_stockService _inv_stockService;
        private readonly Iemr_appointment_mfService _emr_appointment_mfService;        
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iadm_item_logService _adm_item_logService;
        private readonly Iipd_procedure_expenseService _ipd_procedure_expenseService;

        public ipd_procedure_mfController(IUnitOfWorkAsync unitOfWorkAsync, 
            Iipd_procedure_mfService service, Iemr_appointment_mfService emr_appointment_mfService,
            Iipd_procedure_chargedService ipd_procedure_chargedService,
            Iipd_procedure_medicationService ipd_procedure_medicationService,
            Iinv_stockService inv_stockService,
            Isys_drop_down_valueService sys_drop_down_valueService, Iadm_item_logService adm_item_logService, Iipd_procedure_expenseService ipd_procedure_expenseService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = service;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _emr_appointment_mfService = emr_appointment_mfService;
            _inv_stockService = inv_stockService;
            _ipd_procedure_medicationservice = ipd_procedure_medicationService;
            _ipd_procedure_chargedservice = ipd_procedure_chargedService;
            _adm_item_logService = adm_item_logService;
            _ipd_procedure_expenseService = ipd_procedure_expenseService;
        }
        [HttpPost]
        [HttpGet]
        [ActionName("Save")]
        public async Task<ResponseInfo> Save(ipd_procedure_mf Model)
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

                List<ipd_procedure_charged> Objipd_procedure_charged = new List<ipd_procedure_charged>();
                Objipd_procedure_charged.AddRange(Model.ipd_procedure_charged);

                List<ipd_procedure_medication> Objipd_procedure_medication = new List<ipd_procedure_medication>();
                Objipd_procedure_medication.AddRange(Model.ipd_procedure_medication);


                List<ipd_procedure_expense> Objipd_procedure_expense = new List<ipd_procedure_expense>();
                Objipd_procedure_expense.AddRange(Model.ipd_procedure_expense);

                decimal ID = 1;
                if (_service.Queryable().Count() > 0)
                    ID = _service.Queryable().Max(e => e.ID) + 1;
                Model.ID = ID;
                Model.CPTCodeDropdownId = (int)sys_dropdown_mfEnum.CPTCodeDropdownId;
                Model.CompanyId = Request.CompanyID();
                Model.CreatedBy = Request.LoginID();
                Model.CreatedDate = Request.DateTimes();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ipd_procedure_medication = null;
                Model.ipd_procedure_charged = null;
                Model.ipd_procedure_expense = null;
                Model.ObjectState = ObjectState.Added;
                _service.Insert(Model);

                //insert charge
                decimal chargeID = 1;
                if (_ipd_procedure_chargedservice.Queryable().Count() > 0)
                    chargeID = _ipd_procedure_chargedservice.Queryable().Max(e => e.ID) + 1;
                adm_item_log adm_item_log_obj = new adm_item_log();
                decimal LogID = 1;
                if (_adm_item_logService.Queryable().Count() > 0)
                    LogID = _adm_item_logService.Queryable().Max(e => e.ID) + 1;
                foreach (ipd_procedure_charged item in Objipd_procedure_charged)
                {
                    item.ID = chargeID;
                    item.ProcedureId = Model.ID;
                    item.AppointmentId = Model.AppointmentId;
                    item.PatientId = Model.PatientId; 
                    item.CompanyId = CompanyID;                   
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.ObjectState = ObjectState.Added;
                    item.ipd_procedure_mf = null;
                    item.adm_user_mf = null;
                    item.adm_user_mf1 = null;
                    _ipd_procedure_chargedservice.Insert(item);
                    chargeID++;
                    inv_stock inv_stockModel = _inv_stockService.Queryable().
                        Where(e => e.ItemID == item.ItemId
                         && (e.BatchSarialNumber == null && item.Batch == null
                    || e.BatchSarialNumber.ToString() == item.Batch)
                        && e.CompanyId == CompanyID).FirstOrDefault();
                    if (inv_stockModel != null)
                    {
                        inv_stockModel.Quantity -= Convert.ToDecimal(item.Quantity);
                        _inv_stockService.Update(inv_stockModel);
                        //log maintain                               
                        adm_item_log_obj.ID = LogID;
                        adm_item_log_obj.CompanyId = CompanyID;
                        adm_item_log_obj.ItemId = item.ItemId;
                        adm_item_log_obj.Quantity = "-" + item.Quantity;
                        adm_item_log_obj.Type = "Procedure Quantity";
                        adm_item_log_obj.CreatedBy = Request.LoginID();
                        adm_item_log_obj.CreatedDate = Request.DateTimes();
                        adm_item_log_obj.ModifiedBy = Request.LoginID();
                        adm_item_log_obj.ModifiedDate = Request.DateTimes();
                        adm_item_log_obj.ObjectState = ObjectState.Added;
                        _adm_item_logService.Insert(adm_item_log_obj);
                        LogID++;
                    }
                }

                //insert medication
                decimal medicationID = 1;
                if (_ipd_procedure_medicationservice.Queryable().Count() > 0)
                    medicationID = _ipd_procedure_medicationservice.Queryable().Max(e => e.ID) + 1;

                foreach (ipd_procedure_medication item in Objipd_procedure_medication)
                {
                    item.ID = medicationID;
                    item.ProcedureId = Model.ID;
                    item.AppointmentId = Model.AppointmentId;
                    item.PatientId = Model.PatientId;
                    item.CompanyId = CompanyID;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.ObjectState = ObjectState.Added;
                    item.ipd_procedure_mf = null;
                    item.adm_user_mf = null;
                    item.adm_user_mf1 = null;
                    _ipd_procedure_medicationservice.Insert(item);
                    medicationID++;
                }
                //insert procedure_expense
                decimal proExpID = 1;
                if (_ipd_procedure_expenseService.Queryable().Count() > 0)
                    proExpID = _ipd_procedure_expenseService.Queryable().Max(e => e.ID) + 1;

                foreach (ipd_procedure_expense item in Objipd_procedure_expense)
                {
                    item.ID = proExpID;
                    item.ProcedureId = Model.ID;
                    item.CompanyID = CompanyID;
                    item.CategoryDropdownId = (int)sys_dropdown_mfEnum.CategoryDropdownId;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.ObjectState = ObjectState.Added;
                    item.ipd_procedure_mf = null;
                    item.adm_user_mf = null;
                    item.adm_user_mf1 = null;
                    _ipd_procedure_expenseService.Insert(item);
                    proExpID++;
                }



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
        [HttpPut]
        [HttpGet]
        [ActionName("Update")]
        public async Task<ResponseInfo> Update(ipd_procedure_mf Model)
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
                List<ipd_procedure_charged> Objipd_procedure_charged = new List<ipd_procedure_charged>();
                Objipd_procedure_charged.AddRange(Model.ipd_procedure_charged);
                List<ipd_procedure_medication> Objipd_procedure_medication = new List<ipd_procedure_medication>();
                Objipd_procedure_medication.AddRange(Model.ipd_procedure_medication);


                List<ipd_procedure_expense> Objipd_procedure_expense = new List<ipd_procedure_expense>();
                Objipd_procedure_expense.AddRange(Model.ipd_procedure_expense);

                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Modified;
                Model.ipd_procedure_charged = null;
                Model.ipd_procedure_medication = null;
                Model.ipd_procedure_expense = null;
                Model.adm_user_mf = null;
                Model.adm_user_mf1 = null;
                _service.Update(Model);
                //delete charged
                var DelAlLcharged = _ipd_procedure_chargedservice.Queryable().Where(e => e.ProcedureId == Model.ID && e.CompanyId == CompanyID).ToList();
                foreach (var obj in DelAlLcharged)
                {
                    obj.ObjectState = ObjectState.Deleted;
                    _ipd_procedure_chargedservice.Delete(obj);
                }
                //delete medication
                var DelAlLmedication = _ipd_procedure_medicationservice.Queryable().Where(e => e.ProcedureId == Model.ID && e.CompanyId == CompanyID).ToList();
                foreach (var objdia in DelAlLmedication)
                {
                    objdia.ObjectState = ObjectState.Deleted;
                    _ipd_procedure_medicationservice.Delete(objdia);
                }
                //delete procedure_expense
                var DelAlLPro = _ipd_procedure_expenseService.Queryable().Where(e => e.ProcedureId == Model.ID && e.CompanyID == CompanyID).ToList();
                foreach (var objdia in DelAlLPro)
                {
                    objdia.ObjectState = ObjectState.Deleted;
                    _ipd_procedure_expenseService.Delete(objdia);
                }

                //insert charge
                decimal chargeID = 1;
                if (_ipd_procedure_chargedservice.Queryable().Count() > 0)
                    chargeID = _ipd_procedure_chargedservice.Queryable().Max(e => e.ID) + 1;

                foreach (ipd_procedure_charged item in Objipd_procedure_charged)
                {
                    item.ID = chargeID;
                    item.ProcedureId = Model.ID;
                    item.AppointmentId = Model.AppointmentId;
                    item.PatientId = Model.PatientId;
                    item.CompanyId = CompanyID;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.ObjectState = ObjectState.Added;
                    item.ipd_procedure_mf = null;
                    item.adm_user_mf = null;
                    item.adm_user_mf1 = null;
                    _ipd_procedure_chargedservice.Insert(item);
                    chargeID++;
                }

                //insert medication
                decimal medicationID = 1;
                if (_ipd_procedure_medicationservice.Queryable().Count() > 0)
                    medicationID = _ipd_procedure_medicationservice.Queryable().Max(e => e.ID) + 1;

                foreach (ipd_procedure_medication item in Objipd_procedure_medication)
                {
                    item.ID = medicationID;
                    item.ProcedureId = Model.ID;
                    item.AppointmentId = Model.AppointmentId;
                    item.PatientId = Model.PatientId;
                    item.CompanyId = CompanyID;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.ObjectState = ObjectState.Added;
                    item.ipd_procedure_mf = null;
                    item.adm_user_mf = null;
                    item.adm_user_mf1 = null;
                    _ipd_procedure_medicationservice.Insert(item);
                    medicationID++;
                }
                //insert procedure_expense
                decimal proExpID = 1;
                if (_ipd_procedure_expenseService.Queryable().Count() > 0)
                    proExpID = _ipd_procedure_expenseService.Queryable().Max(e => e.ID) + 1;

                foreach (ipd_procedure_expense item in Objipd_procedure_expense)
                {
                    item.ID = proExpID;
                    item.ProcedureId = Model.ID;
                    item.CompanyID = CompanyID;
                    item.CategoryDropdownId = (int)sys_dropdown_mfEnum.CategoryDropdownId;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.ObjectState = ObjectState.Added;
                    item.ipd_procedure_mf = null;
                    item.adm_user_mf = null;
                    item.adm_user_mf1 = null;
                    _ipd_procedure_expenseService.Insert(item);
                    proExpID++;
                }


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
        [ActionName("GetProeduresDropDown")]
        public ResponseInfo GetProeduresDropDown(string PatientId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();

                var DropdownList = _sys_drop_down_valueService.Queryable().Where(a => a.DropDownID == 29 ||a.DropDownID==26 || a.DropDownID == 18 && (a.CompanyID == CompanyID || a.CompanyID == null)).Select(z => new
                {
                    z.ID,
                    z.Value,
                    z.DropDownID
                }).ToList();


                objResponse.ResultSet = new
                {
                    DropdownList = DropdownList,
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
                decimal CompanyID = Request.CompanyID();
                int ID = Convert.ToInt32(Id);
                var result = _service.Queryable().Where(e => e.ID== ID && e.CompanyId == CompanyID).Include(a => a.ipd_procedure_charged).Include(a=>a.ipd_procedure_expense).Include(a => a.ipd_procedure_medication.Select(c => c.emr_medicine)).FirstOrDefault();
                var DropdownList = _sys_drop_down_valueService.Queryable().Where(a => a.DropDownID == 29 || a.DropDownID == 26 || a.DropDownID == 18 && (a.CompanyID == CompanyID || a.CompanyID == null)).Select(z => new
                {
                    z.ID,
                    z.Value,
                    z.DropDownID
                }).ToList();
                objResponse.IsSuccess = true;
                objResponse.ResultSet = new
                {
                    result = result,
                    DropdownList = DropdownList,
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
        [ActionName("ProeduresPagination")]
        public PaginationResult ProeduresPagination(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string AdmitId, string PatientId, string Appointmentid, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResult = _service.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, AdmitId, PatientId, Appointmentid, IgnorePaging);
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
        public async Task<ResponseInfo> Delete(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal ID = Convert.ToDecimal(Id);
                ipd_procedure_mf model = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.ID == ID).FirstOrDefault();

                if (model == null)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
                }
                //delete charged
                var DelAlLcharged = _ipd_procedure_chargedservice.Queryable().Where(e => e.ProcedureId == model.ID && e.CompanyId == CompanyID).ToList();
                foreach (var obj in DelAlLcharged)
                {
                    obj.ObjectState = ObjectState.Deleted;
                    _ipd_procedure_chargedservice.Delete(obj);
                }
                //delete medication
                var DelAlLmedication = _ipd_procedure_medicationservice.Queryable().Where(e => e.ProcedureId == model.ID && e.CompanyId == CompanyID).ToList();
                foreach (var objdia in DelAlLmedication)
                {
                    objdia.ObjectState = ObjectState.Deleted;
                    _ipd_procedure_medicationservice.Delete(objdia);
                }
                
                //delete procedure_expense
                var DelAlLPro = _ipd_procedure_expenseService.Queryable().Where(e => e.ProcedureId == model.ID && e.CompanyID == CompanyID).ToList();
                foreach (var objdia in DelAlLPro)
                {
                    objdia.ObjectState = ObjectState.Deleted;
                    _ipd_procedure_expenseService.Delete(objdia);
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
