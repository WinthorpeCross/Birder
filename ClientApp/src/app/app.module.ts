import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SideMenuComponent } from './side-menu/side-menu.component';
import { MatIconModule, MatButtonModule, MatInputModule,
  MatDatepickerModule, MatNativeDateModule, MatCheckboxModule, MatSelectModule} from '@angular/material';
import { ObservationsFeedComponent } from './observations-feed/observations-feed.component';
import { LoginComponent } from './login/login.component';
import { JwtHelper } from 'angular2-jwt';
import { AuthGuard } from './auth-guard.service';
import { ReactFormExampleComponent } from './react-form-example/react-form-example.component';
import { LogoutComponent } from './logout/logout.component';
import { BirdsIndexComponent } from './birds-index/birds-index.component';
import { BirdsDetailComponent } from './birds-detail/birds-detail.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { httpInterceptorProviders } from '../_httpInterceptors';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    SideMenuComponent,
    ObservationsFeedComponent,
    LoginComponent,
    ReactFormExampleComponent,
    LogoutComponent,
    BirdsIndexComponent,
    BirdsDetailComponent,
    PageNotFoundComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule, ReactiveFormsModule,
    NgbModule,
    AppRoutingModule,
    BrowserAnimationsModule, MatIconModule,
    MatButtonModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatCheckboxModule,
    MatSelectModule
  ],
  providers: [
    JwtHelper,
    AuthGuard,
    httpInterceptorProviders
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
