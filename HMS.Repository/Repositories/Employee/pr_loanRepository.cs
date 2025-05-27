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
    public static class pr_loanRepository
    {
        public static PaginationResult Pagination(this IRepository<pr_loan> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pr_loan, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayEmpName, DisplayDescription, DisplayLoanDate, DisplayLoan,
                    DisplayPayment, DisplayBalance;
                bool OrderByEmpName, OrderByDescription, OrderByLoanDate, OrderByLoan,
                    OrderByPayment, OrderByBalance;




                IQueryable<pr_loan> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";



                //objResult.TotalRecord = filteredData.Count();

                var list = new List<LoanPaginationModel>();
                if (IgnorePaging)
                {
                    list = filteredData.Include(x => x.pr_loan_payment_dt)
                    .Include(x => x.pr_employee_mf).Select(s => new
                    {
                        s.ID,
                        EmpName = s.pr_employee_mf.FirstName + " " + s.pr_employee_mf.LastName,
                        s.Description,
                        LoanDate = s.LoanDate,
                        LoanAmount = s.LoanAmount,
                        AdjusmentAmount = (double)(s.AdjustmentBy == null ? 0 : (s.AdjustmentType == "C" ? (s.AdjustmentAmount ?? 0) : (s.AdjustmentAmount ?? 0) * -1)),
                        LoanDetail = s.pr_loan_payment_dt.Select(z => new
                        {
                            LoanAmount = z.Amount,
                            AdjustmentAmount = (double)(z.AdjustmentBy == null ? 0 : (z.AdjustmentType == "C" ? (z.AdjustmentAmount ?? 0) : (z.AdjustmentAmount ?? 0) * -1)),
                        }),

                    }).ToList().Select(y => new LoanPaginationModel
                    {
                        ID = y.ID,
                        EmpName = y.EmpName,
                        Description = y.Description,
                        LoanDate = y.LoanDate.ToString("dd/MM/yyyy"),
                        LoanAmount = y.LoanAmount + y.AdjusmentAmount,
                        PaymentAmount = y.LoanDetail.Sum(x => x.LoanAmount) + y.LoanDetail.Sum(x => x.AdjustmentAmount),
                        Balance = (y.LoanAmount + y.AdjusmentAmount) - (y.LoanDetail.Sum(x => x.LoanAmount) + y.LoanDetail.Sum(x => x.AdjustmentAmount))
                    }).ToList();

                    //list = filteredData.Include(x => x.pr_loan_payment_dt)
                    //   .Include(x => x.pr_employee_mf).Select(s => new
                    //   {
                    //       s.ID,
                    //       EmpName = s.pr_employee_mf.FirstName + " " + s.pr_employee_mf.LastName,
                    //       s.Description,
                    //       LoanDate = s.LoanDate,
                    //       LoanAmount = s.LoanAmount,
                    //       AdjusmentAmount = (double)(s.AdjustmentBy == null ? 0 : (s.AdjustmentType == "C" ? (s.AdjustmentAmount ?? 0) : (s.AdjustmentAmount ?? 0) * -1)),
                    //       LoanDetail = s.pr_loan_payment_dt.Select(z => new
                    //       {
                    //           LoanAmount = z.Amount,
                    //           AdjustmentAmount = (double)(z.AdjustmentBy == null ? 0 : (z.AdjustmentType == "C" ? (z.AdjustmentAmount ?? 0) : (z.AdjustmentAmount ?? 0) * -1)),
                    //       }),

                    //   }).ToList().Select(y => new
                    //   {
                    //       y.ID,
                    //       EmpName = y.EmpName,
                    //       y.Description,
                    //       y.LoanDate,
                    //       LoanAmount = y.LoanAmount + y.AdjusmentAmount,
                    //       PaymentAmount = y.LoanDetail.Sum(x => x.LoanAmount) + y.LoanDetail.Sum(x => x.AdjustmentAmount),
                    //       Balance = (y.LoanAmount + y.AdjusmentAmount) - (y.LoanDetail.Sum(x => x.LoanAmount) + y.LoanDetail.Sum(x => x.AdjustmentAmount))
                    //   }).ToList();
                }
                else
                {
                    var PageResult = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                    //objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord).Include(x => x.pr_loan_payment_dt)
                    //    .Include(x => x.pr_employee_mf).AsEnumerable().Select(s => new
                    //    {
                    //        s.ID,
                    //        EmpName = s.pr_employee_mf.FirstName + " " + s.pr_employee_mf.LastName,
                    //        s.Description,
                    //        LoanDate = s.LoanDate,
                    //        LoanAmount = s.LoanAmount + (double)(s.AdjustmentBy == null ? 0 : (s.AdjustmentType == "C" ? (s.AdjustmentAmount ?? 0) : (s.AdjustmentAmount ?? 0) * -1)),
                    //        LoanDetail = s.pr_loan_payment_dt.Select(z => new
                    //        {
                    //            LoanAmount = z.Amount + (double)(z.AdjustmentBy == null ? 0 : (z.AdjustmentType == "C" ? (z.AdjustmentAmount ?? 0) : (z.AdjustmentAmount ?? 0) * -1)),
                    //        }),

                    //    }).ToList().Select(y => new
                    //    {
                    //        y.ID,
                    //        EmpName = y.EmpName,
                    //        y.Description,
                    //        y.LoanDate,
                    //        LoanAmount = y.LoanAmount,
                    //        PaymentAmount = y.LoanDetail.Sum(x => x.LoanAmount),
                    //        Balance = y.LoanAmount - y.LoanDetail.Sum(x => x.LoanAmount)
                    //    }).ToList<object>();



                    list = filteredData.OrderBy(x => x.ID).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord).Include(x => x.pr_loan_payment_dt)
                      .Include(x => x.pr_employee_mf).Select(s => new
                      {
                          s.ID,
                          EmpName = s.pr_employee_mf.FirstName + " " + s.pr_employee_mf.LastName,
                          s.Description,
                          LoanDate = s.LoanDate,
                          LoanAmount = s.LoanAmount,
                          AdjusmentAmount = (double)(s.AdjustmentBy == null ? 0 : (s.AdjustmentType == "C" ? (s.AdjustmentAmount ?? 0) : (s.AdjustmentAmount ?? 0) * -1)),
                          LoanDetail = s.pr_loan_payment_dt.Select(z => new
                          {
                              LoanAmount = z.Amount,
                              AdjustmentAmount = (double)(z.AdjustmentBy == null ? 0 : (z.AdjustmentType == "C" ? (z.AdjustmentAmount ?? 0) : (z.AdjustmentAmount ?? 0) * -1)),
                          }),

                      }).ToList().Select(y => new LoanPaginationModel
                      {
                          ID = y.ID,
                          EmpName = y.EmpName,
                          Description = y.Description,
                          LoanDate = y.LoanDate.ToString("dd/MM/yyyy"),
                          LoanAmount = y.LoanAmount + y.AdjusmentAmount,
                          PaymentAmount = y.LoanDetail.Sum(x => x.LoanAmount) + y.LoanDetail.Sum(x => x.AdjustmentAmount),
                          Balance = (y.LoanAmount + y.AdjusmentAmount) - (y.LoanDetail.Sum(x => x.LoanAmount) + y.LoanDetail.Sum(x => x.AdjustmentAmount))
                      }).ToList();

                }

                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayEmpName = PFilter.VisibleColumnInfoList.IndexOf("EmpName") > -1;
                    DisplayDescription = PFilter.VisibleColumnInfoList.IndexOf("Description") > -1;
                    DisplayLoanDate = PFilter.VisibleColumnInfoList.IndexOf("LoanDate") > -1;
                    DisplayLoan = PFilter.VisibleColumnInfoList.IndexOf("LoanAmount") > -1;
                    DisplayPayment = PFilter.VisibleColumnInfoList.IndexOf("PaymentAmount") > -1;
                    DisplayBalance = PFilter.VisibleColumnInfoList.IndexOf("Balance") > -1;


                    // list = list.Where(c =>
                    //(DisplayName && c.ScheduleName.ToLower().Contains(PFilter.SearchText.ToLower())) ||
                    //(DisplayStatus && c.Status.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    // (DisplayPayPeriod && c.PayPeriod.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    //(DisplayPayDate && c.PayDate.ToString("dd/MM/yyyy").Contains(PFilter.SearchText.ToLower()) ||
                    //(DisplayEmployees && c.NoOfEmp.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    //(DisplayTax && c.Tax.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    // (DisplayNetSalary && c.NetSalary.ToString().ToLower().Contains(PFilter.SearchText.ToLower())
                    ////
                    //)))))))).ToList();


                    list = list.Where(c =>
                    (DisplayEmpName && (c.EmpName).ToLower().Contains(PFilter.SearchText.ToLower())) ||
                     (DisplayDescription && c.Description.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayLoanDate && c.LoanDate.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayLoan && c.LoanAmount.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayPayment && (c.PaymentAmount).ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayBalance && (c.Balance).ToString().ToLower().Contains(PFilter.SearchText.ToLower())
                    )))))).ToList();
                }


                OrderByEmpName = PFilter.OrderBy.IndexOf("EmpName") > -1;
                OrderByDescription = PFilter.OrderBy.IndexOf("Description") > -1;
                OrderByLoanDate = PFilter.OrderBy.IndexOf("LoanDate") > -1;
                OrderByLoan = PFilter.OrderBy.IndexOf("LoanAmount") > -1;
                OrderByPayment = PFilter.OrderBy.IndexOf("Payment") > -1;
                OrderByBalance = PFilter.OrderBy.IndexOf("Balance") > -1;

                Expression<Func<LoanPaginationModel, string>> orderingFunction = (c =>
                                                              OrderByEmpName ? (c.EmpName).ToLower() :
                                                              OrderByDescription ? c.Description :
                                                              OrderByLoanDate ? c.LoanDate :
                                                              OrderByLoan ? c.LoanAmount.ToString() :
                                                              OrderByPayment ? c.LoanAmount.ToString() :
                                                               OrderByBalance ? c.Balance.ToString() : ""
                                                              );

                IQueryable<LoanPaginationModel> prList = list.AsQueryable();
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
        public static PaginationResult PaginationDetail(this IRepository<pr_loan> repository, decimal LoanID, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pr_loan, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayName, DisplayDescription, DisplayLoanDate, DisplayLoan,
                    DisplayPayment, DisplayBalance;
                bool OrderByName, OrderByDescription, OrderByLoanDate, OrderByLoan,
                    OrderByPayment, OrderByBalance;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Name") > -1;
                    DisplayDescription = PFilter.VisibleColumnInfoList.IndexOf("Description") > -1;
                    DisplayLoanDate = PFilter.VisibleColumnInfoList.IndexOf("LoanDate") > -1;
                    DisplayLoan = PFilter.VisibleColumnInfoList.IndexOf("LoanAmount") > -1;
                    DisplayPayment = PFilter.VisibleColumnInfoList.IndexOf("Name") > -1;
                    DisplayBalance = PFilter.VisibleColumnInfoList.IndexOf("Description") > -1;
                    predicate = (c =>
                    c.CompanyID == CompanyID &&
                    (DisplayName && (c.pr_employee_mf.FirstName + "" + c.pr_employee_mf.LastName).ToLower().Contains(PFilter.SearchText.ToLower())
                    || (DisplayDescription && c.Description.ToLower().Contains(PFilter.SearchText.ToLower())) ||
                    (DisplayLoanDate && c.LoanDate.ToString().ToLower().Contains(PFilter.SearchText.ToLower())) ||
                    (DisplayLoan && c.LoanAmount.ToString().ToLower().Contains(PFilter.SearchText.ToLower())) ||
                    (DisplayPayment && c.sys_drop_down_value.Value.ToString().ToLower().Contains(PFilter.SearchText.ToLower()))
                    //(DisplayBalance && c.sys_drop_down_value.Value.ToLower().Contains(PFilter.SearchText.ToLower())) 
                    ));
                }

                IQueryable<pr_loan> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByName = PFilter.OrderBy.IndexOf("Name") > -1;
                OrderByDescription = PFilter.OrderBy.IndexOf("Description") > -1;
                OrderByLoanDate = PFilter.OrderBy.IndexOf("LoanDate") > -1;
                OrderByLoan = PFilter.OrderBy.IndexOf("LoanAmount") > -1;
                OrderByPayment = PFilter.OrderBy.IndexOf("Payment") > -1;
                //OrderByBalance = PFilter.OrderBy.IndexOf("Description") > -1;

                Expression<Func<pr_loan, string>> orderingFunction = (c =>
                                                              OrderByName ? c.pr_employee_mf.FirstName :
                                                              OrderByDescription ? c.Description :
                                                              OrderByLoanDate ? c.LoanDate.ToString() :
                                                              OrderByLoan ? c.LoanAmount.ToString() :
                                                              OrderByPayment ? c.sys_drop_down_value.Value.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {
                    objResult.DataList = filteredData.Include(x => x.pr_employee_mf).Select(s => new
                    {
                        s.ID,
                        Name = s.pr_employee_mf.FirstName + "" + s.pr_employee_mf.LastName,
                        s.Description,
                        s.LoanDate,
                        s.LoanAmount,
                        s.sys_drop_down_value.Value,

                    }).ToList<object>();

                    // objResult.DataList = (from c in filteredData select c).ToList<object>();
                }
                else
                {
                    var PageResult = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                        .Include(x => x.pr_employee_mf).Select(s => new
                        {
                            s.ID,
                            Name = s.pr_employee_mf.FirstName + "" + s.pr_employee_mf.LastName,
                            s.Description,
                            s.LoanDate,
                            s.LoanAmount,
                            s.sys_drop_down_value.Value,
                        }).ToList<object>();
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
