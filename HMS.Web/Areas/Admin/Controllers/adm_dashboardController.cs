using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using HMS.Entities.CustomModel;
using HMS.Service;
using HMS.Service.Services.Admin;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using HMS.Entities.Models;
using System.Collections;

namespace HMS.Web.API.Areas.Admin.Controllers
{
    [JwtAuthentication]
    public class adm_dashboardController : ApiController, IDisposable
    {
        private readonly Iadm_companyService _adm_companyService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IStoredProcedureService _procedureService;
        public adm_dashboardController(IUnitOfWorkAsync unitOfWorkAsync,
             Iadm_companyService adm_companyService,
           IStoredProcedureService ProcedureService
            )
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _adm_companyService = adm_companyService;
            _procedureService = ProcedureService;
        }
        [HttpGet]
        [ActionName("UpdateNotificationViewedStatus")]
        public async Task<ResponseInfo> UpdateNotificationViewedStatus(string Ids)
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
                var IdsArr = Ids.Split('#').Select(decimal.Parse).ToArray();
                //List<adm_notification_alert> list = _adm_notification_alertService.Queryable().Where(x => x.CompanyID == CompanyID && x.TypeID == 2 && IdsArr.Contains(x.ID)).ToList();
                //foreach (var item in list)
                //{
                //    item.IsRead = true;
                //    item.ObjectState = ObjectState.Modified;
                //    _adm_notification_alertService.Update(item);
                //}

                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;

                }
                catch (DbUpdateException)
                {
                    //if (!ModelExists(Model.ID.ToString()))
                    //{
                    //    objResponse.IsSuccess = false;
                    //    objResponse.ErrorMessage = MessageStatement.NotFound;
                    //    return objResponse;
                    //}
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
        [ActionName("GetAdmNotifications")]
        public ResponseInfo GetAdmNotifications()
        {
            var objResponse = new ResponseInfo();
            try
            {
                objResponse.ResultSet = GetNotifications();
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        public List<object> GetNotifications()
        {
            var CompanyID = Request.CompanyID();
            List<object> li = new List<object>();
            //return _adm_notification_alertService.Queryable().Where(x => x.CompanyID == CompanyID && x.TypeID == 2)
            //         .Take(100).Include(x => x.pr_employee_mf)
            //         .Select(x => new { x.ID, x.Subject, x.Body, x.IsRead, x.CreatedDate, EmpID = x.UserID, EmpName = x.pr_employee_mf.FirstName + " " + x.pr_employee_mf.LastName, Dept = x.pr_employee_mf.pr_department.DepartmentName, Desig = x.pr_employee_mf.pr_designation.DesignationName })
            //         .OrderByDescending(x => x.CreatedDate).ToList<object>();
            return li;
        }
        [HttpGet]
        [ActionName("Pagination")]
        public PaginationResult Pagination(string filterParams, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                string PayrollRegion = WebConfigurationManager.AppSettings["PayrollRegion"];
                decimal CompanyID = Request.CompanyID();

                //var leaves = _pr_leave_typeService.Queryable().Where(x => x.CompanyID == CompanyID && (x.Category == "V" || x.Category == "S")).ToList();
                //DashbrdIdsmodel.VacationIds = leaves.Where(x => x.Category == "V").Select(x => x.ID).ToArray();
                //DashbrdIdsmodel.SickIds = leaves.Where(x => x.Category == "S").Select(x => x.ID).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }
        [HttpGet]
        [ActionName("Load")]
        public ResponseInfo Load()
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();





                objResponse.ResultSet = new
                {
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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWorkAsync.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpGet]
        [ActionName("DataLoad")]
        public ResponseInfo DataLoad(string FromDate, string ToDate)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                int userid = Convert.ToInt32(Request.LoginID());
                DataAccessManager dataAccessManager = new DataAccessManager();
                var ht = new Hashtable();
                ht.Add("@CompanyId", CompanyID);
                ht.Add("@FromeDate",Convert.ToDateTime(FromDate));
                ht.Add("@ToDate", Convert.ToDateTime(ToDate));
                ht.Add("@UserId", userid);
                
                var result= dataAccessManager.GetDataSet("SP_Dashboard", ht);
                var Deshboard = result.Tables[0];
                var Appointment = result.Tables[1];
                var BirthDay = result.Tables[2];
                var FollowUp = result.Tables[3];
                var IncomeAndExpense = result.Tables[4];
                objResponse.ResultSet = new
                {
                    DeshboardData = Deshboard,
                    Appointment = Appointment,
                    BirthDay = BirthDay,
                    FollowUp = FollowUp,
                    IncomeAndExpense= IncomeAndExpense,
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
        [ActionName("Refresh")]
        public ResponseInfo Refresh()
        {
            var objResponse = new ResponseInfo();           
            return objResponse;
        }

        
    }
}
