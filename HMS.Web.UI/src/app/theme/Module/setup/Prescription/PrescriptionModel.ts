import { SafeHtml } from '@angular/platform-browser';
export class emr_prescription_mf {
    constructor() {
        let objemr_prescription_complaint = new emr_prescription_complaint();
        this.emr_prescription_complaint.push(objemr_prescription_complaint);

        let objemr_prescription_diagnos = new emr_prescription_diagnos();
        this.emr_prescription_diagnos.push(objemr_prescription_diagnos);

        let objemr_prescription_investigation = new emr_prescription_investigation();
        this.emr_prescription_investigation.push(objemr_prescription_investigation);

        let objemr_prescription_observation = new emr_prescription_observation();
        this.emr_prescription_observation.push(objemr_prescription_observation);

        let objemr_prescription_treatment = new emr_prescription_treatment();
        this.emr_prescription_treatment.push(objemr_prescription_treatment);
    }
    ID: number;
    CompanyID: number;
    IsTemplate: boolean = false;
    IsVCObtained: boolean = false;
    AppointmentDate: string;
    RefillNo: string;
    AppointDate: string;
    PatientId: number;
    DoctorId: number;
    ClinicId: number;
    PatientName: string;
    Gender: string;
    Mobile: string;
    FollowUpDate: Date;
    FollowUpNotes: string;
    FollowUpTime: string;
    IsCreateAppointment: boolean = false;
    AppointmentId: number;
    Notes: string;
    TemplateName: string;
    Email: string;
    Day: string;
    ClinicalNote: string;
    PlanNote: string;
    safeClinicalNote: SafeHtml;
    safePlanNote: SafeHtml;
    emr_prescription_complaint: Array<emr_prescription_complaint> = [];
    emr_vital: Array<emr_vital> = [];
    emr_prescription_diagnos: Array<emr_prescription_diagnos> = [];
    emr_prescription_investigation: Array<emr_prescription_investigation> = [];
    emr_prescription_observation: Array<emr_prescription_observation> = [];
    emr_prescription_treatment: Array<emr_prescription_treatment> = [];
}

export class emr_prescription_complaint {
    ID: number;
    Complaint: string;
    ComplaintId: number;
    IsSelected: boolean;
    Isfavorite: number;
    favoriteId: number;
    PatientId: number;
}
export class emr_prescription_diagnos {
    ID: number;
    Diagnos: string;
    DiagnosId: number;
    IsSelected: boolean;
    Isfavorite: number;
    favoriteId: number;
    PatientId: number;
}
export class emr_prescription_investigation {
    ID: number;
    Investigation: string;
    InvestigationId: number;
    IsSelected: boolean;
    Isfavorite: number;
    favoriteId: number;
    PatientId: number;
}
export class emr_prescription_observation {
    ID: number;
    Observation: string;
    ObservationId: number;
    IsSelected: boolean;
    Isfavorite: number;
    favoriteId: number;
    PatientId: number;
}

export class emr_prescription_treatment_template {
    ID: number;
    TemplateName: string;
}
export class emr_instruction {
    ID: number;
    Instructions: string;
}

export class emr_prescription_treatment {
    ID: number;
    MedicineId: number;
    MedicineName: string;
    Duration: number;
    DurationValue: string = "Day (s)";
    Instructions: string;
    InstructionId: string;
    Measure: string;
    Morning: string = "0";
    Evening: string = "0";
    AfterNoon: string = "0";
    Night: string = "0";
    Futureusage: boolean;
    PatientId: number;
    MedicineType: string;
    TypeId: number;
    IsInventory: boolean = true;
    GenericName:string;
}
export class emr_medicine {
    ID: number;
    Medicine: string;
    UnitId: number;
    TypeId: number;
    Price: number;
    Measure: string;
    InstructionId: number;
}
export class emr_vital {
    ID: number;
    Measure: string;
    Measure2: string;
    Date: Date;
    VitalId: number;
    VitalDropdownId: number;
    PatientId: number;
    Name: string;
    Unit: string;
}



