import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { PatientAmountSummary } from './AdmitModel';

@Injectable({
    providedIn: 'root'
})
export class PatientAmountSummaryService {

    private amountSubject =
        new BehaviorSubject<PatientAmountSummary>({
            procedureAmount: 0,
            payAmount: 0,
        });

    amount$ = this.amountSubject.asObservable();


    updateAmount(summary: PatientAmountSummary): void {

        this.amountSubject.next(summary);

    }


    reset(): void {

        this.amountSubject.next({
            procedureAmount: 0,
            payAmount: 0
        });

    }

}