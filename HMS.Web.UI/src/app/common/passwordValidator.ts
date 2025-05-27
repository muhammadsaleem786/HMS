import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

const commonPasswords = [
    '123456', 'password', '123456789', '12345678', '12345', '1234567', '1234567890', 'qwerty', 'abc123'
];

export function strongPasswordValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const value = control.value as string;

        if (!value) {
            return null; // Don't validate empty values
        }

        // Check for minimum length
        if (value.length < 10) {
            return { minLength: true };
        }

        // Check for uppercase letter
        if (!/[A-Z]/.test(value)) {
            return { uppercase: true };
        }

        // Check for lowercase letter
        if (!/[a-z]/.test(value)) {
            return { lowercase: true };
        }

        // Check for number
        if (!/[0-9]/.test(value)) {
            return { number: true };
        }

        // Check for special character
        if (!/[!@#$%^&*(),.?":{}|<>]/.test(value)) {
            return { specialChar: true };
        }

        // Check for common password
        if (commonPasswords.includes(value)) {
            return { commonPassword: true };
        }

        return null;
    };
}
