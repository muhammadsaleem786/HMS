using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
using HMS.Service;
using HMS.Service.Services.Admin;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using HMS.Web.API.Interface;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace HMS.Web.API.Areas.Admin.Controllers
{
    [JwtAuthentication]
    public class adm_companyController : ApiController, IERPAPIInterface<adm_company>, IDisposable
    {
        private readonly Iadm_companyService _service;
        private readonly Isys_drop_down_mfService _drop_down_mfService;
        private readonly Isys_drop_down_valueService _drop_down_valueService;
        private readonly Iadm_user_companyService _adm_user_companyService;
        private readonly Iadm_userService _adm_userService;
        private readonly Iadm_role_mfService _adm_role_mfService;
        private readonly Iadm_role_dtService _adm_role_dtService;
        private readonly IStoredProcedureService _procedureService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        public adm_companyController(IUnitOfWorkAsync unitOfWorkAsync, Iadm_companyService Service,
            Isys_drop_down_mfService drop_down_mfService, Isys_drop_down_valueService drop_down_valueService, Iadm_user_companyService adm_user_companyService,
             Iadm_userService adm_userService, Iadm_role_mfService adm_role_mfService, Iadm_role_dtService adm_role_dtService
            , IStoredProcedureService ProcedureService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = Service;
            _drop_down_mfService = drop_down_mfService;
            _drop_down_valueService = drop_down_valueService;
            _adm_user_companyService = adm_user_companyService;
            _adm_userService = adm_userService;
            _adm_role_mfService = adm_role_mfService;
            _adm_role_dtService = adm_role_dtService;
            _procedureService = ProcedureService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/Admin/adm_company/save")]
        //[ActionName("Save")]
        public async Task<ResponseInfo> Save(adm_company Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                if (!ModelState.IsValid)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.BadRequest;
                    //return objResponse;
                }

                string PayrollRegion = System.Configuration.ConfigurationManager.AppSettings["PayrollRegion"].ToString();
                DateTime datetime = Request.DateTimes();
                decimal LoginID = Request.LoginID();



                decimal adm_companyID = 1;

                if (_service.Queryable().Count() > 0)
                    adm_companyID = _service.Queryable().Max(e => e.ID) + 1;
                Model.ID = adm_companyID;
                Model.IsTrialVersion = true;
                Model.DateFormatDropDownID = (int)sys_dropdown_mfEnum.DateFormatDropDownID;
                Model.CompanyTypeDropDownID = (int)sys_dropdown_mfEnum.CompanyType;
                Model.CreatedBy = LoginID;
                Model.CreatedDate = datetime;
                Model.ObjectState = ObjectState.Added;
                //Model.adm_company_location = null;
                Model.ObjectState = ObjectState.Added;
                _service.Insert(Model);
                adm_role_mf role_mf = new adm_role_mf();
                decimal roleID = 1;
                if (_adm_role_mfService.Queryable().Count() > 0)
                    roleID = _adm_role_mfService.Queryable().Max(e => e.ID) + 1;
                role_mf.ID = roleID;
                role_mf.CompanyID = Model.ID;
                role_mf.RoleName = "Administrator";
                role_mf.SystemGenerated = true;

                role_mf.CreatedBy = LoginID;
                role_mf.CreatedDate = datetime;
                role_mf.ModifiedBy = LoginID;
                role_mf.ModifiedDate = datetime;
                role_mf.ObjectState = ObjectState.Added;
                _adm_role_mfService.Insert(role_mf);


                List<ScreenModel> Screens = _procedureService.GetAllScreen().ToList();
                decimal roleDetID = 1;
                if (_adm_role_dtService.Queryable().Count() > 0)
                    roleDetID = _adm_role_dtService.Queryable().Max(e => e.ID) + 1;
                foreach (var item in Screens)
                {
                    adm_role_dt roleDetail = new adm_role_dt();
                    roleDetail.ID = roleDetID;
                    roleDetail.RoleID = role_mf.ID;
                    roleDetail.CompanyID = Model.ID;
                    roleDetail.DropDownScreenID = (int)sys_dropdown_mfEnum.Screen;
                    roleDetail.ScreenID = item.ID;
                    roleDetail.CreatedBy = LoginID;
                    roleDetail.CreatedDate = datetime;
                    roleDetail.ModifiedBy = LoginID;
                    roleDetail.ModifiedDate = datetime;
                    roleDetail.ObjectState = ObjectState.Added;
                    _adm_role_dtService.Insert(roleDetail);
                    roleDetID++;
                }

                adm_user_company user_company = new adm_user_company();
                decimal userCompanyID = 1;
                if (_adm_user_companyService.Queryable().Count() > 0)
                    userCompanyID = _adm_user_companyService.Queryable().Max(e => e.ID) + 1;

                user_company.ID = userCompanyID;
                user_company.UserID = LoginID;
                user_company.RoleID = role_mf.ID;
                user_company.CompanyID = Model.ID;
                user_company.AdminID = LoginID;
                user_company.ObjectState = ObjectState.Added;
                _adm_user_companyService.Insert(user_company);

                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Save;
                    objResponse.IsSuccess = true;
                    string DateFormat = "";
                    var DateFormatId = Model.DateFormatId;
                    if (DateFormatId != 0)
                    {
                        var date = _drop_down_valueService.Queryable().Where(a => a.DropDownID == 36 && a.ID == DateFormatId).FirstOrDefault();
                        DateFormat = date.Value;
                    }

                    objResponse.ResultSet = new
                    {
                        CompanyID = Model.ID,
                        CompanyName = Model.CompanyName,
                        DateFormat = DateFormat,
                        IsCNICMandatory = Model.IsCNICMandatory,
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
        [HttpGet]
        [ActionName("GetLeaveWorkingHour")]
        public ResponseInfo GetLeaveWorkingHour()
        {
            int companyID = Request.CompanyID();
            var objResponse = new ResponseInfo();
            try
            {
                objResponse.ResultSet = _service.Queryable().Where(a => a.ID == companyID).Select(a => a.StandardShiftHours).FirstOrDefault();
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("api/Admin/adm_company/EditContactInfo")]
        public async Task<ResponseInfo> EditContactInfo(adm_company Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                adm_company company = new adm_company();
                try
                {
                    decimal Compid = Request.CompanyID();
                    company = _service.Queryable().Where(e => e.ID == Compid).FirstOrDefault();
                    if (company != null)
                    {
                        company.ContactPersonFirstName = Model.ContactPersonFirstName;
                        company.ContactPersonLastName = Model.ContactPersonLastName;
                        company.GenderID = Model.GenderID;
                        company.Phone = Model.Phone;
                        company.Fax = Model.Fax;
                        company.ObjectState = ObjectState.Modified;
                        _service.Update(company);
                        try
                        {
                            await _unitOfWorkAsync.SaveChangesAsync();
                            objResponse.IsSuccess = true;
                            objResponse.Message = "Password changed successfully";
                            var Company = new
                            {
                                CompanyID = company.ID,
                                ContactPersonFirstName = company.ContactPersonFirstName,
                                ContactPersonLastName = company.ContactPersonLastName,
                                ContactPersonMobile = company.Phone,
                                Email = company.Email,
                                GenderID = company.GenderID,
                                WorkPhone = company.Fax,
                            };

                            objResponse.ResultSet = new
                            {
                                Company = Company,
                            };

                        }
                        catch (DbUpdateException)
                        {
                            throw;
                        }
                    }
                    else
                    {
                        objResponse.IsSuccess = false;
                    }

                }
                catch (Exception ex)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = ex.Message;
                    Logger.Trace.Error(ex);
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

        public ResponseInfo Load()
        {
            throw new NotImplementedException();
        }

        public ResponseInfo GetList()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [ActionName("GetById")]
        public ResponseInfo GetById(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                objResponse.ResultSet = _service.Queryable().Where(e => e.ID == CompanyID).FirstOrDefault();
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
            throw new NotImplementedException();
        }

        [HttpPut]
        [HttpGet]
        [ActionName("Update")]
        public async Task<ResponseInfo> Update(adm_company Model)
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
                Model.CompanyTypeDropDownID = (int)sys_dropdown_mfEnum.CompanyType;
                Model.DateFormatDropDownID = (int)sys_dropdown_mfEnum.DateFormatDropDownID;
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Modified;
                _service.Update(Model);

                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.IsSuccess = true;
                    objResponse.Message = MessageStatement.Update;
                    string DateFormat = "";
                    var DateFormatId = Model.DateFormatId;
                    if (DateFormatId != 0)
                    {
                        var date = _drop_down_valueService.Queryable().Where(a => a.DropDownID == 36 && a.ID == DateFormatId).FirstOrDefault();
                        DateFormat = date.Value;
                    }
                    objResponse.ResultSet = new
                    {
                        CompanyID= Model.ID,
                        CompanyName = Model.CompanyName,
                        DateFormat = DateFormat,
                        IsCNICMandatory = Model.IsCNICMandatory,
                        ReceiptFooter = Model.ReceiptFooter,
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

        public Task<ResponseInfo> Delete(string Id)
        {
            throw new NotImplementedException();
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

        public ResponseInfo ExportData(int ExportType, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText)
        {
            throw new NotImplementedException();
        }

        private string getGender(int? GenderID)
        {
            if (GenderID == 1)
            {
                return "M";
            }
            if (GenderID == 2)
            {
                return "F";
            }
            if (GenderID == 3)
            {
                return "O";
            }
            return "M";
        }




        private bool ModelExists(string key)
        {
            return _service.Query(e => e.ID.ToString() == key).Select().Any();
        }

        public PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            throw new NotImplementedException();
        }
    }

}
