import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LocationsModel } from './LocationsModel';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { LocationsService } from './LocationsService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';

@Component({
    selector: 'setup-LocationsComponentForm',
    templateUrl: './LocationsComponentForm.html',
    moduleId: module.id,
    providers: [LocationsService],
})

export class LocationsComponentForm implements OnInit, OnChanges {
    public Form1: FormGroup; // our model driven form
    public submitted: boolean; // keep track on whether form is submitted
    @Output() pageClose: EventEmitter<number> = new EventEmitter<number>();
    @Input() ScreenName: string;
    @Input() id: number;
    public IsReadOnly = false;
    public model = new LocationsModel();
    @ViewChild('closeModal') closeModal: ElementRef;
    public Cities = [];
    public CityName = [];
    public FilterCities = [];
    public Countries = [];
    public WasInside: boolean = false;
    public PayrollRegion: string;
    @HostListener('click', ['$event'], )
    Clickoutdocument(event) {
        if (!this.WasInside)
            this.Close();

        this.WasInside = false;
    }

    IsModalClick() {
        this.WasInside = true;
    }
    public Rights: any;
    public ControlRights: any;
    constructor(public _fb: FormBuilder, public loader: LoaderService
        , public _LocationsService: LocationsService, public commonservice: CommonService
        , public toastr: CommonToastrService, private encrypt: EncryptionService) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        //this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        //this.ControlRights = this.commonservice.ScreenRights("56");
    }

    ngOnInit() {

        this.Form1 = this._fb.group({
            LocationName: ['', [Validators.required]],
            Address: ['', [Validators.required]],
            CityID: ['', [Validators.required]],
            CountryID: ['', [Validators.required]],
            ZipCode: [''],
        });

        //loading all dropdowns
        this.commonservice.LoadDropdown("3,4").then(m => {
            if (m.IsSuccess) {
                let list = m.ResultSet;
                this.Cities = list.filter(f => f.DropDownID == 4);
                this.Countries = list.filter(f => f.DropDownID == 3);
                this.DefaultCityCountrySel();
            }
        });
    }
    DefaultCityCountrySel() {
        if (this.Countries.length > 0) {
            if (this.PayrollRegion == 'SA') {
                //SA = 1;
                this.model.CountryID = 1;
            } else {
                //PK=2
                this.model.CountryID = 2;
            }
            this.CountrySelChange();
        }
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
            this._LocationsService.GetById(this.id).then(m => {
                this.model = m.ResultSet;
                this.FilterCities = this.Cities.filter(x => x.DependedDropDownValueID == this.model.CountryID);
                this.loader.HideLoader();
            });
        }
    }

    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._LocationsService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.loader.HideLoader();
                    this.WasInside = true
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
        if (result) {
            this.loader.ShowLoader();
            this._LocationsService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error',m.ErrorMessage);
                
                this.WasInside = true
                this.closeModal.nativeElement.click();
            });
        }
    }
    CountrySelChange() {
        this.FilterCities = this.Cities.filter(x => x.DependedDropDownValueID == this.model.CountryID);
        if (this.FilterCities.length > 0)
            this.model.CityID = this.FilterCities[0].ID;
        else
            this.model.CityID = undefined;
    }

    Close() {
        if (!this.WasInside)
            this.pageClose.emit(0);
        else
            this.pageClose.emit(1);

        this.model = new LocationsModel();
        this.submitted = false;
        this.DefaultCityCountrySel();
    }
}
