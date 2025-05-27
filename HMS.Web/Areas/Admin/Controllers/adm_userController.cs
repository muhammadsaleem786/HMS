using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Appointment;
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static iTextSharp.text.pdf.AcroFields;

namespace HMS.Web.API.Areas.Admin.Controllers
{
    [JwtAuthentication]
    public class adm_userController : ApiController, IERPAPIInterface<adm_user_mf>, IDisposable
    {
        private readonly Iadm_userService _service;
        private readonly Iemr_patientService _emr_patientService;
        private readonly Iadm_user_companyService _adm_user_companyService;
        private readonly Iadm_role_mfService _adm_role_mfService;
        private readonly Iadm_user_tokenService _adm_user_tokenService;
        private readonly Isys_notification_alertService _sys_notification_alertService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly Iuser_paymentService _user_paymentService;
        private readonly IcontactService _contactService;
        private readonly Ipr_employee_mfService _pr_employee_mfService;
        private readonly Ipr_leave_typeService _pr_leave_typeService;
        private readonly Ipr_deduction_contributionService _pr_deduction_contributionService;
        private readonly Ipr_allowanceService _pr_allowanceService;
        private readonly Ipr_pay_scheduleService _pr_pay_scheduleService;
        private readonly Iadm_companyService _adm_companyService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        public adm_userController(IUnitOfWorkAsync unitOfWorkAsync, Iadm_userService service, Iadm_user_companyService adm_user_companyService, IcontactService contactService,
            Iadm_role_mfService adm_role_mfService, Iadm_user_tokenService adm_user_tokenService, Iemr_patientService emr_patientService,
            Isys_notification_alertService sys_notification_alertService, Isys_drop_down_valueService sys_drop_down_valueService, Iuser_paymentService user_paymentService
            , Ipr_employee_mfService pr_employee_mfService,
             Ipr_leave_typeService pr_leave_typeService,
           Ipr_deduction_contributionService pr_deduction_contributionService,
            Ipr_pay_scheduleService pr_pay_scheduleService,
           Ipr_allowanceService pr_allowanceService,
           Iadm_companyService adm_companyService
            )
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = service;
            _pr_employee_mfService = pr_employee_mfService;
            _contactService = contactService;
            _emr_patientService = emr_patientService;
            _adm_user_companyService = adm_user_companyService;
            _adm_role_mfService = adm_role_mfService;
            _adm_user_tokenService = adm_user_tokenService;
            _sys_notification_alertService = sys_notification_alertService;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _user_paymentService = user_paymentService;
            _pr_pay_scheduleService = pr_pay_scheduleService;
            _pr_allowanceService = pr_allowanceService;
            _pr_leave_typeService = pr_leave_typeService;
            _pr_deduction_contributionService = pr_deduction_contributionService;
            _adm_companyService = adm_companyService;
        }
        [HttpPost]
        [HttpGet]
        [ActionName("Save")]
        public async Task<ResponseInfo> Save(adm_user_mf Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    objResponse.IsSuccess = false;
                //    objResponse.ErrorMessage = MessageStatement.BadRequest;
                //    return objResponse;
                //}
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();

                var user = _service.Queryable().Where(y => y.Email.ToLower() == Model.Email.ToLower()).FirstOrDefault();
                if (user == null)
                {
                    adm_user_mf admUser = new adm_user_mf();
                    decimal adm_userID = 1;
                    if (_service.Queryable().Count() > 0)
                        adm_userID = _service.Queryable().Max(e => e.ID) + 1;

                    List<adm_user_company> adm_userCompany = new List<adm_user_company>();
                    adm_userCompany.AddRange(Model.adm_user_company);

                    var guid = Guid.NewGuid().ToString();
                    var token = "GUID=" + guid + "&type=ApplicationUser";
                    string encrytoken = Cryptography.Encrypt(token);
                    encrytoken = HttpServerUtility.UrlTokenEncode(Encoding.ASCII.GetBytes(encrytoken));
                    if (Model.AppStartTime != null)
                    {
                        TimeSpan starttime = TimeSpan.Parse(Model.AppStartTime);
                        Model.StartTime = starttime;
                    }
                    if (Model.AppEndTime != null)
                    {
                        TimeSpan endtime = TimeSpan.Parse(Model.AppEndTime);
                        Model.EndTime = endtime;
                    }
                    if (Model.DocWorkingDay != null)
                    {
                        string result = string.Join(",", Model.DocWorkingDay);
                        Model.OffDay = result;
                    }
                    if (Model.GenderDropdown != null)
                    {
                        string result = string.Join(",", Model.GenderDropdown);
                        Model.IsGenderDropdown = result;
                    }

                    if (Model.SlotTime != null)
                    {
                        Model.SlotTime = Model.SlotTime;
                    }
                    Model.SpecialtyDropdownId = (int)sys_dropdown_mfEnum.SpecialtyDropdownId;
                    Model.adm_user_company = null;
                    Model.user_payment = null;
                    Model.user_payment1 = null;
                    Model.user_payment2 = null;
                    Model.ID = adm_userID;
                    Model.AccountType = "A";
                    Model.ForgotToken = guid;
                    Model.ForgotTokenDate = Request.DateTimes();
                    Model.MultilingualId = Model.MultilingualId;
                    Model.ObjectState = ObjectState.Added;
                    _service.Insert(Model);

                    adm_user_token user_token = new adm_user_token();
                    decimal userTokenID = 1;
                    if (_adm_user_tokenService.Queryable().Count() > 0)
                        userTokenID = _adm_user_tokenService.Queryable().Max(e => e.ID) + 1;
                    user_token.ID = userTokenID;
                    user_token.UserID = Model.ID;
                    DateTime date = Request.DateTimes();
                    date = date.AddDays(3);
                    user_token.ExpiryDate = date;
                    user_token.TokenKey = guid;
                    user_token.IsExpired = false;
                    user_token.DeviceType = "web";
                    int deviceID = -1;
                    user_token.DeviceID = deviceID.ToString();
                    user_token.ObjectState = ObjectState.Added;
                    _adm_user_tokenService.Insert(user_token);

                    decimal adm_user_ComID = 1;
                    if (_adm_user_companyService.Queryable().Count() > 0)
                        adm_user_ComID = _adm_user_companyService.Queryable().Max(e => e.ID) + 1;
                    foreach (adm_user_company item in adm_userCompany)
                    {
                        adm_user_company admUserCompanyModel = new adm_user_company();
                        admUserCompanyModel.ID = adm_user_ComID;
                        admUserCompanyModel.UserID = Model.ID;
                        admUserCompanyModel.CompanyID = CompanyID;
                        admUserCompanyModel.RoleID = item.RoleID;
                        admUserCompanyModel.AdminID = _adm_user_companyService.Queryable()
                            .Where(e => e.CompanyID == CompanyID && e.UserID == e.AdminID)
                            .Select(s => s.UserID).FirstOrDefault();

                        admUserCompanyModel.ObjectState = ObjectState.Added;
                        _adm_user_companyService.Insert(admUserCompanyModel);
                        adm_user_ComID++;
                    }
                    var employeeobj = _pr_employee_mfService.Queryable().Where(a => a.CompanyID == CompanyID && a.ID == Model.EmployeeID).FirstOrDefault();
                    if (employeeobj != null)
                    {
                        employeeobj.EmployeePic = Model.UserImage;
                        employeeobj.ObjectState = ObjectState.Modified;
                        _pr_employee_mfService.Update(employeeobj);
                    }

                    //string path = @"~\Templates\EmailConfirm.txt";
                    //SendEmailforResetPassword(Model, "PayPeople | Verify your account", path, encrytoken);

                    //string path = @"~\Templates\EmailConfirm.txt";
                    //path = HttpContext.Current.Server.MapPath(path);
                    //Task.Run(() => SendEmailConfirmNotify(_unitOfWorkAsync, userobj, "PayPeople | Verify your account", path, "WelcomeEmail", encrytoken));

                    try
                    {
                        await _unitOfWorkAsync.SaveChangesAsync();
                        objResponse.Message = MessageStatement.Save;
                        objResponse.IsSuccess = true;
                        objResponse.ResultSet = new
                        {
                            Model = Model
                        };
                        EmailService objService = new EmailService();
                        string PayrollRegion = System.Configuration.ConfigurationManager.AppSettings["PayrollRegion"].ToString();
                        string path = "";
                        if (PayrollRegion == "PK")
                        {
                            path = @"~\Templates\EmailConfirm.txt";

                        }
                        else
                        {
                            path = "";
                        }

                        path = HttpContext.Current.Server.MapPath(path);
                        objService.SendEmailCofirm(path, Model.Email, Model.Name, CompanyID, encrytoken, Request.LoginID(), "Erpisto | Verify your account");


                        //EmailService objService = new EmailService();

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
                else
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = "This " + Model.Email + " already register please use another email.";
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
        [ActionName("GetRoles")]
        public ResponseInfo GetRoles()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var role = _adm_role_mfService.Queryable().Where(x => x.CompanyID == CompanyID).ToList();
                var dropdown = _sys_drop_down_valueService.Queryable().Where(e => (e.CompanyID == null || e.CompanyID == CompanyID) && e.DropDownID == 12 || e.DropDownID == 2 || e.DropDownID == 24)
                     .Select(s => new { ID = s.ID, name = s.Value, DropDownID = s.DropDownID }).Distinct().ToList();
                var GenderList = dropdown.Where(e => e.DropDownID == 2)
                   .Select(s => new { id = s.ID, name = s.name }).ToList();
                var WorkingdayList = dropdown.Where(e => e.DropDownID == 12).Select(z => new
                {
                    id = z.ID,
                    name = z.name
                }).ToList();
                objResponse.ResultSet = new
                {
                    role = role,
                    dropdown = dropdown,
                    GenderList,
                    WorkingdayList
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
                objResponse.ResultSet = _service.Queryable().ToList();
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
        [ActionName("GetById")]
        public ResponseInfo GetById(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var role = _adm_role_mfService.Queryable().Where(x => x.CompanyID == CompanyID).ToList();
                var UserObj = _service.Queryable().Where(e => e.ID.ToString() == Id).Include(x => x.adm_user_company).Include(a => a.user_payment2).FirstOrDefault();
                var employeeObj = _pr_employee_mfService.Queryable().Where(e => e.CompanyID == CompanyID && e.ID == UserObj.EmployeeID).FirstOrDefault();

                objResponse.ResultSet = new
                {
                    employeeObj = employeeObj,
                    role = role,
                    UserObj = UserObj,
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
        [ActionName("PaymentPagination")]
        public PaginationResult PaymentPagination(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResult = _service.PaymentPagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }

        [HttpGet]
        [ActionName("SubscriberPagination")]
        public PaginationResult SubscriberPagination(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResult = _contactService.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
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
        //[HttpPost]
        //  [ActionName("IsImportDataExistInDB")]
        //  public ResponseInfo IsImportDataExistInDB(List<UserImportModel> ImportEmpList)
        //  {
        //      var objResponse = new ResponseInfo();
        //      try
        //      {
        //          decimal ID = Request.LoginID();
        //          decimal CompanyID = Request.CompanyID();
        //          var Emaillist = ImportEmpList.Where(x => x.EmployeeID != 0).Select(x => x.EmployeeID).Distinct().ToArray();
        //          var EmpList = _sale_customer_mfService.Queryable()
        //         .Where(x => x.CompanyID == CompanyID && Emaillist.Contains(x.ID)).Select(x => x.ID).ToArray();
        //          objResponse.ResultSet = Emaillist.Where(a => !EmpList.Contains(a));

        //      }
        //      catch (Exception ex)
        //      {
        //          objResponse.IsSuccess = false;
        //          objResponse.ErrorMessage = ex.Message;
        //          Logger.Trace.Error(ex);
        //      }
        //      return objResponse;
        //  }
        [HttpPut]
        [HttpGet]
        [ActionName("Update")]
        public async Task<ResponseInfo> Update(adm_user_mf Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    objResponse.IsSuccess = false;
                //    objResponse.ErrorMessage = MessageStatement.BadRequest;
                //    return objResponse;
                //}
                decimal CompanyID = Request.CompanyID();
                adm_user_mf admUser = new adm_user_mf();

                List<adm_user_company> adm_userCompany = new List<adm_user_company>();

                adm_userCompany.AddRange(Model.adm_user_company);

                string currdate = DateTime.Now.ToString("yyyy-MM-dd");
                if (Model.AppStartTime != null)
                {
                    TimeSpan starttime = TimeSpan.Parse(Model.AppStartTime);
                    Model.StartTime = starttime;
                }
                if (Model.AppEndTime != null)
                {
                    TimeSpan endtime = TimeSpan.Parse(Model.AppEndTime);

                    Model.EndTime = endtime;
                }
                if (Model.DocWorkingDay != null)
                {
                    string result = string.Join(",", Model.DocWorkingDay);
                    Model.OffDay = result;
                }
                if (Model.GenderDropdown != null)
                {
                    string result = string.Join(",", Model.GenderDropdown);
                    Model.IsGenderDropdown = result;
                }
                Model.adm_user_company = null;
                Model.adm_user_company1 = null;

                Model.ObjectState = ObjectState.Modified;
                _service.Update(Model);
                foreach (adm_user_company item in adm_userCompany)
                {
                    item.adm_role_mf = null;
                    item.ObjectState = ObjectState.Modified;
                    _adm_user_companyService.Update(item);
                }
                var employeeobj = _pr_employee_mfService.Queryable().Where(a => a.CompanyID == CompanyID && a.ID == Model.EmployeeID).FirstOrDefault();
                if (employeeobj != null)
                {
                    employeeobj.EmployeePic = Model.UserImage;
                    employeeobj.ObjectState = ObjectState.Modified;
                    _pr_employee_mfService.Update(employeeobj);
                }
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.IsSuccess = true;
                    objResponse.Message = MessageStatement.Update;
                    objResponse.ResultSet = new
                    {
                        Model = Model,
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
        [ActionName("UpdatePaymnt")]
        public async Task<ResponseInfo> UpdatePaymnt(adm_user_mf Model)
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
                adm_user_mf admUser = new adm_user_mf();
                List<adm_user_company> adm_userCompany = new List<adm_user_company>();
                List<user_payment> user_paymentObj = new List<user_payment>();
                adm_userCompany.AddRange(Model.adm_user_company);
                user_paymentObj.AddRange(Model.user_payment2);
                Model.adm_user_company = null;
                Model.adm_user_company1 = null;
                Model.user_payment = null;
                Model.user_payment1 = null;
                Model.user_payment2 = null;
                Model.ObjectState = ObjectState.Modified;
                _service.Update(Model);

                //foreach (adm_user_company item in adm_userCompany)
                //{
                //    item.adm_role_mf = null;
                //    item.ObjectState = ObjectState.Modified;
                //    _adm_user_companyService.Update(item);
                //}

                //delete Payment
                var DelAllPayment = _user_paymentService.Queryable().Where(e => e.UserId == Model.ID && e.CompanyId == CompanyID).ToList();
                foreach (var obj in DelAllPayment)
                {
                    obj.ObjectState = ObjectState.Deleted;
                    _user_paymentService.Delete(obj);
                }
                decimal PaymentId = 1;
                if (_user_paymentService.Queryable().Count() > 0)
                    PaymentId = _user_paymentService.Queryable().Max(e => e.ID) + 1;
                foreach (user_payment item in user_paymentObj)
                {

                    item.ID = PaymentId;
                    item.UserId = Model.ID;
                    item.Amount = item.Amount;
                    item.Remarks = item.Remarks;
                    item.Date = item.Date;
                    item.CompanyId = CompanyID;
                    item.CreatedBy = Request.LoginID();
                    item.CreatedDate = Request.DateTimes();
                    item.ModifiedBy = Request.LoginID();
                    item.ModifiedDate = Request.DateTimes();
                    item.ObjectState = ObjectState.Added;
                    _user_paymentService.Insert(item);
                    PaymentId++;
                }
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.IsSuccess = true;
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

                List<adm_user_mf> Models = _service.Queryable().Where(e => IdList.Contains(e.ID)).Include(x => x.adm_user_company).Include(x => x.adm_user_token).ToList();
                if (Models.Count() == 0)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
                }
                foreach (var Model in Models)
                {
                    foreach (var item in Model.adm_user_token.ToList())
                    {
                        item.ObjectState = ObjectState.Deleted;
                        _adm_user_tokenService.Delete(item);
                    }

                    foreach (var item in Model.adm_user_company.ToList())
                    {
                        item.ObjectState = ObjectState.Deleted;
                        _adm_user_companyService.Delete(item);
                    }

                    Model.ObjectState = ObjectState.Deleted;
                    _service.Delete(Model);
                }

                await _unitOfWorkAsync.SaveChangesAsync();
                objResponse.IsSuccess = true;
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
        public static string ResolveEmailConfirm(string Name, string path, string TokenLink = "")
        {
            string pat = HttpContext.Current.Server.MapPath(path);
            string template = File.ReadAllText(pat);
            string webUrl = System.Configuration.ConfigurationManager.AppSettings["WebUrl"];
            if (template.Contains("[[UserName]]"))
            {
                template = template.Replace("[[UserName]]", Name);
            }

            if (template.Contains("[[ResetLinkForShowTotalLink]]"))
            {
                template = template.Replace("[[ResetLinkForShowTotalLink]]", TokenLink);
            }
            if (template.Contains("[[WebUrl]]"))
            {
                template = template.Replace("[[WebUrl]]", webUrl);
            }
            return template;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWorkAsync.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPut]
        [HttpGet]
        [ActionName("UserUpdate")]
        public async Task<ResponseInfo> UserUpdate(string Doctorids)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal UserID = Request.LoginID();
                var userobj = _service.Queryable().Where(a => a.ID == UserID).FirstOrDefault();
                userobj.IsShowDoctor = Doctorids;
                userobj.ObjectState = ObjectState.Modified;
                _service.Update(userobj);


                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.IsSuccess = true;
                    objResponse.Message = MessageStatement.Update;
                    var DoctorList = _emr_patientService.DoctorList(CompanyID, UserID);
                    var userObj = _service.Queryable().Where(a => a.ID == UserID).FirstOrDefault();
                    decimal[] IdList = null;
                    List<DoctorList> DoctorCalander = new List<DoctorList>();
                    if (userObj.IsShowDoctor != null)
                    {
                        IdList = userObj.IsShowDoctor.Split(',').Select(decimal.Parse).ToArray();
                        DoctorCalander = DoctorList.DoctList.Where(a => !IdList.Contains(a.ID)).ToList();
                    }
                    else
                    {
                        DoctorCalander = DoctorList.DoctList.ToList();
                    }
                    objResponse.ResultSet = new
                    {
                        IsShowDoctorIds = userObj.IsShowDoctor,
                        DoctorList = DoctorList,
                        DoctorCalander = DoctorCalander,
                    };
                }
                catch (DbUpdateException)
                {

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

        public PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            throw new NotImplementedException();
        }
    }
}
