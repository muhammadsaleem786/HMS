export class ReminderModel {
    constructor() {
        let objadm_reminder_dt = new adm_reminder_dt();
        this.adm_reminder_dt.push(objadm_reminder_dt);
    }
    ID: number;
    CompanyId: number;
    Name: string;
    IsEnglish: boolean;
    IsUrdu: boolean;
    MessageBody: string;    
    adm_reminder_dt: Array<adm_reminder_dt> = [];
}
export class adm_reminder_dt {
    ID: number;
    CompanyId: number;
    ReminderId: number;
    SMSTypeId: number; 
    SMSTypeDropDownId: number;
    Value: number;
    TimeTypeId: number;
    TimeTypeDropDownId: number;
    BeforeAfter: string="B";   
}