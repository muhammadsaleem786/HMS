import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Input } from '@angular/core';
import { Company } from '../../models/company.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CompanyService } from '../../setup/Setting/Company/company.service';
import { Router } from '@angular/router';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonService } from '../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { SetupDashboardService } from './SetupDashboardService';
import { GlobalVariable } from '../../../../AngularConfig/global';
import { FileService } from '../../../../CommonService/FileService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
@Component({
    selector: 'app-SetupDashboard',
    templateUrl: './SetupDashboardComponent.html',
    providers: [FormBuilder, CompanyService, SetupDashboardService, FileService],
})
export class SetupDashboardComponent implements OnInit {
    public model = new Company();
    public PayrollRegion: string;
    public CompanyTypes = [];
    public LanguageTypes = [];
    public CurrencyList = [];
    public FiscalYearList = [];
    public BusinessLocationType = [];
    public DateFormatList = [];
    public CityTypeList = [];
    public TimeZoneList = [];
    public TotalEmployees = [];
    public IsCheck: boolean = false;
    public Islaststep: boolean;
    public submitted: boolean;
    Form1: FormGroup;
    public isShowError: boolean = true;
    public Rights: number[] = [];
    public Modules: number[] = [];
    public Keywords: any[] = [];
    public PackageID: number;
    public Image: string = '';
    public IsEdit: boolean = false;
    public IsNewImage: boolean = true;
    public IsPaymentShowHide: boolean = false;
    public CompanyObj: any; public UsersObj: any;
    @ViewChild('closeModal') closeModal: ElementRef;
    constructor(public _fb: FormBuilder, public _router: Router, public companyService: CompanyService
        , public commonservice: CommonService, public toastr: CommonToastrService
        , public loader: LoaderService, private encrypt: EncryptionService, public _setupService: SetupDashboardService
        , private http: FileService) {
        this.Modules = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Modules')))
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')))
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.UsersObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
    }
    ngOnInit() {
        this.Form1 = this._fb.group({
           
        });
        this.commonservice.LoadDropdown("1,2,3,4,6,7,8,9").then(m => {
            if (m.IsSuccess) {
                let list = m.ResultSet.dropdownValues;
                //this.CompanyTypes = list.filter(f => f.DropDownID == 1);
                //this.TotalEmployees = list.filter(f => f.DropDownID == 2);
                //this.BusinessLocationType = list.filter(f => f.DropDownID == 3);
                //this.CityTypeList = list.filter(f => f.DropDownID == 4);
                //this.FiscalYearList = list.filter(f => f.DropDownID == 6);
                //this.CurrencyList = list.filter(f => f.DropDownID == 7);
                //this.TimeZoneList = list.filter(f => f.DropDownID == 8);
                //this.DateFormatList = list.filter(f => f.DropDownID == 9);
                this.LanguageTypes = m.ResultSet.languaheList;
            }
            //this.loader.HideLoader();
        });
        this.PayrollRegion = localStorage.getItem("PayrollRegion");
        //this.GetNotifications();
    }
    AddProfile() {
        var id = this.UsersObj.ID;
       this._router.navigate(['home/Profile/saveuser'], { queryParams: { id: id } });
    }
    ReceivePaymentShowHide(event: any) {
        if (event == true)
            this.IsPaymentShowHide = true;
        else
            this.IsPaymentShowHide = false;
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        //this.isShowError = this.isWrkingDaySelected(true);
        if (isValid)
            isValid = this.isShowError;

        //if (isValid && this.model.Website != '') {
        //    var regex = new RegExp(/^((ftp|http|https):\/\/)?(www.)?(?!.*(ftp|http|https|www.))[a-zA-Z0-9_-]+(\.[a-zA-Z]+)+((\/)[\w#]+)*(\/\w+\?[a-zA-Z0-9_]+=\w+(&[a-zA-Z0-9_]+=\w+)*)?$/gm);
        //    if (!regex.test(this.model.Website)) {
        //        this.toastr.Error('Incorrect Url', 'Please enter valid website url. e.g www.google.com (OR) https://www.google.com')
        //        isValid = false;
        //    }
        //}



        if (isValid) {
            this.loader.ShowLoader();
            this.submitted = false;
            this.companyService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    localStorage.removeItem('company');
                    var company = JSON.stringify(result.ResultSet);
                    localStorage.setItem('company', this.encrypt.encryptionAES(company));
                    this.closeModal.nativeElement.click();
                    this.loader.HideLoader();
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage);

                this.loader.HideLoader();
            });
        }
    }
    IsvalidPhone(txtbxName: string) {
        var re = new RegExp('^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$');
        if (txtbxName == 'HP' && !re.test(this.model.Phone))
            this.model.Phone = '';
    }
    GetCompany() {
        this.loader.ShowLoader();
        var companyID = this.CompanyObj.CompanyID;
        this.companyService.GetById(parseInt(companyID)).then(m => {
            if (m.IsSuccess) {
                this.model = m.ResultSet;
                if (this.model.CompanyLogo != null && this.model.CompanyLogo != undefined && this.model.CompanyLogo != "") {
                    this.IsNewImage = false;
                    this.getImageUrlName(this.model.CompanyLogo);
                }
            }
            else {
                this.toastr.Error('Not Found', 'Company not found!');
                this._router.navigate(['/login']);
            }
            this.loader.HideLoader();
        });
    }
    Close() {
        this.model = new Company();
        this.submitted = false;
    }
    IsNewImageEvent(FName) {
        this.IsNewImage = true;
    }
    getFileName(FName) {
        this.model.CompanyLogo = FName;
    }
    ClearImageUrl() {
        $(".ImgCropper").hide();
        this.IsNewImage = true;
        this.model.CompanyLogo = '';
        this.Image = '';
    }
    getImageUrlName(FName) {
        var img = $('.ImgCropper');
        if (img.length > 0) {
            $('.ImgCropper')[0].classList.remove('hide');
        }
        this.model.CompanyLogo = FName;
        if (GlobalVariable.IsUseS3 == "Yes") {
            if (!this.IsNewImage)
                this.Image = this.model.CompanyLogo == FName ? this.model.CompanyLogo : "";
            else
                this.Image = GlobalVariable.BASE_Temp_File_URL + '' + FName;
        }
        else {
            if (this.IsEdit && !this.IsNewImage) {
                this.Image = GlobalVariable.BASE_File_URL + '' + FName;
            } else {
                this.Image = GlobalVariable.BASE_Temp_File_URL + '' + FName;
            }
        }
    }
    PublicHoliday() {
        localStorage.setItem('CallbackType', "S");
        this._router.navigate(['home/holiday']);
    }
}
