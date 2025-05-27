import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { holidayModel } from './holidayModel';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { holidayService } from './holidayService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { GlobalVariable } from '../../../../../AngularConfig/global';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { IMyDateModel } from 'mydatepicker';
import { Router, ActivatedRoute } from '@angular/router';
import { filter } from 'rxjs/operators';
import { CommonService } from '../../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';


@Component({
    selector: 'setup-holidayComponentForm',
    templateUrl: './holidayComponentForm.html',
    moduleId: module.id,
    providers: [holidayService],
})

export class holidayComponentForm implements OnInit {
    public Form1: FormGroup; // our model driven form
    public submitted: boolean; // keep track on whether form is submitted
    @Output() pageClose: EventEmitter<number> = new EventEmitter<number>();
    @Input() ScreenName: string;
    @Input() id: number;
    public IsReadOnly = false;
    public sub: any;
    public IsEdit: boolean = false;
    public model = new holidayModel();
    public Rights: any;
    public ControlRights: any;
    constructor(public _fb: FormBuilder, public loader: LoaderService, public _router: Router
        , public _holidayService: holidayService, public toastr: CommonToastrService, public route: ActivatedRoute,
        public commonservice: CommonService, private encrypt: EncryptionService) {
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this.commonservice.ScreenRights("59");    }

    ngOnInit() {
        this.Form1 = this._fb.group({
            HolidayName: ['', [Validators.required]],
            FromDate: ['', [Validators.required]],
            ToDate: ['', [Validators.required]],
        });
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.id = params.id;
                if (this.id > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this._holidayService.GetById(this.id).then(m => {
                        this.model = m.ResultSet;
                    });
                }
                this.loader.HideLoader();
            });
    }
  
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true

        if (isValid) {
            this.model.FromDate = new Date(this.GetFormatDate(new Date(this.model.FromDate)));
            this.model.ToDate = new Date(this.GetFormatDate(new Date(this.model.ToDate)));
            isValid = this.IsFromToDateValid();
        }

        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._holidayService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.loader.HideLoader();
                    this._router.navigate(['/home/holiday']);

                }
                else {
                    this.loader.HideLoader();
                    this.toastr.Error('Error', result.ErrorMessage);
                }
            });
        }
    }

    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._holidayService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);
                this._router.navigate(['/home/holiday']);

            });
        }
    }


    onFromDateChanged(event: IMyDateModel) {
        if (event) {
            var date = new Date(event.jsdate);
            if (this.model.ToDate == undefined || this.model.ToDate == null)
                this.model.ToDate = event.jsdate;

            this.model.FromDate = event.jsdate;
        }

    }

    onToDateChanged(event: IMyDateModel) {
        if (event) {
            this.model.ToDate = event.jsdate;
        }
    }

    GetFormatDate(date) {
        var yyyy = date.getFullYear();
        var mm = date.getMonth() < 9 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1); // getMonth() is zero-based
        var dd = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        return yyyy + '-' + mm + '-' + dd;
    };


    IsFromToDateValid(): boolean {
        if (this.model.FromDate > this.model.ToDate) {
            this.toastr.Error('Invalid Date', 'From date should be less than or equal To date');
            return false;
        }
        return true;
    }

    Close() {
        
        this.model = new holidayModel();
        this.submitted = false;

    }
}
