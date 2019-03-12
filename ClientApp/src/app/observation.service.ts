import { Injectable } from '@angular/core';
import { Observation } from '../_models/Observation';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { tap, catchError } from 'rxjs/operators';
import { Bird } from '../_models/Bird';
import { HttpErrorHandlerService } from './http-error-handler.service';
import { ErrorReportViewModel } from '../_models/ErrorReportViewModel';

@Injectable({
  providedIn: 'root'
})
export class ObservationService {

  constructor(private http: HttpClient
            , private httpErrorHandlerService: HttpErrorHandlerService) { }

  getObservations(): Observable<Observation[] | ErrorReportViewModel> {
    return this.http.get<Observation[]>('api/Observation')
      .pipe(
        tap(observations => this.log('fetched observations')),
        catchError(error => this.httpErrorHandlerService.handleHttpError(error)));
  }

  getObservation(id: number): Observable<Observation | ErrorReportViewModel> {
    const options = id ?
    { params: new HttpParams().set('id', id.toString()) } : {};

    return this.http.get<Observation>('api/Observation/GetObservation', options)
      .pipe(
        tap(observation => this.log(`fetched observation with id: ${observation.ObservationId}`)),
        catchError(error => this.httpErrorHandlerService.handleHttpError(error)));
  }

  // ************TEMPORARY **********
  getBirds(): Observable<Bird[]> {
    return this.http.get<Bird[]>('api/Birds')
      .pipe(
        tap(birds => this.log('fetched birds for ddl')));
      //   ,
      //   catchError(this.handleError('getBirds',  []))
      // );
  }
  // ************TEMPORARY **********

  /** Log a HeroService message with the MessageService */
  private log(message: string) {
    console.log(message);
    // this.messageService.add(`HeroService: ${message}`);
  }
}
