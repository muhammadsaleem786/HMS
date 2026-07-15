import { Component, OnInit, HostListener } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Login } from '../../model/Login.model';
import { Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthenticationService } from '../../../CommonService/AuthenticationService';
import { AccountService } from '../account.service';
//import { AuthService, FacebookLoginProvider, GoogleLoginProvider } from 'angular5-social-login';
import { ValidationVariables } from '../../../AngularConfig/global';
import { User } from '../../model/User.model';
import { LoaderService } from '../../../CommonService/LoaderService';
import { CommonService } from '../../../CommonService/CommonService';
import { CommonToastrService } from '../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../CommonService/encryption.service';
@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css'],
    providers: [AuthenticationService]
})

export class LoginComponent implements OnInit {
    @HostListener("window:keydown.Ctrl.s", ["$event"])
    public model = new User();
    public item: string;
    Socialmodel: any = {};
    public Form1: FormGroup;
    public Form2: FormGroup;
    public submitted: boolean;
    public AmountDue: number;
    public DueDate: string;
    public IsSiginSocialShow: boolean = false;
    AccountType: string;
    public PayrollRegion: string;
    public Keywords: any[] = [];
    public IsPasswordShow: boolean = true;
    public IsEmailShow: boolean = false;
    public Rights: number[] = [];
    public ShowEmail: string;
    public LoaderClass: string;
    onKeyDownAltM(e) {
        e.preventDefault();
        console.log(e);
        console.log("ALT + M");
    }
    constructor(public _fb: FormBuilder, public _router: Router,
        public _authService: AuthenticationService, public accountService: AccountService,
        public loader: LoaderService, private encrypt: EncryptionService
        , public _commonService: CommonService,
        public toastr: CommonToastrService) {
        this.accountService.IsPayrollRegion();
        this.PayrollRegion = this._commonService.getPayrollRegion();

    }
    ngOnInit() {
        this.resetForm();
        this.Form1 = this._fb.group({
            Pwd: ['', [Validators.required, <any>Validators.minLength(6),]],
            Email: ['', [<any>Validators.required, Validators.pattern(ValidationVariables.EmailPattern)]],
        });
        this.Form2 = this._fb.group({
            Name: ['', [Validators.pattern(ValidationVariables.AlphabetPattern), Validators.required]],
            Email: ['', [<any>Validators.required, Validators.pattern(ValidationVariables.EmailPattern)]],
            PhoneNo: ['', [Validators.pattern(ValidationVariables.NumberPattern), <any>Validators.required, <any>Validators.minLength(10), <any>Validators.maxLength(20)]],
        });
        this.model.Email = localStorage.getItem('email');
    }
    resetForm() {
        this.IsPasswordShow = true;
        this.IsEmailShow = false;
    }
    async login(isValid: boolean): Promise<void> {
        if (!isValid) return;
        debugger
        this.submitted = false;
        this.LoaderClass = "changeloadbtn";
        this.loader.ShowLoader();

        try {
            const response = await this.accountService.Login(this.model);
            const result = JSON.parse(response._body);

            if (!result.IsSuccess) {
                this.toastr.Error('Error', result.ErrorMessage);
                this.loader.HideLoader();
                return;
            }

            const user = result.ResultSet.User;
            const expiryDate = user.ExpiryDate;
            const today = this._commonService.GetFormatDate(new Date());

            this.Rights = JSON.parse(
                this.encrypt.decryptionAES(localStorage.getItem('Rights'))
            );

            // Admin / Special user
            if (user.Email === "saleem4292@gmail.com") {
                this._router.navigate(['/home/dashboard']);
                return;
            }

            // Account Expiry Check
            if (expiryDate && today > this._commonService.GetFormatDate(expiryDate)) {
                this.handleExpiry();
                this.loader.HideLoader();
                return;
            }

            // Rights-based navigation
            this.navigateByRights();
        } catch (error) {
            console.error(error);
            this.toastr.Error('Error', 'Something went wrong');
        } finally {            
            this.LoaderClass = "";
        }
    }
    private navigateByRights(): void {
        const routes: { right: number, path: string }[] = [
            { right: 78, path: '/home/dashboard' },
            { right: 22, path: '/home/Appoint' },
            { right: 11, path: '/home/Patient' },
            { right: 12, path: '/home/Billing' },
            { right: 21, path: '/home/Prescription' },
            { right: 63, path: '/home/Sale' },
            { right: 61, path: '/home/Item' },
            { right: 20, path: '/home/Income' },
            { right: 14, path: '/home/Expense' },
            { right: 62, path: '/home/Invoice' },
            { right: 76, path: '/home/session' }
        ];

        const firstAllowedRoute = routes.find(r => this.Rights.includes(r.right));

        if (firstAllowedRoute) {
            this._router.navigate([firstAllowedRoute.path]);
        } else {
            this._router.navigate(['/home/dashboard']);
        }
   
    }
    private handleExpiry(): void {
        this.toastr.Error("Expire", "Your account is expired. Please contact administrator.");

        const keysToRemove = [
            'IsLogin', 'currentUser', 'UserID', 'Token',
            'PayrollRegion', 'Currency', 'ValidTo',
            'Modules', 'Rights', 'ControlLevelRights'
        ];

        keysToRemove.forEach(k => localStorage.removeItem(k));
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.model.AccountType = this.AccountType;
            this.loader.ShowLoader();
            this.accountService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    //alert(result.Message);
                    this.IsSiginSocialShow = false;
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage);

                this.loader.HideLoader();
            });
        }
    }
    public socialSignIn(socialPlatform: string) {
        this.toastr.Info("Coming Soon", "This page is under construction...");
        //    let socialPlatformProvider;
        //    if (socialPlatform == "facebook")
        //        socialPlatformProvider = FacebookLoginProvider.PROVIDER_ID;
        //    else if (socialPlatform == "google")
        //        socialPlatformProvider = GoogleLoginProvider.PROVIDER_ID;
        //    this.socialAuthService.signIn(socialPlatformProvider).then(
        //        (userData) => {
        //            if (socialPlatform == "facebook")
        //                this.AccountType = "F";
        //            else if (socialPlatform == "google")
        //                this.AccountType = "G";
        //            else
        //                this.AccountType = "A";
        //            this.Socialmodel = {
        //                Name: userData.name,
        //                Email: userData.email,
        //                AccountType: this.AccountType,
        //            }
        //            this.loader.ShowLoader();
        //            this.IsSiginSocialShow = false;
        //            this.accountService.Login(this.Socialmodel).then(
        //                data => {
        //                    var result = JSON.parse(data._body);
        //                    if (result.IsSuccess) {
        //                        if (result.Message == 'CreLocalAcc') {
        //                            this.model.Email = this.Socialmodel.Email;
        //                            this.model.Name = this.Socialmodel.Name;
        //                            this.IsSiginSocialShow = true;
        //                            localStorage.setItem('Acctype', this.AccountType);
        //                        } else {
        //                            if (result.Message)
        //                                this._router.navigate(['/confirmemail']);
        //                            else
        //                                this._router.navigate(['/company']);
        //                        }
        //                    }
        //                    this.loader.HideLoader();
        //                },
        //                error => {
        //                    this.loader.HideLoader();
        //                    //this.toastr.Error('Error', error);
        //                });
        //        }
        //    );
    }
}
