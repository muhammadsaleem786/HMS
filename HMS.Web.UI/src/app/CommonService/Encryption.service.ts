import { Injectable } from "@angular/core";
import * as CryptoTS from 'crypto-ts';

@Injectable({
    providedIn: "root"
})
export class EncryptionService {
    key: string = "z!!!!!!!1sdfadsf56adf456asdfasdf";
    appProperties = {
        VALUES: {
            KEY: "MTIzNDU2Nzg5MEFCQ0RFRkdISUpLTE1O",
            IV: "MTIzNDU2Nzg="
        }
    }

    constructor() { }

    encryptionAES(msg) {
        // Encrypt
        if (msg != null) {
            const ciphertext = CryptoTS.AES.encrypt(msg, 'secretkey123');
            return ciphertext.toString();
        } else
            return "";
    }

    decryptionAES(msg) {
        // Decrypt
        if (msg != null) {
            const bytes = CryptoTS.AES.decrypt(msg, 'secretkey123');
            const plaintext = bytes.toString(CryptoTS.enc.Utf8);
            return plaintext;
        } else
            return "";

    }
}