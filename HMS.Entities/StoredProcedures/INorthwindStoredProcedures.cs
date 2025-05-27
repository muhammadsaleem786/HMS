using HMS.Entities.CustomModel;
using System;
using System.Collections.Generic;

namespace HMS.Entities.Models
{
    public interface IERPStoredProcedures
    {
        IEnumerable<ScreenModel> GetAllScreen();
        IEnumerable<TemplateModel> GetAlLTemplate(decimal CompanyID);
        int SP_PR_CalculateSalary(decimal CompanyID, decimal PayScheduleID, string EmployeeIds, double LoginID);
        IEnumerable<int> SP_PR_CalculateDashboardSalary(decimal CompanyID, DateTime PeriodStart, DateTime PeriodEnd, decimal PayScheduleID, decimal LocationID, decimal DepartmentId, double LoginID);
        IEnumerable<decimal> SP_GetOpenPayrollPayScheduleIds(decimal CompanyID);
    }
}