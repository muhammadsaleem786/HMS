using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Common;
using Repository.Pattern.Ef6;
using Repository.Pattern.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HMS.Repository.Repositories.Items
{
    public static class adm_itemRepository
    {
        public static PaginationResult Pagination(this IRepository<adm_item> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<adm_item, bool>> predicate = (e => e.CompanyId == CompanyID);

                bool DisplayName, DisplaySKU, DisplayDescription, DisplayRate;
                bool OrderByName, OrderBySKU, OrderByDescription, OrderByRate;

                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Name") > -1;
                    DisplaySKU = PFilter.VisibleColumnInfoList.IndexOf("SKU") > -1;
                    DisplayDescription = PFilter.VisibleColumnInfoList.IndexOf("SaleDescription") > -1;
                    DisplayRate = PFilter.VisibleColumnInfoList.IndexOf("SalePrice") > -1;
                    predicate = (c => c.CompanyId == CompanyID &&
                    (DisplayName && (c.Name).Contains(PFilter.SearchText.ToLower()) ||
                    (DisplaySKU && (c.SKU).ToLower().Contains(PFilter.SearchText.ToLower())
                  )));
                }

                IQueryable<adm_item> filteredData = repository.Queryable().Where(predicate);
                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";
                OrderByName = PFilter.OrderBy.IndexOf("Name") > -1;
                OrderBySKU = PFilter.OrderBy.IndexOf("SKU") > -1;
                Expression<Func<adm_item, string>> orderingFunction = (c =>
                                                OrderByName ? c.Name :
                                                OrderBySKU ? c.SKU : ""
                                                );
                if (PFilter.OrderBy == "ID")
                {
                    filteredData = filteredData.OrderByDescending(d => (d.ID)).AsQueryable();
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
                    objResult.DataList = filteredData.Include(a => a.adm_company).Include(a => a.inv_stock).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                      .Select(s => new ItemResponse
                      {
                          ID = s.ID,
                          Name = s.Name,
                          Code = s.SKU,
                          Stock = s.inv_stock.Any() ? s.inv_stock.Sum(a => a.Quantity) : 0,
                          Unit = s.sys_drop_down_value.Value,
                          Status = s.IsActive == true && s.SaveStatus == 1 ? "Draft"
                                   : s.IsActive == false && s.SaveStatus == 2 ? "InActive" : "Publish",
                          SalePrice = s.SalePrice,
                          CostPrice = s.CostPrice,
                          SaveStatus = s.SaveStatus == 1 ? "Draft" : "Publish",
                          Group = s.sys_drop_down_value3.Value,
                      }).ToList<object>();
                }
                else
                {
                    objResult.DataList = filteredData.Include(a => a.adm_company).Include(a => a.inv_stock).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
              .Select(s => new ItemResponse
              {
                  ID = s.ID,
                  Name = s.Name,
                  Code = s.SKU,
                  Stock = s.inv_stock.Any() ? s.inv_stock.Sum(a => a.Quantity) : 0,
                  Unit = s.sys_drop_down_value.Value,
                  Status = s.IsActive == true && s.SaveStatus == 1 ? "Draft"
                                   : s.IsActive == false && s.SaveStatus == 2 ? "InActive" : "Publish",
                  SalePrice = s.SalePrice,
                  CostPrice = s.CostPrice,
                  SaveStatus = s.SaveStatus == 1 ? "Draft" : "Publish",
                  Group = s.sys_drop_down_value3.Value,
              }).ToList<object>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
        public static PaginationResult PaginationWithParm(this IRepository<adm_item> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<adm_item, bool>> predicate;
                bool DisplayName, DisplaySKU, DisplayDescription, DisplayRate;
                bool OrderByName, OrderBySKU, OrderByDescription, OrderByRate;
                if (IgnorePaging)
                {
                    predicate = (e => e.CompanyId == CompanyID && e.ItemTypeId.ToString() == FilterID);
                    if (!string.IsNullOrEmpty(PFilter.SearchText))
                    {
                        DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Name") > -1;
                        DisplaySKU = PFilter.VisibleColumnInfoList.IndexOf("SKU") > -1;
                        DisplayDescription = PFilter.VisibleColumnInfoList.IndexOf("SaleDescription") > -1;
                        DisplayRate = PFilter.VisibleColumnInfoList.IndexOf("SalePrice") > -1;
                        predicate = (c => c.CompanyId == CompanyID && c.ItemTypeId.ToString() == FilterID &&
                        (DisplayName && (c.Name).Contains(PFilter.SearchText.ToLower()) ||
                        (DisplaySKU && (c.SKU).ToLower().Contains(PFilter.SearchText.ToLower())
                      )));
                    }
                }
                else
                {
                    predicate = (e => e.CompanyId == CompanyID && e.CategoryID.ToString() == FilterID);
                    if (!string.IsNullOrEmpty(PFilter.SearchText))
                    {
                        DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Name") > -1;
                        DisplaySKU = PFilter.VisibleColumnInfoList.IndexOf("SKU") > -1;
                        DisplayDescription = PFilter.VisibleColumnInfoList.IndexOf("SaleDescription") > -1;
                        DisplayRate = PFilter.VisibleColumnInfoList.IndexOf("SalePrice") > -1;
                        predicate = (c => c.CompanyId == CompanyID && c.CategoryID.ToString() == FilterID &&
                        (DisplayName && (c.Name).Contains(PFilter.SearchText.ToLower()) ||
                        (DisplaySKU && (c.SKU).ToLower().Contains(PFilter.SearchText.ToLower())
                      )));
                    }
                }

                IQueryable<adm_item> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";
                OrderByName = PFilter.OrderBy.IndexOf("Name") > -1;
                OrderBySKU = PFilter.OrderBy.IndexOf("SKU") > -1;
                Expression<Func<adm_item, string>> orderingFunction = (c =>
                                                OrderByName ? c.Name :
                                                OrderBySKU ? c.SKU : ""
                                                );

                if (PFilter.OrderBy == "ID")
                {
                    filteredData = filteredData.OrderByDescending(d => (d.ID)).AsQueryable();
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
                    var result = filteredData.Include(a => a.adm_company).Include(a => a.inv_stock).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
              .Select(s => new
              {
                  ID = s.ID,
                  Name = s.Name,
                  Code = s.SKU,
                  Stock = s.inv_stock.Any() ? s.inv_stock.Sum(a => a.Quantity) : 0,
                  Unit = s.sys_drop_down_value.Value,
                  Status = s.IsActive == true && s.SaveStatus == 1 ? "Draft"
                                   : s.IsActive == false && s.SaveStatus == 2 ? "InActive" : "Publish",
                  SalePrice = s.SalePrice,
                  CostPrice = s.CostPrice,
                  SaveStatus = s.SaveStatus == 1 ? "Draft" : "Publish",
                  BatchSarialNumber = s.inv_stock.Select(z => new { BatchSarialNumbers = z.BatchSarialNumber }).ToList(),
                  TypeValue = s.sys_drop_down_value2.Value,
                  Group = s.sys_drop_down_value3.Value
              }).OrderByDescending(a => a.ID).ToList();
                    objResult.DataList = result.Select(s => new ItemResponse
                    {
                        ID = s.ID,
                        Name = s.Name,
                        Code = s.Code,
                        Stock = s.Stock,
                        Unit = s.Unit,
                        Status = s.Status,
                        SalePrice = s.SalePrice,
                        CostPrice = s.CostPrice,
                        SaveStatus = s.SaveStatus,
                        BatchSarialNumber = string.Join(", ", s.BatchSarialNumber),
                        TypeValue = s.TypeValue,
                        Group = s.Group,
                    }).ToList<object>();
                }
                else
                {
                    var result = filteredData.Include(a => a.adm_company).Include(a => a.inv_stock).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
              .Select(s => new
              {
                  ID = s.ID,
                  Name = s.Name,
                  Code = s.SKU,
                  Stock = s.inv_stock.Any() ? s.inv_stock.Sum(a => a.Quantity) : 0,
                  Unit = s.sys_drop_down_value.Value,
                  Status = s.IsActive == true && s.SaveStatus == 1 ? "Draft"
                                   : s.IsActive == false && s.SaveStatus == 2 ? "InActive" : "Publish",
                  SalePrice = s.SalePrice,
                  CostPrice = s.CostPrice,
                  SaveStatus = s.SaveStatus == 1 ? "Draft" : "Publish",
                  BatchSarialNumber = s.inv_stock.Select(z => new { BatchSarialNumbers = z.BatchSarialNumber }).ToList(),
                  TypeValue = s.sys_drop_down_value2.Value,
                  Group = s.sys_drop_down_value3.Value,
              }).OrderByDescending(a => a.ID).ToList();
                    objResult.DataList = result.Select(s => new ItemResponse
                    {
                        ID = s.ID,
                        Name = s.Name,
                        Code = s.Code,
                        Stock = s.Stock,
                        Unit = s.Unit,
                        Status = s.Status,
                        SalePrice = s.SalePrice,
                        CostPrice = s.CostPrice,
                        SaveStatus = s.SaveStatus,
                        BatchSarialNumber = string.Join(", ", s.BatchSarialNumber),
                        TypeValue = s.TypeValue,
                        Group = s.Group
                    }).ToList<object>();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
        public static PaginationResult PaginationWithGroupParm(this IRepository<adm_item> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<adm_item, bool>> predicate = (e => e.CompanyId == CompanyID && (FilterID == "0" || e.GroupId.ToString() == FilterID));

                bool DisplayName, DisplaySKU, DisplayDescription, DisplayRate;
                bool OrderByName, OrderBySKU, OrderByDescription, OrderByRate;

                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Name") > -1;
                    DisplaySKU = PFilter.VisibleColumnInfoList.IndexOf("SKU") > -1;
                    DisplayDescription = PFilter.VisibleColumnInfoList.IndexOf("SaleDescription") > -1;
                    DisplayRate = PFilter.VisibleColumnInfoList.IndexOf("SalePrice") > -1;
                    predicate = (c => c.CompanyId == CompanyID && (FilterID == "0" || c.GroupId.ToString() == FilterID) &&
                    (DisplayName && (c.Name).Contains(PFilter.SearchText.ToLower()) ||
                    (DisplaySKU && (c.SKU).ToLower().Contains(PFilter.SearchText.ToLower())
                  )));
                }
                IQueryable<adm_item> filteredData = repository.Queryable().Where(predicate);
                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";
                OrderByName = PFilter.OrderBy.IndexOf("Name") > -1;
                OrderBySKU = PFilter.OrderBy.IndexOf("SKU") > -1;
                Expression<Func<adm_item, string>> orderingFunction = (c =>
                                                OrderByName ? c.Name :
                                                OrderBySKU ? c.SKU : ""
                                                );
                if (PFilter.OrderBy == "ID")
                {
                    filteredData = filteredData.OrderByDescending(d => (d.ID)).AsQueryable();
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
                    objResult.DataList = filteredData.Include(a => a.adm_company).Include(a => a.inv_stock).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                      .Select(s => new ItemResponse
                      {
                          ID = s.ID,
                          Name = s.Name,
                          Code = s.SKU,
                          Stock = s.inv_stock.Any() ? s.inv_stock.Sum(a => a.Quantity) : 0,
                          Unit = s.sys_drop_down_value.Value,
                          Status = s.IsActive == true && s.SaveStatus == 1 ? "Draft"
                                   : s.IsActive == false && s.SaveStatus == 2 ? "InActive" : "Publish",
                          SalePrice = s.SalePrice,
                          CostPrice = s.CostPrice,
                          SaveStatus = s.SaveStatus == 1 ? "Draft" : "Publish",
                          Group = s.sys_drop_down_value3.Value,
                      }).ToList<object>();
                }
                else
                {
                    objResult.DataList = filteredData.Include(a => a.adm_company).Include(a => a.inv_stock).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
              .Select(s => new ItemResponse
              {
                  ID = s.ID,
                  Name = s.Name,
                  Code = s.SKU,
                  Stock = s.inv_stock.Any() ? s.inv_stock.Sum(a => a.Quantity) : 0,
                  Unit = s.sys_drop_down_value.Value,
                  Status = s.IsActive == true && s.SaveStatus == 1 ? "Draft"
                                   : s.IsActive == false && s.SaveStatus == 2 ? "InActive" : "Publish",
                  SalePrice = s.SalePrice,
                  CostPrice = s.CostPrice,
                  SaveStatus = s.SaveStatus == 1 ? "Draft" : "Publish",
                  Group = s.sys_drop_down_value3.Value,
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
