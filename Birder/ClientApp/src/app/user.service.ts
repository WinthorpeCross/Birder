import { Injectable } from '@angular/core';
import { ErrorReportViewModel } from '../_models/ErrorReportViewModel';
import { HttpErrorHandlerService } from './http-error-handler.service';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { UserProfileViewModel, NetworkUserViewModel } from '../_models/UserProfileViewModel';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient
    , private httpErrorHandlerService: HttpErrorHandlerService) { }

  getUser(username: string): Observable<UserProfileViewModel | ErrorReportViewModel> {
    const options = username ?
      { params: new HttpParams().set('username', username) } : {};

    return this.http.get<UserProfileViewModel>('api/User/GetUser', options)
      .pipe(
        catchError(error => this.httpErrorHandlerService.handleHttpError(error)));
  }

  getNetwork(searchCriterion: string): Observable<NetworkUserViewModel[] | ErrorReportViewModel> {
    const options = searchCriterion ?
      { params: new HttpParams().set('searchCriterion', searchCriterion) } : {};

    return this.http.get<NetworkUserViewModel[]>('api/User/GetNetwork', options)
      .pipe(
        catchError(error => this.httpErrorHandlerService.handleHttpError(error)));
  }

  postFollowUser(viewModel: NetworkUserViewModel): Observable<NetworkUserViewModel | ErrorReportViewModel> {
    // const options = username ?
    //   { params: new HttpParams().set('username', username) } : {};
    //   console.log(options);

    return this.http.post<NetworkUserViewModel>('api/User/Follow', viewModel, httpOptions)
    .pipe(
      catchError(error => this.httpErrorHandlerService.handleHttpError(error)));
  }

  postUnfollowUser(viewModel: NetworkUserViewModel): Observable<NetworkUserViewModel | ErrorReportViewModel> {
    // const options = username ?
    // { params: new HttpParams().set('username', username) } : {};

    return this.http.post<NetworkUserViewModel>('api/User/Unfollow', viewModel, httpOptions)
    .pipe(
      catchError(error => this.httpErrorHandlerService.handleHttpError(error)));
  }
}