import { Component } from '@angular/core';
import { LoginFormComponent } from '../../login-form/login-form';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [LoginFormComponent],
  templateUrl: './login-page.html',
  styleUrls: ['./login-page.scss']
})
export class LoginPageComponent {}