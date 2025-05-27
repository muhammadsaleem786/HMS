import { NgModule } from '@angular/core';
import { EmployeeRoutingModule } from './employee-routing.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonUtilsModule } from '../../../../common/common-utils.module';
import { CookieService } from 'ngx-cookie-service';
import { AuthGuard } from '../../../../CommonService/AuthGuard';
import { DashboardMenuService } from '../../../../CommonService/DashboardMenuService';

import { AllowanceComponentList } from '../../../../theme/Module/setup/Employee/Allowance/AllowanceComponentList';
import { AllowanceComponentForm } from '../../../../theme/Module/setup/Employee/Allowance/AllowanceComponentForm';
import { DeductionContributionComponentList } from '../../../../theme/Module/setup/Employee/DeductionContribution/DeductionContributionComponentList';
import { DeductionContributionComponentForm } from '../../../../theme/Module/setup/Employee/DeductionContribution/DeductionContributionComponentForm';
import { PayScheduleComponentList } from '../../../../theme/Module/setup/Employee/Payschedule/PayScheduleComponentList';
import { PayScheduleComponentForm } from '../../../../theme/Module/setup/Employee/Payschedule/PayScheduleComponentForm';
import { holidayComponentList } from '../../../../theme/Module/setup/Employee/Holidays/holidayComponentList';
import { holidayComponentForm } from '../../../../theme/Module/setup/Employee/Holidays/holidayComponentForm';

import { LocationsComponentList } from '../../../../theme/Module/setup/Employee/Locations/LocationsComponentList';
import { LocationsComponentForm } from '../../../../theme/Module/setup/Employee/Locations/LocationsComponentForm';

import { VacationSickLeaveComponentList } from '../../../../theme/Module/setup/Employee/VacationSickLeave/VacationSickLeaveComponentList';
import { VacationSickLeaveComponentForm } from '../../../../theme/Module/setup/Employee/VacationSickLeave/VacationSickLeaveComponentForm';
@NgModule({
    imports: [
        CommonUtilsModule, NgbModule, 
        EmployeeRoutingModule,
    ],
    declarations: [
        AllowanceComponentList, AllowanceComponentForm, DeductionContributionComponentList, DeductionContributionComponentForm
, PayScheduleComponentList
        , PayScheduleComponentForm, LocationsComponentList, LocationsComponentForm, VacationSickLeaveComponentList
        , VacationSickLeaveComponentForm, holidayComponentList, holidayComponentForm
    ],
    providers: [
        CookieService,
        AuthGuard,
        DashboardMenuService    ]

})
export class EmployeeModule { }
