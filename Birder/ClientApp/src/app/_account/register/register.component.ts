import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl, AbstractControl } from '@angular/forms';
import { UsernameValidator, PasswordValidator, ParentErrorStateMatcher } from '../../../validators';
import { AccountService } from '../../_services/account.service';
import { first } from 'rxjs/operators';
import { Router } from '@angular/router';
import { RegisterViewModel } from '../../_models/RegisterViewModel';
import { ErrorReportViewModel } from '../../_models/ErrorReportViewModel';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class RegisterComponent implements OnInit {
  requesting: boolean;
  invalidRegistration: boolean;
  isUsernameAvailable: boolean;
  errorReport: ErrorReportViewModel;
  userRegisterForm: FormGroup;
  matching_passwords_group: FormGroup;
  parentErrorStateMatcher = new ParentErrorStateMatcher();

  userRegister_validation_messages = {
    'userName': [
      { type: 'required', message: 'Username is required' },
      { type: 'minlength', message: 'Username must be at least 5 characters long' },
      { type: 'maxlength', message: 'Username cannot be more than 25 characters long' },
      { type: 'pattern', message: 'Your username must be alphanumeric (no special characters) and must not contain spaces' },
    ],
    'email': [
      { type: 'required', message: 'Email is required' },
      { type: 'pattern', message: 'Enter a valid email' }
    ],
    'confirmPassword': [
      { type: 'required', message: 'Confirm password is required' },
      { type: 'areEqual', message: 'Password mismatch' }
    ],
    'password': [
      { type: 'required', message: 'Password is required' },
      { type: 'minlength', message: 'Password must be at least 8 characters long' },
      { type: 'pattern', message: 'Your password must contain at least one number and one letter' }
      // { type: 'pattern', message: 'Your password must contain at least one uppercase, one lowercase, and one number' }
    ]
  };

  constructor(private formBuilder: FormBuilder
            , private accountService: AccountService
            , private router: Router) { }

  ngOnInit() {
    this.createForms();
  }

  validateUsernameIsAvailable(username: string) {
    return this.accountService.checkValidUsername(username)
    .subscribe(
      (data: boolean) => {
        this.isUsernameAvailable = data;
      },
      (error: ErrorReportViewModel) => {
        this.isUsernameAvailable = false;
        // ToDo: Toast notification
      }
    );
  }

  checkUsernameIsAvailable(): void {
    if (this.userRegisterForm.get('userName').valid) {
      this.validateUsernameIsAvailable(this.userRegisterForm.get('userName').value);
    } else {
      // alert('do nothing');
    }
  }

  createForms() {
    // matching passwords validation
    this.matching_passwords_group = new FormGroup({
      password: new FormControl('', Validators.compose([
        Validators.minLength(8),
        Validators.required, // regex: accept letters, numbers and !@#$%.  Must have at least one letter and number
        Validators.pattern('^(?=.*[a-z])(?=.*[0-9])[a-zA-Z0-9!@#$%]+$') // ^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])[a-zA-Z0-9]+$')
      ])),
      confirmPassword: new FormControl('', Validators.required)
    }, (formGroup: FormGroup) => {
      return PasswordValidator.areEqual(formGroup);
    });

    this.userRegisterForm = this.formBuilder.group({
      userName: new FormControl('', Validators.compose([
       // ValidateEmailNotTaken.createValidator(this.accountService),
       // this.validateEmailNotTaken.bind(this),
       // UsernameValidator.validUsername,
       Validators.maxLength(25),
       Validators.minLength(5),
       Validators.pattern('^(?=.*[a-zA-Z])[a-zA-Z0-9]+$'), // ^(?=.*[a-zA-Z])(?=.*[0-9])[a-zA-Z0-9]+$
       Validators.required
      ])),
      email: new FormControl('', Validators.compose([
        Validators.required,
        Validators.pattern('^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$')
      ])),
      matching_passwords: this.matching_passwords_group
      // terms: new FormControl(false, Validators.pattern('true'))
    });
  }

  onSubmit(value): void {

    if (this.isUsernameAvailable === false) {
      const unavailableUsername = this.userRegisterForm.get('userName').value;
      alert(`The username '${unavailableUsername}' is already taken.  Please choose a different username.`);
      return;
    }

    this.requesting = true;

    const viewModelObject = <RegisterViewModel> {
      userName: value.userName,
      email: value.email,
      password: value.matching_passwords.password,
      confirmPassword: value.matching_passwords.confirmPassword
    };

    this.accountService.register(viewModelObject)
    .pipe(first())
    .subscribe(_ => {
         this.router.navigate(['/confirm-email']);
       },
      (error: ErrorReportViewModel) => {
        this.requesting = false;
        this.errorReport = error;
        this.invalidRegistration = true;
        console.log(error.friendlyMessage);
      });
  }
}
