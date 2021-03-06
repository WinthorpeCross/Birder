import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { first, tap } from 'rxjs/operators';
import { HttpParams, HttpClient, HttpHeaders } from '@angular/common/http';
import { NetworkUserViewModel } from '@app/_models/UserProfileViewModel';
import { NetworkSummaryDto } from '@app/_models/NetworkSummaryDto';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root'
})
export class NetworkService {

  private networkChanged = new Subject<any>();
  networkChanged$ = this.networkChanged.asObservable();

  constructor(private http: HttpClient) { }

  getNetworkSummary(): Observable<NetworkSummaryDto> {
    return this.http.get<NetworkSummaryDto>('api/Network')
      .pipe();
  }

  getFollowers(username: string): Observable<NetworkUserViewModel[]> {
    const options = username ?
      { params: new HttpParams().set('requestedUsername', username) } : {};

    return this.http.get<NetworkUserViewModel[]>('api/Network/GetFollowers', options)
      .pipe(first());
  }

  getFollowing(username: string): Observable<NetworkUserViewModel[]> {
    const options = username ?
      { params: new HttpParams().set('requestedUsername', username) } : {};

    return this.http.get<NetworkUserViewModel[]>('api/Network/GetFollowing', options)
      .pipe(first());
  }

  getSearchNetwork(searchCriterion: string): Observable<NetworkUserViewModel[]> {
    const options = searchCriterion ?
      { params: new HttpParams().set('searchCriterion', searchCriterion) } : {};

    return this.http.get<NetworkUserViewModel[]>('api/Network/SearchNetwork', options)
      .pipe(first());
  }

  postFollowUser(viewModel: NetworkUserViewModel): Observable<NetworkUserViewModel> {
    return this.http.post<NetworkUserViewModel>('api/Network/Follow', viewModel, httpOptions)
      .pipe(first(),
        tap(_ => { this.announceNetworkChanged(); }));
  }

  postUnfollowUser(viewModel: NetworkUserViewModel): Observable<NetworkUserViewModel> {
    return this.http.post<NetworkUserViewModel>('api/Network/Unfollow', viewModel, httpOptions)
      .pipe(first(),
        tap(_ => { this.announceNetworkChanged(); }));
  }

  announceNetworkChanged(): void {
    this.networkChanged.next();
  }
}
