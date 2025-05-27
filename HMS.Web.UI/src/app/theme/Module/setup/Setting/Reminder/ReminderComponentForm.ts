import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Component, ViewChild, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { ReminderService } from './ReminderService';
import { ReminderModel, adm_reminder_dt } from './ReminderModel';
import { ActivatedRoute } from '@angular/router';
import { IMyDateModel } from 'mydatepicker';
import jsPDF from 'jspdf';
import { ValidationVariables, GlobalVariable } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { filter } from 'rxjs/operators';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { EncryptionService } from '../../../../../CommonService/encryption.service';

declare var $: any;

@Component({
    moduleId: module.id,
    templateUrl: 'ReminderComponentForm.html',
    providers: [ReminderService],
})
export class ReminderComponentForm {
    public model = new ReminderModel();
    public Reminder_itemdynamicArray = [];
    public Id: string;
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public PayrollRegion: string;
    public sub: any;
    public IsEdit: boolean = false;
    public Form1: FormGroup;
    public SMSTypeList: any[] = [];
    public TimeList: any[] = [];
    public BeforeList: any[] = [];
    public submitted: boolean;
    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    public ControlRights: any;
    constructor(public _CommonService: CommonService, public loader: LoaderService
        , public _router: Router, public _ReminderService: ReminderService, public _fb: FormBuilder
        , public route: ActivatedRoute, public toastr: CommonToastrService
        , private encrypt: EncryptionService
    ) {
        this.loader.ShowLoader();
        this.PayrollRegion = this._CommonService.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("67");
    }
    ngOnInit() {
        this.LoadDropDown();
        this.BeforeList.push({ "id": 'A', "Name": "After" });
        this.BeforeList.push({ "id": 'B', "Name": "Before" });
        this.Add();
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.model.ID = params.id;
                if (this.model.ID > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this._ReminderService.GetById(this.model.ID).then(m => {
                        if (m.ResultSet != null) {
                            this.Reminder_itemdynamicArray = [];

                            this.model = m.ResultSet;
                            this.model.adm_reminder_dt.forEach((item, index) => {
                                this.Reminder_itemdynamicArray.push(item);
                            });
                            if (this.model.adm_reminder_dt.length > 0) {

                            } else {
                                this.Add();
                            }
                            this.loader.HideLoader();
                        } else
                            this.loader.HideLoader();
                    });
                }
            });
        this.Form1 = this._fb.group({
            Name: ['', [Validators.required]],
            IsUrdu: [''],
            IsEnglish: [''],
            MessageBody: ['']
        });
    }
    LoadDropDown() {
        this.loader.ShowLoader();
        this._ReminderService.LoadDropdown("65,66").then(m => {
            if (m.IsSuccess) {
                let list = m.ResultSet.dropdownValues;
                this.SMSTypeList = list.filter(a => a.DropDownID == 66);
                this.TimeList = list.filter(a => a.DropDownID == 65);
                this.loader.HideLoader();
            }
        });
    }
    SaveOrUpdate(isValid: boolean): void {
        if (this.model.Name == undefined || this.model.Name == null) {
            this.toastr.Error("Name is required.");
            return;
        }
        this.model.adm_reminder_dt = this.Reminder_itemdynamicArray;
        this.submitted = true;
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._ReminderService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this._router.navigate(['/home/reminder']);
                    this.toastr.Success(result.Message);
                }
                else {
                    this.loader.HideLoader();
                    this.toastr.Error('Error', result.ErrorMessage);
                }
            });
        }
    }
    CheckValue(selectedLanguage: string) {
        if (selectedLanguage === 'english') {
            this.model.IsUrdu = false;
        } else if (selectedLanguage === 'urdu') {
            this.model.IsEnglish = false;
        }
    }
    Add() {
        var obj = new adm_reminder_dt();
        obj.Value = 0;
        obj.SMSTypeId = 1;
        obj.TimeTypeId = 1;
        this.Reminder_itemdynamicArray.push(obj);
    }
    RemoveRow(rowno: any) {
        this.Reminder_itemdynamicArray.splice(rowno, 1);
    }
    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._ReminderService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null) {
                    alert(m.ErrorMessage);
                }
                this.toastr.Success(m.Message);
                this._router.navigate(['/home/reminder']);
            });
        }
    }
}