using HMS.Service;
using HMS.Web.API.Common;
using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Service.Services.Admin;
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

using HMS.Entities.Enum;
using HMS.Service.Services.Appointment;
using System.Net.Mail;
using HMS.Service.Services.Employee;
using System.IO.IsolatedStorage;

namespace HMS.Web.API.Controllers
{
    public class UserController : ApiController
    {
        private readonly Iadm_userService _service;
        private readonly Iadm_user_tokenService _adm_user_tokenService;
        private readonly Iadm_companyService _adm_companyService;
        private readonly Iadm_role_dtService _adm_role_dtService;
        private readonly Iadm_role_mfService _adm_role_mfService;
        private readonly Iadm_user_companyService _adm_user_companyService;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly Iadm_multilingual_mfService _adm_multilingual_mfservice;
        private readonly IStoredProcedureService _procedureService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iemr_service_mfService _emr_service_mfService;
        private readonly Ipr_employee_mfService _pr_employee_mfService;
        private readonly Ipr_leave_typeService _pr_leave_typeService;
        private readonly Ipr_deduction_contributionService _pr_deduction_contributionService;
        private readonly Ipr_allowanceService _pr_allowanceService;
        private readonly Ipr_pay_scheduleService _pr_pay_scheduleService;
        public UserController(IUnitOfWorkAsync unitOfWorkAsync, Iadm_userService Service, Iemr_service_mfService emr_service_mfService,
            Iadm_companyService adm_companyService, Iadm_multilingual_mfService adm_multilingual_mfservice,
            Iadm_role_dtService adm_role_dtService,
            Iadm_role_mfService adm_role_mfService,
            IStoredProcedureService procedureService,
            Iadm_user_companyService adm_user_companyService,
            Isys_drop_down_valueService sys_drop_down_valueService,
            Ipr_employee_mfService pr_employee_mfService,
            Ipr_leave_typeService pr_leave_typeService,
           Ipr_deduction_contributionService pr_deduction_contributionService,
            Ipr_pay_scheduleService pr_pay_scheduleService,
           Ipr_allowanceService pr_allowanceService,
            Iadm_user_tokenService iadm_user_tokenService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = Service;
            _pr_pay_scheduleService = pr_pay_scheduleService;
            _pr_allowanceService = pr_allowanceService;
            _pr_leave_typeService = pr_leave_typeService;
            _pr_deduction_contributionService = pr_deduction_contributionService;
            _emr_service_mfService = emr_service_mfService;
            _procedureService = procedureService;
            _adm_user_companyService = adm_user_companyService;
            _adm_companyService = adm_companyService;
            _adm_role_dtService = adm_role_dtService;
            _adm_role_mfService = adm_role_mfService;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _adm_user_tokenService = iadm_user_tokenService;
            _adm_multilingual_mfservice = adm_multilingual_mfservice;
            _pr_employee_mfService = pr_employee_mfService;
        }
        [AllowAnonymous]
        [HttpPost]
        [ActionName("Login")]

        public async Task<ResponseInfo> Login(adm_user_mf Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                adm_user_mf user = new adm_user_mf();
                try
                {
                    user = _service.Queryable().Where(e => e.Email == Model.Email && e.Pwd == Model.Pwd && e.IsActivated == true && e.UserLock == false).FirstOrDefault();

                    if (user != null)
                    {
                        //if (user.IsActivated)
                        //{
                        adm_user_token user_token = new adm_user_token();
                        user.LastSignIn = Request.DateTimes();
                        user.ObjectState = ObjectState.Modified;
                        _service.Update(user);
                        var Token = JwtManager.GenerateToken(user.ID.ToString(), 4320);
                        string token = Token.Split('|')[0];
                        DateTime expDate = Convert.ToDateTime(Token.Split('|')[1]);
                        decimal userId = user.ID;
                        user_token = _adm_user_tokenService.Queryable().Where(e => e.UserID == userId).FirstOrDefault();

                        user_token.TokenKey = token;
                        user_token.ExpiryDate = expDate;
                        user_token.ObjectState = ObjectState.Modified;
                        _adm_user_tokenService.Update(user_token);

                        try
                        {
                            await _unitOfWorkAsync.SaveChangesAsync();
                            var Companyobj = _adm_user_companyService.Queryable()
                            .Where(e => e.UserID == user.ID).Include(x => x.adm_company).Select(x => new { x.CompanyID, x.adm_company.CompanyName, x.adm_company.DateFormatId, x.adm_company.IsCNICMandatory, x.adm_company.ReceiptFooter }).FirstOrDefault();
                            //var MultiKeyword = _adm_multilingual_mfservice.Queryable()
                            //                 .Where(e => e.MultilingualId == user.MultilingualId)
                            //                 .SelectMany(s => s.adm_multilingual_dt)
                            //                 .ToList();
                            var CompanyID = Companyobj == null ? 0 : Companyobj.CompanyID;
                            var RoleIds = _adm_user_companyService.Queryable().Where(e => e.CompanyID == CompanyID && e.UserID == user.ID)
                           .Select(s => s.RoleID).ToArray();
                            var RoleName = _adm_role_mfService.Queryable().Where(a => a.CompanyID == CompanyID && RoleIds.Contains(a.ID))
                                .Select(s => s.RoleName).FirstOrDefault();
                            var Rights = _adm_role_dtService.Queryable().Where(e => e.CompanyID == CompanyID && e.ViewRights && RoleIds.Contains(e.RoleID))
                      .Select(s => s.ScreenID).Distinct().ToList();
                            var ControlLevelRights = _adm_role_dtService.Queryable().Where(e => e.CompanyID == CompanyID && e.ViewRights && RoleIds.Contains(e.RoleID))
                                  .Select(s => new { s.ScreenID, s.ViewRights, s.DeleteRights, s.EditRights, s.CreateRights }).Distinct().ToList();
                            var Modules = _sys_drop_down_valueService.Queryable().Where(e => (e.CompanyID == null || e.CompanyID == CompanyID) && e.DropDownID == 7 && Rights.Contains(e.ID))
                                    .Select(s => s.DependedDropDownValueID).Distinct().ToArray();
                            string DateFormat = "";
                            var DateFormatId = Companyobj == null ? 0 : Companyobj.DateFormatId;
                            if (DateFormatId != 0)
                            {
                                var date = _sys_drop_down_valueService.Queryable().Where(a => a.DropDownID == 36 && a.ID == DateFormatId).FirstOrDefault();
                                DateFormat = date.Value;
                            }
                            objResponse.ResultSet = new
                            {
                                User = user,
                                Token = user_token.TokenKey,
                                ValidTo = user_token.ExpiryDate,
                                Company = Companyobj,
                                PayrollRegion = System.Configuration.ConfigurationManager.AppSettings["PayrollRegion"],
                                Currency = System.Configuration.ConfigurationManager.AppSettings["Currency"],
                                ControlLevelRights = ControlLevelRights,
                                Modules = Modules,
                                Rights = Rights,
                                RoleName = RoleName,
                            };
                            objResponse.IsSuccess = true;
                        }
                        catch (DbUpdateException)
                        {
                            if (ModelExists(user.ID.ToString()))
                            {
                                objResponse.IsSuccess = false;
                                objResponse.ErrorMessage = MessageStatement.Conflict;
                                return objResponse;
                            }
                            throw;
                        }
                        objResponse.IsSuccess = true;
                        //}
                        //else
                        //{
                        //    objResponse.IsSuccess = true;
                        //    objResponse.Message = "RedirectToConfirmPage";
                        //}

                    }
                    else
                    {
                        user = _service.Queryable().Where(e => e.Email == Model.Email).FirstOrDefault();
                        user.LoginFailureNo += 1;
                        if (user.LoginFailureNo > 5)
                            user.UserLock = true;
                        _service.Update(user);
                        await _unitOfWorkAsync.SaveChangesAsync();
                        objResponse.IsSuccess = false;
                        if (user.UserLock)
                            objResponse.ErrorMessage = "Your account has been locked due to multiple unsuccessful login attempts. Please reset your password to regain access.";
                        else
                            objResponse.ErrorMessage = "User Name or Password is incorrect.";
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
        [AllowAnonymous]
        [HttpGet]
        [ActionName("GetPayrollRegion")]
        public ResponseInfo GetPayrollRegion()
        {
            var objResponse = new ResponseInfo();
            objResponse.ResultSet = new
            {
                PayrollRegion = System.Configuration.ConfigurationManager.AppSettings["PayrollRegion"],
                Currency = System.Configuration.ConfigurationManager.AppSettings["Currency"],
            };
            objResponse.IsSuccess = true;
            return objResponse;
        }
        [AllowAnonymous]
        [HttpGet]
        //[ActionName("forgotpassword")]
        [Route("api/User/forgotpassword")]
        public async Task<ResponseInfo> forgotpassword(string Email)
        {
            var objResponse = new ResponseInfo();
            try
            {
                adm_user_mf user = new adm_user_mf();
                try
                {
                    user = _service.Queryable().Where(e => e.Email == Email).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = ex.Message;
                    Logger.Trace.Error(ex);
                }

                if (user != null)
                {
                    //var Token = JwtManager.GenerateToken(user.ID.ToString(), 4320);
                    //string token = Token.Split('|')[0];
                    //token = token + "&type=ForgotToken";

                    var guid = Guid.NewGuid().ToString();
                    var token = "GUID=" + guid + "&type=ForgotToken";
                    string encrytoken = Cryptography.Encrypt(token);
                    encrytoken = HttpServerUtility.UrlTokenEncode(Encoding.ASCII.GetBytes(encrytoken));

                    //string encrytoken = guid;
                    user.ForgotToken = guid;
                    //DateTime ValidTo = Convert.ToDateTime(Token.Split('|')[1]);
                    user.ForgotTokenDate = Request.DateTimes().AddDays(1);
                    user.ObjectState = ObjectState.Modified;
                    _service.Update(user);

                    try
                    {
                        await _unitOfWorkAsync.SaveChangesAsync();
                        objResponse.Message = MessageStatement.Update;

                        string path = @"~\Templates\changepassword.txt";
                        path = HttpContext.Current.Server.MapPath(path);
                        Task.Run(() => SendEmailConfirmNotify(_unitOfWorkAsync, user, "Change Password", path, "changepasswordlink", encrytoken));

                    }
                    catch (DbUpdateException)
                    {
                        if (ModelExists(user.ID.ToString()))
                        {
                            objResponse.IsSuccess = false;
                            objResponse.ErrorMessage = MessageStatement.Conflict;
                            return objResponse;
                        }
                        throw;
                    }
                    objResponse.IsSuccess = true;
                }
                else
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = "No account found against " + Email + " . Please provide valid account email";
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

        [AllowAnonymous]
        [HttpPost]
        //[ActionName("resetpassword")]
        [Route("api/User/resetpassword")]
        public async Task<ResponseInfo> resetpassword(adm_user_mf model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                adm_user_mf user = new adm_user_mf();
                string key = "";
                try
                {

                    if (model.ForgotToken.EndsWith("="))
                        model.ForgotToken = model.ForgotToken.Substring(0, model.ForgotToken.Length - 1);
                    byte[] tok = HttpServerUtility.UrlTokenDecode(model.ForgotToken);
                    string tokenToValid = Encoding.ASCII.GetString(tok);
                    string forgotToken = Cryptography.Decrypt(tokenToValid);
                    string guid = Convert.ToString(forgotToken.Split('=')[1].Split('&')[0]);
                    string type = Convert.ToString(forgotToken.Split('&')[1].Split('=')[1]);
                    key = type;
                    if (key == "ForgotToken")
                        user = _service.Queryable().Where(e => e.ForgotToken == guid).FirstOrDefault();
                    if (key == "ApplicationUser")
                        user = _service.Queryable().Where(e => e.ForgotToken == guid).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = ex.Message;
                    Logger.Trace.Error(ex);
                }
                if (user != null)
                {
                    DateTime date = Request.DateTimes();
                    date = date.AddDays(-1);
                    if (key == "ApplicationUser")
                    {
                        user.ActivatedDate = Request.DateTimes();
                        user.IsActivated = true;
                    }
                    user.Pwd = model.Pwd;
                    user.LoginFailureNo = 0;
                    user.UserLock = false;
                    user.ForgotTokenDate = date;
                    user.ObjectState = ObjectState.Modified;
                    _service.Update(user);
                    try
                    {
                        await _unitOfWorkAsync.SaveChangesAsync();
                        objResponse.Message = MessageStatement.Update;
                        //string path = @"~\Templates\changepassword.txt";
                        //path = HttpContext.Current.Server.MapPath(path);
                        //Task.Run(() => SendEmailConfirmNotify(_unitOfWorkAsync, user, "Change Password", path, "changepasswordlink", ""));
                    }
                    catch (DbUpdateException)
                    {
                        if (ModelExists(user.ID.ToString()))
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
                    objResponse.ErrorMessage = "Link is not correct.";
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
        [AllowAnonymous]
        [HttpGet]
        [ActionName("IsEmailExist")]
        public ResponseInfo IsEmailExist(string Email)
        {
            var objResponse = new ResponseInfo();
            try
            {
                adm_user_mf user = new adm_user_mf();
                try
                {
                    user = _service.Queryable().Where(e => e.Email == Email).FirstOrDefault();
                    if (user != null)
                    {
                        objResponse.IsSuccess = true;
                        //objResponse.ErrorMessage = "User Name or Password is incorrect.";
                    }
                    else
                    {
                        objResponse.IsSuccess = false;
                        objResponse.ErrorMessage = "Email does not exist";
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
        [AllowAnonymous]
        [HttpGet]
        [ActionName("IsForgotTokenExist")]
        public ResponseInfo IsForgotTokenExist(string token)
        {
            var objResponse = new ResponseInfo();
            try
            {
                adm_user_mf user = new adm_user_mf();
                try
                {
                    if (token.EndsWith("="))
                        token = token.Substring(0, token.Length - 1);

                    byte[] tok = HttpServerUtility.UrlTokenDecode(token);
                    string tokenToValid = Encoding.ASCII.GetString(tok);
                    string forgotToken = Cryptography.Decrypt(tokenToValid);
                    string guid = Convert.ToString(forgotToken.Split('=')[1].Split('&')[0]);
                    user = _service.Queryable().Where(e => e.ForgotToken == guid).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = ex.Message;
                    Logger.Trace.Error(ex);
                }

                if (user != null)
                {
                    DateTime date = Request.DateTimes();
                    if (user.ForgotTokenDate >= date)
                    {
                        objResponse.IsSuccess = true;
                        objResponse.Message = "true";
                    }
                    else
                    {
                        objResponse.IsSuccess = true;
                        objResponse.Message = "TokenExpired";
                    }

                    objResponse.ResultSet = user;

                    //objResponse.ErrorMessage = "User Name or Password is incorrect.";
                }
                else
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = "Email Already Exist";
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
        [AllowAnonymous]
        [HttpGet]
        //[ActionName("ResendEmail")]
        [Route("api/User/ResendEmail")]
        public async Task<ResponseInfo> ResendEmail(string Email)
        {
            var objResponse = new ResponseInfo();
            try
            {
                adm_user_mf user = new adm_user_mf();
                try
                {
                    user = _service.Queryable().Where(e => e.Email == Email).FirstOrDefault();

                    if (user != null)
                    {
                        if (user.IsActivated)
                        {
                            objResponse.IsSuccess = true;
                            objResponse.ResultSet = new
                            {
                                Email = user.Email,
                                isActivated = user.IsActivated,
                            };
                            objResponse.Message = "AlreadyActivated";
                        }
                        else
                        {
                            // Generate Token
                            // 3 days = 4320 minuts   expire after three days
                            //var Token = JwtManager.GenerateToken(user.ID.ToString(), 4320);
                            //string token = Token.Split('|')[0];
                            //user.ActivationToken = Cryptography.Encrypt(token);

                            var guid = Guid.NewGuid().ToString();
                            var token = "GUID=" + guid;
                            string encrytoken = Cryptography.Encrypt(token);
                            encrytoken = HttpServerUtility.UrlTokenEncode(Encoding.ASCII.GetBytes(encrytoken));
                            user.ActivationToken = guid;
                            user.ActivationTokenDate = Request.DateTimes();
                            user.ObjectState = ObjectState.Modified;
                            _service.Update(user);


                            try
                            {
                                await _unitOfWorkAsync.SaveChangesAsync();
                                objResponse.ResultSet = new
                                {
                                    Email = user.Email,
                                    isActivated = user.IsActivated,
                                };
                                objResponse.Message = "Email sent successfully";

                                string path = @"~\Templates\EmailConfirm.txt";
                                path = HttpContext.Current.Server.MapPath(path);
                                Task.Run(() => SendEmailConfirmNotify(_unitOfWorkAsync, user, "Confirm your account", path, "tokenlink", encrytoken));

                            }
                            catch (DbUpdateException)
                            {
                                if (ModelExists(user.ID.ToString()))
                                {
                                    objResponse.IsSuccess = false;
                                    objResponse.ErrorMessage = MessageStatement.Conflict;
                                    return objResponse;
                                }
                                throw;
                            }
                        }

                    }
                    else
                    {
                        objResponse.IsSuccess = false;
                        objResponse.ErrorMessage = "Email is not correct";
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
        [AllowAnonymous]
        [HttpGet]
        [ActionName("Confirm")]
        public async Task<ResponseInfo> Confirm(string userId)
        {
            var objResponse = new ResponseInfo();
            try
            {
                adm_user_mf user = new adm_user_mf();
                try
                {
                    //var CompanyID = Request.CompanyID();
                    DateTime date = Request.DateTimes();
                    date = date.AddDays(-3);

                    if (userId.EndsWith("="))
                        userId = userId.Substring(0, userId.Length - 1);

                    byte[] tok = HttpServerUtility.UrlTokenDecode(userId);
                    string tokenToValid = Encoding.ASCII.GetString(tok);
                    string forgotToken = Cryptography.Decrypt(tokenToValid);
                    string guid = Convert.ToString(forgotToken.Split('=')[1]);

                    user = _service.Queryable().Where(e => e.ActivationToken == guid && e.ActivationTokenDate >= date).FirstOrDefault();

                    if (user != null)
                    {
                        if (!user.IsActivated)
                        {
                            user.IsActivated = true;
                            user.ActivatedDate = Request.DateTimes();
                            user.ObjectState = ObjectState.Modified;
                            _service.Update(user);

                            try
                            {
                                await _unitOfWorkAsync.SaveChangesAsync();
                                objResponse.Message = MessageStatement.Update;

                                string path = @"~\Templates\WelcomeEmail.txt";
                                path = HttpContext.Current.Server.MapPath(path);
                                Task.Run(() => SendEmailConfirmNotify(_unitOfWorkAsync, user, "Welcome to ", path, "WelcomeEmail", ""));

                            }
                            catch (DbUpdateException)
                            {
                                if (ModelExists(user.ID.ToString()))
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

                            objResponse.IsSuccess = true;
                            objResponse.Message = "AlreadyConfirmed";
                        }
                        objResponse.ResultSet = new
                        {
                            Email = user.Email,
                        };
                    }
                    else
                    {
                        objResponse.IsSuccess = false;
                        objResponse.ErrorMessage = "Link is not correct.";
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
        private void SendEmailConfirmNotify(IUnitOfWorkAsync _UOWAsync, adm_user_mf user, string subject, string path, string EmailType, string EncryptionToken)
        {
            string link = "";
            decimal sys_notificationID = 1;
            string webUrl = System.Configuration.ConfigurationManager.AppSettings["WebUrl"];
            string EmailFrom = System.Configuration.ConfigurationManager.AppSettings["EmailFrom"];
            string EmailDisplayName = System.Configuration.ConfigurationManager.AppSettings["EmailDisplayName"];
            string EmailPassword = System.Configuration.ConfigurationManager.AppSettings["EmailPassword"];
            string EmailSMTP = System.Configuration.ConfigurationManager.AppSettings["EmailSMTP"];
            int EmailPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["EmailPort"]);

            //sys_notification_alert sys_notification = new sys_notification_alert();

            //if (_UOWAsync.Repository<sys_notification_alert>().Queryable().Count() > 0)
            //    sys_notificationID = _UOWAsync.Repository<sys_notification_alert>().Queryable().Max(e => e.ID) + 1;

            //sys_notification.ID = sys_notificationID;
            //sys_notification.TypeID = 2;
            //sys_notification.EmailFrom = EmailFrom;
            //sys_notification.EmailTo = user.Email;
            //sys_notification.Subject = subject + ' ' + webUrl;

            if (EmailType == "tokenlink")
                link = webUrl + "/#/confirmed?" + EncryptionToken;
            else if (EmailType == "changepasswordlink")
                link = webUrl + "/#/resetpassword?" + EncryptionToken;

            //sys_notification.Body = ResolveEmailConfirm(user.Name, path, link);
            //sys_notification.CreatedBy = Convert.ToInt32(user.ID);
            //sys_notification.CreatedDate = Request.DateTimes();
            //sys_notification.ObjectState = ObjectState.Added;

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            MailMessage mail = new MailMessage();
            SmtpClient smtpC = new SmtpClient(EmailSMTP);
            smtpC.EnableSsl = true;
            smtpC.Port = EmailPort;
            smtpC.Credentials = new System.Net.NetworkCredential(EmailFrom, EmailPassword);
            //From address to send email
            if (string.IsNullOrEmpty(EmailDisplayName))
                mail.From = new MailAddress(EmailFrom);
            else
                mail.From = new MailAddress(EmailFrom, EmailDisplayName);
            //To address to send email
            mail.To.Add(user.Email);
            mail.Subject = subject;
            string link1 = webUrl + "/#/resetpassword?" + EncryptionToken;
            mail.Body = ResolveEmailConfirm(user.Name, path, link1);
            mail.IsBodyHtml = true;

            try
            {
                //Send Email
                smtpC.Send(mail);
                //sys_notification.SentTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                //sys_notification.FailureCount = 1;
                //sys_notification.SentTime = null;
            }

            //_sys_notification_alertService.Insert(sys_notification);
            //_UOWAsync.Repository<sys_notification_alert>().Insert(sys_notification);
            _UOWAsync.SaveChanges();
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("api/User/SaveUser")]
        public async Task<ResponseInfo> SaveUser(adm_user_mf Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                if (!ModelState.IsValid)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.BadRequest;
                }
                objResponse = IsEmailExist(Model.Email);
                decimal LoginID = Request.LoginID();
                decimal empID = 1;
                if (!objResponse.IsSuccess)
                {                  
                    if (_pr_employee_mfService.Queryable().Count() > 0)
                        empID = _pr_employee_mfService.Queryable().Max(e => e.ID) + 1;

                    decimal adm_userID = 1;
                    if (_service.Queryable().Count() > 0)
                        adm_userID = _service.Queryable().Max(e => e.ID) + 1;
                    Model.ID = adm_userID;
                    var guid = Guid.NewGuid().ToString();
                    var token = "GUID=" + guid;
                    string encrytoken = Cryptography.Encrypt(token);
                    encrytoken = HttpServerUtility.UrlTokenEncode(Encoding.ASCII.GetBytes(encrytoken));
                    Model.ActivationToken = guid;
                    Model.ActivationTokenDate = Request.DateTimes();
                    Model.ActivatedDate = null;
                    Model.LastSignIn = Request.DateTimes();
                    Model.LoginFailureNo = 0;
                    Model.IsActivated = false;
                    Model.UserLock = false;
                    Model.EmployeeID = empID;
                    Model.ObjectState = ObjectState.Added;
                    _service.Insert(Model);
                    adm_user_token user_token = new adm_user_token();
                    decimal userTokenID = 1;
                    if (_adm_user_tokenService.Queryable().Count() > 0)
                        userTokenID = _adm_user_tokenService.Queryable().Max(e => e.ID) + 1;
                    user_token.ID = userTokenID;
                    user_token.UserID = Model.ID;
                    user_token.IsExpired = false;
                    user_token.DeviceType = "web";
                    int deviceID = -1;

                    int TokenexpireDays = 4320;
                    var Token = JwtManager.GenerateToken(Model.ID.ToString(), TokenexpireDays);
                    string token1 = Token.Split('|')[0];
                    DateTime expDate = Convert.ToDateTime(Token.Split('|')[1]);
                    user_token.TokenKey = token1;
                    user_token.ExpiryDate = expDate;

                    user_token.DeviceID = deviceID.ToString();
                    user_token.ObjectState = ObjectState.Added;

                    _adm_user_tokenService.Insert(user_token);
                    // saveCompany 
                    adm_company obj = new adm_company();
                    decimal adm_companyID = 1;
                    if (_adm_companyService.Queryable().Count() > 0)
                        adm_companyID = _adm_companyService.Queryable().Max(e => e.ID) + 1;
                    obj.ID = adm_companyID;
                    obj.IsTrialVersion = true;
                    obj.CompanyName = Model.CompanyName;
                    obj.Email = Model.Email;
                    obj.ContactPersonFirstName = Model.ContactPersonFirstName;
                    obj.ContactPersonLastName = Model.ContactPersonLastName;
                    obj.GenderID = Model.GenderID;
                    obj.Phone = Model.Phone;
                    obj.Fax = Model.Fax;
                    obj.CountryDropdownId = 1;
                    obj.CityDropDownId = 56;
                    obj.CompanyTypeDropDownID = (int)sys_dropdown_mfEnum.CompanyType;
                    obj.DateFormatDropDownID = (int)sys_dropdown_mfEnum.DateFormatDropDownID;
                    obj.IsCNICMandatory = false;
                    obj.IsBackDatedAppointment = false;
                    obj.DateFormatId = 1;
                    obj.CreatedBy = LoginID;
                    obj.CreatedDate = Request.DateTimes();
                    obj.ObjectState = ObjectState.Added;
                    _adm_companyService.Insert(obj);
                    //create employeee
                    pr_employee_mf employee_mf = new pr_employee_mf();
                    employee_mf.ID = empID;
                    employee_mf.CompanyID = adm_companyID;
                    employee_mf.FirstName = Model.Name;
                    employee_mf.LastName = Model.Name;
                    employee_mf.Gender = "O";
                    employee_mf.StreetAddress = null;
                    employee_mf.PayTypeDropDownID = (int)sys_dropdown_mfEnum.PayType;
                    employee_mf.PayTypeID = 1;
                    employee_mf.StatusDropDownID = (int)sys_dropdown_mfEnum.EmployeeStatus;
                    employee_mf.StatusID = 1;
                    employee_mf.PaymentMethodDropDownID = (int)sys_dropdown_mfEnum.SalaryPaymentMethod;
                    employee_mf.PaymentMethodID = 1;

                    employee_mf.EmployeeTypeDropDownID = (int)sys_dropdown_mfEnum.EmployeeType;
                    employee_mf.EmployeeTypeID = 1;
                    employee_mf.CityDropDownID = (int)sys_dropdown_mfEnum.City;
                    employee_mf.CityID = 1;
                    employee_mf.ZipCode = null;
                    employee_mf.CountryDropDownID = (int)sys_dropdown_mfEnum.Country;
                    employee_mf.CountryID = 1;
                    employee_mf.ContractTypeDropDownID = (int)sys_dropdown_mfEnum.ContractType;
                    employee_mf.ContractTypeID = 1;
                    employee_mf.Email = Model.Email;
                    employee_mf.HomePhone = Model.Phone;
                    employee_mf.Mobile = Model.Phone;
                    employee_mf.StreetAddress = string.Empty;
                    employee_mf.CreatedBy = LoginID;
                    employee_mf.CreatedDate = Request.DateTimes();
                    employee_mf.AirTicketProvided = "N";
                    employee_mf.MedicalInsuranceProvided = "N";
                    employee_mf.ObjectState = ObjectState.Added;
                    _pr_employee_mfService.Insert(employee_mf);

                    pr_pay_schedule pay_schedule = new pr_pay_schedule();
                    decimal pay_scheduleID = 1;
                    if (_pr_pay_scheduleService.Queryable().Count() > 0)
                        pay_scheduleID = _pr_pay_scheduleService.Queryable().Max(e => e.ID) + 1;
                    pay_schedule.ID = pay_scheduleID;
                    pay_schedule.CompanyID = adm_companyID;
                    pay_schedule.PayTypeDropDownID = (int)sys_dropdown_mfEnum.PayType;
                    pay_schedule.FallInHolidayDropDownID = (int)sys_dropdown_mfEnum.PayDayFallOnHoliday;
                    pay_schedule.PayTypeID = 1; // By default pay Period ID "Per Month"
                    pay_schedule.ScheduleName = "Monthly - Payroll";
                    DateTime date = Request.DateTimes();
                    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                    pay_schedule.PeriodStartDate = firstDayOfMonth;
                    pay_schedule.PeriodEndDate = lastDayOfMonth;
                    pay_schedule.FallInHolidayID = 2; //by default fall in holiday id;
                    pay_schedule.PayDate = new DateTime(firstDayOfMonth.Year, firstDayOfMonth.AddMonths(1).Month, 1);
                    pay_schedule.Active = true;
                    pay_schedule.CreatedBy = LoginID;
                    pay_schedule.CreatedDate = Request.DateTimes();
                    pay_schedule.ModifiedBy = LoginID;
                    pay_schedule.ModifiedDate = Request.DateTimes();
                    pay_schedule.ObjectState = ObjectState.Added;
                    _pr_pay_scheduleService.Insert(pay_schedule);


                    decimal LeavetypeID = 1;
                    if (_pr_leave_typeService.Queryable().Count() > 0)
                        LeavetypeID = _pr_leave_typeService.Queryable().Max(e => e.ID) + 1;
                    for (int i = 0; i < 3; i++)
                    {
                        pr_leave_type leavetype = new pr_leave_type();
                        leavetype.ID = LeavetypeID;
                        leavetype.CompanyID = adm_companyID;
                        if (i == 0)
                        {
                            leavetype.Category = "S";
                            leavetype.TypeName = "Sick";
                            leavetype.EarnedValue = 80;
                        }
                        else if (i == 1)
                        {
                            leavetype.Category = "V";
                            leavetype.TypeName = "Vacation";
                            leavetype.EarnedValue = 80;
                        }
                        else
                        {
                            leavetype.Category = "L";
                            leavetype.TypeName = "LWP";
                            leavetype.EarnedValue = 0;
                        }

                        leavetype.AccuralDropDownID = (int)sys_dropdown_mfEnum.AccrualFrequency;
                        leavetype.AccrualFrequencyID = 2; //by default Beginning of Calender;
                                                          // by defauly 80 hours per year
                        leavetype.SystemGenerated = true;
                        leavetype.CreatedBy = LoginID;
                        leavetype.CreatedDate = Request.DateTimes();
                        leavetype.ModifiedBy = LoginID;
                        leavetype.ModifiedDate = Request.DateTimes();
                        leavetype.ObjectState = ObjectState.Added;
                        _pr_leave_typeService.Insert(leavetype);
                        LeavetypeID++;
                    }
                    decimal DedAndcontrID = 1;
                    if (_pr_deduction_contributionService.Queryable().Count() > 0)
                        DedAndcontrID = _pr_deduction_contributionService.Queryable().Max(e => e.ID) + 1;
                    int noOfLoop = 6;
                    for (int i = 0; i < noOfLoop; i++)
                    {
                        pr_deduction_contribution deduction_contribution = new pr_deduction_contribution();
                        deduction_contribution.ID = DedAndcontrID;
                        deduction_contribution.CompanyID = adm_companyID;
                        if (i == 0)
                        {
                            deduction_contribution.Category = "C";
                            deduction_contribution.DeductionContributionName = "EOBI";
                        }
                        else if (i == 1)
                        {
                            deduction_contribution.Category = "D";
                            deduction_contribution.DeductionContributionName = "EOBI";
                        }
                        else if (i == 2)
                        {
                            deduction_contribution.Category = "C";
                            deduction_contribution.DeductionContributionName = "Provident Fund";
                        }
                        else if (i == 3)
                        {
                            deduction_contribution.Category = "D";
                            deduction_contribution.DeductionContributionName = "Provident Fund";
                            deduction_contribution.DeductionContributionType = "F";
                        }
                        else if (i == 4)
                        {
                            deduction_contribution.Category = "D";
                            deduction_contribution.DeductionContributionName = "Loan";
                        }
                        else if (i == 5)
                        {
                            deduction_contribution.Category = "D";
                            deduction_contribution.DeductionContributionName = "Income Tax";
                        }

                        deduction_contribution.DeductionContributionType = "F";
                        deduction_contribution.DeductionContributionValue = 0;
                        deduction_contribution.Taxable = false;
                        deduction_contribution.Default = true;
                        deduction_contribution.StartingBalance = false;
                        deduction_contribution.SystemGenerated = true;
                        deduction_contribution.CreatedBy = LoginID;
                        deduction_contribution.CreatedDate = Request.DateTimes();
                        deduction_contribution.ModifiedBy = LoginID;
                        deduction_contribution.ModifiedDate = Request.DateTimes();
                        deduction_contribution.ObjectState = ObjectState.Added;
                        _pr_deduction_contributionService.Insert(deduction_contribution);
                        DedAndcontrID++;
                    }
                    decimal AdminRoleID = 0, roleID = 1;
                    if (_adm_role_mfService.Queryable().Count() > 0)
                        roleID = _adm_role_mfService.Queryable().Max(e => e.ID) + 1;

                    List<ScreenModel> Screens = _procedureService.GetAllScreen().ToList();
                    decimal roleDetID = 1;
                    if (_adm_role_dtService.Queryable().Count() > 0)
                        roleDetID = _adm_role_dtService.Queryable().Max(e => e.ID) + 1;

                    for (int i = 0; i < 4; i++)
                    {
                        adm_role_mf role_mf = new adm_role_mf();
                        role_mf.ID = roleID;
                        role_mf.CompanyID = obj.ID;
                        if (i == 0)
                        {
                            AdminRoleID = roleID;
                            role_mf.RoleName = "Administrator";
                        }
                        if (i == 1)
                        {
                            role_mf.RoleName = "Doctor";
                            role_mf.IsUpdateText = true;
                        }
                        if (i == 2)
                        {
                            role_mf.RoleName = "Receptionist";
                            role_mf.IsUpdateText = true;
                        }
                        if (i == 3)
                        {
                            role_mf.RoleName = "Nurse";
                            role_mf.IsUpdateText = true;
                        }
                        role_mf.SystemGenerated = true;
                        role_mf.CreatedBy = LoginID;
                        role_mf.CreatedDate = Request.DateTimes();
                        role_mf.ModifiedBy = LoginID;
                        role_mf.ModifiedDate = Request.DateTimes();
                        role_mf.ObjectState = ObjectState.Added;
                        _adm_role_mfService.Insert(role_mf);
                        // Insert Admin Role
                        foreach (var item in Screens)
                        {
                            adm_role_dt roleDetail = new adm_role_dt();
                            roleDetail.ID = roleDetID;
                            roleDetail.RoleID = roleID;
                            roleDetail.CompanyID = obj.ID;
                            roleDetail.DropDownScreenID = (int)sys_dropdown_mfEnum.Screen;
                            roleDetail.ScreenID = item.ID;
                            if (role_mf.RoleName == "Administrator")
                            {
                                roleDetail.CreateRights = true;
                                roleDetail.EditRights = true;
                                roleDetail.ViewRights = true;
                                roleDetail.DeleteRights = true;
                            }
                            else
                            {
                                roleDetail.CreateRights = false;
                                roleDetail.EditRights = false;
                                roleDetail.ViewRights = false;
                                roleDetail.DeleteRights = false;
                            }
                            roleDetail.CreatedBy = Model.ID;
                            roleDetail.CreatedDate = Request.DateTimes();
                            roleDetail.ModifiedBy = Model.ID;
                            roleDetail.ModifiedDate = Request.DateTimes();
                            roleDetail.ObjectState = ObjectState.Added;
                            _adm_role_dtService.Insert(roleDetail);
                            roleDetID++;
                        }
                        roleID++;
                    }
                    adm_user_company user_company = new adm_user_company();
                    decimal userCompanyID = 1;
                    if (_adm_user_companyService.Queryable().Count() > 0)
                        userCompanyID = _adm_user_companyService.Queryable().Max(e => e.ID) + 1;
                    user_company.ID = userCompanyID;
                    user_company.UserID = Model.ID;
                    user_company.RoleID = AdminRoleID;
                    user_company.CompanyID = obj.ID;
                    user_company.AdminID = Model.ID;
                    user_company.ObjectState = ObjectState.Added;
                    _adm_user_companyService.Insert(user_company);
                    //add service
                    decimal billID = 1;
                    if (_emr_service_mfService.Queryable().Count() > 0)
                        billID = _emr_service_mfService.Queryable().Max(e => e.ID) + 1;
                    for (int i = 0; i <= 2; i++)
                    {
                        emr_service_mf objbill = new emr_service_mf();
                        if (i == 0)
                        {
                            objbill.ID = billID;
                            objbill.CreatedBy = LoginID;
                            objbill.CreatedDate = Request.DateTimes();
                            objbill.ModifiedBy = LoginID;
                            objbill.ModifiedDate = Request.DateTimes();
                            objbill.CompanyId = adm_companyID;
                            objbill.ServiceName = "Consultation";
                            objbill.IsSystemGenerated = true;
                            objbill.IsItem = false;
                            objbill.Price = 500;
                            objbill.ObjectState = ObjectState.Added;
                            _emr_service_mfService.Insert(objbill);
                        }

                        if (i == 1)
                        {
                            objbill.ID = billID;
                            objbill.CreatedBy =LoginID;
                            objbill.CreatedDate = Request.DateTimes();
                            objbill.ModifiedBy =LoginID;
                            objbill.ModifiedDate = Request.DateTimes();
                            objbill.CompanyId = adm_companyID;
                            objbill.ServiceName = "Item";
                            objbill.IsSystemGenerated = true;
                            objbill.IsItem = true;
                            objbill.Price = 1000;
                            objbill.ObjectState = ObjectState.Added;
                            _emr_service_mfService.Insert(objbill);
                        }
                        if (i == 2)
                        {
                            objbill.ID = billID;
                            objbill.CreatedBy = LoginID;
                            objbill.CreatedDate = Request.DateTimes();
                            objbill.ModifiedBy = LoginID;
                            objbill.ModifiedDate = Request.DateTimes();
                            objbill.CompanyId = adm_companyID;
                            objbill.ServiceName = "InPatient";
                            objbill.IsSystemGenerated = true;
                            objbill.IsItem = false;
                            objbill.Price = 0;
                            objbill.ObjectState = ObjectState.Added;
                            _emr_service_mfService.Insert(objbill);
                        }
                        billID++;
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
                else
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = "Email already exist";
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
        [AllowAnonymous]
        [HttpPost]
        [Route("api/User/saveCompany")]
        //[ActionName("Save")]
        public async Task<ResponseInfo> saveCompany(adm_company Model)
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
                decimal adm_companyID = 1;
                if (_adm_companyService.Queryable().Count() > 0)
                    adm_companyID = _adm_companyService.Queryable().Max(e => e.ID) + 1;
                PayrollRegion = System.Configuration.ConfigurationManager.AppSettings["PayrollRegion"];

                //if (PayrollRegion == "PK")
                //    Model.CurrencyDropDownTypeID = 13;
                //if (PayrollRegion == "SA")
                //    Model.CurrencyDropDownTypeID = 10;
                Model.ID = adm_companyID;
                Model.IsTrialVersion = true;
                Model.CompanyTypeDropDownID = (int)sys_dropdown_mfEnum.CompanyType;
                Model.CreatedBy = Request.LoginID();
                Model.CreatedDate = datetime;
                Model.ObjectState = ObjectState.Added;
                _adm_companyService.Insert(Model);

                decimal AdminRoleID = 0, roleID = 1;
                if (_adm_role_mfService.Queryable().Count() > 0)
                    roleID = _adm_role_mfService.Queryable().Max(e => e.ID) + 1;

                List<ScreenModel> Screens = _procedureService.GetAllScreen().ToList();
                decimal roleDetID = 1;
                if (_adm_role_dtService.Queryable().Count() > 0)
                    roleDetID = _adm_role_dtService.Queryable().Max(e => e.ID) + 1;

                for (int i = 0; i < 4; i++)
                {
                    adm_role_mf role_mf = new adm_role_mf();
                    role_mf.ID = roleID;
                    role_mf.CompanyID = Model.ID;
                    if (i == 0)
                    {
                        AdminRoleID = roleID;
                        role_mf.RoleName = "Administrator";
                    }
                    if (i == 1)
                    {
                        role_mf.RoleName = "Doctor";
                        role_mf.IsUpdateText = true;
                    }
                    if (i == 2)
                    {
                        role_mf.RoleName = "Receptionist";
                        role_mf.IsUpdateText = true;
                    }
                    if (i == 3)
                    {
                        role_mf.RoleName = "Nurse";
                        role_mf.IsUpdateText = true;
                    }
                    role_mf.SystemGenerated = true;

                    role_mf.CreatedBy = Request.LoginID();
                    role_mf.CreatedDate = datetime;
                    role_mf.ModifiedBy = Request.LoginID();
                    role_mf.ModifiedDate = datetime;
                    role_mf.ObjectState = ObjectState.Added;
                    _adm_role_mfService.Insert(role_mf);

                    // Insert Admin Role
                    foreach (var item in Screens)
                    {
                        adm_role_dt roleDetail = new adm_role_dt();
                        roleDetail.ID = roleDetID;
                        roleDetail.RoleID = roleID;
                        roleDetail.CompanyID = Model.ID;
                        roleDetail.DropDownScreenID = (int)sys_dropdown_mfEnum.Screen;
                        roleDetail.ScreenID = item.ID;
                        roleDetail.CreateRights = true;
                        roleDetail.EditRights = true;
                        roleDetail.ViewRights = true;
                        roleDetail.DeleteRights = true;
                        roleDetail.CreatedBy = Request.LoginID();
                        roleDetail.CreatedDate = datetime;
                        roleDetail.ModifiedBy = Request.LoginID();
                        roleDetail.ModifiedDate = datetime;
                        roleDetail.ObjectState = ObjectState.Added;
                        _adm_role_dtService.Insert(roleDetail);
                        roleDetID++;
                    }
                    roleID++;
                }
                adm_user_company user_company = new adm_user_company();
                decimal userCompanyID = 1;
                if (_adm_user_companyService.Queryable().Count() > 0)
                    userCompanyID = _adm_user_companyService.Queryable().Max(e => e.ID) + 1;

                user_company.ID = userCompanyID;
                user_company.UserID = Request.LoginID();
                user_company.RoleID = AdminRoleID;
                user_company.CompanyID = Model.ID;
                user_company.AdminID = Request.LoginID();
                user_company.ObjectState = ObjectState.Added;
                _adm_user_companyService.Insert(user_company);

                //update Token info
                adm_user_mf user = new adm_user_mf();
                user = _service.Queryable().Where(e => e.Email == Model.Email).FirstOrDefault();

                // 3 days = 4320 minuts   expire after three days
                int TokenexpireDays = 4320;
                adm_user_token user_token = new adm_user_token();
                decimal userId = user.ID;
                user_token = _adm_user_tokenService.Queryable().Where(e => e.UserID == userId).FirstOrDefault();
                user.LastSignIn = Request.DateTimes();
                user.LoginFailureNo = 0;
                user.UserLock = false;
                user.ObjectState = ObjectState.Modified;
                _service.Update(user);
                var Token = JwtManager.GenerateToken(user.ID.ToString(), TokenexpireDays);
                string token = Token.Split('|')[0];
                DateTime expDate = Convert.ToDateTime(Token.Split('|')[1]);
                user_token.TokenKey = token;
                user_token.ExpiryDate = expDate;
                user_token.ObjectState = ObjectState.Modified;
                _adm_user_tokenService.Update(user_token);
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Save;
                    objResponse.IsSuccess = true;

                    var Companyobj = _adm_user_companyService.Queryable()
                            .Where(e => e.UserID == user.ID).Include(x => x.adm_company).Select(x => new { x.CompanyID, x.adm_company.CompanyName, x.adm_company.DateFormatId, x.adm_company.IsCNICMandatory, x.adm_company.ReceiptFooter }).FirstOrDefault();

                    var CompanyID = Companyobj == null ? 0 : Companyobj.CompanyID;

                    var RoleIds = _adm_user_companyService.Queryable().Where(e => e.CompanyID == CompanyID && e.UserID == user.ID)
                       .Select(s => s.RoleID).ToArray();
                    var Rights = _adm_role_dtService.Queryable().Where(e => e.CompanyID == CompanyID && RoleIds.Contains(e.RoleID))
                          .Select(s => s.ScreenID).Distinct().ToList();
                    var ControlLevelRights = _adm_role_dtService.Queryable().Where(e => e.CompanyID == CompanyID && e.ViewRights && RoleIds.Contains(e.RoleID))
                                  .Select(s => new { s.ScreenID, s.ViewRights, s.DeleteRights, s.EditRights, s.CreateRights }).Distinct().ToList();
                    var Modules = _sys_drop_down_valueService.Queryable().Where(e => (e.CompanyID == null || e.CompanyID == CompanyID) && e.DropDownID == 7 && Rights.Contains(e.ID))
                            .Select(s => s.DependedDropDownValueID).Distinct().ToArray();
                    //var MultiKeyword = _adm_multilingual_mfservice.Queryable()
                    //                         .Where(e => e.MultilingualId == user.MultilingualId)
                    //                         .SelectMany(s => s.adm_multilingual_dt)
                    //                         .ToList();
                    var admCompany = _adm_companyService.Queryable().Where(a => a.ID == CompanyID).FirstOrDefault();

                    objResponse.ResultSet = new
                    {
                        User = user,
                        Token = user_token.TokenKey,
                        ValidTo = user_token.ExpiryDate,
                        PayrollRegion = System.Configuration.ConfigurationManager.AppSettings["PayrollRegion"],
                        Currency = System.Configuration.ConfigurationManager.AppSettings["Currency"],
                        ControlLevelRights = ControlLevelRights,
                        Modules = Modules,
                        Rights = Rights,
                        Company = Companyobj,
                        admCompany = admCompany,
                    };
                    objResponse.IsSuccess = true;
                }
                catch (DbUpdateException ex)
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
        private bool ModelExists(string key)
        {
            return _service.Query(e => e.ID.ToString() == key).Select().Any();
        }
        public static string ResolveEmailConfirm(string Name, string path, string TokenLink = "")
        {
            //  string pat = HttpContext.Current.Server.MapPath(path);
            string template = File.ReadAllText(path);
            string webUrl = System.Configuration.ConfigurationManager.AppSettings["WebUrl"];
            if (template.Contains("[[UserName]]"))
            {
                template = template.Replace("[[UserName]]", Name);
            }

            if (template.Contains("[[TokenLinkForShowTotalLink]]"))
            {
                template = template.Replace("[[TokenLinkForShowTotalLink]]", TokenLink);
            }
            if (template.Contains("[[tokenLink]]"))
            {
                template = template.Replace("[[tokenLink]]", TokenLink);
            }

            if (template.Contains("[[ResetLinkForShowTotalLink]]"))
            {
                template = template.Replace("[[ResetLinkForShowTotalLink]]", TokenLink);
            }
            if (template.Contains("[[ResetLink]]"))
            {
                template = template.Replace("[[ResetLink]]", TokenLink);
            }
            if (template.Contains("[[WebUrl]]"))
            {
                template = template.Replace("[[WebUrl]]", webUrl);
            }
            return template;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("api/User/ChangePassword")]
        public async Task<ResponseInfo> ChangePassword(adm_user_mf Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                adm_user_mf user = new adm_user_mf();
                try
                {
                    decimal id = Request.LoginID();
                    user = _service.Queryable().Where(e => e.ID == id && e.Pwd == Model.CurrentPassword).FirstOrDefault();

                    if (user != null)
                    {
                        user.Pwd = Model.Pwd;
                        user.ObjectState = ObjectState.Modified;
                        _service.Update(user);
                        try
                        {
                            await _unitOfWorkAsync.SaveChangesAsync();
                            objResponse.IsSuccess = true;
                            objResponse.Message = "Password changed successfully";
                        }
                        catch (DbUpdateException)
                        {
                            if (ModelExists(user.ID.ToString()))
                            {
                                objResponse.IsSuccess = false;
                                objResponse.ErrorMessage = MessageStatement.Conflict;
                                return objResponse;
                            }
                            throw;
                        }
                        objResponse.IsSuccess = true;
                    }
                    else
                    {
                        objResponse.Message = "PwdNotExist";
                        objResponse.ErrorMessage = "Current password is not correct.";
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
        [AllowAnonymous]
        [HttpPost]
        [Route("api/User/createPassword")]
        public async Task<ResponseInfo> createPassword(adm_user_mf Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                adm_user_mf user = new adm_user_mf();
                try
                {
                    decimal id = Request.LoginID();
                    user = _service.Queryable().Where(e => e.ID == id).FirstOrDefault();
                    if (user != null)
                    {
                        user.Pwd = Model.Pwd;
                        user.ObjectState = ObjectState.Modified;
                        _service.Update(user);
                        try
                        {
                            await _unitOfWorkAsync.SaveChangesAsync();
                            objResponse.IsSuccess = true;
                            objResponse.Message = "Password changed successfully";
                        }
                        catch (DbUpdateException)
                        {
                            if (ModelExists(user.ID.ToString()))
                            {
                                objResponse.IsSuccess = false;
                                objResponse.ErrorMessage = MessageStatement.Conflict;
                                return objResponse;
                            }
                            throw;
                        }
                        objResponse.IsSuccess = true;
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
    }
}
