using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Service.Services.Admin;
using HMS.Web.API.Common;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ModelBinding;

namespace HMS.Web.API.Controllers
{
    [EnableCors(origins: "https://easyhms.com", headers: "*", methods: "*")]
    public class ContactController : ApiController
    {
        DataAccessManager dataAccessManager = new DataAccessManager();
        [AllowAnonymous]
        [HttpPost]
        public async Task<ResponseInfo> Save(contact Model)
        {
            var objResponse = new ResponseInfo();
            var WebToken = System.Configuration.ConfigurationManager.AppSettings["WebToken"];
            var token = Request.Headers.Authorization;
            if (token.ToString() == WebToken)
            {
                try
                {
                    Hashtable ht = new Hashtable();
                    ht.Add("@Name", Model.Name);
                    ht.Add("@Phone", Model.Phone);
                    ht.Add("@Email", Model.Email);
                    ht.Add("@Speciality", Model.Speciality);
                    ht.Add("@Message", Model.Message);
                    try
                    {
                        dataAccessManager.ExecuteNonQuery("SP_InsertContact", ht);
                        objResponse.Message = MessageStatement.Save;
                        objResponse.IsSuccess = true;
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
            }
            return objResponse;
        }
    }
}
