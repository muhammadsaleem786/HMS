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
using System.Net;
using System.Web.Http.Results;
using static iTextSharp.text.pdf.AcroFields;

namespace HMS.Web.API.Controllers
{
    public class AttendanceSyncController : ApiController
    {
        private readonly Ipr_attendanceService _service;
        private readonly Ipr_time_logService _time_log;
        private readonly IStoredProcedureService _procedureService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        public AttendanceSyncController(IUnitOfWorkAsync unitOfWorkAsync, Ipr_attendanceService Service,
                      IStoredProcedureService procedureService, Ipr_time_logService time_log)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _service = Service;
            _procedureService = procedureService;
            _time_log = time_log;
        }

        [AllowAnonymous]
        [HttpGet]
        [ActionName("api/GetAllAttendanceMachine")]
        public IHttpActionResult GetAllAttendanceMachine([FromUri] decimal CompanyID,
        [FromUri] string Tokenkey, [FromUri] string LocationCode)
        {
            if (Tokenkey != "32eqHhxJvsW0d2NPl9a4")
                return Unauthorized();
            var result = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.LocationCode == LocationCode && e.IsActive).Select(z => new Attendance
            {
                CompanyId = z.CompanyId,
                LocationCode = z.LocationCode,
                IPAddress = z.IPAddress,
                Password = z.Password,
                PortNo = z.PortNo,
                LastDataSync = z.LastDataSync
            }).ToList();
            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost]
        [ActionName("api/DatabaseSyncProcess")]
        public async Task<IHttpActionResult> DatabaseSyncProcess([FromUri] decimal CompanyID,
        [FromUri] string LocationCode,
        [FromUri] string Tokenkey,
        [FromBody] List<AttendanceModel> attendanceList)
        {
            try
            {
                if (attendanceList == null || !attendanceList.Any())
                    return BadRequest("No data received.");
                if (Tokenkey != "32eqHhxJvsW0d2NPl9a4")
                    return Unauthorized();
                foreach (var attendance in attendanceList)
                {
                    bool exists = _time_log.Queryable().Any(x =>
                                    x.CompanyId == CompanyID &&
                                    x.EmployeeCode == attendance.EmployeeCode &&
                                    x.AttendanceTime == attendance.AttendanceTime);
                    if (exists)
                        continue;
                    pr_time_log obj = new pr_time_log
                    {
                        CompanyId = CompanyID,
                        AttendanceTime = attendance.AttendanceTime,
                        AttendanceMode = attendance.AttendanceMode,
                        EmployeeCode = attendance.EmployeeCode,
                        CreatedDate = DateTime.Now,
                        IPAddress = "",
                        Remarks = "",
                        LocationCode = LocationCode
                    };
                    _time_log.Insert(obj);
                }

                var deviceUpdate = _service.Queryable().Where(e => e.CompanyId == CompanyID && e.LocationCode == LocationCode && e.IsActive).FirstOrDefault();
                if (deviceUpdate != null)
                {
                    deviceUpdate.LastDataSync = DateTime.Now.ToString();
                    _service.Update(deviceUpdate);
                }
                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                    throw;
                }

                return Ok(new
                {
                    IsSuccess = true,
                    Message = "Attendance data received successfully."
                });
            }
            catch (Exception ex)
            {
                // Log the error
                return InternalServerError(ex);
            }
        }
        public class AttendanceModel
        {
            public decimal CompanyID { get; set; }
            public string LocationCode { get; set; }
            public string EmployeeCode { get; set; }
            public Int16 AttendanceMode { get; set; }
            public DateTime AttendanceTime { get; set; }
        }
        public class Attendance
        {
            public decimal CompanyId { get; set; }
            public string LocationCode { get; set; }
            public string IPAddress { get; set; }
            public string Password { get; set; }
            public int PortNo { get; set; }
            public string LastDataSync { get; set; }
        }
    }
}
