using HMS.Entities.CustomModel;
using HMS.Entities.Enum;
using HMS.Entities.Models;
using HMS.Service;
using HMS.Service.Services.Admin;
using HMS.Service.Services.Employee;
using HMS.Web.API.Common;
using HMS.Web.API.Filters;
using HMS.Web.API.Interface;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.OleDb;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace HMS.Web.API.Areas.Employee
{
    [JwtAuthentication]
    public class pr_employee_mfController : ApiController, IERPAPIInterface<pr_employee_mf>, IDisposable
    {
        private readonly Ipr_employee_mfService _service;
        private readonly Isys_drop_down_valueService _sys_drop_down_valueService;
        private readonly Ipr_departmentService _pr_departmentService;
        private readonly Ipr_designationService _pr_designationService;
        private readonly Ipr_pay_scheduleService _pr_pay_scheduleService;
        private readonly Ipr_allowanceService _pr_allowanceService;
        private readonly Ipr_deduction_contributionService _pr_deduction_contributionService;
        private readonly Ipr_leave_typeService _pr_leave_typeService;
        private readonly Ipr_employee_allowanceService _pr_employee_allowanceService;
        private readonly Ipr_employee_ded_contributionService _pr_employee_ded_contributionService;
        private readonly Ipr_employee_leaveService _pr_employee_leaveService;
        private readonly Ipr_employee_documentService _pr_employee_documentService;
        private readonly Ipr_employee_dependentService _pr_employee_dependentService;
        private readonly Ipr_leave_applicationService _pr_leave_applicationService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        public pr_employee_mfController(IUnitOfWorkAsync unitOfWorkAsync, Ipr_employee_mfService Service, Ipr_employee_mfService pr_employee_mfService
            , Isys_drop_down_valueService sys_drop_down_valueService, Ipr_departmentService pr_departmentService
            , Ipr_designationService pr_designationService, Ipr_pay_scheduleService pr_pay_scheduleService, Ipr_allowanceService pr_allowanceService, Ipr_deduction_contributionService pr_deduction_contributionService
            , Ipr_leave_typeService pr_leave_typeService, Ipr_employee_allowanceService pr_employee_allowanceService
            , Ipr_employee_ded_contributionService pr_employee_ded_contributionService, Ipr_employee_leaveService pr_employee_leaveService
            , Ipr_employee_documentService pr_employee_documentService,
            Ipr_leave_applicationService pr_Leave_ApplicationService,
            pr_employee_dependentService pr_employee_dependentService
         )
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _sys_drop_down_valueService = sys_drop_down_valueService;
            _pr_departmentService = pr_departmentService;
            _pr_designationService = pr_designationService;
            _pr_leave_applicationService = pr_Leave_ApplicationService;
            _pr_pay_scheduleService = pr_pay_scheduleService;
            _pr_allowanceService = pr_allowanceService;
            _pr_deduction_contributionService = pr_deduction_contributionService;
            _pr_leave_typeService = pr_leave_typeService;
            _pr_employee_allowanceService = pr_employee_allowanceService;
            _pr_employee_ded_contributionService = pr_employee_ded_contributionService;
            _pr_employee_leaveService = pr_employee_leaveService;
            _pr_employee_documentService = pr_employee_documentService;
            _pr_employee_dependentService = pr_employee_dependentService;
            _service = Service;
        }
        public async Task<ResponseInfo> Save(pr_employee_mf Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                if (!ModelState.IsValid)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.BadRequest;
                    return objResponse;
                }

                decimal CompanyID = Request.CompanyID();
                decimal LoginID = Request.LoginID();

                pr_employee_mf emp = _service.Queryable().Where(x => x.CompanyID == CompanyID && (x.FirstName + " " + x.LastName).ToLower() == (Model.FirstName + " " + Model.LastName).ToLower()).FirstOrDefault();
                if (emp == null)
                {
                    List<pr_employee_allowance> employee_allowance = new List<pr_employee_allowance>();
                    employee_allowance.AddRange(Model.pr_employee_allowance);
                    List<pr_employee_ded_contribution> employee_ded_contribution = new List<pr_employee_ded_contribution>();
                    employee_ded_contribution.AddRange(Model.pr_employee_ded_contribution);
                    List<pr_employee_leave> employee_leave = new List<pr_employee_leave>();
                    employee_leave.AddRange(Model.pr_employee_leave);
                    List<pr_employee_document> employee_document = new List<pr_employee_document>();
                    employee_document.AddRange(Model.pr_employee_document);
                    List<pr_employee_Dependent> employee_dependent = new List<pr_employee_Dependent>();
                    employee_dependent.AddRange(Model.pr_employee_Dependent);

                    decimal ID = 1;
                    if (_service.Queryable().Count() > 0)
                        ID = _service.Queryable().Max(e => e.ID) + 1;

                    Model.ID = ID;
                    Model.CompanyID = CompanyID;
                    Model.PayTypeDropDownID = (int)sys_dropdown_mfEnum.PayType;
                    Model.StatusDropDownID = (int)sys_dropdown_mfEnum.EmployeeStatus;
                    Model.PaymentMethodDropDownID = (int)sys_dropdown_mfEnum.SalaryPaymentMethod;
                    Model.EmployeeTypeDropDownID = (int)sys_dropdown_mfEnum.EmployeeType;
                    Model.NationalityDropDownID = (int)sys_dropdown_mfEnum.Nationality;

                    Model.InsuranceClassTypeDropdownID = (int)sys_dropdown_mfEnum.InsuranceClassType;
                    Model.AirTicketClassTypeDropdownID = (int)sys_dropdown_mfEnum.AirTicketClassType;
                    Model.AirTicketFrequencyTypeDropdownID = (int)sys_dropdown_mfEnum.AirTicketFrequency;
                    Model.SpecialtyTypeDropdownID = (int)sys_dropdown_mfEnum.SpecialtyType;
                    Model.ClassificationTypeDropdownID = (int)sys_dropdown_mfEnum.ClassificationType;
                    Model.CityDropDownID = (int)sys_dropdown_mfEnum.City;
                    Model.CountryDropDownID = (int)sys_dropdown_mfEnum.Country;

                    Model.DestinationCityDropdownTypeID = (int)sys_dropdown_mfEnum.City;
                    Model.DestinationCountryDropdownTypeID = (int)sys_dropdown_mfEnum.Country;

                    Model.OriginCityDropdownTypeID = (int)sys_dropdown_mfEnum.City;
                    Model.OriginCountryDropdownTypeID = (int)sys_dropdown_mfEnum.Country;

                    Model.MaritalStatusTypeDropdownID = (int)sys_dropdown_mfEnum.MaritalType;

                    Model.ContractTypeDropDownID = (int)sys_dropdown_mfEnum.ContractType;


                    Model.CreatedBy = LoginID;
                    Model.CreatedDate = Request.DateTimes();
                    if (!string.IsNullOrEmpty(Model.EmployeePic))
                    {
                        Model.EmployeePic = GetValidImage(Model.EmployeePic);
                        Utility.MoveFile(DocumentInfo.getTempDocumentPathInfo(), Model.EmployeePic, DocumentInfo.getProductPathInfo());
                    }

                    Model.pr_employee_allowance = null;
                    Model.pr_employee_ded_contribution = null;
                    Model.pr_employee_leave = null;
                    Model.pr_employee_document = null;
                    Model.pr_employee_Dependent = null;
                    //Model.ModifiedBy = LoginID;
                    //Model.ModifiedDate = Request.DateTimes();
                    Model.ObjectState = ObjectState.Added;
                    _service.Insert(Model);


                    decimal EmpAllowID = 1;
                    if (_pr_employee_allowanceService.Queryable().Count() > 0)
                        EmpAllowID = _pr_employee_allowanceService.Queryable().Max(e => e.ID) + 1;

                    foreach (pr_employee_allowance item in employee_allowance)
                    {
                        item.ID = EmpAllowID;
                        item.EmployeeID = Model.ID;
                        item.CompanyID = CompanyID;
                        item.ObjectState = ObjectState.Added;
                        _pr_employee_allowanceService.Insert(item);
                        EmpAllowID++;
                    }

                    decimal DedConID = 1;
                    if (_pr_employee_ded_contributionService.Queryable().Count() > 0)
                        DedConID = _pr_employee_ded_contributionService.Queryable().Max(e => e.ID) + 1;

                    foreach (pr_employee_ded_contribution item in employee_ded_contribution)
                    {
                        item.ID = DedConID;
                        item.EmployeeID = Model.ID;
                        item.CompanyID = CompanyID;
                        item.EffectiveFrom = Convert.ToDateTime(Model.JoiningDate);
                        item.ObjectState = ObjectState.Added;
                        _pr_employee_ded_contributionService.Insert(item);
                        DedConID++;
                    }

                    decimal EmpLeaveID = 1;
                    if (_pr_employee_leaveService.Queryable().Count() > 0)
                        EmpLeaveID = _pr_employee_leaveService.Queryable().Max(e => e.ID) + 1;

                    foreach (pr_employee_leave item in employee_leave)
                    {
                        item.ID = EmpLeaveID;
                        item.EmployeeID = Model.ID;
                        item.CompanyID = CompanyID;
                        item.ObjectState = ObjectState.Added;
                        _pr_employee_leaveService.Insert(item);
                        EmpLeaveID++;
                    }

                    decimal DocID = 1;
                    if (_pr_employee_documentService.Queryable().Count() > 0)
                        DocID = _pr_employee_documentService.Queryable().Max(e => e.ID) + 1;

                    foreach (pr_employee_document item in employee_document)
                    {
                        item.ID = DocID;
                        item.EmployeeID = Model.ID;
                        item.CompanyID = CompanyID;
                        item.AttachmentPath = GetValidImage(item.AttachmentPath);
                        Utility.MoveFile(DocumentInfo.getTempDocumentPathInfo(), item.AttachmentPath, DocumentInfo.getProductPathInfo());
                        item.DocumentTypeDropdownID = (int)sys_dropdown_mfEnum.EmpDocumentType;
                        item.UploadDate = Request.DateTimes();
                        item.ObjectState = ObjectState.Added;
                        _pr_employee_documentService.Insert(item);
                        DocID++;
                    }


                    decimal DependentID = 1;
                    if (_pr_employee_dependentService.Queryable().Count() > 0)
                        DependentID = _pr_employee_dependentService.Queryable().Max(e => e.ID) + 1;

                    foreach (pr_employee_Dependent item in employee_dependent)
                    {
                        item.ID = DependentID;
                        item.EmployeeID = Model.ID;
                        item.CompanyID = CompanyID;
                        item.RelationshipDropdownID = (int)sys_dropdown_mfEnum.RelationshipType;
                        item.NationalityTypeDropdownID = (int)sys_dropdown_mfEnum.Nationality;
                        item.MaritalStatusTypeDropdownID = (int)sys_dropdown_mfEnum.MaritalType;
                        item.ObjectState = ObjectState.Added;
                        _pr_employee_dependentService.Insert(item);
                        DependentID++;
                    }



                    try
                    {
                        await _unitOfWorkAsync.SaveChangesAsync();
                        objResponse.Message = MessageStatement.Save;
                        objResponse.IsSuccess = true;
                        objResponse.ResultSet = new
                        {
                            IsUpdate = false,
                        };
                    }
                    catch (DbUpdateException ex)
                    {
                        if (ModelExists(Model.ID.ToString()))
                        {
                            objResponse.IsSuccess = false;
                            objResponse.ErrorMessage = MessageStatement.Conflict;
                            return objResponse;
                        }
                        throw ex;
                    }
                }
                else
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = Model.FirstName + " already exist. Please enter a different name and try again.";
                }
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        private string GetValidImage(string Image)
        {
            if (Image == null)
                Image = "";
            else
            {
                if (Image.LastIndexOf("/") > -1)
                    Image = Image.Substring(Image.LastIndexOf("/") + 1);
            }

            return Image;
        }
        [HttpGet]
        [ActionName("GetAllEmps")]
        public ResponseInfo GetAllEmps()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal companyID = Request.CompanyID();
                objResponse.ResultSet = _service.Queryable().Where(x => x.CompanyID == companyID)
                    .Select(x => new { ID = x.ID, Name = x.FirstName + " " + x.LastName, Email = x.Email }).ToList();
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        public ResponseInfo GetList()
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal companyID = Request.CompanyID();
                objResponse.ResultSet = _service.Queryable().Where(x => x.CompanyID == companyID).ToList();
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        public ResponseInfo Load()
        {
            throw new NotImplementedException();
        }
        private bool ModelExists(string key)
        {
            return _service.Query(e => e.ID.ToString() == key).Select().Any();
        }
        public pr_employee_mf GetPREmployeeByID(string Id)
        {
            var CompanyID = Request.CompanyID();

            return _service.Queryable()
           .Where(e => e.CompanyID == CompanyID && e.ID.ToString() == Id)
       .Include(x => x.pr_employee_allowance)
       .Include(x => x.pr_employee_ded_contribution)
       .Include(x => x.pr_employee_leave)
       .Include(x => x.pr_employee_document)
       .Include(x => x.pr_employee_Dependent)
       .Include(x => x.adm_user_company)
       .FirstOrDefault();
        }
        [HttpGet]
        [ActionName("GetById")]
        public ResponseInfo GetById(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                //objResponse.ResultSet = _service.Queryable()
                //    .Where(e => e.CompanyID == CompanyID && e.ID.ToString() == Id)
                //.Include(x => x.pr_employee_allowance)
                //.Include(x => x.pr_employee_ded_contribution)
                //.Include(x => x.pr_employee_leave)
                //.Include(x => x.adm_user_company)
                //.FirstOrDefault();

                decimal CompanyID = Request.CompanyID();

                var Employee = GetPREmployeeByID(Id);
                DateTime date = Request.DateTimes();
                date = date.AddDays(7);
                DateTime dateNow = Request.DateTimes();

                var ExpiredDocs = Employee.pr_employee_document.Where(x => x.ExpireDate.Date < dateNow.Date)
                    .Select(x => new { ID = x.ID, DocTypeID = x.DocumentTypeID, DocName = x.Description, AttachmentPath = x.AttachmentPath, ExpireDate = x.ExpireDate })
                  .ToList<object>();
                var NeartoExpireDocs = Employee.pr_employee_document.Where(x => x.ExpireDate.Date >= dateNow.Date && x.ExpireDate.Date <= date.Date)
                     .Select(x => new { ID = x.ID, DocTypeID = x.DocumentTypeID, DocName = x.Description, AttachmentPath = x.AttachmentPath, ExpireDate = x.ExpireDate })
                   .ToList<object>();


                decimal EID = Convert.ToDecimal(Id);
                var TakenLeavesByEmp = _pr_leave_applicationService.Queryable().Where(x => x.CompanyID == CompanyID && x.EmployeeID == EID)
                .Include(x => x.pr_leave_type)
                .Select(x => new
                {
                    Hours = x.Hours,
                    LeaveTypeID = x.LeaveTypeID,
                    x.pr_leave_type.Category,
                }).ToList();
                var groupedSum = TakenLeavesByEmp
    .GroupBy(x => new { x.LeaveTypeID, x.Category })
    .Select(g => new
    {
        LeaveTypeID = g.Key.LeaveTypeID,
        Category = g.Key.Category,
        TotalHours = g.Sum(x => x.Hours)
    })
    .ToList();
                objResponse.ResultSet = new
                {
                    Employee = Employee,
                    NeartoExpireDocs = NeartoExpireDocs,
                    TakenLeavesByEmp = groupedSum
                };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        [HttpGet]
        [ActionName("GetBulkFilterData")]
        public ResponseInfo GetBulkFilterData(string AllConDedType)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                var AllConDedTypes = new List<object>();
                if (AllConDedType == "A")
                {
                    AllConDedTypes = _pr_employee_allowanceService.Queryable()
                   .Where(e => e.CompanyID == CompanyID).Include(x => x.pr_allowance)
                   .Select(x => new { ID = x.pr_allowance.ID, Name = x.pr_allowance.AllowanceName, Category = "A", Amount = x.pr_allowance.AllowanceValue, ValueType = x.pr_allowance.AllowanceType, Taxable = x.pr_allowance.Taxable }).Distinct().ToList<object>();
                }
                else if (AllConDedType == "C" || AllConDedType == "D")
                {
                    AllConDedTypes = _pr_employee_ded_contributionService.Queryable()
                  .Where(e => e.CompanyID == CompanyID && e.Category == AllConDedType).Include(x => x.pr_deduction_contribution)
                  .Select(x => new { ID = x.pr_deduction_contribution.ID, Name = x.pr_deduction_contribution.DeductionContributionName, Category = x.pr_deduction_contribution.Category, Amount = x.pr_deduction_contribution.DeductionContributionValue, ValueType = x.pr_deduction_contribution.DeductionContributionType, Taxable = x.pr_deduction_contribution.Taxable }).Distinct().ToList<object>();
                }
                else
                {
                    // or basic salary
                    AllConDedTypes = _service.Queryable()
                  .Where(e => e.CompanyID == CompanyID)
                  .Select(x => new { ID = x.ID, Amount = x.BasicSalary, Category = "B" }).Distinct().ToList<object>();
                }


                var Departments = new List<object>();// _service.Queryable().Where(x => x.CompanyID == CompanyID).Distinct()
                                                     //.Where(x => x.pr_department != null)
                                                     //.Select(x => new { id = x.pr_department.ID, name = x.pr_department.DepartmentName }).Distinct().ToList<object>();
                var Designations = new List<object>();//  _service.Queryable().Where(x => x.CompanyID == CompanyID).Distinct()
                                                      //.Where(x => x.pr_designation != null)
                                                      //.Select(x => new { id = x.pr_designation.ID, name = x.pr_designation.DesignationName }).Distinct().ToList<object>();
                var EmpTypes = new List<object>();// _service.Queryable().Where(x => x.CompanyID == CompanyID).Distinct()
                                                  //.Where(x => x.pr_designation != null)
                                                  //.Select(x => new { id = x.pr_designation.ID, name = x.pr_designation.DesignationName }).Distinct().ToList<object>();
                                                  //_service.Queryable().Where(x => x.CompanyID == CompanyID).Distinct()
                                                  //.Where(x => x.EmployeeTypeList != null)
                                                  // .Select(x => new { id = x.EmployeeTypeList.ID, name = x.EmployeeTypeList.Value }).Distinct().ToList<object>();
                var Emplist = _service.Queryable().Where(x => x.CompanyID == CompanyID).Distinct()
                     .Where(x => x != null)
                    .Select(x => new { id = x.ID, name = x.FirstName + " " + x.LastName }).Distinct().ToList<object>();
                objResponse.ResultSet = new
                {
                    AllConDedTypes = AllConDedTypes,
                    Departments = Departments,
                    Designations = Designations,
                    EmpTypes = EmpTypes,
                    Emplist = Emplist,
                };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        [HttpPost]
        [ActionName("GetBulkFilterEmployees")]
        public ResponseInfo GetBulkFilterEmployees(EmpBulkUpdateModel BulkModel)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();

                var bulkFilterEmployees = new List<object>();
                if (BulkModel.BulkUpdateCategoryType == "A")
                {
                    bulkFilterEmployees = _service.Queryable()
                   .Where(e => e.CompanyID == CompanyID
                   && (BulkModel.DepartmentsIds.Count() == 0 || BulkModel.DepartmentsIds.Contains(e.DepartmentID))
                   && (BulkModel.DesignationsIds.Count() == 0 || BulkModel.DesignationsIds.Contains(e.DesignationID))
                   && (BulkModel.EmployeeTypeIds.Count() == 0 || BulkModel.EmployeeTypeIds.Contains(e.EmployeeTypeID))
                   && (BulkModel.EmployeeIds.Count() == 0 || BulkModel.EmployeeIds.Contains(e.ID))
                   )
                   .Include(x => x.pr_employee_allowance)
                   .Select(x => new BulkEmpModel
                   {
                       ID = x.ID,
                       Employee = x.FirstName + " " + x.LastName,
                       ExistingAmount = x.pr_employee_allowance.Where(y => y.AllowanceID == BulkModel.AllConDedId)
                   .Select(z => z.Amount).FirstOrDefault(),
                       Amount = BulkModel.Amount,
                       Category = BulkModel.BulkUpdateCategoryType,
                       Taxable = x.pr_employee_allowance.Where(y => y.AllowanceID == BulkModel.AllConDedId)
                   .Select(z => z.Taxable).FirstOrDefault(),
                       Remove = false,
                       UseExistingAmount = false,
                       AllwConDedBSalID = BulkModel.AllConDedId,
                       BasicSalary = x.BasicSalary ?? 0,
                   }).Distinct().ToList<object>();
                }
                else if (BulkModel.BulkUpdateCategoryType == "C" || BulkModel.BulkUpdateCategoryType == "D")
                {
                    bulkFilterEmployees = _service.Queryable()
                    .Where(e => e.CompanyID == CompanyID
                   && (BulkModel.DepartmentsIds.Count() == 0 || BulkModel.DepartmentsIds.Contains(e.DepartmentID))
                   && (BulkModel.DesignationsIds.Count() == 0 || BulkModel.DesignationsIds.Contains(e.DesignationID))
                   && (BulkModel.EmployeeTypeIds.Count() == 0 || BulkModel.EmployeeTypeIds.Contains(e.EmployeeTypeID))
                   && (BulkModel.EmployeeIds.Count() == 0 || BulkModel.EmployeeIds.Contains(e.ID)))
                    .Include(x => x.pr_employee_ded_contribution)
                    .Select(x => new BulkEmpModel
                    {
                        ID = x.ID,
                        Employee = x.FirstName + " " + x.LastName,
                        ExistingAmount = x.pr_employee_ded_contribution.Where(y => y.DeductionContributionID == BulkModel.AllConDedId)
                    .Select(z => z.Amount).FirstOrDefault(),
                        Amount = BulkModel.Amount,
                        Category = BulkModel.BulkUpdateCategoryType,
                        Taxable = x.pr_employee_ded_contribution.Where(y => y.DeductionContributionID == BulkModel.AllConDedId)
                    .Select(z => z.Taxable).FirstOrDefault() ?? false,
                        Remove = false,
                        UseExistingAmount = false,
                        AllwConDedBSalID = BulkModel.AllConDedId,
                        BasicSalary = x.BasicSalary ?? 0,
                    }).Distinct().ToList<object>();
                }
                else
                {
                    bulkFilterEmployees = _service.Queryable()
                     .Where(e => e.CompanyID == CompanyID
                   && (BulkModel.DepartmentsIds.Count() == 0 || BulkModel.DepartmentsIds.Contains(e.DepartmentID))
                   && (BulkModel.DesignationsIds.Count() == 0 || BulkModel.DesignationsIds.Contains(e.DesignationID))
                   && (BulkModel.EmployeeTypeIds.Count() == 0 || BulkModel.EmployeeTypeIds.Contains(e.EmployeeTypeID))
                   && (BulkModel.EmployeeIds.Count() == 0 || BulkModel.EmployeeIds.Contains(e.ID)))
                     .Select(x => new BulkEmpModel
                     {
                         ID = x.ID,
                         Employee = x.FirstName + " " + x.LastName,
                         ExistingAmount = x.BasicSalary ?? 0,
                         Amount = BulkModel.Amount,
                         Category = BulkModel.BulkUpdateCategoryType,
                         Remove = false,
                         UseExistingAmount = false,
                         AllwConDedBSalID = BulkModel.AllConDedId,
                         BasicSalary = x.BasicSalary ?? 0,
                     }).Distinct().ToList<object>();
                }
                objResponse.ResultSet = new
                {
                    bulkFilterEmployees = bulkFilterEmployees,
                    TotalRecord = bulkFilterEmployees.Count(),
                };
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        [HttpPost]
        [ActionName("UpdateBulkEmp")]
        public async Task<ResponseInfo> UpdateBulkEmp(List<BulkEmpModel> Models)
        {
            var objResponse = new ResponseInfo();
            try
            {
                if (!ModelState.IsValid)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.BadRequest;
                    return objResponse;
                }

                var EmpIds = Models.Select(x => x.ID).ToArray();
                decimal CompanyID = Request.CompanyID();
                var AllowanceList = _pr_allowanceService.Queryable().Select(s => new { s.ID, s.AllowanceType }).ToList();
                var ConDedList = _pr_deduction_contributionService.Queryable().Select(s => new { s.ID, s.DeductionContributionType }).ToList();
                var Employees = _service.Queryable().Where(x => x.CompanyID == CompanyID && EmpIds.Contains(x.ID))
                    .Include(x => x.pr_employee_allowance)
                    .Include(x => x.pr_employee_ded_contribution)
                    .ToList();

                foreach (var item in Models)
                {
                    if (item.Category == "A")
                    {

                        var empAllow = Employees.Where(x => x.ID == item.ID).FirstOrDefault()
                            .pr_employee_allowance.Where(x => x.AllowanceID == item.AllwConDedBSalID).FirstOrDefault();
                        if (empAllow != null)
                        {
                            if (!item.Remove)
                            {
                                empAllow.Taxable = item.Taxable;
                                empAllow.Amount = item.Amount;
                                empAllow.Percentage = (double)Math.Round((double)(100 * item.Amount) / item.BasicSalary);
                                empAllow.ObjectState = ObjectState.Modified;
                                _pr_employee_allowanceService.Update(empAllow);
                            }
                            else
                            {
                                empAllow.ObjectState = ObjectState.Deleted;
                                _pr_employee_allowanceService.Delete(empAllow);
                            }
                        }
                        else
                        {
                            pr_employee_allowance allow = new pr_employee_allowance();
                            var empl = Employees.Where(x => x.ID == item.ID).Select(x => new { x.PayScheduleID, x.JoiningDate }).FirstOrDefault();
                            decimal EmpAllowID = 1;
                            if (_pr_employee_allowanceService.Queryable().Count() > 0)
                                EmpAllowID = _pr_employee_allowanceService.Queryable().Max(e => e.ID) + 1;

                            allow.ID = EmpAllowID;
                            allow.EmployeeID = item.ID;
                            allow.CompanyID = CompanyID;
                            allow.EffectiveFrom = (DateTime)empl.JoiningDate;
                            allow.PayScheduleID = (decimal)empl.PayScheduleID;
                            allow.AllowanceID = item.AllwConDedBSalID;
                            allow.Percentage = (double)Math.Round((double)(100 * item.Amount) / item.BasicSalary);
                            allow.Amount = item.Amount;
                            allow.Taxable = item.Taxable;
                            allow.ObjectState = ObjectState.Added;
                            _pr_employee_allowanceService.Insert(allow);
                        }

                    }
                    else if (item.Category == "C" || item.Category == "D")
                    {
                        var empDedCon = Employees.Where(x => x.ID == item.ID).FirstOrDefault()
                            .pr_employee_ded_contribution.Where(x => x.DeductionContributionID == item.AllwConDedBSalID)
                            .FirstOrDefault();
                        if (empDedCon != null)
                        {
                            if (!item.Remove)
                            {
                                empDedCon.Taxable = item.Category == "C" ? false : item.Taxable;
                                empDedCon.Amount = item.Amount;
                                empDedCon.Percentage = (double)Math.Round((double)(100 * item.Amount) / item.BasicSalary);
                                empDedCon.ObjectState = ObjectState.Modified;
                                _pr_employee_ded_contributionService.Update(empDedCon);
                            }
                            else
                            {
                                empDedCon.ObjectState = ObjectState.Deleted;
                                _pr_employee_allowanceService.Delete(empDedCon);
                            }
                        }
                        else
                        {
                            pr_employee_ded_contribution ConDed = new pr_employee_ded_contribution();
                            decimal DedConID = 1;
                            if (_pr_employee_ded_contributionService.Queryable().Count() > 0)
                                DedConID = _pr_employee_ded_contributionService.Queryable().Max(e => e.ID) + 1;

                            var empl = Employees.Where(x => x.ID == item.ID).Select(x => new { x.PayScheduleID, x.JoiningDate }).FirstOrDefault();

                            ConDed.ID = DedConID;
                            ConDed.EmployeeID = item.ID;
                            ConDed.CompanyID = CompanyID;
                            ConDed.EffectiveFrom = (DateTime)empl.JoiningDate;
                            ConDed.PayScheduleID = (decimal)empl.PayScheduleID;
                            ConDed.Category = item.Category;
                            ConDed.DeductionContributionID = item.AllwConDedBSalID;
                            ConDed.Percentage = (double)Math.Round((double)(100 * item.Amount) / item.BasicSalary);
                            ConDed.Amount = item.Amount;
                            ConDed.Taxable = item.Taxable;
                            ConDed.StartingBalance = 0;
                            ConDed.ObjectState = ObjectState.Added;
                            _pr_employee_ded_contributionService.Insert(ConDed);
                        }

                    }
                    else
                    {
                        // for basic salary bulk update
                        var emp = Employees.Where(x => x.ID == item.ID).FirstOrDefault();
                        if (emp.pr_employee_allowance != null && emp.pr_employee_allowance.Count() > 0)
                        {
                            var EmpAllows = emp.pr_employee_allowance.ToList();
                            foreach (var allow in EmpAllows)
                            {
                                string allowType = AllowanceList.Where(e => e.ID == allow.AllowanceID).FirstOrDefault().AllowanceType;
                                if (allowType == "F")
                                    allow.Percentage = (double)Math.Round((double)(100 * allow.Amount) / item.Amount);
                                else
                                    allow.Amount = (allow.Percentage / 100) * item.Amount;

                                allow.ObjectState = ObjectState.Modified;
                                _pr_employee_allowanceService.Update(allow);
                            }
                        }
                        if (emp.pr_employee_ded_contribution != null && emp.pr_employee_ded_contribution.Count() > 0)
                        {
                            var EmpDedConts = emp.pr_employee_ded_contribution.ToList();

                            foreach (var Dedcon in EmpDedConts)
                            {
                                string ConDedType = ConDedList.Where(e => e.ID == Dedcon.DeductionContributionID).FirstOrDefault().DeductionContributionType;
                                if (ConDedType == "F")
                                    Dedcon.Percentage = (double)Math.Round((double)(100 * Dedcon.Amount) / item.Amount);
                                else
                                    Dedcon.Amount = (Dedcon.Percentage / 100) * item.Amount;

                                Dedcon.ObjectState = ObjectState.Modified;
                                _pr_employee_ded_contributionService.Update(Dedcon);
                            }
                        }
                        emp.BasicSalary = item.Amount;
                        emp.ObjectState = ObjectState.Modified;
                        _service.Update(emp);
                    }

                }

                try
                {
                    await _unitOfWorkAsync.SaveChangesAsync();
                    objResponse.Message = MessageStatement.Update;
                }
                catch (DbUpdateException)
                {
                    //if (!ModelExists(Model.ID.ToString()))
                    //{
                    //    objResponse.IsSuccess = false;
                    //    objResponse.ErrorMessage = MessageStatement.NotFound;
                    //    return objResponse;
                    //}
                    throw;
                }
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
        [HttpPost]
        [ActionName("IsImportDataExistInDB")]
        public ResponseInfo IsImportDataExistInDB(List<ImportDataCheckingInDBModel> ImportEmpList)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                var Namelist = ImportEmpList.Select(x => (x.FirstName.ToLower().Trim() + " " + x.LastName.ToLower().Trim())).ToArray();
                var Emaillist = ImportEmpList.Where(x => x.Email != null).Select(x => x.Email).ToArray();

                var EmpList = _service.Queryable()
                    .Where(x => x.CompanyID == CompanyID && x.StatusID == 1).Select(x => new { x.CompanyID, x.StatusID, x.FirstName, x.LastName, x.Email }).ToList();

                var AlreadyExistEmployees = new List<object>();
                if (Emaillist.Count() > 0)
                {
                    AlreadyExistEmployees = EmpList
                  .Where(x => Emaillist.Contains(x.Email))
                                  .Select(t => new
                                  {
                                      t.FirstName,
                                      t.LastName,
                                      t.Email
                                  }).ToList<object>();
                    objResponse.Message = "Email already exist";
                }


                if (AlreadyExistEmployees.Count() == 0)
                {
                    AlreadyExistEmployees = EmpList
                 .Where(x => Namelist.Contains((x.FirstName.ToLower().Trim() + " " + x.LastName.ToLower().Trim())))
                                 .Select(t => new
                                 {
                                     t.FirstName,
                                     t.LastName,
                                     t.Email
                                 }).ToList<object>();
                    objResponse.Message = "Name already exist";
                }


                if (AlreadyExistEmployees.Count > 0)
                {
                    objResponse.ResultSet = AlreadyExistEmployees;
                }
                else
                {
                    objResponse.ResultSet = null;
                    objResponse.Message = "NotExist";
                }




            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        [HttpPost]
        [ActionName("ImportEmpData")]
        public ResponseInfo ImportEmpData(List<EmployeeImportModel> ExcelDataList)
        {
            var objResponse = new ResponseInfo();
            try
            {

                var DuplicateRecords = ExcelDataList.GroupBy(x => (x.FirstName + " " + x.LastName).ToLower().Trim())
                                         .Where(g => g.Count() > 1)
                                         .Select(y => new { DuplicateRecords = y.ToList(), Element = y.Key, Counter = y.Count() })
                                         .ToList();
                var DuplicateEmail = ExcelDataList.GroupBy(x => x.Email)
                                          .Where(g => g.Count() > 1)
                                          .Select(y => new { DuplicateEmails = y.ToList(), Element = y.Key, Counter = y.Count() })
                                          .ToList();

                var val = 0210;


            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        private DataTable FetchExcelData(string FileName)
        {
            DataTable dt = new DataTable();
            string FileBasePath = ConfigurationManager.AppSettings["TempDocumentPath"];
            //string BasePath = @"C:\Haseeb\Working Projects\ERPISTO\ERPISTO.Web\Files\Temp\";
            string CString = "provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + FileBasePath + FileName + "';Extended Properties=Excel 12.0;";

            try
            {
                var SheetName = "";

                using (OleDbConnection conn = new OleDbConnection(CString))
                {

                    conn.Open();
                    SheetName = conn.GetSchema("Tables").Rows[0]["TABLE_NAME"].ToString();

                    OleDbCommand cmdc = new OleDbCommand("Select * from [" + SheetName + "]", conn);
                    cmdc.CommandTimeout = 30;
                    OleDbDataAdapter oda = new OleDbDataAdapter(cmdc);
                    oda.Fill(dt);
                    conn.Close();

                }
                int totalRecord = dt.Rows.Count;
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ResponseInfo GetById(string Id, int NextPreviousIndex)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var CompanyID = Request.CompanyID();
                objResponse.ResultSet = _service.Queryable().Where(e => e.CompanyID == CompanyID && e.ID.ToString() == Id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        [HttpGet]
        [ActionName("Pagination")]
        public PaginationResult Pagination(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResult = _service.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }
        [HttpGet]
        [ActionName("BulkEmpPagination")]
        public PaginationResult BulkEmpPagination(EmpBulkUpdateModel BulkModel, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal CompanyID = Request.CompanyID();
                objResult = _service.BulkEmpPagination(BulkModel, CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            return objResult;
        }
        [HttpGet]
        [ActionName("ExportData")]
        public ResponseInfo ExportData(int ExportType, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText)
        {
            var objResponse = new ResponseInfo();
            try
            {
                var ObjList = Pagination(0, 0, VisibleColumnInfo, SortName, SortOrder, SearchText, true);
                objResponse.FilePath = Documents.ExportWithType(ExportType, VisibleColumnInfo, ObjList.DataList);

            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        [HttpPut]
        [HttpGet]
        [ActionName("Update")]
        public async Task<ResponseInfo> Update(pr_employee_mf Model)
        {
            var objResponse = new ResponseInfo();
            try
            {
                if (!ModelState.IsValid)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.BadRequest;
                    return objResponse;
                }

                decimal CompanyID = Request.CompanyID();
                pr_employee_mf emp = _service.Queryable().Where(x => x.CompanyID == CompanyID && x.ID != Model.ID && (x.FirstName + " " + x.LastName).ToLower() == (Model.FirstName + " " + Model.LastName).ToLower()).FirstOrDefault();
                if (emp == null)
                {
                    List<pr_employee_allowance> employee_allowanceList = new List<pr_employee_allowance>();
                    employee_allowanceList.AddRange(Model.pr_employee_allowance);
                    List<pr_employee_ded_contribution> employee_ded_contributionList = new List<pr_employee_ded_contribution>();
                    employee_ded_contributionList.AddRange(Model.pr_employee_ded_contribution);
                    List<pr_employee_leave> employee_leaveList = new List<pr_employee_leave>();
                    employee_leaveList.AddRange(Model.pr_employee_leave);


                    Model.EmployeePic = GetValidImage(Model.EmployeePic);
                    Utility.MoveFile(DocumentInfo.getTempDocumentPathInfo(), Model.EmployeePic, DocumentInfo.getProductPathInfo());

                    Model.pr_employee_allowance = null;
                    Model.pr_employee_ded_contribution = null;
                    Model.pr_employee_leave = null;
                    Model.adm_user_company = null;
                    //Model.CityList = null;
                    //Model.CountryList = null;
                    //Model.EmployeeTypeList = null;
                    //Model.PaymentMethodList = null;
                    //Model.PayTypeList = null;
                    //Model.StatusList = null;
                    Model.pr_pay_schedule = null;

                    //Model.pr_department = null;
                    //Model.pr_designation = null;
                    Model.CityDropDownID = (int)sys_dropdown_mfEnum.City;
                    Model.CountryDropDownID = (int)sys_dropdown_mfEnum.Country;
                    Model.OriginCityDropdownTypeID = (int)sys_dropdown_mfEnum.City;
                    Model.OriginCountryDropdownTypeID = (int)sys_dropdown_mfEnum.Country;
                    Model.DestinationCityDropdownTypeID = (int)sys_dropdown_mfEnum.City;
                    Model.DestinationCountryDropdownTypeID = (int)sys_dropdown_mfEnum.Country;

                    Model.ModifiedBy = Request.LoginID();
                    Model.ModifiedDate = Request.DateTimes();
                    Model.ObjectState = ObjectState.Modified;

                    _service.Update(Model);




                    //var empAllowIds = employee_allowanceList.Where(e => e.ID != 0).Select(s => s.ID).ToArray();
                    //var DelEmpAllowModels = _pr_employee_allowanceService.Queryable().Where(e => e.EmployeeID == Model.ID && !empAllowIds.Contains(e.ID)).ToList();

                    var DelEmpAllowModels = _pr_employee_allowanceService.Queryable().Where(e => e.EmployeeID == Model.ID && e.CompanyID == CompanyID).ToList();
                    foreach (var obj in DelEmpAllowModels)
                    {
                        obj.ObjectState = ObjectState.Deleted;
                        _pr_employee_allowanceService.Delete(obj);
                    }

                    decimal EmpAllowID = 1;
                    if (_pr_employee_allowanceService.Queryable().Count() > 0)
                        EmpAllowID = _pr_employee_allowanceService.Queryable().Max(e => e.ID) + 1;
                    foreach (pr_employee_allowance item in employee_allowanceList)
                    {
                        //if (item.ID > 0)
                        //{
                        //    item.EffectiveFrom = item.EffectiveFrom.Date;
                        //    item.EffectiveTo = Request.DateTimes().Date;
                        //    item.ObjectState = ObjectState.Modified;
                        //    _pr_employee_allowanceService.Update(item);
                        //}
                        //else
                        //{
                        item.ID = EmpAllowID;
                        item.EmployeeID = Model.ID;
                        item.CompanyID = CompanyID;
                        item.EffectiveFrom = item.EffectiveFrom.Date;
                        item.pr_allowance = null;
                        item.pr_pay_schedule = null;
                        item.ObjectState = ObjectState.Added;
                        _pr_employee_allowanceService.Insert(item);
                        EmpAllowID++;
                        //}
                    }




                    //var DedContIds = employee_ded_contributionList.Where(e => e.ID != 0).Select(s => s.ID).ToArray();
                    //var DelEmpContDedModels = _pr_employee_ded_contributionService.Queryable().Where(e => e.EmployeeID == Model.ID && !DedContIds.Contains(e.ID)).ToList();

                    var DelEmpContDedModels = _pr_employee_ded_contributionService.Queryable().Where(e => e.EmployeeID == Model.ID && e.CompanyID == CompanyID).ToList();
                    foreach (var obj in DelEmpContDedModels)
                    {
                        obj.ObjectState = ObjectState.Deleted;
                        _pr_employee_ded_contributionService.Delete(obj);
                    }

                    decimal DedConID = 1;
                    if (_pr_employee_ded_contributionService.Queryable().Count() > 0)
                        DedConID = _pr_employee_ded_contributionService.Queryable().Max(e => e.ID) + 1;


                    foreach (pr_employee_ded_contribution item in employee_ded_contributionList)
                    {
                        //if (item.ID > 0)
                        //{
                        //    item.EffectiveFrom = item.EffectiveFrom.Date;
                        //    item.EffectiveTo = Request.DateTimes().Date;
                        //    item.ObjectState = ObjectState.Modified;
                        //    _pr_employee_ded_contributionService.Update(item);
                        //}
                        //else
                        //{
                        item.ID = DedConID;
                        item.EmployeeID = Model.ID;
                        item.CompanyID = CompanyID;
                        item.EffectiveFrom = Convert.ToDateTime(Model.JoiningDate).Date;
                        item.pr_deduction_contribution = null;
                        item.pr_pay_schedule = null;
                        item.ObjectState = ObjectState.Added;
                        _pr_employee_ded_contributionService.Insert(item);
                        DedConID++;
                        //}
                    }



                    var DedLeaveIds = employee_leaveList.Where(e => e.ID != 0).Select(s => s.ID).ToArray();
                    var DelEmpLeaveDedModels = _pr_employee_leaveService.Queryable().Where(e => e.EmployeeID == Model.ID && !DedLeaveIds.Contains(e.ID)).ToList();
                    //var DelEmpLeaveDedModels = _pr_employee_leaveService.Queryable().Where(e => e.EmployeeID == Model.ID && e.CompanyID == CompanyID).ToList();
                    foreach (var obj in DelEmpLeaveDedModels)
                    {
                        obj.ObjectState = ObjectState.Deleted;
                        _pr_employee_leaveService.Delete(obj);
                    }
                    decimal EmpLeaveID = 1;
                    if (_pr_employee_leaveService.Queryable().Count() > 0)
                        EmpLeaveID = _pr_employee_leaveService.Queryable().Max(e => e.ID) + 1;

                    foreach (pr_employee_leave item in employee_leaveList)
                    {
                        if (item.ID > 0)
                        {
                            item.pr_leave_type = null;
                            item.ObjectState = ObjectState.Modified;
                            _pr_employee_leaveService.Update(item);
                        }
                        else
                        {
                            item.ID = EmpLeaveID;
                            item.EmployeeID = Model.ID;
                            item.CompanyID = CompanyID;
                            item.ObjectState = ObjectState.Added;
                            _pr_employee_leaveService.Insert(item);
                            EmpLeaveID++;
                        }
                    }






                    try
                    {
                        await _unitOfWorkAsync.SaveChangesAsync();
                        objResponse.Message = MessageStatement.Update;
                        //      pr_employee_mf emp2 = _service.Queryable()
                        //    .Where(e => e.CompanyID == CompanyID && e.ID == Model.ID)
                        //.Include(x => x.pr_employee_allowance)
                        //.Include(x => x.pr_employee_ded_contribution)
                        //.Include(x => x.pr_employee_leave)
                        //.Include(x => x.pr_employee_document)
                        //.Include(x => x.adm_user_company)
                        //.FirstOrDefault();



                        var emp2 = GetPREmployeeByID(Model.ID.ToString());

                        DateTime date = Request.DateTimes();
                        date = date.AddDays(7);
                        DateTime dateNow = Request.DateTimes();

                        objResponse.ResultSet = new
                        {

                            UpdatedModel = emp2,
                            IsUpdate = true,
                        };
                    }
                    catch (DbUpdateException)
                    {
                        if (!ModelExists(Model.ID.ToString()))
                        {
                            objResponse.IsSuccess = false;
                            objResponse.ErrorMessage = MessageStatement.NotFound;
                            return objResponse;
                        }
                        throw;
                    }
                }
                else
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = Model.FirstName + " already exist. Please enter a different name and try again.";
                }
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        [HttpGet]
        [ActionName("FormLoad")]
        public ResponseInfo FormLoad()
        {
            var objResponse = new ResponseInfo();
            try
            {
                var Employee = new pr_employee_mf();
                var NeartoExpireDocs = new List<object>();
                decimal CompanyID = Request.CompanyID();
                //     Employee = _service.Queryable()
                //    .Where(e => e.CompanyID == CompanyID && e.ID.ToString() == Id)
                //.Include(x => x.pr_employee_allowance)
                //.Include(x => x.pr_employee_ded_contribution)
                //.Include(x => x.pr_employee_leave)
                //.Include(x => x.pr_employee_document)
                //.Include(x => x.adm_user_company)
                //.FirstOrDefault();
                DateTime date = Request.DateTimes();
                date = date.AddDays(7);
                DateTime dateNow = Request.DateTimes();
                //NeartoExpireDocs = Employee.pr_employee_document.Where(x => x.ExpireDate >= dateNow && x.ExpireDate <= date)
                //    .Select(x => new { ID = x.ID, DocName = x.Description, ExpireDate = x.ExpireDate })
                //  .ToList<object>();
                var AllDropdownsData = _sys_drop_down_valueService.Queryable().Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.SpecialtyType || e.DropDownID == (int)sys_dropdown_mfEnum.ClassificationType || e.DropDownID == (int)sys_dropdown_mfEnum.PayType || e.DropDownID == (int)sys_dropdown_mfEnum.EmployeeType || e.DropDownID == (int)sys_dropdown_mfEnum.City || e.DropDownID == (int)sys_dropdown_mfEnum.Country || e.DropDownID == (int)sys_dropdown_mfEnum.EmployeeStatus || e.DropDownID == (int)sys_dropdown_mfEnum.SalaryPaymentMethod || e.DropDownID == (int)sys_dropdown_mfEnum.PayDayFallOnHoliday || e.DropDownID == (int)sys_dropdown_mfEnum.AccrualFrequency || e.DropDownID == (int)sys_dropdown_mfEnum.AllowanceCategory || e.DropDownID == (int)sys_dropdown_mfEnum.Nationality || e.DropDownID == (int)sys_dropdown_mfEnum.DocumentType || e.DropDownID == (int)sys_dropdown_mfEnum.MaritalType || e.DropDownID == (int)sys_dropdown_mfEnum.ContractType || e.DropDownID == (int)sys_dropdown_mfEnum.RelationshipType || e.DropDownID == (int)sys_dropdown_mfEnum.AirTicketClassType || e.DropDownID == (int)sys_dropdown_mfEnum.AirTicketFrequency || e.DropDownID == (int)sys_dropdown_mfEnum.InsuranceClassType).Select(x => new { x.ID, x.Value, x.DropDownID, x.DependedDropDownID, x.DependedDropDownValueID }).ToList();
                var Paytypes = AllDropdownsData.Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.PayType).Select(x => new { x.ID, x.Value }).ToList();
                var EmployeeTypes = AllDropdownsData.Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.EmployeeType).Select(x => new { x.ID, x.Value }).ToList();
                var Cities = AllDropdownsData.Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.City).ToList();
                var Countries = AllDropdownsData.Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.Country).Select(x => new { x.ID, x.Value }).ToList();
                var Statuses = AllDropdownsData.Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.EmployeeStatus).Select(x => new { x.ID, x.Value }).ToList();
                var PaymentMethodTypes = AllDropdownsData.Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.SalaryPaymentMethod).Select(x => new { x.ID, x.Value }).ToList();
                var PayDayFallOnHolidayTypes = AllDropdownsData.Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.PayDayFallOnHoliday).Select(x => new { x.ID, x.Value }).ToList();
                var AccrualFrequencies = AllDropdownsData.Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.AccrualFrequency).Select(x => new { x.ID, x.Value }).ToList();
                var AllowanceCategoryTypes = AllDropdownsData.Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.AllowanceCategory).Select(x => new { x.ID, x.Value }).ToList();
                var Nationalities = AllDropdownsData.Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.Nationality).Select(x => new { x.ID, x.Value }).ToList();
                var DocumentTypes = AllDropdownsData.Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.EmpDocumentType).Select(x => new { x.ID, x.Value }).ToList();
                var MaritalStatusList = AllDropdownsData.Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.MaritalType).Select(x => new { x.ID, x.Value }).ToList();
                var ContractTypeList = AllDropdownsData.Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.ContractType).Select(x => new { x.ID, x.Value }).ToList();
                var RelationshipTypeList = AllDropdownsData.Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.RelationshipType).Select(x => new { x.ID, x.Value }).ToList();
                var AirTicketClassTypeList = AllDropdownsData.Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.AirTicketClassType).Select(x => new { x.ID, x.Value }).ToList();
                var AirTicketFrequencyList = AllDropdownsData.Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.AirTicketFrequency).Select(x => new { x.ID, x.Value }).ToList();
                var InsuranceClassTypeList = AllDropdownsData.Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.InsuranceClassType).Select(x => new { x.ID, x.Value }).ToList();
                var SpecialtyTypeList = AllDropdownsData.Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.SpecialtyType).Select(x => new { x.ID, x.Value }).ToList();
                var ClassificationTypeList = AllDropdownsData.Where(e => e.DropDownID == (int)sys_dropdown_mfEnum.ClassificationType).Select(x => new { x.ID, x.Value }).ToList();
                var Designations = _pr_designationService.Queryable().Where(e => e.CompanyID == CompanyID).Select(x => new { x.ID, x.DesignationName }).ToList();
                var Departments = _pr_departmentService.Queryable().Where(e => e.CompanyID == CompanyID).Select(x => new { x.ID, x.DepartmentName }).ToList();
                var PaySchedules = _pr_pay_scheduleService.Queryable().Where(e => e.CompanyID == CompanyID).ToList();
                var DefaultAllowancesList = _pr_allowanceService.Queryable().Where(e => e.CompanyID == CompanyID).ToList();
                var ConDedList = _pr_deduction_contributionService.Queryable().Where(e => e.CompanyID == CompanyID && (e.Category == "C" || e.Category == "D")).ToList();
                var ContributionList = ConDedList.Where(x => x.Category == "C").ToList();
                var DeductionList = ConDedList.Where(x => x.Category == "D").ToList();
                var VacationAndSickLeaves = _pr_leave_typeService.Queryable().Where(e => e.CompanyID == CompanyID).ToList();
                objResponse.ResultSet = new
                {
                    Paytypes = Paytypes,
                    EmployeeTypes = EmployeeTypes,
                    PaymentMethodTypes = PaymentMethodTypes,
                    PayDayFallOnHolidayTypes = PayDayFallOnHolidayTypes,
                    AllowanceCategoryTypes = AllowanceCategoryTypes,
                    Designations = Designations,
                    Departments = Departments,
                    Cities = Cities,
                    Countries = Countries,
                    PaySchedules = PaySchedules,
                    DefaultAllowancesList = DefaultAllowancesList,
                    ContributionList = ContributionList,
                    DeductionList = DeductionList,
                    Statuses = Statuses,
                    VacationAndSickLeaves = VacationAndSickLeaves,
                    AccrualFrequencies = AccrualFrequencies,
                    Nationalities = Nationalities,
                    DocumentTypes = DocumentTypes,
                    Employee = Employee,
                    MaritalStatusList = MaritalStatusList,
                    ContractTypeList = ContractTypeList,
                    RelationshipTypeList = RelationshipTypeList,
                    AirTicketClassTypeList = AirTicketClassTypeList,
                    AirTicketFrequencyList = AirTicketFrequencyList,
                    InsuranceClassTypeList = InsuranceClassTypeList,
                    SpecialtyTypeList = SpecialtyTypeList,
                    ClassificationTypeList = ClassificationTypeList,
                };

                objResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        [HttpGet]
        [ActionName("GetFilterEmployees")]
        public ResponseInfo GetFilterEmployees(string Keyword)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                //active status id = 1
                objResponse.ResultSet = _service.Queryable().Where(x => x.CompanyID == CompanyID && x.StatusID == 1 && string.Concat(x.FirstName.ToLower(), " ", x.LastName.ToLower()).Contains(Keyword.ToLower()))
                    .Select(x => new
                    {
                        x.ID,
                        x.FirstName,
                        x.LastName,
                        x.BasicSalary,
                        x.EmployeePic
                    }).ToList();
            }
            catch (Exception ex)
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = ex.Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        [HttpGet]
        [ActionName("Delete")]
        public async Task<ResponseInfo> Delete(string Id)
        {
            var objResponse = new ResponseInfo();
            try
            {
                decimal CompanyID = Request.CompanyID();
                decimal[] IdList = Id.Split(',').Select(decimal.Parse).ToArray();
                //int[] IdList = Id.Split(',').Select(int.Parse).ToArray();

                pr_employee_mf Model = GetPREmployeeByID(Id);

                if (Model == null)
                {
                    objResponse.IsSuccess = false;
                    objResponse.ErrorMessage = MessageStatement.NotFound;
                    return objResponse;
                }

                foreach (var item in Model.pr_employee_allowance.ToList())
                {
                    item.ObjectState = ObjectState.Deleted;
                    _pr_employee_allowanceService.Delete(item);
                }

                foreach (var item in Model.pr_employee_ded_contribution.ToList())
                {
                    item.ObjectState = ObjectState.Deleted;
                    _pr_employee_ded_contributionService.Delete(item);
                }

                foreach (var item in Model.pr_employee_leave.ToList())
                {
                    item.ObjectState = ObjectState.Deleted;
                    _pr_employee_leaveService.Delete(item);
                }



                Model.ObjectState = ObjectState.Deleted;
                _service.Delete(Model);

                await _unitOfWorkAsync.SaveChangesAsync();
                objResponse.Message = MessageStatement.Delete;
                objResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                string Message = "";
                objResponse.IsSuccess = false;

                if (ex.InnerException == null)
                    Message = ex.Message;
                else if (ex.InnerException.InnerException == null)
                    Message = ex.InnerException.Message;
                else
                    Message = ex.InnerException.InnerException.Message;

                if (Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                    Message = MessageStatement.RelationExists;

                objResponse.ErrorMessage = Message;
                Logger.Trace.Error(ex);
            }
            return objResponse;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWorkAsync.Dispose();
            }
            base.Dispose(disposing);
        }

        public PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            throw new NotImplementedException();
        }
    }
}
