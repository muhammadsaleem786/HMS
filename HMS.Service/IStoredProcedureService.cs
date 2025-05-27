#region
using System.Collections.Generic;
using HMS.Entities.CustomModel;

#endregion

namespace HMS.Service
{
    public interface IStoredProcedureService
    {
        IEnumerable<ScreenModel> GetAllScreen();
        IEnumerable<TemplateModel> GetAlLTemplate(decimal CompanyID);
        int SP_PR_CalculateSalary(decimal CompanyID, decimal PayScheduleID, string EmployeeIds, double LoginID);
        IEnumerable<decimal> SP_GetOpenPayrollPayScheduleIds(decimal CompanyID);
    }
}