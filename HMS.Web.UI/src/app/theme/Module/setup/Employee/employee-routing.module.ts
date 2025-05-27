import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../../../../CommonService/AuthGuard';
import { ThemeComponent } from '../../../../theme/theme.component';

import { AllowanceComponentList } from '../../../../theme/Module/setup/Employee/Allowance/AllowanceComponentList';
import { AllowanceComponentForm } from '../../../../theme/Module/setup/Employee/Allowance/AllowanceComponentForm';
import { DeductionContributionComponentList } from '../../../../theme/Module/setup/Employee/DeductionContribution/DeductionContributionComponentList';
import { DeductionContributionComponentForm } from '../../../../theme/Module/setup/Employee/DeductionContribution/DeductionContributionComponentForm';
import { PayScheduleComponentList } from '../../../../theme/Module/setup/Employee/Payschedule/PayScheduleComponentList';
import { PayScheduleComponentForm } from '../../../../theme/Module/setup/Employee/Payschedule/PayScheduleComponentForm';
import { LocationsComponentList } from '../../../../theme/Module/setup/Employee/Locations/LocationsComponentList';
import { LocationsComponentForm } from '../../../../theme/Module/setup/Employee/Locations/LocationsComponentForm';
import { holidayComponentList } from '../../../../theme/Module/setup/Employee/Holidays/holidayComponentList';
import { holidayComponentForm } from '../../../../theme/Module/setup/Employee/Holidays/holidayComponentForm';

import { VacationSickLeaveComponentList } from '../../../../theme/Module/setup/Employee/VacationSickLeave/VacationSickLeaveComponentList';
import { VacationSickLeaveComponentForm } from '../../../../theme/Module/setup/Employee/VacationSickLeave/VacationSickLeaveComponentForm';
const routes: Routes = [
    {
        'path': '',
        'component': ThemeComponent,
        'canActivate': [AuthGuard],
        children: [
            { path: 'Schedule', component: PayScheduleComponentList, canActivate: [AuthGuard] },
            { path: 'Deduction', component: DeductionContributionComponentList, canActivate: [AuthGuard]  },
            { path: 'Allowance', component: AllowanceComponentList, canActivate: [AuthGuard] },
               { path: 'holiday', component: holidayComponentList, canActivate: [AuthGuard] },
            { path: 'holiday/saveholiday', component: holidayComponentForm, canActivate: [AuthGuard] },
            { path: 'holiday/saveholiday:id', component: holidayComponentForm, canActivate: [AuthGuard] },


            { path: 'locations', component: LocationsComponentList },
            { path: 'vacationsickleave', component: VacationSickLeaveComponentList },
        ]
    }
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class EmployeeRoutingModule { }
