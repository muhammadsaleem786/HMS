using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Employee;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using HMS.Web.API.Interface;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace HMS.Web.API.Areas.Employee
{
    [JwtAuthentication]
    public class pr_time_entryController : ApiController, IERPAPIInterface<pr_time_entry>, IDisposable
    {
        private readonly Ipr_time_entryService _service;
        private readonly Iadm_companyService _adm_companyService;
        private readonly Ipr_employee_mfService _pr_employee_mfService;
        private readonly Isys_holidaysService _sys_holidays;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly Iadm_userService _adm_userService;
        private readonly Iadm_user_companyService _adm_user_companyService;

        public pr_time_entryController(IUnitOfWorkAsync unitOfWorkAsync, Iadm_user_companyService adm_user_companyService, Iadm_userService adm_userService, Ipr_time_entryService Service
            , Iadm_companyService adm_companyService, Isys_holidaysService sys_holidays, Ipr_employee_mfService pr_employee_mfService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = Service;
            _adm_companyService = adm_companyService;
            _pr_employee_mfService = pr_employee_mfService;
            _sys_holidays = sys_holidays;
            _adm_userService = adm_userService;
            _adm_user_companyService = adm_user_companyService;
        }
        public async Task<ResponseInfo> Save(pr_time_entry Model)
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
                //pr_time_entry dept = _service.Queryable()
                //    .Where(x => x.CompanyID == CompanyID && x.EmployeeID == Model.EmployeeID && x.CreatedDate.Date == Model.CreatedDate.Date)
                //    .FirstOrDefault();
                //if (dept == null)
                //{
                if (Model.TimIntxt != null)
                {
                    int TimeInhh = Convert.ToInt16(Model.TimIntxt.Split(':')[0]);
                    int TimeInmm = Convert.ToInt16(Model.TimIntxt.Split(':')[1]);
                    TimeSpan tis = new TimeSpan(TimeInhh, TimeInmm, 0);
                    Model.TimeIn = Model.TimeIn?.Date + tis;
                }
                else
                    Model.TimeIn = null;
                if (Model.TimOuttxt != null)
                {
                    int TimeOuthh = Convert.ToInt16(Model.TimOuttxt.Split(':')[0]);
                    int TimeOutmm = Convert.ToInt16(Model.TimOuttxt.Split(':')[1]);
                    TimeSpan tos = new TimeSpan(TimeOuthh, TimeOutmm, 0);
                    Model.TimeOut = Model.TimeOut?.Date + tos;
                }
                else
                    Model.TimeOut = null;
                decimal ID = 1;
                if (_service.Queryable().Count() > 0)
                    ID = (_service.Queryable().Where(e => e.CompanyID == CompanyID).Max(x => (decimal?)x.ID) ?? 0) + 1;
                Model.ID = ID;
                Model.AttendanceDate = Model.AttendanceDate;
                Model.CompanyID = CompanyID;
                Model.CreatedBy = UserID;
                Model.CreatedDate = Request.DateTimes();
                Model.ModifiedBy = UserID;
                Model.ModifiedDate = Request.DateTimes();
                Model.StatusDropDownID = (int)sys_dropdown_mfEnum.TimeStatus;
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
                //}
                //else
                //{
                //    objResponse.IsSuccess = false;
                //    objResponse.ErrorMessage = Model.DepartmentName + " already exist. Please enter a different name and try again.";
                //}

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
                objResponse.ResultSet = _service.Queryable()
                    .Where(e => e.CompanyID == CompanyID && e.ID.ToString() == Id)
                    .Include(x => x.pr_employee_mf).Include(x => x.sys_drop_down_value)
                    .Select(x => new
                    {
                        model = x,
                        EmpName = x.pr_employee_mf.FirstName + " " + x.pr_employee_mf.LastName,
                    }).FirstOrDefault();
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
        [ActionName("TimePagination")]
        public PaginationResult TimePagination(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResult = _service.GetTimePaginationList(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
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
        public async Task<ResponseInfo> Update(pr_time_entry Model)
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

                int TimeInhh = Convert.ToInt16(Model.TimIntxt.Split(':')[0]);
                int TimeInmm = Convert.ToInt16(Model.TimIntxt.Split(':')[1]);
                int TimeOuthh = Convert.ToInt16(Model.TimOuttxt.Split(':')[0]);
                int TimeOutmm = Convert.ToInt16(Model.TimOuttxt.Split(':')[1]);

                TimeSpan tis = new TimeSpan(TimeInhh, TimeInmm, 0);
                TimeSpan tos = new TimeSpan(TimeOuthh, TimeOutmm, 0);
                Model.TimeIn = Model.TimeIn?.Date + tis;
                Model.TimeOut = Model.TimeOut?.Date + tos;


                decimal CompanyID = Request.CompanyID();
                //pr_time_entry dept = _service.Queryable().Where(x => x.CompanyID == CompanyID && x.ID != Model.ID && x.ID != Model.ID && x.DepartmentName.ToLower() == Model.DepartmentName.ToLower()).FirstOrDefault();
                //if (dept == null)
                //{
                Model.pr_employee_mf = null;
                Model.sys_drop_down_value = null;
                Model.StatusDropDownID = (int)sys_dropdown_mfEnum.TimeStatus;
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
                //}
                //else
                //{
                //    objResponse.IsSuccess = false;
                //    objResponse.ErrorMessage = Model.DepartmentName + " already exist. Please enter a different name and try again.";
                //}

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
                //decimal[] IdList = Id.Split(',').Select(decimal.Parse).ToArray();
                //int[] IdList = Id.Split(',').Select(int.Parse).ToArray();
                decimal ID = Convert.ToDecimal(Id);
                pr_time_entry model = _service.Queryable().Where(e => e.CompanyID == CompanyID && e.ID == ID)
                    .FirstOrDefault();

                if (model == null)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
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


        [HttpGet]
        [ActionName("GetTimeAttendanceList")]
        public ResponseInfo GetTimeAttendanceList()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();

                // Active Status = 1;
                var datalist = _pr_employee_mfService.Queryable().Where(x => x.CompanyID == CompanyID && x.StatusID == 1)
                    .Include(x => x.pr_time_entry).Include(x => x.pr_time_entry.Select(t => t.sys_drop_down_value))
                     .Select(z => new
                     {
                         EmployeeID = z.ID,
                         Employee = z.FirstName + " " + z.LastName,
                         JoiningDate = z.JoiningDate,
                         z.WDSunday,
                         z.WDMonday,
                         z.WDTuesday,
                         z.WDWednesday,
                         z.WDThursday,
                         z.WDFriday,
                         z.WDSaturday,
                         pr_time = z.pr_time_entry.Select(d => new { ID = d.ID, d.TimeIn, d.TimeOut, Status = d.sys_drop_down_value.Value, z.StatusID }).ToList(),
                     }).OrderBy(d => d.Employee).ToList();
                var TimeAttList = datalist.Select(z => new
                {
                    z.EmployeeID,
                    z.Employee,
                    z.JoiningDate,z.WDSunday,z.WDMonday,z.WDTuesday,z.WDWednesday,z.WDThursday,z.WDFriday,z.WDSaturday,
                }).Distinct().Select(s => new
                {
                    s.EmployeeID,
                    s.Employee,
                    s.JoiningDate,
                    s.WDSunday,
                    s.WDMonday,
                    s.WDTuesday,
                    s.WDWednesday,
                    s.WDThursday,
                    s.WDFriday,
                    s.WDSaturday,
                    TimeAttendanceListdt = datalist.Where(d => d.EmployeeID == s.EmployeeID).SelectMany(t => t.pr_time).ToList<object>()
                }).ToList<object>();
                var result = GetWorkingDaysAndHolidays();
                objResponse.ResultSet = new { TimeAttList = TimeAttList, result = result };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }

        public Object GetWorkingDaysAndHolidays()
        {
            var objResponse = new ResponseInfo();
            decimal CompanyID = Request.CompanyID();
            var PublicHolidays = _sys_holidays.Queryable().Where(x => x.CompanyID == CompanyID)
.Select(t => new
{
t.ID,
t.HolidayName,
t.FromDate,
t.ToDate
}).ToList();

            objResponse.ResultSet = new
            {
                PublicHolidays = PublicHolidays
            };

            return objResponse.ResultSet;
        }

        [HttpGet]
        [ActionName("GetTimeAttendanceByEmployeeID")]
        public ResponseInfo GetTimeAttendanceByEmployeeID(string EmpID)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal EID = Convert.ToDecimal(EmpID);
                var datalist = _service.Queryable().Where(x => x.CompanyID == CompanyID && x.EmployeeID == EID)
                   .Include(x => x.pr_employee_mf).Include(x => x.pr_employee_mf.pr_designation).Include(x => x.sys_drop_down_value)
                   .Select(z => new
                   {
                       z.EmployeeID,
                       Employee = z.pr_employee_mf.FirstName + " " + z.pr_employee_mf.LastName,
                       Designation = z.pr_employee_mf.pr_designation.DesignationName,
                       z.ID,
                       z.TimeIn,
                       z.TimeOut,
                       Status = z.sys_drop_down_value.Value,
                       z.pr_employee_mf.StatusID,
                       z.pr_employee_mf.WDSunday,
                       z.pr_employee_mf.WDMonday,
                       z.pr_employee_mf.WDTuesday,
                       z.pr_employee_mf.WDWednesday,
                       z.pr_employee_mf.WDThursday,
                       z.pr_employee_mf.WDFriday,
                       z.pr_employee_mf.WDSaturday,
                   }).ToList()
                   .Select(s => new
                   {
                       s.ID,
                       s.EmployeeID,
                       s.Employee,
                       s.Designation,
                       InTime = s.TimeIn,
                       Date = s.TimeIn?.ToString("dd/MM/yyyy") ?? "",
                       TimeIn = s.TimeIn?.ToString("hh:mm tt") ?? "",
                       TimeOut = s.TimeOut?.ToString("hh:mm tt") ?? "",
                       WorkingHours = WorkingHours("W", s.TimeIn, s.TimeOut),
                       DelayTime = WorkingHours("D", s.TimeIn, s.TimeOut),
                       ExcusedTime = WorkingHours("E", s.TimeIn, s.TimeOut),
                       s.StatusID,
                       s.Status,
                       s.WDSunday,
                       s.WDMonday,
                       s.WDTuesday,
                       s.WDWednesday,
                       s.WDThursday,
                       s.WDFriday,
                       s.WDSaturday,
                   })
                .ToList()
                .OrderBy(t => t.InTime).ToList();
                var obj = datalist.Select(x => new
                {
                    x.EmployeeID,
                    x.Employee,
                    x.Designation,
                    x.WDSunday,
                    x.WDMonday,
                    x.WDTuesday,
                    x.WDWednesday,
                    x.WDThursday,
                    x.WDFriday,
                    x.WDSaturday,
                }).Distinct().Select(z => new
                {
                    z.EmployeeID,
                    z.Employee,
                    z.Designation,
                    z.WDSunday,
                    z.WDMonday,
                    z.WDTuesday,
                    z.WDWednesday,
                    z.WDThursday,
                    z.WDFriday,
                    z.WDSaturday,
                    TotalWorkingHours = datalist.Aggregate(TimeSpan.Zero, (sumSoFar, nextMyObject) => sumSoFar + nextMyObject.WorkingHours ?? TimeSpan.Zero).ToString(@"hh\:mm"),
                    TotalDelayHours = datalist.Aggregate(TimeSpan.Zero, (sumSoFar, nextMyObject) => sumSoFar + nextMyObject.DelayTime ?? TimeSpan.Zero).ToString(@"hh\:mm"),
                    TotalExcusedHours = datalist.Aggregate(TimeSpan.Zero, (sumSoFar, nextMyObject) => sumSoFar + nextMyObject.ExcusedTime ?? TimeSpan.Zero).ToString(@"hh\:mm"),
                    AtttimeListDet = datalist.Select(t => new
                    {
                        t.ID,
                        t.Date,
                        t.TimeIn,
                        t.TimeOut,
                        WorkingHours = t.WorkingHours?.ToString(@"hh\:mm") ?? "0",
                        DelayTime = t.DelayTime?.ToString(@"hh\:mm") ?? "0",
                        ExcusedTime = t.ExcusedTime?.ToString(@"hh\:mm") ?? "0",
                        t.StatusID,
                        t.Status,
                    }).ToList(),

                }).FirstOrDefault();




                objResponse.ResultSet = new { obj = obj, result = GetWorkingDaysAndHolidays() };

            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }



        public TimeSpan? WorkingHours(string HoursType, DateTime? TimeIn, DateTime? TimeOut)
        {
            TimeSpan CompanyWorkingHours = new TimeSpan(9, 0, 0);
            TimeSpan CompanyStartingTime = new TimeSpan(9, 0, 0);
            TimeSpan CompanyEndingTime = new TimeSpan(18, 0, 0);
            if (HoursType == "W")
                if (TimeIn.HasValue && TimeOut.HasValue)
                    return TimeOut.Value - TimeIn.Value;
                else
                    return null;
            else if (HoursType == "D")
                if (TimeIn.HasValue)
                {
                    TimeSpan TimCheckIn = new TimeSpan(TimeIn.Value.Hour, TimeIn.Value.Minute, 0);
                    return TimCheckIn - CompanyWorkingHours;
                }
                else
                    return null;
            else
                 if (TimeOut.HasValue)
            {
                TimeSpan TimCheckout = new TimeSpan(TimeOut.Value.Hour, TimeOut.Value.Minute, 0);
                return CompanyEndingTime - TimCheckout;
            }
            else
                return null;

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
