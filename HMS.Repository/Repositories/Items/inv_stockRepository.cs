using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Common;
using Repository.Pattern.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HMS.Repository.Repositories.Items
{
    public static class inv_stockRepository
    {
        public static PaginationResult Pagination(this IRepository<inv_stock> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<inv_stock, bool>> predicate = (e => e.CompanyId == CompanyID);

                bool DisplayName, DisplaySKU, DisplayDescription, DisplayRate;
                bool OrderByName, OrderBySKU, OrderByDescription, OrderByRate;

                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("ItemID") > -1;
                    DisplaySKU = PFilter.VisibleColumnInfoList.IndexOf("Quantity") > -1;
                    //DisplayDescription = PFilter.VisibleColumnInfoList.IndexOf("SaleDescription") > -1;
                    //DisplayRate = PFilter.VisibleColumnInfoList.IndexOf("SalePrice") > -1;
                    predicate = (c => c.CompanyId == CompanyID);
                }

                IQueryable<inv_stock> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";
                OrderByName = PFilter.OrderBy.IndexOf("ItemID") > -1;
                OrderBySKU = PFilter.OrderBy.IndexOf("Quantity") > -1;
                Expression<Func<inv_stock, string>> orderingFunction = (c =>
                                                OrderByName ? c.ItemID.ToString() :
                                                OrderBySKU ? c.Quantity.ToString() : ""
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
                    objResult.DataList = filteredData.Include(a => a.adm_company).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                      .Select(s => new
                      {
                          s.ID,
                          ItemId = s.ItemID,
                          Quantity = s.Quantity,
                          BatchSarial = s.BatchSarialNumber,
                          ExpiredDate = s.ExpiredWarrantyDate,
                      }).ToList<object>();
                }
                else
                {
                    objResult.DataList = filteredData.Include(a => a.adm_company).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
              .Select(s => new
              {
                  s.ID,
                  ItemId = s.ItemID,
                  Quantity = s.Quantity,
                  BatchSarial = s.BatchSarialNumber,
                  ExpiredDate = s.ExpiredWarrantyDate,
              }).ToList<object>();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
        public static PaginationResult PaginationWithParm(this IRepository<inv_stock> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<inv_stock, bool>> predicate = (e => e.CompanyId == CompanyID && e.Quantity > 0 && e.adm_item.POSItem == true &&
                (IgnorePaging ?
        e.adm_item.ID.ToString() == FilterID :
        e.adm_item.CategoryID.ToString() == FilterID));

                bool DisplayName, DisplaySKU, DisplayDescription, DisplayRate;
                bool OrderByName, OrderBySKU, OrderByDescription, OrderByRate;

                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("ItemID") > -1;
                    DisplaySKU = PFilter.VisibleColumnInfoList.IndexOf("Quantity") > -1;
                    //DisplayDescription = PFilter.VisibleColumnInfoList.IndexOf("SaleDescription") > -1;
                    //DisplayRate = PFilter.VisibleColumnInfoList.IndexOf("SalePrice") > -1;
                    predicate = (c => c.CompanyId == CompanyID && IgnorePaging ? c.adm_item.ItemTypeId.ToString() == FilterID : c.adm_item.CategoryID.ToString() == FilterID &&
                    (DisplayName && (c.adm_item.Name).Contains(PFilter.SearchText.ToLower()) ||
                    (DisplaySKU && (c.adm_item.SKU).ToLower().Contains(PFilter.SearchText.ToLower())
                  )));
                }

                IQueryable<inv_stock> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";
                OrderByName = PFilter.OrderBy.IndexOf("ItemID") > -1;
                OrderBySKU = PFilter.OrderBy.IndexOf("Quantity") > -1;
                Expression<Func<inv_stock, string>> orderingFunction = (c =>
                                                OrderByName ? c.adm_item.Name :
                                                OrderBySKU ? c.adm_item.SKU : ""
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
                    objResult.DataList = filteredData.Include(a => a.adm_company).Include(a => a.adm_item).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
              .Select(s => new
              {
                  ID = s.adm_item.ID,
                  Name = s.adm_item.Name,
                  Code = s.adm_item.SKU,
                  Stock = s.Quantity,
                  Unit = s.adm_item.sys_drop_down_value.Value,
                  Status = s.adm_item.IsActive == true && s.adm_item.SaveStatus == 1 ? "Draft"
                                   : s.adm_item.IsActive == false && s.adm_item.SaveStatus == 2 ? "InActive" : "Publish",
                  SalePrice = s.adm_item.SalePrice,
                  CostPrice = s.adm_item.CostPrice,
                  SaveStatus = s.adm_item.SaveStatus == 1 ? "Draft" : "Publish",
                  BatchSarialNumber = s.BatchSarialNumber,
                  ExpiredWarrantyDate = s.ExpiredWarrantyDate,
                  TypeValue = s.adm_item.sys_drop_down_value2.Value,
                  pos = s.adm_item.POSItem,
                  GroupId = s.adm_item.GroupId
              }).ToList<object>();
                }
                else
                {
                    objResult.DataList = filteredData.Include(a => a.adm_company).Include(a => a.adm_item).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
             .Select(s => new
             {
                 ID = s.adm_item.ID,
                 Name = s.adm_item.Name,
                 Code = s.adm_item.SKU,
                 Stock = s.Quantity,
                 Unit = s.adm_item.sys_drop_down_value.Value,
                 Status = s.adm_item.IsActive == true && s.adm_item.SaveStatus == 1 ? "Draft"
                                  : s.adm_item.IsActive == false && s.adm_item.SaveStatus == 2 ? "InActive" : "Publish",
                 SalePrice = s.adm_item.SalePrice,
                 CostPrice = s.adm_item.CostPrice,
                 SaveStatus = s.adm_item.SaveStatus == 1 ? "Draft" : "Publish",
                 BatchSarialNumber = s.BatchSarialNumber,
                 ExpiredWarrantyDate = s.ExpiredWarrantyDate,
                 TypeValue = s.adm_item.sys_drop_down_value2.Value,
                 pos = s.adm_item.POSItem,
                 GroupId = s.adm_item.GroupId
             }).ToList<object>();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }

        public static PaginationResult RestockPagination(this IRepository<inv_stock> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<inv_stock, bool>> predicate = (e => e.CompanyId == CompanyID);
                bool DisplayName, DisplaySKU, DisplayDescription, DisplayRate;
                bool OrderByName, OrderBySKU, OrderByDescription, OrderByRate;

                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Name") > -1;
                    DisplaySKU = PFilter.VisibleColumnInfoList.IndexOf("SKU") > -1;
                    DisplayDescription = PFilter.VisibleColumnInfoList.IndexOf("SaleDescription") > -1;
                    DisplayRate = PFilter.VisibleColumnInfoList.IndexOf("SalePrice") > -1;
                    predicate = (c => c.CompanyId == CompanyID &&
                    (DisplayName && (c.adm_item.Name).Contains(PFilter.SearchText.ToLower()) ||
                    (DisplaySKU && (c.adm_item.SKU).ToLower().Contains(PFilter.SearchText.ToLower())
                  )));
                }

                IQueryable<inv_stock> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";
                OrderByName = PFilter.OrderBy.IndexOf("Name") > -1;
                OrderBySKU = PFilter.OrderBy.IndexOf("SKU") > -1;
                Expression<Func<inv_stock, string>> orderingFunction = (c =>
                                                OrderByName ? c.adm_item.Name :
                                                OrderBySKU ? c.adm_item.SKU : ""
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
                    objResult.DataList = filteredData.GroupBy(z => z.ItemID)
.Select(g => new
{
    ID = g.Key,
    Name = g.FirstOrDefault().adm_item.Name,
    Code = g.FirstOrDefault().adm_item.SKU,
    Stock = g.Sum(s => s.Quantity),
    Unit = g.FirstOrDefault().adm_item.sys_drop_down_value.Value,
    SalePrice = g.FirstOrDefault().adm_item.SalePrice,
    CostPrice = g.FirstOrDefault().adm_item.CostPrice,
    BatchSarialNumber = g.FirstOrDefault().BatchSarialNumber,
    ExpiredWarrantyDate = g.FirstOrDefault().ExpiredWarrantyDate,
    TypeValue = g.FirstOrDefault().adm_item.sys_drop_down_value2.Value,
    MinimumQuantity = g.FirstOrDefault().adm_item.InventoryStockQuantity,
    IsBelowMinimumStock = g.Sum(s => s.Quantity) < g.FirstOrDefault().adm_item.InventoryStockQuantity
})
    .Where(g => g.IsBelowMinimumStock).OrderBy(g => g.ID)
    .Skip(PFilter.SkipRecord)
    .Take(PFilter.TakeRecord)
    .ToList<object>();

                }
                else
                {
                    objResult.DataList = filteredData.GroupBy(z => z.ItemID)
.Select(g => new
{
    ID = g.Key,
    Name = g.FirstOrDefault().adm_item.Name,
    Code = g.FirstOrDefault().adm_item.SKU,
    Stock = g.Sum(s => s.Quantity),
    Unit = g.FirstOrDefault().adm_item.sys_drop_down_value.Value,
    SalePrice = g.FirstOrDefault().adm_item.SalePrice,
    CostPrice = g.FirstOrDefault().adm_item.CostPrice,
    BatchSarialNumber = g.FirstOrDefault().BatchSarialNumber,
    ExpiredWarrantyDate = g.FirstOrDefault().ExpiredWarrantyDate,
    TypeValue = g.FirstOrDefault().adm_item.sys_drop_down_value2.Value,
    MinimumQuantity = g.FirstOrDefault().adm_item.InventoryStockQuantity,
    IsBelowMinimumStock = g.Sum(s => s.Quantity) <= g.FirstOrDefault().adm_item.InventoryStockQuantity
})
    .Where(g => g.IsBelowMinimumStock).OrderBy(g => g.ID)
    .Skip(PFilter.SkipRecord)
    .Take(PFilter.TakeRecord)
    .ToList<object>();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
        public static PaginationResult GetItemStockList(this IRepository<inv_stock> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<inv_stock, bool>> predicate = (e => e.CompanyId == CompanyID && e.ItemID.ToString() == FilterID);
                bool DisplayName, DisplaySKU, DisplayDescription, DisplayRate;
                bool OrderByName, OrderBySKU, OrderByDescription, OrderByRate;
                PFilter.SearchText = "";
                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Name") > -1;
                    DisplaySKU = PFilter.VisibleColumnInfoList.IndexOf("CostPrice") > -1;
                    DisplayDescription = PFilter.VisibleColumnInfoList.IndexOf("Unit") > -1;
                    DisplayRate = PFilter.VisibleColumnInfoList.IndexOf("Stock") > -1;
                    predicate = (c => c.CompanyId == CompanyID && 
                    (DisplayName && (c.adm_item.Name).Contains(PFilter.SearchText.ToLower()) ||
                    (DisplaySKU && (c.adm_item.SKU).ToLower().Contains(PFilter.SearchText.ToLower())
                  )));
                }

                IQueryable<inv_stock> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";
                OrderByName = PFilter.OrderBy.IndexOf("Name") > -1;
                OrderBySKU = PFilter.OrderBy.IndexOf("CostPrice") > -1;
                Expression<Func<inv_stock, string>> orderingFunction = (c =>
                                                OrderByName ? c.adm_item.Name :
                                                OrderBySKU ? c.adm_item.SKU : ""
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
                    objResult.DataList = filteredData.Include(a => a.adm_company).Include(a => a.adm_item).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
              .Select(s => new
              {
                  ID = s.adm_item.ID,
                  Name = s.adm_item.Name,
                  Code = s.adm_item.SKU,
                  Stock = s.Quantity,
                  Unit = s.adm_item.sys_drop_down_value.Value,
                  SalePrice = s.adm_item.SalePrice,
                  CostPrice = s.adm_item.CostPrice,
                  BatchSarialNumber = s.BatchSarialNumber,
                  ExpiredWarrantyDate = s.ExpiredWarrantyDate,
                  TypeValue = s.adm_item.sys_drop_down_value2.Value,
              }).ToList<object>();
                }
                else
                {
                    objResult.DataList = filteredData.Include(a => a.adm_company).Include(a => a.adm_item).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
             .Select(s => new
             {
                 ID = s.adm_item.ID,
                 Name = s.adm_item.Name,
                 Code = s.adm_item.SKU,
                 Stock = s.Quantity,
                 Unit = s.adm_item.sys_drop_down_value.Value,
                 SalePrice = s.adm_item.SalePrice,
                 CostPrice = s.adm_item.CostPrice,
                 BatchSarialNumber = s.BatchSarialNumber,
                 ExpiredWarrantyDate = s.ExpiredWarrantyDate,
                 TypeValue = s.adm_item.sys_drop_down_value2.Value,
             }).ToList<object>();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }

        public static PaginationResult ExpirePagination(this IRepository<inv_stock> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                DateTime currentDate = DateTime.Now;
                DateTime expiryDateLimit = currentDate.AddDays(90);
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<inv_stock, bool>> predicate = (e => e.CompanyId == CompanyID &&
                                                          e.Quantity > 0 &&
                                                          e.ExpiredWarrantyDate <= expiryDateLimit);
                bool DisplayName, DisplaySKU, DisplayDescription, DisplayRate;
                bool OrderByName, OrderBySKU, OrderByDescription, OrderByRate;

                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Name") > -1;
                    DisplaySKU = PFilter.VisibleColumnInfoList.IndexOf("SKU") > -1;
                    DisplayDescription = PFilter.VisibleColumnInfoList.IndexOf("SaleDescription") > -1;
                    DisplayRate = PFilter.VisibleColumnInfoList.IndexOf("SalePrice") > -1;
                    predicate = (c => c.CompanyId == CompanyID &&
                    (DisplayName && (c.adm_item.Name).Contains(PFilter.SearchText.ToLower()) ||
                    (DisplaySKU && (c.adm_item.SKU).ToLower().Contains(PFilter.SearchText.ToLower())
                  )));
                }

                IQueryable<inv_stock> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";
                OrderByName = PFilter.OrderBy.IndexOf("Name") > -1;
                OrderBySKU = PFilter.OrderBy.IndexOf("SKU") > -1;
                Expression<Func<inv_stock, string>> orderingFunction = (c =>
                                                OrderByName ? c.adm_item.Name :
                                                OrderBySKU ? c.adm_item.SKU : ""
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
                    objResult.DataList = filteredData.Include(a => a.adm_company).Include(a => a.adm_item).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
              .Select(s => new
              {
                  ID = s.adm_item.ID,
                  Name = s.adm_item.Name,
                  Code = s.adm_item.SKU,
                  Stock = s.Quantity,
                  Unit = s.adm_item.sys_drop_down_value.Value,
                  SalePrice = s.adm_item.SalePrice,
                  CostPrice = s.adm_item.CostPrice,
                  BatchSarialNumber = s.BatchSarialNumber,
                  ExpiredWarrantyDate = s.ExpiredWarrantyDate,
                  TypeValue = s.adm_item.sys_drop_down_value2.Value,
              }).ToList<object>();
                }
                else
                {
                    objResult.DataList = filteredData.Include(a => a.adm_company).Include(a => a.adm_item).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
             .Select(s => new
             {
                 ID = s.adm_item.ID,
                 Name = s.adm_item.Name,
                 Code = s.adm_item.SKU,
                 Stock = s.Quantity,
                 Unit = s.adm_item.sys_drop_down_value.Value,
                 SalePrice = s.adm_item.SalePrice,
                 CostPrice = s.adm_item.CostPrice,
                 BatchSarialNumber = s.BatchSarialNumber,
                 ExpiredWarrantyDate = s.ExpiredWarrantyDate,
                 TypeValue = s.adm_item.sys_drop_down_value2.Value,
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
