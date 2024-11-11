import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { tap } from 'rxjs';
import { Router } from '@angular/router';
import { AuthenticateService } from '../../shared/services/authenticate.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SharedService } from '../../shared/services/shared.service';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})

export class LoginComponent {

  loginForm: FormGroup = new FormGroup({
    email: new FormControl(null, [Validators.required, Validators.email]),
    password: new FormControl(null, [Validators.required])
  });

  constructor(private fb: FormBuilder,
    private authService: AuthenticateService,
    private router: Router,
    private snackbar: MatSnackBar,
    private sharedService: SharedService)
  {  }

  public navToRegister(): void {
    this.router.navigate(['../public/signup']);
  }

  onLogin(): void {

    if (!this.loginForm.valid) {
      return;
    }
    this.authService.login(this.loginForm.value).subscribe((response) => {
      if (response.responseCode == 100) {
        this.sharedService.sendData(response); // Send data via service

        // route to home/dashboard, if login was successfull
        this.router.navigate(['../home/dashboard']);
      }
      else {
        this.snackbar.open(response.message, 'Close', {
          duration: 3000, horizontalPosition: 'right', verticalPosition: 'bottom'
        });
      }
    });
  }
}
