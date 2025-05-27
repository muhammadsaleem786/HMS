using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Common;
using Repository.Pattern.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Configuration;

namespace HMS.Repository.Repositories.Employee
{
    public static class pr_employee_payroll_mfRepository
    {

        public static PaginationResult Pagination(this IRepository<pr_employee_payroll_mf> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pr_employee_payroll_mf, bool>> predicate = (e => e.CompanyId == CompanyID);

                bool DisplayName, DisplayStatus, DisplayPayPeriod, DisplayPayDate, DisplayEmployees, DisplayTax, DisplayNetSalary;
                bool OrderByName, OrderByStatus, OrderByPayPeriod, OrderByPayDate, OrderByEmployees, OrderByTax, OrderByNetSalary;

                IQueryable<pr_employee_payroll_mf> filteredData = repository.Queryable().Where(predicate);
                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";


                var dataList = filteredData.Include(I => I.pr_employee_payroll_dt).Select(x => new
                {
                    PayScheduleID = x.PayScheduleID,
                    PayScheduleName = x.pr_pay_schedule.ScheduleName,
                    x.Status,
                    x.PayDate,
                    x.PayScheduleFromDate,
                    x.PayScheduleToDate,
                    EmpID = x.EmployeeID,
                    BasicSalary = x.BasicSalary + (x.AdjustmentBy == null ? 0 : (x.AdjustmentType == "C" ? x.AdjustmentAmount ?? 0 : (x.AdjustmentAmount ?? 0) * -1)),
                    Payrolldetail = x.pr_employee_payroll_dt.Select(y => new
                    {
                        x.PayScheduleID,
                        x.PayScheduleFromDate,
                        x.PayScheduleToDate,
                        y.EmployeeID,
                        y.Type,
                        y.AllowDedID,
                        Amount = y.Amount + (y.AdjustmentBy == null ? 0 : (y.AdjustmentType == "C" ? (y.AdjustmentAmount ?? 0) : (y.AdjustmentAmount ?? 0) * -1)),
                        y.Taxable
                    }).ToList(),
                }).ToList();

                var Ids = repository.Queryable().Where(x => x.CompanyId == CompanyID && x.Status == "O")
                         .Select(x => x.PayScheduleID).Distinct().ToList();

                //var PayrollDtl = dataList.SelectMany(s => s.Payrolldetail).ToList();

                List<PayrollMasterPaginationSearchSortModel> list = new List<PayrollMasterPaginationSearchSortModel>();
                if (IgnorePaging)
                {
                    list = (from s in dataList
                            group s by new { s.PayScheduleID, s.PayScheduleName, s.Status, s.PayDate, s.PayScheduleFromDate, s.PayScheduleToDate } into grp
                            select new PayrollMasterPaginationSearchSortModel
                            {
                                UniqueID = grp.Key.PayScheduleID + "#" + grp.Key.PayDate,
                                ScheduleName = grp.Key.PayScheduleName,
                                Status = (grp.Key.Status == "O") ? "Open" : "Published",
                                PayPeriod = grp.Key.PayScheduleFromDate.ToString("dd/MM/yyyy") + " - " + grp.Key.PayScheduleToDate.ToString("dd/MM/yyyy"),
                                PayDate =grp.Key.PayDate,
                                NoOfEmp = grp.Select(x => x.EmpID).Distinct().Count(),
                                Tax = 0,
                                NetSalary = Math.Round(((grp.Select(x => x.BasicSalary).Sum()) + grp.Select(x => x.Payrolldetail.Where(y => y.PayScheduleID == grp.Key.PayScheduleID && y.PayScheduleFromDate == grp.Key.PayScheduleFromDate && y.PayScheduleToDate == grp.Key.PayScheduleToDate && y.Type == "A").Select(z => z.Amount).Sum()).Sum() - grp.Select(x => x.Payrolldetail.Where(y => y.PayScheduleID == grp.Key.PayScheduleID && y.PayScheduleFromDate == grp.Key.PayScheduleFromDate && y.PayScheduleToDate == grp.Key.PayScheduleToDate && y.Type == "D").Select(z => z.Amount).Sum()).Sum()) ?? 0),
                            })
                            .ToList();
                }
                else
                {

                    list = (from s in dataList
                            group s by new { s.PayScheduleID, s.PayScheduleName, s.Status, s.PayDate, s.PayScheduleFromDate, s.PayScheduleToDate } into grp
                            select new PayrollMasterPaginationSearchSortModel
                            {
                                UniqueID = grp.Key.PayScheduleID + "#" + grp.Key.PayDate,
                                ScheduleName = grp.Key.PayScheduleName,
                                Status = (grp.Key.Status == "O") ? "Open" : "Published",
                                PayPeriod = grp.Key.PayScheduleFromDate.ToString("dd/MM/yyyy") + " - " + grp.Key.PayScheduleToDate.ToString("dd/MM/yyyy"),
                                PayDate = grp.Key.PayDate,
                                NoOfEmp = grp.Select(x => x.EmpID).Distinct().Count(),
                                Tax = 0,
                                NetSalary = Math.Round(((grp.Select(x => x.BasicSalary).Sum()) + grp.Select(x => x.Payrolldetail.Where(y => y.PayScheduleID == grp.Key.PayScheduleID && y.PayScheduleFromDate == grp.Key.PayScheduleFromDate && y.PayScheduleToDate == grp.Key.PayScheduleToDate && y.Type == "A").Select(z => z.Amount).Sum()).Sum() - grp.Select(x => x.Payrolldetail.Where(y => y.PayScheduleID == grp.Key.PayScheduleID && y.PayScheduleFromDate == grp.Key.PayScheduleFromDate && y.PayScheduleToDate == grp.Key.PayScheduleToDate && y.Type == "D").Select(z => z.Amount).Sum()).Sum()) ?? 0),
                            }).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                            //.OrderBy(o => o.UniqueID).OrderBy(x=>x.PayDate)
                            .ToList();

                }

                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("ScheduleName") > -1;
                    DisplayStatus = PFilter.VisibleColumnInfoList.IndexOf("Status") > -1;
                    DisplayPayPeriod = PFilter.VisibleColumnInfoList.IndexOf("PayPeriod") > -1;
                    DisplayPayDate = PFilter.VisibleColumnInfoList.IndexOf("PayDate") > -1;
                    DisplayEmployees = PFilter.VisibleColumnInfoList.IndexOf("NoOfEmp") > -1;
                    DisplayTax = PFilter.VisibleColumnInfoList.IndexOf("Tax") > -1;
                    DisplayNetSalary = PFilter.VisibleColumnInfoList.IndexOf("NetSalary") > -1;

                    list = list.Where(c =>
                    (DisplayName && c.ScheduleName.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayStatus && c.Status.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayPayPeriod && c.PayPeriod.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayPayDate && c.PayDate.ToString("dd/MM/yyyy").Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayEmployees && c.NoOfEmp.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayTax && c.Tax.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayNetSalary && c.NetSalary.ToString().ToLower().Contains(PFilter.SearchText.ToLower())
                    //
                    )))))))).ToList();
                }

                OrderByName = PFilter.OrderBy.IndexOf("ScheduleName") > -1;
                OrderByStatus = PFilter.OrderBy.IndexOf("Status") > -1;
                OrderByPayPeriod = PFilter.OrderBy.IndexOf("PayPeriod") > -1;
                OrderByPayDate = PFilter.OrderBy.IndexOf("PayDate") > -1;
                OrderByEmployees = PFilter.OrderBy.IndexOf("NoOfEmp") > -1;
                OrderByTax = PFilter.OrderBy.IndexOf("Tax") > -1;
                OrderByNetSalary = PFilter.OrderBy.IndexOf("NetSalary") > -1;

                Expression<Func<PayrollMasterPaginationSearchSortModel, string>> orderingFunction = (c =>
                                                              OrderByName ? c.ScheduleName :
                                                              OrderByStatus ? c.Status :
                                                                OrderByPayPeriod ? c.PayPeriod :
                                                                  OrderByPayDate ? c.PayDate.ToString("dd/MM/yyyy") :
                                                                   OrderByEmployees ? c.NoOfEmp.ToString() :
                                                                  OrderByTax ? c.Tax.ToString() :
                                                              OrderByNetSalary ? c.NetSalary.ToString() : ""
                                                              );

                IQueryable<PayrollMasterPaginationSearchSortModel> prList = list.AsQueryable();
                if (PFilter.IsOrderAsc)
                    prList = prList.OrderBy(orderingFunction);
                else
                    prList = prList.OrderByDescending(orderingFunction);

                objResult.TotalRecord = prList.Count();
                objResult.DataList = prList.ToList<Object>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }

        public static PaginationResult PaginationDetail(this IRepository<pr_employee_payroll_mf> repository, decimal CompanyID, string UniqueID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal PayScheduleID = 0;
                DateTime PayDate = new DateTime();
                var item = UniqueID.Split('#');
                if (item.Length > 0)
                {
                    PayScheduleID = Convert.ToDecimal(item[0]);
                    PayDate = Convert.ToDateTime(item[1]);
                }

                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pr_employee_payroll_mf, bool>> predicate = (e => e.CompanyId == CompanyID && e.PayScheduleID == PayScheduleID && e.PayDate == PayDate);

                bool DisplayEmpName, DisplayBaseSalary, DisplayAllowance, DisplayDeduction, DisplayGrossSalary, DisplayTax, DisplayNetSalary;
                bool OrderByEmpName, OrderByBaseSalary, OrderByAllowance, OrderByDeduction, OrderByGrossSalary, OrderByTax, OrderByNetSalary;

                IQueryable<pr_employee_payroll_mf> filteredData = repository.Queryable().Where(predicate);

                List<PayrollDetailPaginationSearchSortModel> DetList = new List<PayrollDetailPaginationSearchSortModel>();
                var dataList = filteredData.Include(x => x.pr_employee_payroll_dt).Include(y => y.pr_employee_mf)
                    //.Include(z => z.pr_employee_mf.adm_company_location)
                    //.Include(x => x.pr_employee_mf.pr_department)
                    //.Include(x => x.pr_employee_mf.pr_designation)//.Include(x => x.pr_employee_mf.EmployeeTypeList)
                    .Select(x => new
                    {
                        PayrollID = x.ID,
                        BasicSalary = x.BasicSalary + (x.AdjustmentBy == null ? 0 : (x.AdjustmentType == "C" ? x.AdjustmentAmount ?? 0 : (x.AdjustmentAmount ?? 0) * -1)),
                        x.Status,
                        x.PayDate,
                        EmpID = x.pr_employee_mf.ID,
                        ScheduleName = x.pr_pay_schedule.ScheduleName,
                        x.pr_employee_mf.FirstName,
                        x.pr_employee_mf.LastName,
                        PayrollDetail = x.pr_employee_payroll_dt.Select(d => new
                        {
                            x.PayScheduleFromDate,
                            d.EmployeeID,
                            d.PayrollID,
                            d.Type,
                            d.AllowDedID,
                            Amount = d.Amount + (d.AdjustmentBy == null ? 0 : (d.AdjustmentType == "C" ? (d.AdjustmentAmount ?? 0) : (d.AdjustmentAmount ?? 0) * -1)),
                            d.Taxable
                        }).ToList(),

                        Emplist = new { id = x.pr_employee_mf.ID, name = x.pr_employee_mf.FirstName + " " + x.pr_employee_mf.LastName },
                        Locations = "",// x.pr_employee_mf.adm_company_location == null ? null : new { id = x.pr_employee_mf.adm_company_location.ID, name = x.pr_employee_mf.adm_company_location.LocationName },
                        Departments = "",//x.pr_employee_mf.pr_department == null ? null : new { id = x.pr_employee_mf.pr_department.ID, name = x.pr_employee_mf.pr_department.DepartmentName },
                        Designations = "",// x.pr_employee_mf.pr_designation == null ? null : new { id = x.pr_employee_mf.pr_designation.ID, name = x.pr_employee_mf.pr_designation.DesignationName },
                        EmpTypes ="",// x.pr_employee_mf.EmployeeTypeList == null ? null : new { id = x.pr_employee_mf.EmployeeTypeList.ID, name = x.pr_employee_mf.EmployeeTypeList.Value },

                    }).ToList();

                var PayrollDtl = dataList.SelectMany(s => s.PayrollDetail).ToList();
                //var ListForFilterModal = dataList.Select(x => new { x.Emplist, x.Departments, x.Designations, x.EmpTypes, x.Locations }).ToList();

                //var ListForFilterModal = filteredData.Include(x => x.pr_employee_payroll_dt).Include(y => y.pr_employee_mf)
                //    .Select(x => x.pr_employee_mf)
                //.Select(y => new
                //{
                //    Locations = y.adm_company_location,
                //    Departments = y.pr_department,
                //    Designations = y.pr_designation,
                //    EmpTypes = y.EmployeeTypeList,
                //    Emplist = y,
                //}).ToList();


                prEmpDtFilterListModel filterModel = new prEmpDtFilterListModel();
                filterModel.Emplist = dataList.Where(x => x.Emplist != null).Select(x => x.Emplist).Distinct().ToList<object>();
                filterModel.Locations = dataList.Where(x => x.Locations != null).Select(x => x.Locations).Distinct().ToList<object>();
                filterModel.Departments = dataList.Where(x => x.Departments != null).Select(x => x.Departments).Distinct().ToList<object>();
                filterModel.Designations = dataList.Where(x => x.Designations != null).Select(x => x.Designations).Distinct().ToList<object>();
                filterModel.EmpTypes = dataList.Where(x => x.EmpTypes != null).Select(x => x.EmpTypes).Distinct().ToList<object>();

                //filterModel.Emplist = ListForFilterModal.Where(x => x.Emplist != null).Select(x => x.Emplist).Select(x => new { id = x.ID, name = x.FirstName + " " + x.LastName }).Distinct().ToList<Object>();
                //filterModel.Locations = ListForFilterModal.Where(x => x.Locations != null).Select(x => x.Locations).Select(x => new { id = x.ID, name = x.LocationName }).Distinct().ToList<Object>();
                //filterModel.Departments = ListForFilterModal.Where(x => x.Departments != null).Select(x => x.Departments).Select(x => new { id = x.ID, name = x.DepartmentName }).Distinct().ToList<Object>();
                //filterModel.Designations = ListForFilterModal.Where(x => x.Designations != null).Select(x => x.Designations).Select(x => new { id = x.ID, name = x.DesignationName }).Distinct().ToList<Object>();
                //filterModel.EmpTypes = ListForFilterModal.Where(x => x.EmpTypes != null).Select(x => x.EmpTypes).Select(x => new { id = x.ID, name = x.Value }).Distinct().ToList<Object>();

                if (IgnorePaging)
                {
                    DetList = (from prmf in dataList
                               group prmf by new
                               {
                                   prmf.EmpID,
                                   prmf.FirstName,
                                   prmf.LastName,
                                   emp_payrollmfID = prmf.PayrollID,
                                   prmf.BasicSalary,
                                   prmf.PayDate,
                                   prmf.Status,
                               } into grp
                               orderby grp.Key.BasicSalary
                               select new PayrollDetailPaginationSearchSortModel
                               {
                                   UniqueID = grp.Key.emp_payrollmfID,
                                   ScheduleName = dataList.Select(x => x.ScheduleName).FirstOrDefault(),
                                   Status = grp.Key.Status,
                                   EmployeeName = grp.Key.FirstName + " " + grp.Key.LastName,
                                   PayDate = grp.Key.PayDate,
                                   BaseSalary = Math.Round(dataList.Where(x => x.EmpID == grp.Key.EmpID).Select(x => x.BasicSalary).Sum() ?? 0),
                                   Deduction = Math.Round(PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "D").Select(a => a.Amount).Sum()),
                                   Allowance = Math.Round(PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "A").Select(a => a.Amount).Sum()),
                                   //GrossSalary = Math.Round(((dataList.Where(x => x.EmpID == grp.Key.EmpID).Select(x => x.BasicSalary).Sum()) + PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Taxable == true && e.Type == "A").Select(a => a.Amount).Sum() + PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Taxable == true && e.Type == "D").Select(a => a.Amount).Sum() - PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "D").Select(a => a.Amount).Sum()) ?? 0),
                                   GrossSalary = Math.Round(((dataList.Where(x => x.EmpID == grp.Key.EmpID).Select(x => x.BasicSalary).Sum()) + PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "A").Select(a => a.Amount).Sum()) ?? 0),
                                   NetSalary = Math.Round(((dataList.Where(x => x.EmpID == grp.Key.EmpID).Select(x => x.BasicSalary).Sum()) + PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "A").Select(a => a.Amount).Sum() - PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "D").Select(a => a.Amount).Sum()) ?? 0),
                                   Tax = 0,
                               }).ToList();
                }
                else
                {
                    DetList = (from prmf in dataList
                               group prmf by new
                               {
                                   prmf.EmpID,
                                   prmf.FirstName,
                                   prmf.LastName,
                                   emp_payrollmfID = prmf.PayrollID,
                                   prmf.BasicSalary,
                                   prmf.PayDate,
                                   prmf.Status,
                               } into grp
                               orderby grp.Key.BasicSalary
                               select new PayrollDetailPaginationSearchSortModel
                               {
                                   UniqueID = grp.Key.emp_payrollmfID,
                                   Status = grp.Key.Status,
                                   EmployeeName = grp.Key.FirstName + " " + grp.Key.LastName,
                                   ScheduleName = dataList.Select(x => x.ScheduleName).FirstOrDefault(),
                                   PayDate = grp.Key.PayDate,
                                   BaseSalary = Math.Round(dataList.Where(x => x.EmpID == grp.Key.EmpID).Select(x => x.BasicSalary).Sum() ?? 0),
                                   Deduction = Math.Round(PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "D").Select(a => a.Amount).Sum()),
                                   Allowance = Math.Round(PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "A").Select(a => a.Amount).Sum()),
                                   GrossSalary = Math.Round(((dataList.Where(x => x.EmpID == grp.Key.EmpID).Select(x => x.BasicSalary).Sum()) + PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "A").Select(a => a.Amount).Sum()) ?? 0),
                                   //GrossSalary = Math.Round(((dataList.Where(x => x.EmpID == grp.Key.EmpID).Select(x => x.BasicSalary).Sum()) + PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Taxable == true && e.Type == "A").Select(a => a.Amount).Sum() + PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Taxable == true && e.Type == "D").Select(a => a.Amount).Sum() - PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "D").Select(a => a.Amount).Sum()) ?? 0),
                                   NetSalary = Math.Round(((dataList.Where(x => x.EmpID == grp.Key.EmpID).Select(x => x.BasicSalary).Sum()) + PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "A").Select(a => a.Amount).Sum() - PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "D").Select(a => a.Amount).Sum()) ?? 0),
                                   Tax = 0,
                               }).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord).ToList();
                }

                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayEmpName = PFilter.VisibleColumnInfoList.IndexOf("EmployeeName") > -1;
                    DisplayBaseSalary = PFilter.VisibleColumnInfoList.IndexOf("BaseSalary") > -1;
                    DisplayAllowance = PFilter.VisibleColumnInfoList.IndexOf("Allowance") > -1;
                    DisplayDeduction = PFilter.VisibleColumnInfoList.IndexOf("Deduction") > -1;
                    DisplayTax = PFilter.VisibleColumnInfoList.IndexOf("Tax") > -1;
                    DisplayGrossSalary = PFilter.VisibleColumnInfoList.IndexOf("GrossSalary") > -1;
                    DisplayNetSalary = PFilter.VisibleColumnInfoList.IndexOf("NetSalary") > -1;

                    DetList = DetList.Where(c =>
                     (DisplayEmpName && c.EmployeeName.ToLower().Replace("  ", " ").Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayBaseSalary && c.BaseSalary.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                      (DisplayAllowance && c.Allowance.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayDeduction && c.Deduction.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                      (DisplayTax && c.Tax.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayGrossSalary && c.GrossSalary.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayNetSalary && c.NetSalary.ToString().ToLower().Contains(PFilter.SearchText.ToLower())

                     )))))))).ToList();
                }


                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "EmployeeName";

                OrderByEmpName = PFilter.OrderBy.IndexOf("EmployeeName") > -1;
                OrderByBaseSalary = PFilter.OrderBy.IndexOf("BaseSalary") > -1;
                OrderByAllowance = PFilter.OrderBy.IndexOf("Allowance") > -1;
                OrderByDeduction = PFilter.OrderBy.IndexOf("Deduction") > -1;
                OrderByGrossSalary = PFilter.OrderBy.IndexOf("GrossSalary") > -1;
                OrderByTax = PFilter.OrderBy.IndexOf("Tax") > -1;
                OrderByNetSalary = PFilter.OrderBy.IndexOf("NetSalary") > -1;
                Expression<Func<PayrollDetailPaginationSearchSortModel, string>> orderingFunction = (c =>
                                                                 OrderByEmpName ? c.EmployeeName :
                                                                 OrderByBaseSalary ? c.BaseSalary.ToString() :
                                                                 OrderByAllowance ? c.Allowance.ToString() :
                                                                 OrderByDeduction ? c.Deduction.ToString() :
                                                                 OrderByGrossSalary ? c.GrossSalary.ToString() :
                                                                 OrderByTax ? c.Tax.ToString() :
                                                                 OrderByNetSalary ? c.NetSalary.ToString() : ""
                                                              );

                IQueryable<PayrollDetailPaginationSearchSortModel> prDetList = DetList.AsQueryable();
                if (PFilter.IsOrderAsc)
                    prDetList = prDetList.OrderBy(orderingFunction);
                else
                    prDetList = prDetList.OrderByDescending(orderingFunction);

                objResult.TotalRecord = prDetList.Count();
                objResult.DataList = prDetList.ToList<Object>();
                objResult.OtherDataModel = filterModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }

        public static PaginationResult DashboardEmpPagination(this IRepository<pr_employee_payroll_mf> repository, string filterParams, DashboardDefaultConDedModel DashbrdIdsmodel, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                string PayrollRegion = WebConfigurationManager.AppSettings["PayrollRegion"];
                var Dashfilters = filterParams.Split('#').ToArray();
                DashboardFilterModel FilterModel = new DashboardFilterModel();
                if (Dashfilters.Length > 5)
                {
                    FilterModel.Range = Dashfilters[0].ToString();
                    FilterModel.PeriodStart = Dashfilters[1] == "null" ? (DateTime?)null : Convert.ToDateTime(Dashfilters[1].ToString());
                    FilterModel.PeriodEnd = Dashfilters[2] == "null" ? (DateTime?)null : Convert.ToDateTime(Dashfilters[2].ToString());
                    FilterModel.PayScheduleID = Dashfilters[3] == "null" ? 0 : Convert.ToDecimal(Dashfilters[3]);
                    FilterModel.LocationID = Dashfilters[4] == "null" ? 0 : Convert.ToDecimal(Dashfilters[4]);
                    FilterModel.DepartmentID = Dashfilters[5] == "null" ? 0 : Convert.ToDecimal(Dashfilters[5]);
                }

                List<DateTime> dates = new List<DateTime>();
                //while (FilterModel.PeriodStart < FilterModel.PeriodEnd)
                //{
                //    FilterModel.PeriodStart = FilterModel.PeriodStart.Value.AddMonths(1);

                //    string monthName = new DateTime(2010, 8, 1)
                //        .ToString("MMM", CultureInfo.InvariantCulture);
                //    dates.Add((DateTime)(FilterModel.PeriodStart));
                //}

                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pr_employee_payroll_mf, bool>> predicate = (e => e.CompanyId == CompanyID && e.Status == "P");

                bool DisplayName, DisplayDesignation, DisplayGrossSalary, DisplayDeduction, DisplayTax, DisplayNetSalary;
                bool OrderByName, OrderByDesignation, OrderByGrossSalary, OrderByDeduction, OrderByTax, OrderByNetSalary;

                IQueryable<pr_employee_payroll_mf> filteredData = repository.Queryable().Where(predicate);

                List<DashboardPrEmployeeModel> DList = new List<DashboardPrEmployeeModel>();

                var DetaList = filteredData
                    .Where(x =>
                    (FilterModel.PeriodStart == null || x.PayScheduleFromDate >= FilterModel.PeriodStart)
                    && (FilterModel.PeriodEnd == null || x.PayScheduleToDate <= FilterModel.PeriodEnd)
                    && (FilterModel.PayScheduleID == 0 || x.PayScheduleID == FilterModel.PayScheduleID)
                    && (FilterModel.DepartmentID == 0 || x.pr_employee_mf.DepartmentID == FilterModel.DepartmentID))
                    .Include(x => x.pr_employee_payroll_dt).Include(y => y.pr_employee_mf).Include(y => y.pr_employee_mf.pr_loan)
                    .Select(s => new
                    {
                        PayrollID = s.ID,
                        s.EmployeeID,
                        Name = s.pr_employee_mf.FirstName + " " + s.pr_employee_mf.LastName,
                        Designation = "",// s.pr_employee_mf.pr_designation.DesignationName,
                        BasicSalary = s.BasicSalary + (s.AdjustmentType == null ? 0 : (s.AdjustmentType == "C" ? (s.AdjustmentAmount ?? 0) : ((s.AdjustmentAmount ?? 0) * -1))),
                        PayScheduleFromDate = s.PayScheduleFromDate,
                        //TotalVacationHours = s.pr_employee_mf.pr_employee_leave.Where(x => DashbrdIdsmodel.VacationIds.Contains(x.LeaveTypeID)).Count() == 0 ? 0 : s.pr_employee_mf.pr_employee_leave.Where(x => DashbrdIdsmodel.VacationIds.Contains(x.LeaveTypeID)).Select(x => x.Hours).Sum(),
                        //TotalSickHours = s.pr_employee_mf.pr_employee_leave.Where(x => DashbrdIdsmodel.SickIds.Contains(x.LeaveTypeID)).Count() == 0 ? 0 : s.pr_employee_mf.pr_employee_leave.Where(x => DashbrdIdsmodel.SickIds.Contains(x.LeaveTypeID)).Select(x => x.Hours).Sum(),
                        //VacationLeaveHours = s.pr_employee_mf.pr_leave_application.Where(x => DashbrdIdsmodel.VacationIds.Contains(x.LeaveTypeID)).Count() == 0 ? 0 : s.pr_employee_mf.pr_leave_application.Where(x => DashbrdIdsmodel.VacationIds.Contains(x.LeaveTypeID)).Select(x => x.Hours).Sum(),
                        //SickLeaveHours = s.pr_employee_mf.pr_leave_application.Where(x => DashbrdIdsmodel.SickIds.Contains(x.LeaveTypeID)).Count() == 0 ? 0 : s.pr_employee_mf.pr_leave_application.Where(x => DashbrdIdsmodel.SickIds.Contains(x.LeaveTypeID)).Select(x => x.Hours).Sum(),
                        PayrollDetail = s.pr_employee_payroll_dt.Select(d => new
                        {
                            s.PayScheduleFromDate,
                            d.EmployeeID,
                            d.PayrollID,
                            d.Type,
                            d.AllowDedID,
                            Amount = d.Amount + (d.AdjustmentType == null ? 0 : (d.AdjustmentType == "C" ? (d.AdjustmentAmount ?? 0) : ((d.AdjustmentAmount ?? 0) * -1))),
                            d.Taxable
                        }).ToList(),
                        pr_loan = s.pr_employee_mf.pr_loan
                        .AsEnumerable().Select(a => new
                        {
                            a.ID,
                            a.EmployeeID,
                            LoanAmount = a.LoanAmount + (double)(a.AdjustmentType == null ? 0 : (a.AdjustmentType == "C" ? (a.AdjustmentAmount ?? 0) : ((a.AdjustmentAmount ?? 0) * -1))),
                            a.LoanDate,
                            a.PaymentStartDate,
                            a.InstallmentByBaseSalary,
                            pr_loan_dt = a.pr_loan_payment_dt.Select(g => new
                            {
                                Amount = g.Amount + (double)(g.AdjustmentType == null ? 0 : (g.AdjustmentType == "C" ? (g.AdjustmentAmount ?? 0) : ((g.AdjustmentAmount ?? 0) * -1))),
                                g.PaymentDate,
                                g.ID,
                                g.LoanID
                            }).ToList()
                        }).ToList(),
                    })
                    .ToList();

                var PayrollDtl = DetaList.SelectMany(s => s.PayrollDetail).ToList();
                var prLoans = DetaList.SelectMany(x => x.pr_loan).ToList();
                var prLoanDetails = prLoans.SelectMany(s => s.pr_loan_dt).ToList();

                var prloan = prLoans.Select(u => new { u.ID, u.EmployeeID }).Distinct().Select(z => new
                {
                    z.ID,
                    z.EmployeeID,
                    //TotalGivenLoan = prLoans.Where(f=>f.ID == z.ID)
                    //.Select(t => new { t.ID, t.LoanAmount }).Distinct().Sum(s => s.LoanAmount),

                    GivenLoan = prLoans.Where(c => c.ID == z.ID)
                     .Select(t => new { t.ID, t.LoanAmount }).Distinct().Sum(s => s.LoanAmount),

                    LoanRecoveredAmountList = prLoanDetails.Where(x => x.LoanID == z.ID)
                     .Select(t => new { t.ID, t.Amount }).Distinct().ToList(),

                    //LoanDetails = prLoanDetails.Where(x => x.LoanID == z.ID)
                    //.Select(e => new { e.ID,e.LoanID, e.Amount }).Distinct().Sum(a=>a.Amount),

                    //TotalRecoveredLoan = prLoanDetails.Where(x=>x.LoanID == z.ID)
                    //.Sum(m=>m.Amount)

                    //.Distinct()
                    //.Select(d => new { d.ID, d.dtAmount })

                }).ToList();

                var Result = DetaList
                    .Select(g => new
                    {
                        g.EmployeeID,
                        g.Name,
                        g.Designation,
                        //g.TotalSickHours,
                        //g.TotalVacationHours,
                        //g.VacationLeaveHours,
                        //g.SickLeaveHours,
                    }).Distinct()
                    .Select(s => new DashboardPrEmployeeModel
                    {
                        EmployeeID = s.EmployeeID,
                        Name = s.Name,
                        Designation = s.Designation,
                        //TotalVacationHours = s.TotalVacationHours,
                        //TotalSickHours = s.TotalSickHours,
                        //VacationLeaveHours = s.VacationLeaveHours,
                        //SickLeaveHours = s.SickLeaveHours,
                        BasicSalary = DetaList.Where(x => x.EmployeeID == s.EmployeeID).Select(x => x.BasicSalary).Sum() ?? 0,
                        Deduction = PayrollDtl.Where(e => e.EmployeeID == s.EmployeeID && e.Type == "D").Select(a => a.Amount).Sum(),
                        Contribution = PayrollDtl.Where(e => e.EmployeeID == s.EmployeeID && e.Type == "C").Select(a => a.Amount).Sum(),
                        GrossSalary = ((DetaList.Where(x => x.EmployeeID == s.EmployeeID).Select(x => x.BasicSalary).Sum()) + PayrollDtl.Where(e => e.EmployeeID == s.EmployeeID && e.Taxable == true && e.Type == "A").Select(a => a.Amount).Sum()
                   + PayrollDtl.Where(e => e.EmployeeID == s.EmployeeID && e.Taxable == true && e.Type == "D").Select(a => a.Amount).Sum()
                   - PayrollDtl.Where(e => e.EmployeeID == s.EmployeeID && e.Type == "D").Select(a => a.Amount).Sum()) ?? 0,
                        NetSalary = ((DetaList.Where(x => x.EmployeeID == s.EmployeeID).Select(x => x.BasicSalary).Sum()) + PayrollDtl.Where(e => e.EmployeeID == s.EmployeeID && e.Type == "A").Select(a => a.Amount).Sum() - PayrollDtl.Where(e => e.EmployeeID == s.EmployeeID && e.Type == "D").Select(a => a.Amount).Sum()) ?? 0,
                        Tax = 0,
                        EmployeeProvidentFund = PayrollDtl.Where(x => x.EmployeeID == s.EmployeeID && x.AllowDedID == DashbrdIdsmodel.EmployeePFID && x.Type == "D").Sum(x => x.Amount),
                        EmployerProvidentFund = PayrollDtl.Where(x => x.EmployeeID == s.EmployeeID && x.AllowDedID == DashbrdIdsmodel.EmployerPFID && x.Type == "C").Sum(x => x.Amount),
                        EmployeeEOBI = PayrollDtl.Where(x => x.EmployeeID == s.EmployeeID && x.AllowDedID == DashbrdIdsmodel.EmployeeEOBIID && x.Type == "D").Select(x => x.Amount).Sum(),
                        EmployerEOBI = PayrollDtl.Where(x => x.EmployeeID == s.EmployeeID && x.AllowDedID == DashbrdIdsmodel.EmployerEOBIID && x.Type == "C").Select(x => x.Amount).Sum(),
                        EmployeeGOSI = PayrollRegion == "SA" ? PayrollDtl.Where(x => x.EmployeeID == s.EmployeeID && x.AllowDedID == DashbrdIdsmodel.EmployeeGOSIID && x.Type == "D").Select(x => x.Amount).Sum() : 0,
                        EmployerGOSI = PayrollRegion == "SA" ? PayrollDtl.Where(x => x.EmployeeID == s.EmployeeID && x.AllowDedID == DashbrdIdsmodel.EmployerGOSIID && x.Type == "C").Select(x => x.Amount).Sum() : 0,
                        GivenLoan = prloan.Where(t => t.EmployeeID == s.EmployeeID).Sum(x => x.GivenLoan),
                        RecoveredLoan = prloan.Where(z => z.EmployeeID == s.EmployeeID)
                        .SelectMany(c => c.LoanRecoveredAmountList).Sum(f => f.Amount)
                    }).ToList();

                var DashboardTotals = new
                {
                    TotalTax = 0,
                    NoOfEmployees = Result.Count(),
                    TotalDeductions = Result.Select(x => x.Deduction).Sum(),
                    TotalNetSalary = Result.Select(x => x.NetSalary).Sum(),
                    TotalGrossSalary = Result.Select(x => x.GrossSalary).Sum(),
                    TotalEmployeeProvidentFund = PayrollDtl.Where(x => x.AllowDedID == DashbrdIdsmodel.EmployeePFID && x.Type == "D").Sum(x => x.Amount),
                    TotalEmployerProvidentFund = PayrollDtl.Where(x => x.AllowDedID == DashbrdIdsmodel.EmployerPFID && x.Type == "C").Sum(x => x.Amount),
                    TotalEmployeeEOBI = PayrollDtl.Where(x => x.AllowDedID == DashbrdIdsmodel.EmployeeEOBIID && x.Type == "D").Sum(x => x.Amount),
                    TotalEmployerEOBI = PayrollDtl.Where(x => x.AllowDedID == DashbrdIdsmodel.EmployerEOBIID && x.Type == "C").Sum(x => x.Amount),
                    TotalEmployeeGOSI = PayrollRegion == "SA" ? PayrollDtl.Where(x => x.AllowDedID == DashbrdIdsmodel.EmployeeGOSIID && x.Type == "D").Sum(x => x.Amount) : 0,
                    TotalEmployerGOSI = PayrollRegion == "SA" ? PayrollDtl.Where(x => x.AllowDedID == DashbrdIdsmodel.EmployerGOSIID && x.Type == "C").Sum(x => x.Amount) : 0,
                    TotalGivenLoan = prloan.Sum(x => x.GivenLoan),
                    TotalRecoveredLoan = prloan.Select(x => x.LoanRecoveredAmountList.Select(d => d.Amount).Sum()).Sum()
                };

                var StudenSalaryGraph = DetaList
                  .Select(g => new
                  {
                      g.EmployeeID,
                      g.PayScheduleFromDate,
                  }).Distinct()
                  .Select(s => new
                  {
                      s.EmployeeID,
                      Month = GetMonthName(Convert.ToDateTime(s.PayScheduleFromDate)),
                      TotalTax = 0,
                      TotalDeductions = PayrollDtl.Where(e => e.EmployeeID == s.EmployeeID && e.PayScheduleFromDate.Year == s.PayScheduleFromDate.Year && e.PayScheduleFromDate.Month == s.PayScheduleFromDate.Month && e.Type == "D").Select(a => a.Amount).Sum(),
                      TotalContributions = PayrollDtl.Where(e => e.EmployeeID == s.EmployeeID && e.Type == "C" && e.PayScheduleFromDate.Year == s.PayScheduleFromDate.Year && e.PayScheduleFromDate.Month == s.PayScheduleFromDate.Month).Select(a => a.Amount).Sum(),
                      TotalNetSalary = ((DetaList.Where(x => x.EmployeeID == s.EmployeeID && x.PayScheduleFromDate.Year == s.PayScheduleFromDate.Year && x.PayScheduleFromDate.Month == s.PayScheduleFromDate.Month).Select(x => x.BasicSalary).Sum()) + PayrollDtl.Where(e => e.EmployeeID == s.EmployeeID && e.PayScheduleFromDate.Year == s.PayScheduleFromDate.Year && e.PayScheduleFromDate.Month == s.PayScheduleFromDate.Month && e.Type == "A").Select(a => a.Amount).Sum() - PayrollDtl.Where(e => e.EmployeeID == s.EmployeeID && e.PayScheduleFromDate.Year == s.PayScheduleFromDate.Year && e.PayScheduleFromDate.Month == s.PayScheduleFromDate.Month && e.Type == "D").Select(a => a.Amount).Sum()) ?? 0,
                      //TotalEmployeeProvidentFund = PayrollDtl.Where(x => x.EmployeeID == s.EmployeeID && x.AllowDedID == DashbrdIdsmodel.EmployeePFID && x.Type == "D").Sum(x => x.Amount),
                      //TotalEmployerProvidentFund = PayrollDtl.Where(x => x.EmployeeID == s.EmployeeID && x.AllowDedID == DashbrdIdsmodel.EmployerPFID && x.Type == "C").Sum(x => x.Amount),
                      //TotalEmployeeEOBI = PayrollDtl.Where(x => x.EmployeeID == s.EmployeeID && x.AllowDedID == DashbrdIdsmodel.EmployeeEOBIID && x.Type == "D").Sum(x => x.Amount),
                      //TotalEmployerEOBI = PayrollDtl.Where(x => x.EmployeeID == s.EmployeeID && x.AllowDedID == DashbrdIdsmodel.EmployerEOBIID && x.Type == "C").Sum(x => x.Amount),
                      //TotalEmployeeGOSI = PayrollRegion == "SA" ? PayrollDtl.Where(x => x.EmployeeID == s.EmployeeID && x.AllowDedID == DashbrdIdsmodel.EmployeeGOSIID && x.Type == "D").Sum(x => x.Amount) : 0,
                      //TotalEmployerGOSI = PayrollRegion == "SA" ? PayrollDtl.Where(x => x.EmployeeID == s.EmployeeID && x.AllowDedID == DashbrdIdsmodel.EmployerGOSIID && x.Type == "C").Sum(x => x.Amount) : 0,
                  }).ToList();

                var SalaryGraph = DetaList
                  .Select(g => new
                  {
                      g.PayScheduleFromDate,
                  }).Distinct()
                  .Select(s => new
                  {
                      Month = GetMonthName(Convert.ToDateTime(s.PayScheduleFromDate)),
                      TotalTax = 0,
                      TotalDeductions = PayrollDtl.Where(e => e.PayScheduleFromDate.Year == s.PayScheduleFromDate.Year && e.PayScheduleFromDate.Month == s.PayScheduleFromDate.Month && e.Type == "D").Select(a => a.Amount).Sum(),
                      TotalContributions = PayrollDtl.Where(e => e.PayScheduleFromDate.Year == s.PayScheduleFromDate.Year && e.PayScheduleFromDate.Month == s.PayScheduleFromDate.Month && e.Type == "C").Select(a => a.Amount).Sum(),
                      TotalNetSalary = ((DetaList.Where(e => e.PayScheduleFromDate.Year == s.PayScheduleFromDate.Year && e.PayScheduleFromDate.Month == s.PayScheduleFromDate.Month).Select(x => x.BasicSalary).Sum()) + PayrollDtl.Where(e => e.PayScheduleFromDate.Year == s.PayScheduleFromDate.Year && e.PayScheduleFromDate.Month == s.PayScheduleFromDate.Month && e.Type == "A").Select(a => a.Amount).Sum() - PayrollDtl.Where(e => e.PayScheduleFromDate.Year == s.PayScheduleFromDate.Year && e.PayScheduleFromDate.Month == s.PayScheduleFromDate.Month && e.Type == "D").Select(a => a.Amount).Sum()) ?? 0,
                  }).ToList();

                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Name") > -1;
                    DisplayDesignation = PFilter.VisibleColumnInfoList.IndexOf("Designation") > -1;
                    DisplayGrossSalary = PFilter.VisibleColumnInfoList.IndexOf("GrossSalary") > -1;
                    DisplayDeduction = PFilter.VisibleColumnInfoList.IndexOf("Deduction") > -1;
                    DisplayTax = PFilter.VisibleColumnInfoList.IndexOf("Tax") > -1;
                    DisplayNetSalary = PFilter.VisibleColumnInfoList.IndexOf("NetSalary") > -1;

                    Result = Result.Where(c =>
                     (DisplayName && c.Name.ToLower().Replace("  ", " ").Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayDesignation && (c.Designation ?? "").ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                      (DisplayGrossSalary && c.GrossSalary.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayDeduction && c.Deduction.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                      (DisplayTax && (c.Tax).ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayNetSalary && c.NetSalary.ToString().ToLower().Contains(PFilter.SearchText.ToLower())

                     ))))))).ToList();
                }

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "EmployeeID";

                OrderByName = PFilter.OrderBy.IndexOf("Name") > -1;
                OrderByDesignation = PFilter.OrderBy.IndexOf("Designation") > -1;
                OrderByGrossSalary = PFilter.OrderBy.IndexOf("GrossSalary") > -1;
                OrderByDeduction = PFilter.OrderBy.IndexOf("Deduction") > -1;
                OrderByTax = PFilter.OrderBy.IndexOf("Tax") > -1;
                OrderByNetSalary = PFilter.OrderBy.IndexOf("NetSalary") > -1;
                Expression<Func<DashboardPrEmployeeModel, string>> orderingFunction = (c =>
                                                                 OrderByName ? c.Name :
                                                                 OrderByDesignation ? (c.Designation ?? "").ToString() :
                                                                 OrderByGrossSalary ? c.GrossSalary.ToString() :
                                                                 OrderByDeduction ? c.Deduction.ToString() :
                                                                 OrderByTax ? c.Tax.ToString() :
                                                                 OrderByNetSalary ? c.NetSalary.ToString() : ""
                                                              );

                IQueryable<DashboardPrEmployeeModel> prDetList = Result.AsQueryable();
                if (PFilter.IsOrderAsc)
                    prDetList = prDetList.OrderBy(orderingFunction);
                else
                    prDetList = prDetList.OrderByDescending(orderingFunction);


                if (IgnorePaging)
                {
                    prDetList.ToList();

                }
                else
                {
                    prDetList.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord).ToList();
                }



                objResult.TotalRecord = prDetList.Count();
                objResult.OtherDataModel = DashboardTotals;
                objResult.OtherData2Model = StudenSalaryGraph;
                objResult.OtherData3Model = SalaryGraph;
                objResult.DataList = prDetList.ToList<Object>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }

        public static string GetMonthName(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day)
                   .ToString("MMM", CultureInfo.InvariantCulture);
        }

        public static PaginationResult FilterPaginationDetail(this IRepository<pr_employee_payroll_mf> repository, string FilterParams, decimal CompanyID, string UniqueID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal PayScheduleID = 0;
                DateTime PayDate = new DateTime();
                var item = UniqueID.Split('#');
                if (item.Length > 0)
                {
                    PayScheduleID = Convert.ToDecimal(item[0]);
                    PayDate = Convert.ToDateTime(item[1]);
                }

                var LocationsIds = FilterParams.Split('#')[0].Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(decimal.Parse).ToArray();
                var DepartmentsIds = FilterParams.Split('#')[1].Split(',').Where(e => e != "").Select(decimal.Parse).ToArray();
                var DesignationsIds = FilterParams.Split('#')[2].Split(',').Where(e => e != "").Select(decimal.Parse).ToArray();
                var EmployeeTypeIds = FilterParams.Split('#')[3].Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(decimal.Parse).ToArray();
                var EmployeeIds = FilterParams.Split('#')[4].Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(decimal.Parse).ToArray();


                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pr_employee_payroll_mf, bool>> predicate = (e => e.CompanyId == CompanyID && e.PayScheduleID == PayScheduleID && e.PayDate == PayDate);

                bool DisplayEmpName, DisplayBaseSalary, DisplayAllowance, DisplayDeduction, DisplayGrossSalary, DisplayTax, DisplayNetSalary;
                bool OrderByEmpName, OrderByBaseSalary, OrderByAllowance, OrderByDeduction, OrderByGrossSalary, OrderByTax, OrderByNetSalary;

                IQueryable<pr_employee_payroll_mf> filteredData = repository.Queryable().Where(predicate);

                //objResult.TotalRecord = filteredData.Count();


                List<PayrollDetailPaginationSearchSortModel> DetList = new List<PayrollDetailPaginationSearchSortModel>();
                var dataList = filteredData.Include(x => x.pr_employee_payroll_dt).Include(y => y.pr_employee_mf)
                    .Where(z => (DepartmentsIds.Count() == 0 || DepartmentsIds.Contains(z.pr_employee_mf.DepartmentID.Value))
                    && (DesignationsIds.Count() == 0 || DesignationsIds.Contains(z.pr_employee_mf.DesignationID.Value))
                    && (EmployeeTypeIds.Count() == 0 || EmployeeTypeIds.Contains(z.pr_employee_mf.EmployeeTypeID))
                    && (EmployeeIds.Count() == 0 || EmployeeIds.Contains(z.pr_employee_mf.ID)))
                     .Select(x => new
                     {
                         PayrollID = x.ID,
                         BasicSalary = x.BasicSalary + (x.AdjustmentBy == null ? 0 : (x.AdjustmentType == "C" ? (x.AdjustmentAmount ?? 0) : (x.AdjustmentAmount ?? 0) * -1)),
                         x.Status,
                         x.PayDate,
                         EmpID = x.pr_employee_mf.ID,
                         x.pr_employee_mf.FirstName,
                         x.pr_employee_mf.LastName,
                         ScheduleName = x.pr_pay_schedule.ScheduleName,
                         PayrollDetail = x.pr_employee_payroll_dt.Select(d => new
                         {
                             x.PayScheduleFromDate,
                             d.EmployeeID,
                             d.PayrollID,
                             d.Type,
                             d.AllowDedID,
                             Amount = d.Amount + (d.AdjustmentBy == null ? 0 : (d.AdjustmentType == "C" ? (d.AdjustmentAmount ?? 0) : (d.AdjustmentAmount ?? 0) * -1)),
                             d.Taxable
                         }).ToList(),
                     }).ToList();


                var PayrollDtl = dataList.SelectMany(s => s.PayrollDetail).ToList();
                //var ListForFilterModal = filteredData.Include(x => x.pr_employee_payroll_dt).Include(y => y.pr_employee_mf)
                //    .Select(x => x.pr_employee_mf)
                //.Select(y => new
                //{
                //    Locations = y.adm_company_location,
                //    Departments = y.pr_department,
                //    Designations = y.pr_designation,
                //}).ToList();
                //var Emplist = dataList.Select(x => x.pr).ToList();
                //var Locations = Emplist.Select(x => x.adm_company_location).ToList();
                //var Departments = Emplist.Select(x => x.pr_department).ToList();
                //var Designations = Emplist.Select(x => x.pr_designation).ToList();
                //var  = emplist.Select(x => x.adm_company_location).ToList();


                if (IgnorePaging)
                {
                    //DetList = (from prmf in dataList
                    //           join prdt in dataList.SelectMany(i => i.pr_employee_payroll_dt) on prmf.ID equals prdt.PayrollID
                    //           group prmf by new
                    //           {
                    //               prmf.pr_employee_mf.ID,
                    //               prmf.pr_employee_mf.FirstName,
                    //               prmf.pr_employee_mf.LastName,
                    //               emp_payrollmfID = prmf.ID,
                    //               prmf.BasicSalary,
                    //               prmf.PayDate,
                    //               prmf.Status,
                    //           } into grp
                    //           orderby grp.Key.BasicSalary
                    //           select new PayrollDetailPaginationSearchSortModel
                    //           {
                    //               UniqueID = grp.Key.emp_payrollmfID,
                    //               Status = grp.Key.Status,
                    //               EmployeeName = grp.Key.FirstName + " " + grp.Key.LastName,
                    //               BaseSalary = Math.Round(grp.Key.BasicSalary ?? 0) + (grp.Where(y => y.AdjustmentType == "C").Select(y => y.AdjustmentAmount).FirstOrDefault() ?? 0) - (grp.Where(y => y.AdjustmentType == "D").Select(y => y.AdjustmentAmount).FirstOrDefault() ?? 0),
                    //               PayDate = grp.Key.PayDate,
                    //               Allowance = Math.Round((grp.Select(x => x.pr_employee_payroll_dt.Where(y => y.Type == "A").Sum(y => y.Amount)).FirstOrDefault() + grp.Select(x => x.pr_employee_payroll_dt.Where(y => y.Type == "A" && y.AdjustmentType == "C").Sum(y => y.AdjustmentAmount)).FirstOrDefault() - grp.Select(x => x.pr_employee_payroll_dt.Where(y => y.Type == "A" && y.AdjustmentType == "D").Sum(y => y.AdjustmentAmount)).FirstOrDefault()) ?? 0),
                    //               AllowanceTaxable = Math.Round((grp.Select(x => x.pr_employee_payroll_dt.Where(y => y.Type == "A" && y.Taxable).Sum(y => y.Amount)).FirstOrDefault() + grp.Select(x => x.pr_employee_payroll_dt.Where(y => y.Type == "A" && y.Taxable && y.AdjustmentType == "C").Sum(y => y.AdjustmentAmount)).FirstOrDefault() - grp.Select(x => x.pr_employee_payroll_dt.Where(y => y.Type == "A" && y.Taxable && y.AdjustmentType == "D").Sum(y => y.AdjustmentAmount)).FirstOrDefault()) ?? 0),

                    //               Deduction = Math.Round((grp.Select(x => x.pr_employee_payroll_dt.Where(y => y.Type == "D").Sum(y => y.Amount)).FirstOrDefault() + grp.Select(x => x.pr_employee_payroll_dt.Where(y => y.Type == "D" && y.AdjustmentType == "C").Sum(y => y.AdjustmentAmount)).FirstOrDefault() - grp.Select(x => x.pr_employee_payroll_dt.Where(y => y.Type == "D" && y.AdjustmentType == "D").Sum(y => y.AdjustmentAmount)).FirstOrDefault()) ?? 0),
                    //               DeductionTaxable = Math.Round((grp.Select(x => x.pr_employee_payroll_dt.Where(y => y.Type == "D" && y.Taxable).Sum(y => y.Amount)).FirstOrDefault() + grp.Select(x => x.pr_employee_payroll_dt.Where(y => y.Type == "D" && y.Taxable && y.AdjustmentType == "C").Sum(y => y.AdjustmentAmount)).FirstOrDefault() - grp.Select(x => x.pr_employee_payroll_dt.Where(y => y.Type == "D" && y.Taxable && y.AdjustmentType == "D").Sum(y => y.AdjustmentAmount)).FirstOrDefault()) ?? 0),

                    //               Tax = Math.Round((grp.Where(e => e.pr_employee_payroll_dt.Any(a => a.Type == "A" && a.AllowDedID == 6)).Count() == 0 ? 0 :
                    //                grp.Where(e => e.pr_employee_payroll_dt.Any(a => a.Type == "A" && a.AllowDedID == 6))
                    //               .SelectMany(s => s.pr_employee_payroll_dt
                    //               .Select(f => f.Amount)).Sum())),

                    //               NetSalary = Math.Round((grp.Where(e => e.pr_employee_payroll_dt.Any(a => a.Type == "A" || a.Type == "D")).Count() == 0 ? 0 :
                    //                grp.Where(e => e.pr_employee_payroll_dt.Any(a => a.Type == "A" || a.Type == "D"))
                    //               .SelectMany(s => s.pr_employee_payroll_dt
                    //               .Select(f => (f.Type == "A" ? f.Amount : f.Amount * -1))).Sum())),

                    //           }).ToList();

                    DetList = (from prmf in dataList
                               group prmf by new
                               {
                                   prmf.EmpID,
                                   prmf.FirstName,
                                   prmf.LastName,
                                   emp_payrollmfID = prmf.PayrollID,
                                   prmf.BasicSalary,
                                   prmf.PayDate,
                                   prmf.Status,
                               } into grp
                               orderby grp.Key.BasicSalary
                               select new PayrollDetailPaginationSearchSortModel
                               {
                                   UniqueID = grp.Key.emp_payrollmfID,
                                   Status = grp.Key.Status,
                                   EmployeeName = grp.Key.FirstName + " " + grp.Key.LastName,
                                   ScheduleName = dataList.Select(x => x.ScheduleName).FirstOrDefault(),
                                   PayDate = grp.Key.PayDate,
                                   BaseSalary = Math.Round(dataList.Where(x => x.EmpID == grp.Key.EmpID).Select(x => x.BasicSalary).Sum() ?? 0),
                                   Deduction = Math.Round(PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "D").Select(a => a.Amount).Sum()),
                                   Allowance = Math.Round(PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "A").Select(a => a.Amount).Sum()),
                                   GrossSalary = Math.Round(((dataList.Where(x => x.EmpID == grp.Key.EmpID).Select(x => x.BasicSalary).Sum()) + PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Taxable == true && e.Type == "A").Select(a => a.Amount).Sum() + PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Taxable == true && e.Type == "D").Select(a => a.Amount).Sum() - PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "D").Select(a => a.Amount).Sum()) ?? 0),
                                   NetSalary = Math.Round(((dataList.Where(x => x.EmpID == grp.Key.EmpID).Select(x => x.BasicSalary).Sum()) + PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "A").Select(a => a.Amount).Sum() - PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "D").Select(a => a.Amount).Sum()) ?? 0),
                                   Tax = 0,
                               }).ToList();
                }
                else
                {


                    DetList = (from prmf in dataList
                               group prmf by new
                               {
                                   prmf.EmpID,
                                   prmf.FirstName,
                                   prmf.LastName,
                                   emp_payrollmfID = prmf.PayrollID,
                                   prmf.BasicSalary,
                                   prmf.PayDate,
                                   prmf.Status,
                               } into grp
                               orderby grp.Key.BasicSalary
                               select new PayrollDetailPaginationSearchSortModel
                               {
                                   UniqueID = grp.Key.emp_payrollmfID,
                                   Status = grp.Key.Status,
                                   EmployeeName = grp.Key.FirstName + " " + grp.Key.LastName,
                                   ScheduleName = dataList.Select(x => x.ScheduleName).FirstOrDefault(),
                                   PayDate = grp.Key.PayDate,
                                   BaseSalary = Math.Round(dataList.Where(x => x.EmpID == grp.Key.EmpID).Select(x => x.BasicSalary).Sum() ?? 0),
                                   Deduction = Math.Round(PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "D").Select(a => a.Amount).Sum()),
                                   Allowance = Math.Round(PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "A").Select(a => a.Amount).Sum()),
                                   GrossSalary = Math.Round(((dataList.Where(x => x.EmpID == grp.Key.EmpID).Select(x => x.BasicSalary).Sum()) + PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Taxable == true && e.Type == "A").Select(a => a.Amount).Sum() + PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Taxable == true && e.Type == "D").Select(a => a.Amount).Sum() - PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "D").Select(a => a.Amount).Sum()) ?? 0),
                                   NetSalary = Math.Round(((dataList.Where(x => x.EmpID == grp.Key.EmpID).Select(x => x.BasicSalary).Sum()) + PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "A").Select(a => a.Amount).Sum() - PayrollDtl.Where(e => e.EmployeeID == grp.Key.EmpID && e.Type == "D").Select(a => a.Amount).Sum()) ?? 0),
                                   Tax = 0,
                               }).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord).ToList();
                }

                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayEmpName = PFilter.VisibleColumnInfoList.IndexOf("EmployeeName") > -1;
                    DisplayBaseSalary = PFilter.VisibleColumnInfoList.IndexOf("BaseSalary") > -1;
                    DisplayAllowance = PFilter.VisibleColumnInfoList.IndexOf("Allowance") > -1;
                    DisplayDeduction = PFilter.VisibleColumnInfoList.IndexOf("Deduction") > -1;
                    DisplayTax = PFilter.VisibleColumnInfoList.IndexOf("Tax") > -1;
                    DisplayGrossSalary = PFilter.VisibleColumnInfoList.IndexOf("GrossSalary") > -1;
                    DisplayNetSalary = PFilter.VisibleColumnInfoList.IndexOf("NetSalary") > -1;

                    DetList = DetList.Where(c =>
                     (DisplayEmpName && c.EmployeeName.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayBaseSalary && c.BaseSalary.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                      (DisplayAllowance && c.Allowance.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayDeduction && c.Deduction.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                      (DisplayTax && c.Tax.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayGrossSalary && c.GrossSalary.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayNetSalary && c.NetSalary.ToString().ToLower().Contains(PFilter.SearchText.ToLower())

                     )))))))).ToList();
                }


                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "EmployeeName";

                OrderByEmpName = PFilter.OrderBy.IndexOf("EmployeeName") > -1;
                OrderByBaseSalary = PFilter.OrderBy.IndexOf("BaseSalary") > -1;
                OrderByAllowance = PFilter.OrderBy.IndexOf("Allowance") > -1;
                OrderByDeduction = PFilter.OrderBy.IndexOf("Deduction") > -1;
                OrderByGrossSalary = PFilter.OrderBy.IndexOf("GrossSalary") > -1;
                OrderByTax = PFilter.OrderBy.IndexOf("Tax") > -1;
                OrderByNetSalary = PFilter.OrderBy.IndexOf("NetSalary") > -1;
                Expression<Func<PayrollDetailPaginationSearchSortModel, string>> orderingFunction = (c =>
                                                                 OrderByEmpName ? c.EmployeeName :
                                                                 OrderByBaseSalary ? c.BaseSalary.ToString() :
                                                                 OrderByAllowance ? c.Allowance.ToString() :
                                                                 OrderByDeduction ? c.Deduction.ToString() :
                                                                 OrderByGrossSalary ? c.GrossSalary.ToString() :
                                                                 OrderByTax ? c.Tax.ToString() :
                                                                 OrderByNetSalary ? c.NetSalary.ToString() : ""
                                                              );

                IQueryable<PayrollDetailPaginationSearchSortModel> prDetList = DetList.AsQueryable();
                if (PFilter.IsOrderAsc)
                    prDetList = prDetList.OrderBy(orderingFunction);
                else
                    prDetList = prDetList.OrderByDescending(orderingFunction);

                objResult.TotalRecord = prDetList.Count();
                objResult.DataList = prDetList.ToList<Object>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }

        public static decimal[] GetPayschedueIds(this IRepository<pr_employee_payroll_mf> repository, decimal CompanyID)
        {
            IQueryable<pr_employee_payroll_mf> filteredData = repository.Queryable().Where(x => x.CompanyId == CompanyID && x.Status == "O");

            var Ids = filteredData.Select(x => x.PayScheduleID).ToList().Distinct().ToList();

            return Ids.ToArray();
        }
    }
}
