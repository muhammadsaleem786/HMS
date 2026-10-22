
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class VoiceRecognitionService {
  private recognition: any;
  private resultSubject = new Subject<string>();
  private errorSubject = new Subject<string>();
  private listeningSubject = new Subject<boolean>();

  result$: Observable<string> = this.resultSubject.asObservable();
  error$: Observable<string> = this.errorSubject.asObservable();
  listening$: Observable<boolean> =
    this.listeningSubject.asObservable();

  constructor() {
    const w = window as any;
    const SpeechRecognition =
      w.SpeechRecognition || w.webkitSpeechRecognition;

    if (SpeechRecognition) {
      this.recognition = new SpeechRecognition();

      this.recognition.continuous = true;
      this.recognition.interimResults = true;
      this.recognition.lang = 'en-US';

      this.recognition.onresult = (event: any) => {
        let finalText = '';
        let interimText = '';

        for (
          let i = event.resultIndex;
          i < event.results.length;
          i++
        ) {
          const transcript =
            event.results[i][0].transcript;

          if (event.results[i].isFinal) {
            finalText += transcript + ' ';
          } else {
            interimText += transcript;
          }
        }

        // Only final results should be appended to the form.
        if (finalText.trim()) {
          this.resultSubject.next(finalText.trim());
        }
      };

      this.recognition.onstart = () => {
        this.listeningSubject.next(true);
      };

      this.recognition.onend = () => {
        this.listeningSubject.next(false);
      };

      this.recognition.onerror = (event: any) => {
        this.listeningSubject.next(false);
        this.errorSubject.next(
          event.error || 'Speech recognition failed.'
        );
      };
    }
  }

  isSupported(): boolean {
    return !!this.recognition;
  }

  start(): void {
    if (!this.recognition) {
      this.errorSubject.next(
        'Speech recognition is not supported by this browser.'
      );
      return;
    }

    try {
      this.recognition.start();
    } catch (error) {
      this.errorSubject.next(
        'Recognition is already running or could not start.'
      );
    }
  }

  stop(): void {
    if (this.recognition) {
      this.recognition.stop();
    }
  }

  setLanguage(language: string): void {
    if (this.recognition) {
      this.recognition.lang = language;
    }
  }
}
