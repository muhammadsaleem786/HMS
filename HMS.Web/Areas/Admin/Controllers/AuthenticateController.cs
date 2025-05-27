using HMS.Web.API.Common;
using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Service.Services.Admin;
using Repository.Pattern.UnitOfWork;
using System;
using System.Linq;
using System.Web.Http;

namespace HMS.Web.API.Areas.Admin.Controllers
{
    public class AuthenticateController : ApiController
    {
       // private readonly Isys_multilingual_mfService _adm_multilingual_mfservice;
        private readonly Iadm_userService _service;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        public AuthenticateController(IUnitOfWorkAsync unitOfWorkAsync, Iadm_userService Service
            )
        {
            _unitOfWorkAsync = unitOfWorkAsync; _service = Service;
            //_adm_multilingual_mfservice = adm_multilingual_mfservice;
        }
        [AllowAnonymous]
        [HttpPost]
        [ActionName("Login")]
        public ResponseInfo Login(adm_user_mf Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                adm_user_mf user = new adm_user_mf();
                try
                {
                    //var CompanyID = Request.CompanyID();
                    user = _service.Queryable().Where(e => e.Email == Model.Email && e.Pwd == Model.Pwd).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = ex.Message;
                    Logger.Trace.Error(ex);
                }

                if (user != null)
                {

                    // Generate Token
                    string PayrollRegion = System.Configuration.ConfigurationManager.AppSettings["PayrollRegion"].ToString();
                    var Token = JwtManager.GenerateToken(user.Email);
                    //var MultiKeyword = _adm_multilingual_mfservice.Queryable()
                    //       .Where(e => e.MultilingualId == user.MultilingualId)
                    //       .SelectMany(s => s.sys_multilingual_dt)
                    //       .ToList();

                    objResponse.ResultSet = new
                    {
                        User = user,
                        Token = Token.Split('|')[0],
                        ValidTo = Token.Split('|')[1],
                       // MultiKeyword = MultiKeyword,
                        PayrollRegion= PayrollRegion
                    };
                }
                else
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = "User Name or Password is incorrect.";
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
