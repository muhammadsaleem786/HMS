using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
using HMS.Service;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Appointment;
using HMS.Service.Services.Employee;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Util;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace HMS.Web.API.Controllers
{
    [JwtAuthentication]
    public class DeepAIController : ApiController
    {
        private readonly IStoredProcedureService _procedureService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IDeepSeekService _deepSeekService;
        public DeepAIController(IUnitOfWorkAsync unitOfWorkAsync, 
            IStoredProcedureService procedureService, IDeepSeekService deepSeekService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _procedureService = procedureService;
            _deepSeekService = deepSeekService;
        }

        [HttpPost]
        [ActionName("chat")]
        [Route("api/deepai/chat")]
        public async Task<ResponseInfo> Chat(DeepSeekRequest request)
        {
            var objResponse = new ResponseInfo();

            try
            {

                var result = await _deepSeekService.GeneratePrescription(request);

                objResponse.IsSuccess = true;

                objResponse.ResultSet = new
                {
                    Response = result
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
}
