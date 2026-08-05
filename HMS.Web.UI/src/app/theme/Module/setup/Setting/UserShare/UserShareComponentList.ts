import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { UserShareService } from './UserShareService';
import { UserShareModel } from '../UserShare/UserShareModel';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
declare var $: any;
@Component({
    moduleId: module.id,
    templateUrl: 'UserShareComponentList.html',
    providers: [UserShareService],
})
export class UserShareComponentList implements OnInit {
    public Form1: FormGroup;
    @Input() ScreenName: string;
    @Input() id: number;
    public IsReadOnly = false; public submitted: boolean;
    public ScreenLists = [];
    public Modules = [];
    public model = new UserShareModel();
    public IsAdmin: boolean = false;
    public PayrollRegion: string;
    public Keywords: any[] = [];
    public DoctorList: any[] = [];
    public sub: any;
    public IsEdit: boolean = false;
    public CompanyInfo: any[] = [];
    public Rights: any;
    public ControlRights: any;
    public shareList: any[] = [];
    public categoryList: any[] = [];
    public UserSharedynamicArray = [];
    constructor(public _CommonService: CommonService, public _fb: FormBuilder, public loader: LoaderService
        , public _UserShareService: UserShareService, private encrypt: EncryptionService, public commonservice: CommonService
        , public toastr: CommonToastrService, public route: ActivatedRoute, public _router: Router) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("77");
    }
    ngOnInit() {
        this.categoryList = [
            { ID: 1, Name: 'Procedure' },
            { ID: 2, Name: 'Laboratory' },
            { ID: 3, Name: 'Pharmacy' }
        ];
        this.loadDropdown();
        this.Form1 = this._fb.group({
            DoctorId: ['', [Validators.required]],
            DisDoctorShare: [''],
            DisOwnerShare: [''],
            DeductOTCharges: [''],
            DeductMedicineCost: [''],
            DeductConsumables: [''],
            DeductImplantCost: [''],
            DeductLabConsumables: [''],
            DeductRoomCharges: ['']
        });
    }
    onDoctorChange(doctorId: any) {
        if (!doctorId) {
            return;
        }
        this.loader.ShowLoader();
        this._UserShareService.GetByDoctorId(doctorId).then(res => {
            if (res.ResultSet != null) {
                this.model = res.ResultSet.Master;
                this.UserSharedynamicArray = res.ResultSet.Details;
            }
            this.loader.HideLoader();

        }).catch(() => {
            this.loader.HideLoader();
        });

    }
    onCategoryChange() {
        this.model.ServiceName = '';

        this.model.ServiceId = null;
        this.model.LabId = null;
        this.model.ItemId = null;

        const $service = $('#serviceSearch');

        if ($service.data('ui-autocomplete')) {
            $service.autocomplete('destroy');
        }

        this.LoadSearchableService();
    }
    LoadSearchableService() {

        $('#serviceSearch').autocomplete({
            source: (request, response) => {

                this.loader.ShowLoader();

                let apiCall: Promise<any>;

                switch (+this.model.CategoryId) {

                    case 1: // Procedure
                        apiCall = this._UserShareService.searchServices(request.term);
                        break;

                    case 2: // Laboratory
                        apiCall = this._UserShareService.SearchLabTest(request.term);
                        break;

                    case 3: // Pharmacy
                        apiCall = this._UserShareService.SearchItemByName(request.term);
                        break;

                    default:
                        response([]);
                        return;
                }

                apiCall.then(res => {

                    let data = [];

                    if (+this.model.CategoryId === 1)
                        data = res.ResultSet.ServiceInfo;

                    else if (+this.model.CategoryId === 2)
                        data = res.ResultSet.investigation;

                    else if (+this.model.CategoryId === 3)
                        data = res.ResultSet.ItemInfo;

                    response(data);
                    this.loader.HideLoader();

                }).catch(() => {
                    this.loader.HideLoader();
                });

            },

            minLength: 3,

            select: (event, ui) => {
                this.model.ServiceId = null;
                this.model.LabId = null;
                this.model.ItemId = null;

                // Assign based on Category
                switch (+this.model.CategoryId) {

                    case 1: // Procedure
                        this.model.ServiceId = ui.item.value;
                        break;

                    case 2: // Laboratory
                        this.model.LabId = ui.item.value;
                        break;

                    case 3: // Pharmacy
                        this.model.ItemId = ui.item.value;
                        break;
                }

                this.model.ServiceName = ui.item.label;

                return false;
            }
        });
    }
    loadDropdown() {
        this.loader.ShowLoader();
        this._UserShareService.FormLoad().then(m => {
            if (m.IsSuccess) {
                this.DoctorList = m.ResultSet.result;
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    SaveExpense(): void {
        if (!this.model.DoctorId) {
            this.toastr.Error('Error', 'Please select doctor.');
            return;
        }
        this.loader.ShowLoader();
        this._UserShareService.SaveExpense(this.model).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.toastr.Success(result.Message);
                this.UserSharedynamicArray = [];
                this.model = result.ResultSet.Master;
                this.UserSharedynamicArray = result.ResultSet.Details;
                this.loader.HideLoader();
            }
            else {
                this.toastr.Error('Error', result.ErrorMessage);
                this.loader.HideLoader();
            }
        });
    }
    SaveDiscount(): void {
        if (!this.model.DoctorId) {
            this.toastr.Error('Error', 'Please select doctor.');
            return;
        }
        this.loader.ShowLoader();
        this._UserShareService.SaveDiscount(this.model).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.toastr.Success(result.Message);
                this.UserSharedynamicArray = [];
                this.model = result.ResultSet.Master;
                this.UserSharedynamicArray = result.ResultSet.Details;
                this.loader.HideLoader();
            }
            else {
                this.toastr.Error('Error', result.ErrorMessage);
                this.loader.HideLoader();
            }
        });
    }
    SaveService(): void {
        if (!this.model.DoctorId) {
            this.toastr.Error('Error', 'Please select doctor.');
            return;
        }
        const category = this.categoryList.find(x => x.ID == this.model.CategoryId);
        this.model.Category = category ? category.Name : '';
        const isDuplicate = this.UserSharedynamicArray.some(x => {
            // Ignore the current record during update
            if (x.ID === this.model.ID) {
                return false;
            }
           
            switch (this.model.Category) {
                case 'Procedure':
                    return x.CategoryName === this.model.Category &&
                        x.ServiceId === this.model.ServiceId;

                case 'Laboratory':
                    return x.CategoryName === this.model.Category &&
                        x.LabId === this.model.LabId;

                case 'Pharmacy':
                    return x.CategoryName === this.model.Category &&
                        x.ItemId === this.model.ItemId;

                default:
                    return false;
            }
        });

        if (isDuplicate) {
            this.toastr.Warning('This service already exists for the selected category.');
            return;
        }
        this.submitted = true;
        this.loader.ShowLoader();
        this.model.ServiceId = this.model.ServiceId || null;
        this.model.LabId = this.model.LabId || null;
        this.model.ItemId = this.model.ItemId || null;
        debugger
        this._UserShareService.SaveOrUpdate(this.model).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.UserSharedynamicArray = [];
                this.UserSharedynamicArray = result.ResultSet.Details;
                this.model = new UserShareModel();
                this.toastr.Success(result.Message);
                this.loader.HideLoader();
            }
            else {
                this.toastr.Error('Error', result.ErrorMessage);
                this.loader.HideLoader();
            }
        });
    }
    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result) {
            this.loader.ShowLoader();
            this._UserShareService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);
                else
                    this._router.navigate(['/home/share']);
                this.loader.HideLoader();
            });
        }
    }
    Close() {
        this.model = new UserShareModel();
        this.submitted = false;
        this.IsAdmin = false;
    }
    calculateOwnerShare() {
        if (this.model.SharedMethod === 'Percentage') {
            const doctor = Number(this.model.DoctorShare) || 0;
            if (doctor > 100) {
                this.model.DoctorShare = 100;
                this.model.OwnerShare = '0';
            } else {
                this.model.OwnerShare = (100 - doctor).toString();
            }
        } else {
            // Fixed Amount
            this.model.OwnerShare = '-';
        }
    }
    calculateDisOwnerShare() {
        let doctorShare = this.model.DisDoctorShare;
        if (isNaN(doctorShare)) {
            this.model.DisOwnerShare = 0;
            return;
        }

        // Prevent values greater than 100
        if (doctorShare > 100) {
            doctorShare = 100;
            this.model.DisDoctorShare = 100;
        }

        if (doctorShare < 0) {
            doctorShare = 0;
            this.model.DisDoctorShare = 0;
        }

        this.model.DisOwnerShare = (100 - doctorShare);
    }
    editShare(Id: any) {
        this.loader.ShowLoader();
        this._UserShareService.GetById(Id).then(m => {
            if (m.IsSuccess) {
                this.model = m.ResultSet.result;
            }
            this.loader.HideLoader();
        });
    }
    deleteShare(Id: any) {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result) {
            this.loader.ShowLoader();
            this._UserShareService.Delete(Id).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);
                else {
                    this.toastr.Success(m.Message);
                    this.UserSharedynamicArray = [];
                    this.UserSharedynamicArray = m.ResultSet.Details;
                }
                this.loader.HideLoader();
            });
        }
    }
}