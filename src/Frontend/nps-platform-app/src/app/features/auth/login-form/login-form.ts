import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { ButtonComponent } from '../../../shared/atoms/button/button';

@Component({
  selector: 'app-login-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent],
  templateUrl: './login-form.html',
  styleUrls: ['./login-form.scss'],
})
export class LoginFormComponent implements OnInit {
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);

  loginForm!: FormGroup;

  loading = signal(false);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  onSubmit(): void {
    this.errorMessage.set(null);

    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      this.errorMessage.set('Por favor, ingresa tu usuario y contraseña válidos.');
      return;
    }

    const { email, password } = this.loginForm.value;

    this.loading.set(true);

    this.authService.login({ username: email, password: password }).subscribe({
      next: () => {
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Login fallido:', err);
        this.errorMessage.set('Error en el login. Por favor, revisa tus credenciales.');
        this.loading.set(false);
      },
      complete: () => {
        this.loading.set(false);
      },
    });
  }
}
