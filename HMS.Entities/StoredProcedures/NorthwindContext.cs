#region

using HMS.Entities.CustomModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;

#endregion

namespace HMS.Entities.Models
{
    public partial class HMSContext : IERPStoredProcedures
    {
        public IEnumerable<ScreenModel> GetAllScreen()
        {
            return Database.SqlQuery<ScreenModel>("SP_Adm_GetAllScreen");
        }
        public IEnumerable<TemplateModel> GetAlLTemplate(decimal CompanyID)
        {
            var CompanyIDParameter = new SqlParameter("@CompanyId", CompanyID);
            return Database.SqlQuery<TemplateModel>("LoadTemplate @CompanyId", CompanyIDParameter);
        }
        public int SP_PR_CalculateSalary(decimal CompanyID, decimal PayScheduleID, string EmployeeIds, double LoginID)
        {
            var CompanyIDParameter = new SqlParameter("@CompanyID", CompanyID);
            var PayScheduleIDParameter = new SqlParameter("@PayScheduleID", PayScheduleID);
            var EmployeeIdsParameter = new SqlParameter("@EmployeeIds", EmployeeIds);
            var LoginIDParameter = new SqlParameter("@LoginID", LoginID);


            return Database.ExecuteSqlCommand("SP_PR_CalculateSalary @CompanyID,@PayScheduleID,@EmployeeIds,@LoginID", CompanyIDParameter, PayScheduleIDParameter, EmployeeIdsParameter, LoginIDParameter);

        }
        public IEnumerable<int> SP_PR_CalculateDashboardSalary(decimal CompanyID, DateTime PeriodStart, DateTime PeriodEnd, decimal PayScheduleID, decimal LocationID, decimal DepartmentID, double LoginID)
        {
            var CompanyIDParameter = new SqlParameter("@CompanyID", CompanyID);
            var LoginIDParameter = new SqlParameter("@LoginID", LoginID);
            //var RangeParameter = new SqlParameter("@Range", Range);
            var PeriodStartParameter = new SqlParameter("@PeriodStart", PeriodStart);
            var PeriodEndParameter = new SqlParameter("@PeriodEnd", PeriodEnd);
            var PayScheduleIDParameter = new SqlParameter("@PayScheduleID", PayScheduleID);
            var LocationIDParameter = new SqlParameter("@LocationID", LocationID);
            var DepartmentIDParameter = new SqlParameter("@DepartmentID", DepartmentID);

            return Database.SqlQuery<int>("SP_PR_CalculateSalary @CompanyID,@PeriodStart,@PeriodEnd,@PayScheduleID,@LocationID,@DepartmentID", CompanyIDParameter, PayScheduleIDParameter, LoginIDParameter);
        }

        public IEnumerable<decimal> SP_GetOpenPayrollPayScheduleIds(decimal CompanyID)
        {
            var CompanyIDParameter = new SqlParameter("@CompanyID", CompanyID);
            return Database.SqlQuery<decimal>("SP_GetOpenPayrollPayScheduleIds @CompanyID", CompanyIDParameter);
        }
    }
}