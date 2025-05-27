import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { emr_medicine } from '../../Prescription/PrescriptionModel';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { MedicineService } from './MedicineService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { filter } from 'rxjs/operators';
@Component({
    templateUrl: './MedicineComponentForm.html',
    moduleId: module.id,
    providers: [MedicineService],
})
export class MedicineComponentForm implements OnInit {
    public Form1: FormGroup;
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public IsReadOnly = false;
    public ScreenLists = [];
    public Modules = [];
    public filterdData = [];
    public UnitList: any[] = [];
    public MadicineTypeList: any[] = [];
    public InstructionsList: any[] = [];
    public model = new emr_medicine();
    public IsAdmin: boolean = false;
    public PayrollRegion: string;
    public Keywords: any[] = [];
    public sub: any;
    public IsEdit: boolean = false;
    public CompanyInfo: any[] = [];
    public Rights: any;
    public ControlRights: any;
    constructor(public _CommonService: CommonService,public _fb: FormBuilder, public loader: LoaderService
        , public _MedicineService: MedicineService, private encrypt: EncryptionService, public commonservice: CommonService
        , public toastr: CommonToastrService, public route: ActivatedRoute, public _router: Router) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("9");
        //this.Keywords = this.commonservice.GetKeywords("medicine");
    }
    ngOnInit() {
        this.loadDropdown();
        this.Form1 = this._fb.group({
            Medicine: ['', [Validators.required]],
            UnitId: ['', [Validators.required]],
            TypeId: ['', [Validators.required]],
            Price: ['', [Validators.required]],
            Measure: [''],
            InstructionId:['']
        });
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.model.ID = params.id;
                if (this.model.ID > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this._MedicineService.GetMedicineById(this.model.ID).then(m => {
                        if (m.ResultSet != null) {
                            this.model = m.ResultSet.result;
                        }
                        this.loader.HideLoader();
                    });
                } else {
                }
            });
    }
    loadDropdown() {
        this.loader.ShowLoader();
        this._MedicineService.FormLoad().then(m => {
            if (m.IsSuccess) {
                
                this.UnitList = m.ResultSet.UnitList.filter(a => a.DropDownID ==14);
                this.MadicineTypeList = m.ResultSet.UnitList.filter(a => a.DropDownID == 15);
                this.InstructionsList = m.ResultSet.InstructionList;
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; 
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._MedicineService.SaveOrUpdate(this.model).then(m => {
                    var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                        this._router.navigate(['/home/Medicine']);
                    }
                    else {
                        this.toastr.Error('Error', result.ErrorMessage);
                        this.loader.HideLoader();
                    }
                });
        }
    }
    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result) {
            this.loader.ShowLoader();
            this._MedicineService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);
                else
                    this._router.navigate(['/home/Medicine']);
                    this.loader.HideLoader();
            });
        }
    }
    Close() {
        this.model = new emr_medicine();
        this.submitted = false;
        this.IsAdmin = false;
    }
}
