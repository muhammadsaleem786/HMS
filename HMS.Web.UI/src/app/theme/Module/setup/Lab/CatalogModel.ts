export class lab_patient_test {
    ID: number = 0;
    CompanyId: number;
    PatientId: number;
    PatientName: string;
    PriorityId: number;
    StatusId: number;
    PriorityName: string;
    ClinicalNotes: string;
    CatalogIds: string;
    Age: number;
    Gender: string;
}
export class lab_result_mf {
    constructor() {
        let objlab_result_dt = new lab_result_dt();
        this.lab_result_dt.push(objlab_result_dt);
    }
    ID: number;
    CompanyID: number;
    PatientId: number;
    CatalogId: number;
    PatientTestId: number;
    Comment: string;   
    lab_result_dt: Array<lab_result_dt> = [];
}
export class lab_result_dt {
    ID: number;
    CompanyID: number;
    ResultId: number;
    ParameterId: number;
    Result: number;
    Status: string;
    Comment: string;    
}

