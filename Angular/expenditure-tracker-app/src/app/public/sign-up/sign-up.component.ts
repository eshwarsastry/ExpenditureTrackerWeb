import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthenticateService } from '../../shared/services/authenticate.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { pipe, tap } from 'rxjs';
import { User } from '../../shared/interfaces/user';

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrl: './sign-up.component.css'
})
export class SignUpComponent {

  signupForm: FormGroup = new FormGroup({
    email: new FormControl(null, [Validators.required, Validators.email]),
    password: new FormControl(null, [Validators.required]),
    firstname: new FormControl (null, [Validators.required]),
    lastname: new FormControl(null, [Validators.required])
  });

  constructor(private fb: FormBuilder,
    private authService: AuthenticateService,
    private router: Router,
    private snackbar: MatSnackBar) { }

  public navToLogin(): void {
    this.router.navigate(['../public/login']);
  }

  onRegister(): void {
    if (!this.signupForm.valid) {
      return;
    }
    this.authService.register(this.signupForm.value).subscribe((response) => {
      if (response.responseCode == 200) {
        // route to login screen, if user creation was successfull.
        this.router.navigate(['../public/login']);
      }
      else {
        this.snackbar.open('User created successfully', 'Close', {
          duration: 2000, horizontalPosition: 'right', verticalPosition: 'bottom'
        });
      }
    });   
  }
}
