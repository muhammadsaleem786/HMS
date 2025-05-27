using System.Collections.Generic;
using System;
using HMS.Entities.Models;
using HMS.Entities.CustomModel;

namespace HMS.Service
{
    public class StoredProcedureService : IStoredProcedureService
    {
        private readonly IERPStoredProcedures _storedProcedures;

        public StoredProcedureService(IERPStoredProcedures storedProcedures)
        {
            _storedProcedures = storedProcedures;
        }
        public IEnumerable<ScreenModel> GetAllScreen()
        {
            return _storedProcedures.GetAllScreen();
        }
        public IEnumerable<TemplateModel> GetAlLTemplate(decimal CompanyID)
        {
            return _storedProcedures.GetAlLTemplate(CompanyID);
        }
        public int SP_PR_CalculateSalary(decimal CompanyID, decimal PayScheduleID, string EmployeeIds, double LoginID)
        {
            return _storedProcedures.SP_PR_CalculateSalary(CompanyID, PayScheduleID, EmployeeIds, LoginID);
        }
        public IEnumerable<decimal> SP_GetOpenPayrollPayScheduleIds(decimal CompanyID)
        {
            return _storedProcedures.SP_GetOpenPayrollPayScheduleIds(CompanyID);
        }
    }
}