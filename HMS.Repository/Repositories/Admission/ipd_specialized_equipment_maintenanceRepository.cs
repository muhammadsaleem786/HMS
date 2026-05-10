using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Common;
using Repository.Pattern.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Repository.Repositories.Admission
{
    public static class ipd_specialized_equipment_maintenanceRepository
    {
        public static PaginationResult Pagination(this IRepository<ipd_specialized_equipment_maintenance> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<ipd_specialized_equipment_maintenance, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayPatientName, DisplayDOB, DisplayEmail, DisplayMobile, DisplayMRNO, DisplayCNIC;
                bool OrderByPatientName, OrderByDOB, OrderByEmail, OrderByMobile, OrderByMRNO, OrderByCNIC;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayPatientName = PFilter.VisibleColumnInfoList.IndexOf("EquipmentName") > -1;
                    DisplayDOB = PFilter.VisibleColumnInfoList.IndexOf("PurchaseDate") > -1;
                    predicate = (c => c.CompanyID == CompanyID &&
                    (DisplayPatientName && c.EquipmentName.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayDOB && c.PurchaseDate.ToString().Contains(PFilter.SearchText.ToLower()))));
                }

                IQueryable<ipd_specialized_equipment_maintenance> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByPatientName = PFilter.OrderBy.IndexOf("EquipmentName") > -1;
                OrderByDOB = PFilter.OrderBy.IndexOf("PurchaseDate") > -1;


                Expression<Func<ipd_specialized_equipment_maintenance, string>> orderingFunction = (c =>
                                                              OrderByPatientName ? c.EquipmentName.ToString() :
                                                                OrderByDOB ? c.PurchaseDate.ToString()  : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                filteredData = filteredData.OrderByDescending(d => (d.ID)).AsQueryable();
                objResult.TotalRecord = filteredData.Count();
                if (IgnorePaging)
                {

                    objResult.DataList = filteredData
                        .Select(s => new
                        {
                            s.ID,
                            EquipmentName = s.EquipmentName,
                            PurchaseDate = s.PurchaseDate,
                            s.Department,s.InstallationDate,
                            s.SupplierNameAndAddress,
                            s.WarrantyPeriod,
                            s.CondemnationDate,
                            s.ScheduledMaintenance,
                            s.MaintenanceDate,
                            s.CalibrationDate,
                            s.ServiceRepairDate,
                            s.Remarks,
                        }).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           EquipmentName = s.EquipmentName,
                           PurchaseDate = s.PurchaseDate,
                           s.Department,
                           s.InstallationDate,
                           s.SupplierNameAndAddress,
                           s.WarrantyPeriod,
                           s.CondemnationDate,
                           s.ScheduledMaintenance,
                           s.MaintenanceDate,
                           s.CalibrationDate,
                           s.ServiceRepairDate,
                           s.Remarks,
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
