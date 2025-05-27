using HMS.Entities.CustomModel;
using HMS.Entities.Models;
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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace HMS.Web.API.Areas.Employee
{
    [JwtAuthentication]
    public class pr_leave_applicationController : ApiController, IERPAPIInterface<pr_leave_application>, IDisposable
    {
        private readonly Ipr_leave_applicationService _service;

        private readonly Ipr_leave_typeService _pr_leave_typeService;
        private readonly Ipr_employee_mfService _pr_employee_mfService;
        private readonly Ipr_employee_leaveService _pr_employee_leaveService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        public pr_leave_applicationController(IUnitOfWorkAsync unitOfWorkAsync, Ipr_leave_applicationService Service, Ipr_leave_typeService pr_leave_typeService
            , Ipr_employee_mfService pr_employee_mfService, Ipr_employee_leaveService pr_employee_leaveService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = Service;
            _pr_leave_typeService = pr_leave_typeService;
            _pr_employee_mfService = pr_employee_mfService;
            _pr_employee_leaveService = pr_employee_leaveService;
        }

        public async Task<ResponseInfo> Save(pr_leave_application Model)
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

                decimal ID = 1;
                if (_service.Queryable().Count() > 0)
                    ID = _service.Queryable().Max(e => e.ID) + 1;

                Model.ID = ID;
                Model.CompanyID = Request.CompanyID();
                Model.CreatedBy = Request.LoginID();
                Model.CreatedDate = Request.DateTimes();
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();
                Model.ObjectState = ObjectState.Added;
                _service.Insert(Model);

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



        public ResponseInfo GetList()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal companyID = Request.CompanyID();
                objResponse.ResultSet = _service.Queryable().Where(x => x.CompanyID == companyID).ToList();
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        public ResponseInfo GetLeaveTypes()
        {
            var objResponse = new ResponseInfo();
            try
            {
                objResponse.ResultSet = _pr_leave_typeService.Queryable().Select(x => new { x.ID, x.TypeName }).ToList();
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        public ResponseInfo GetFilterEmployees(string Keyword)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResponse.ResultSet = _pr_employee_mfService.Queryable().Where(x => x.CompanyID == CompanyID && x.StatusID == 1 && string.Concat(x.FirstName.ToLower(), " ", x.LastName.ToLower()).Contains(Keyword.ToLower()) /*&& x.pr_employee_leave.Any(a => a.CompanyID == x.CompanyID && a.EmployeeID == x.ID)*/)
                    .Select(s => new
                    {
                        s.ID,
                        s.FirstName,
                        s.LastName,
                    }).ToList();
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }

        public ResponseInfo GetLeavesByTypes(string Keyword)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResponse.ResultSet = _pr_leave_typeService.Queryable().Where(x => x.CompanyID == CompanyID && x.Category.ToLower().Contains(Keyword.ToLower())).ToList();
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
        [ActionName("GetLeavesByEmpID")]
        public ResponseInfo GetLeavesByEmpID(string EmpID)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var obj = this.GetLeaves(EmpID, "");
                objResponse.ResultSet = obj;
                //objResponse.ResultSet = _pr_leave_typeService.Queryable().Where(x => x.CompanyID == CompanyID && x.Category.ToLower().Contains(Keyword.ToLower())).ToList();
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }

        public object GetLeaves(string EmpID, string ID)
        {
            decimal CompanyID = Request.CompanyID();
            decimal EID = Convert.ToDecimal(EmpID);
            decimal Id = ID == "" ? 0 : Convert.ToDecimal(ID);
            int date = DateTime.Now.Year;
            var TakenLeavesByEmp = _service.Queryable().Where(x => x.CompanyID == CompanyID && x.EmployeeID == EID && (Id == 0 || x.ID != Id))
                .Include(x => x.pr_leave_type)
                .Select(x => new
                {
                    EmpID = x.EmployeeID,
                    Hours = x.Hours,
                    LeaveTypeID = x.LeaveTypeID,
                    x.pr_leave_type.Category,
                }).ToList();

            var TotalLeaves = _pr_employee_leaveService.Queryable()
                .Where(x => x.CompanyID == CompanyID && x.EmployeeID == EID)
                 .Include(x => x.pr_leave_type)
                 .Select(x => new
                 {
                     EmpID = x.EmployeeID,
                     LeaveTypeID = x.LeaveTypeID,
                     TotalHours = x.Hours,
                     Category = x.pr_leave_type.Category
                 })
                .ToList();

            var obj = new
            {
                TakenLeavesByEmp = TakenLeavesByEmp,
                TotalLeavesOfEmp = TotalLeaves
            };

            return obj;
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
                var LeaveObj = _service.Queryable().Where(e => e.CompanyID == CompanyID && e.ID.ToString() == Id).Include(x => x.pr_employee_mf)
                    .Include(x => x.pr_leave_type)
                    .Select(x => new
                    {
                        EmpLeave = x,
                        pr_employee_mf = new
                        {
                            ID = x.pr_employee_mf.ID,
                            x.pr_employee_mf.FirstName,
                            x.pr_employee_mf.LastName
                        },
                        pr_leave_type = new
                        {
                            x.pr_leave_type.ID,
                            x.pr_leave_type.Category
                        }
                    })
                    .FirstOrDefault();
                objResponse.ResultSet = new
                {
                    LeaveObj = LeaveObj,
                    TotalAndTakenLeaves = GetLeaves(LeaveObj.pr_employee_mf.ID.ToString(), LeaveObj.EmpLeave.ID.ToString())
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
                objResponse.ResultSet = _service.Queryable().Where(e => e.CompanyID == CompanyID && e.ID.ToString() == Id).FirstOrDefault();
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
        public async Task<ResponseInfo> Update(pr_leave_application Model)
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

                Model.pr_leave_type = null;
                Model.pr_employee_mf = null;
                Model.ModifiedBy = Request.LoginID();
                Model.ModifiedDate = Request.DateTimes();

                Model.ObjectState = ObjectState.Modified;

                _service.Update(Model);

                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
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

                List<pr_leave_application> Models = _service.Queryable().Where(e => e.CompanyID == CompanyID && IdList.Contains(e.ID)).ToList();

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



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWorkAsync.Dispose();
            }
            base.Dispose(disposing);
        }

        public PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            throw new NotImplementedException();
        }
    }
}
