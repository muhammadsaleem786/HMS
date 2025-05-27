using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Common;
using Repository.Pattern.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace HMS.Repository.Repositories.Employee
{
    public static class pr_employee_mfRepository
    {
        public static PaginationResult Pagination(this IRepository<pr_employee_mf> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pr_employee_mf, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayName, DisplayDesignation, DisplayLocation, DisplayDepartment, DisplayStatus, DisplayModifiedDate;
                bool OrderByName, OrderByDesignation, OrderByLocation, OrderByDepartment, OrderByStatus, ModifiedDate;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Name") > -1;
                    DisplayDesignation = PFilter.VisibleColumnInfoList.IndexOf("Designation") > -1;
                    DisplayLocation = PFilter.VisibleColumnInfoList.IndexOf("Location") > -1;
                    DisplayDepartment = PFilter.VisibleColumnInfoList.IndexOf("Department") > -1;
                    DisplayModifiedDate = PFilter.VisibleColumnInfoList.IndexOf("ModifiedDate") > -1;
                    DisplayStatus = PFilter.VisibleColumnInfoList.IndexOf("Status") > -1;
                    predicate = (c => c.CompanyID == CompanyID &&
                    (DisplayName && (c.FirstName + " " + c.LastName).ToLower().Replace("  ", " ").Contains(PFilter.SearchText.ToLower())||
                    (DisplayDesignation && c.pr_designation.DesignationName.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    // (DisplayLocation && c.adm_company_location.LocationName.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayDepartment && c.pr_department.DepartmentName.ToLower().Contains(PFilter.SearchText.ToLower())
                    //(DisplayStatus && c.StatusList.Value.ToLower().Contains(PFilter.SearchText.ToLower())
                    ))));
                }

                IQueryable<pr_employee_mf> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ModifiedDate";

                OrderByName = PFilter.OrderBy.IndexOf("Name") > -1;
                OrderByDesignation = PFilter.OrderBy.IndexOf("Designation") > -1;
                OrderByLocation = PFilter.OrderBy.IndexOf("Location") > -1;
                OrderByDepartment = PFilter.OrderBy.IndexOf("Department") > -1;
                ModifiedDate = PFilter.OrderBy.IndexOf("ModifiedDate") > -1;
                OrderByStatus = PFilter.OrderBy.IndexOf("Status") > -1;


                Expression<Func<pr_employee_mf, dynamic>> orderingFunction;

                //if (PFilter.OrderBy == "ModifiedDate")
                //    orderingFunction = (c => (DateTime?)c.CreatedDate);

                //else
                orderingFunction = (c =>
                                                              OrderByName ? c.FirstName :
                                                              OrderByDesignation ? c.pr_designation.DesignationName :
                                                              //OrderByLocation ? c.adm_company_location.LocationName :
                                                               OrderByDepartment ? c.pr_department.DepartmentName :""
                                                              // OrderByStatus ? c.StatusList.Value.ToString() : ""
                                                              );

                if (PFilter.OrderBy == "ModifiedDate")
                {
                    filteredData = filteredData.OrderByDescending(d => (d.ModifiedDate == null ? d.ModifiedDate : d.CreatedDate)).AsQueryable();
                    objResult.TotalRecord = filteredData.Count();
                }
                else
                {
                    if (PFilter.IsOrderAsc)
                        filteredData = filteredData.OrderBy(orderingFunction);
                    else
                        filteredData = filteredData.OrderByDescending(orderingFunction);
                    objResult.TotalRecord = filteredData.Count();
                }

                if (IgnorePaging)
                {
                    //objResult.DataList = (from c in filteredData select c).ToList<object>();

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                        .Select(s => new
                        {
                            s.ID,
                            Name = s.FirstName + " " + s.LastName,
                            Designation = s.pr_designation.DesignationName,
                            Location = "", //s.adm_company_location.LocationName,
                            Department = s.pr_department.DepartmentName,
                            Status = s.StatusList.Value,
                            ModifiedDate = s.ModifiedDate,
                            CreatedDate = s.CreatedDate,
                        }).ToList<object>();
                }
                else
                {
                    //var PageResult = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                        .Select(s => new
                        {
                            s.ID,
                            Name = s.FirstName + " " + s.LastName,
                            Designation = s.pr_designation.DesignationName,
                            Location = "",//s.adm_company_location.LocationName,
                            Department = s.pr_department.DepartmentName,
                            Status = s.StatusList.Value,
                            ModifiedDate = s.ModifiedDate,
                            CreatedDate = s.CreatedDate,
                        }).ToList<object>();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
        public static PaginationResult BulkEmpPagination(this IRepository<pr_employee_mf> repository, EmpBulkUpdateModel BulkModel, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pr_employee_mf, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayName, DisplayDesignation, DisplayLocation, DisplayDepartment, DisplayStatus;
                bool OrderByName, OrderByDesignation, OrderByLocation, OrderByDepartment, OrderByStatus;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Name") > -1;
                    DisplayDesignation = PFilter.VisibleColumnInfoList.IndexOf("Designation") > -1;
                    DisplayLocation = PFilter.VisibleColumnInfoList.IndexOf("Location") > -1;
                    DisplayDepartment = PFilter.VisibleColumnInfoList.IndexOf("Department") > -1;
                    DisplayStatus = PFilter.VisibleColumnInfoList.IndexOf("Status") > -1;
                    predicate = (c => c.CompanyID == CompanyID &&
                    (DisplayName && (c.FirstName + " " + c.LastName).ToLower().Contains(PFilter.SearchText.ToLower())
                    //(DisplayDesignation && c.pr_designation.DesignationName.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    //(DisplayLocation && c.adm_company_location.LocationName.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    //(DisplayDepartment && c.pr_department.DepartmentName.ToLower().Contains(PFilter.SearchText.ToLower())
                    //(DisplayStatus && c.StatusList.Value.ToLower().Contains(PFilter.SearchText.ToLower())

                    ));
                }

                IQueryable<pr_employee_mf> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByName = PFilter.OrderBy.IndexOf("Name") > -1;
                OrderByDesignation = PFilter.OrderBy.IndexOf("Designation") > -1;
                OrderByLocation = PFilter.OrderBy.IndexOf("Location") > -1;
                OrderByDepartment = PFilter.OrderBy.IndexOf("Department") > -1;
                OrderByStatus = PFilter.OrderBy.IndexOf("Status") > -1;
                Expression<Func<pr_employee_mf, string>> orderingFunction = (c =>
                                                              OrderByName ? c.FirstName : ""
                                                              //OrderByDesignation ? c.pr_designation.DesignationName :
                                                              //OrderByLocation ? c.adm_company_location.LocationName :
                                                              //OrderByDepartment ? c.pr_department.DepartmentName :""
                                                              //OrderByStatus ? c.StatusList.Value.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {
                    //objResult.DataList = (from c in filteredData select c).ToList<object>();

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                        .Select(s => new
                        {
                            s.ID,
                            Name = s.FirstName + " " + s.LastName,
                            Designation = "",//s.pr_designation.DesignationName,
                            Location = "",//s.adm_company_location.LocationName,
                            Department = "",//s.pr_department.DepartmentName,
                            Status = "",// s.StatusList.Value,
                        }).ToList<object>();
                }
                else
                {
                    var PageResult = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                        .Select(s => new
                        {
                            s.ID,
                            Name = s.FirstName + " " + s.LastName,
                            Designation = "",//s.pr_designation.DesignationName,
                            Location = "",//s.adm_company_location.LocationName,
                            Department = "",//s.pr_department.DepartmentName,
                            Status = "",// s.StatusList.Value,
                        }).ToList<object>();

                    //objResult.DataList = (from c in PageResult select c).ToList<object>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
    }
}
