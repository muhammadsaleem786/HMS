import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { designationModel } from './designationModel';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { designationService } from './designationService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { CommonService } from '../../../../../CommonService/CommonService';

@Component({
    selector: 'setup-designationComponentForm',
    templateUrl: './designationComponentForm.html',
    moduleId: module.id,
    providers: [designationService],
})

export class designationComponentForm implements OnInit, OnChanges {
    public Form1: FormGroup; // our model driven form
    public submitted: boolean; // keep track on whether form is submitted
    @Output() pageClose: EventEmitter<number> = new EventEmitter<number>();
    @Input() ScreenName: string;
    @Input() id: number;
    public IsReadOnly = false;
    public model = new designationModel();
    public IsDisplayModal: boolean = true;
    public IsEdit: boolean = false;
    @ViewChild('closeModal') closeModal: ElementRef;
    public WasInside: boolean = false;
    public Rights: any;
    public ControlRights: any;
    @HostListener('click', ['$event'] )
    Clickoutdocument(event) {
        if (!this.WasInside) 
            this.Close();
        
        this.WasInside = false;
    }

    IsModalClick() {
        this.WasInside = true;
    }

    constructor(public _fb: FormBuilder, public loader: LoaderService, public _designationService: designationService
        , public toastr: CommonToastrService, private encrypt: EncryptionService, public commonservice: CommonService
    ) {
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this.commonservice.ScreenRights("54");
    }

    ngOnInit() {
        
        this.Form1 = this._fb.group({
            DesignationName: ['', [Validators.required]],
        });
    }
    ngOnChanges() {
        
        if (typeof (this.id) == "undefined") return;

        if (isNaN(this.id)) {
            this.IsReadOnly = true;
            this.id = +this.id.toString().substring(1); // (+) converts string 'id' to a number
        }
        else {
            this.id = +this.id; // (+) converts string 'id' to a number
            this.IsReadOnly = false;
        }

        if (this.id != 0) {
            //self.loader.ShowLoader();
            this._designationService.GetById(this.id).then(m => {
                this.model = m.ResultSet;
                this.loader.HideLoader();
            });
        }
    }


    SaveOrUpdate(isValid: boolean): void {

        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._designationService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.loader.HideLoader();
                    this.WasInside = true;
                    this.closeModal.nativeElement.click();
                }
                else {
                    this.loader.HideLoader();
                    this.toastr.Error('Error',result.ErrorMessage);
                }
            });
        }
    }


    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._designationService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error',m.ErrorMessage);

                this.WasInside = true;
                this.closeModal.nativeElement.click();
            });
        }
    }

    Close() {
        if (!this.WasInside)
            this.pageClose.emit(0);
        else
            this.pageClose.emit(1);
        this.model = new designationModel();
        this.submitted = false;
    }

}
