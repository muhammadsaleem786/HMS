import { Component, OnInit, ViewChild, ElementRef, TemplateRef, Input, Output } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { emr_patient, ipd_admission_vital, ipd_procedure_mf, ipd_procedure_charged, ipd_procedure_medication, ipd_procedure_expense } from './AdmitModel';
import { Observable } from 'rxjs';
import { AdmitService } from './AdmitService';
import { AppointmentService } from './../Appointment/AppointmentService';
import { Service } from './../Setting/Service/Service';
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { filter } from 'rxjs/operators';
import { IMyDateModel } from 'mydatepicker';
import { CommonService } from '../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { GlobalVariable } from '../../../../AngularConfig/global';
import { SaleService } from './../Items/Sale/SaleService';

declare var $: any;
@Component({
    templateUrl: './AdmitProcedureComponentList.html',
    moduleId: module.id,
    providers: [AdmitService, AppointmentService, Service, SaleService],
})
export class AdmitProcedureComponentList implements OnInit {
    public Form1: FormGroup;
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public Id: string;
    public IsReadOnly = false;
    public ScreenLists = [];
    public Modules = [];
    public ID: number = 10;
    public IsAdmin: boolean = false;
    public model = new ipd_procedure_mf();
    public PayrollRegion: string;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public Keywords: any[] = [];
    public AdmitId: any;
    public sub: any;
    public IsEdit: boolean = false;
    public CompanyInfo: any[] = [];
    public ProcedureList: any[] = [];
    public CPTCodeList: any[] = [];
    public CategoryList: any[] = [];
    public ClinicName: string;
    public Rights: any;
    public ProeduresRights: any;
    public PatientId: any;
    nextPage: number = 1; public CompanyObj: any;
    totalPages: number = 0; public InvoiceBillModel: any[] = [];
    pagesRange: number[] = []; previousPage: number = 1;
    public ipd_procedure_charged_dynamicArray = [];
    public ipd_procedure_medication_dynamicArray = [];
    public ipd_procedure_expense_dynamicArray = [];
    @ViewChild("NewProcedure") NewProcedure: TemplateRef<any>;
    constructor(public _fb: FormBuilder,
        public loader: LoaderService,
        public commonservice: CommonService
        , public toastr: CommonToastrService, public _CommonService: CommonService,
        public _AppointmentService: AppointmentService,
        public route: ActivatedRoute, public _SaleService: SaleService, private encrypt: EncryptionService, public _router: Router, public _AdmitService: AdmitService, public _Service: Service, private modalService: NgbModal) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ProeduresRights = this._CommonService.ScreenRights("46");
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.ClinicName = this.CompanyObj.CompanyName;
    }
    ngOnInit() {
        this.AdmitId = localStorage.getItem('AdmitId');
        this.PatientId = localStorage.getItem('PatientId');
        this.PModel.SortName = "";
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);
        this.Form1 = this._fb.group({
            CompanyId: [''],
            AdmissionId: [''],
            PatientId: [''],
            ServiceId: [''],
            PatientProcedure: ['', [Validators.required]],
            Date: [''],
            Time: [''],
            CPTCodeId: [''],
            CPTCodeDropdownId: [''],
            Location: [''],
            Physician: ['', [Validators.required]],
            Assistant: [''],
            Notes: [''],
            Price: [''],
            Discount: [''],
            PaidAmount: ['']
        });
    }
    selectPage(page: number) {
        if (page == 0 || (page != 1 && this.PModel.CurrentPage == page && this.pagesRange.length > 0)) return;
        this.PModel.CurrentPage = page;
        this.GetProcedureList();
    }
    getPages(totalRecord: number, recordPerPage: number) {

        if (!isNaN(totalRecord))
            this.totalPages = this.getTotalPages(totalRecord, recordPerPage);
        this.getpagesRange();
    }
    getpagesRange() {
        if (this.pagesRange.indexOf(this.PModel.CurrentPage) == -1 || this.totalPages <= 10)
            this.papulatePagesRange();
        else if (this.pagesRange.length == 1 && this.totalPages > 10)
            this.papulatePagesRange();
    }
    papulatePagesRange() {
        this.pagesRange = [];
        var Result = Math.ceil(Math.max(this.PModel.CurrentPage, 1) / Math.max(this.PModel.RecordPerPage, 1));
        this.previousPage = ((Result - 1) * this.PModel.RecordPerPage)
        this.nextPage = (Result * this.PModel.RecordPerPage) + 1;
        if (this.nextPage > this.totalPages)
            this.nextPage = this.totalPages;
        for (var i = 1; i <= 10; i++) {
            if ((this.previousPage + i) > this.totalPages) return;
            this.pagesRange.push(this.previousPage + i)
        }
    }
    getTotalPages(totalRecord: number, recordPerPage: number): number {

        return Math.ceil(Math.max(totalRecord, 1) / Math.max(recordPerPage, 1));
    }
    GetProcedureList() {
        this.ProcedureGridList();
    }
    ProcedureGridList() {
        this.PModel.VisibleColumnInfo = "DateRecorded#DateRecorded,Temperature#Temperature,Weight#Weight,Height#Height,SBP#SBP";
        this.loader.ShowLoader();
        this.Id = "0";
        this._AdmitService.GetProeduresList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText, this.AdmitId, this.PatientId, '0').then(m => {
            this.PModel.TotalRecord = m.TotalRecord;
            this.ProcedureList = m.DataList;
            this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
            this.loader.HideLoader();
        });
    }
    LoadService() {
        //Search By Name
        $('#txtService').autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._Service.searchService(request.term).then(m => {
                    const items = m.ResultSet.serviceInfo;
                    response(items);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.model.PatientProcedure = ui.item.label;
                this.model.ServiceId = ui.item.value;
                this.loader.ShowLoader();
                this._Service.GetById(ui.item.value).then(m => {
                    if (m.IsSuccess) {
                        this.ipd_procedure_charged_dynamicArray = [];
                        this.model.Price = m.ResultSet.Result.Price;
                        this.model.Discount = 0;
                        this.model.PaidAmount = this.model.Price - this.model.Discount;
                        const itemList = m.ResultSet.itemList;
                        m.ResultSet.Result.emr_service_item.forEach((item, index) => {
                            let itemObj = itemList.filter(x => x.ID == item.ItemId);
                            item.Item = itemObj[0].Name;
                            item.ItemId = item.ItemId;
                            item.Date = item.CreatedDate;
                            if (itemObj[0].ItemTypeId == 3)
                                item.IsBatch = true;
                            else
                                item.IsBatch = false;
                            this.ipd_procedure_charged_dynamicArray.push(item);
                        });
                    }
                    this.loader.HideLoader();
                });
            }
        });
    }
    LoadItem(indx: any) {
        var id = ("#txtItem_" + indx);
        $(id).autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._SaleService.searchByName(request.term).then(m => {
                    response(m.ResultSet.ItemInfo);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.ipd_procedure_charged_dynamicArray[indx].ItemId = ui.item.value;
                this.ipd_procedure_charged_dynamicArray[indx].Item = ui.item.label;
                this.ipd_procedure_charged_dynamicArray[indx].Quantity = 1;
                if (ui.item.Type == 3)
                    this.ipd_procedure_charged_dynamicArray[indx].IsBatch = true;
                else
                    this.ipd_procedure_charged_dynamicArray[indx].IsBatch = false;
            }
        });
    }
    LoadBatch(indx: any) {
        var id = ("#txtBatch_" + indx);
        var itemid = this.ipd_procedure_charged_dynamicArray[indx].ItemId;
        $(id).autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._SaleService.searchBatchNo(itemid, request.term).then(m => {
                    response(m.ResultSet.BatchInfo);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.ipd_procedure_charged_dynamicArray[indx].Batch = ui.item.label;
                this.ipd_procedure_charged_dynamicArray[indx].BatchId = ui.item.value;
            }
        });
    }
    LoadMedicine(indx: any) {
        var id = ("#txtMedicine_" + indx);
        $(id).autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._AppointmentService.searchMedicine(request.term).then(m => {
                    response(m.ResultSet.medicineInfo);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.ipd_procedure_medication_dynamicArray[indx].MedicineId = ui.item.value;
                this.ipd_procedure_medication_dynamicArray[indx].MedicineName = ui.item.label;
                this.ipd_procedure_medication_dynamicArray[indx].Quantity = 1;
                $(id).val(ui.item.label);
                return ui.item.label;
            }
        });
    }
    Addprocedure_charged() {
        var obj = new ipd_procedure_charged();
        obj.Date = new Date();
        obj.IsBatch = false;
        this.ipd_procedure_charged_dynamicArray.push(obj);
    }
    Addprocedure_medication() {
        var obj = new ipd_procedure_medication();
        this.ipd_procedure_medication_dynamicArray.push(obj);
    }
    Addprocedure_expense() {
        var obj = new ipd_procedure_expense();
        this.ipd_procedure_expense_dynamicArray.push(obj);
    }

    Remove_procedure_expense_row(rowno: any) {
        this.ipd_procedure_expense_dynamicArray.splice(rowno, 1);

    }
    RemoveMedicationRow(rowno: any) {
        //if (this.ipd_procedure_medication_dynamicArray.length > 1) {
        this.ipd_procedure_medication_dynamicArray.splice(rowno, 1);
        //}
    }
    RemoveChargedRow(rowno: any) {
        if (this.ipd_procedure_charged_dynamicArray.length > 1) {
            this.ipd_procedure_charged_dynamicArray.splice(rowno, 1);
        }
    }
    ProcedureSaveOrUpdate(isValid: boolean): void {
        this.ipd_procedure_charged_dynamicArray.forEach((item, index) => {
            if (item.IsBatch == true) {
                if (item.BatchId == "" || item.BatchId == null) {
                    isValid = false;
                    this.toastr.Error("Batch# is required at the row no. " + index++);
                    return true;
                }
            }
        });
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this.model.AdmissionId = this.AdmitId;
            this.model.PatientId = this.PatientId;
            this.model.ipd_procedure_charged = this.ipd_procedure_charged_dynamicArray;
            this.model.ipd_procedure_medication = this.ipd_procedure_medication_dynamicArray;
            this.model.ipd_procedure_expense = this.ipd_procedure_expense_dynamicArray;
            this._AdmitService.ProcedureSaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this.modalService.dismissAll();
                    this.ProcedureGridList();
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    AddProedures(NewProcedure) {
        this.loader.ShowLoader();
        this.ipd_procedure_charged_dynamicArray = [];
        this.ipd_procedure_expense_dynamicArray = [];
        this.model = new ipd_procedure_mf();
        let PatientId = localStorage.getItem('PatientId');
        this._AdmitService.GetProeduresDropDown(PatientId).then(m => {
            if (m.IsSuccess) {
                var date = new Date();
                this.model.Date = new Date();
                this.model.Time = this.getCurrentTime();
                this.CategoryList = m.ResultSet.DropdownList.filter(a => a.DropDownID == 18);
                this.CPTCodeList = m.ResultSet.DropdownList.filter(a => a.DropDownID == 29);
                this.modalService.open(NewProcedure, { size: 'lg' });
            }
            this.loader.HideLoader();
        });
    }
    getCurrentTime(): string {
        const now = new Date();
        const hours = now.getHours().toString().padStart(2, '0');
        const minutes = now.getMinutes().toString().padStart(2, '0');
        return `${hours}:${minutes}`;
    }
    Delete(id: any) {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._AdmitService.DeleteProcedure(id).then(m => {
                if (m.ErrorMessage != null) {
                    alert(m.ErrorMessage);
                }
                this.ProcedureGridList();
                this.loader.HideLoader();
            });
        }
    }
    Edit(id: any) {
        this.loader.ShowLoader();
        this._AdmitService.ProcedureGetById(id).then(m => {
            if (m.ResultSet != null) {
                this.IsEdit = true;
                this.CPTCodeList = m.ResultSet.DropdownList.filter(a => a.DropDownID == 29);
                this.CategoryList = m.ResultSet.DropdownList.filter(a => a.DropDownID == 18);
                this.model = m.ResultSet.result;
                this.ipd_procedure_charged_dynamicArray = [];
                this.ipd_procedure_medication_dynamicArray = [];
                this.ipd_procedure_expense_dynamicArray = [];
                m.ResultSet.result.ipd_procedure_charged.forEach((item, index) => {
                    this.ipd_procedure_charged_dynamicArray.push(item);
                });
                m.ResultSet.result.ipd_procedure_expense.forEach((item, index) => {
                    this.ipd_procedure_expense_dynamicArray.push(item);
                });
                m.ResultSet.result.ipd_procedure_medication.forEach((item, index) => {
                    item.MedicineName = item.emr_medicine.Medicine;
                    this.ipd_procedure_medication_dynamicArray.push(item);
                });
                this.modalService.open(this.NewProcedure, { size: 'lg' });
            }
            this.loader.HideLoader();
        });
    }
}
