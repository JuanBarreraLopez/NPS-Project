import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AdminService } from '../../../../core/services/admin';
import { ButtonComponent } from '../../../../shared/atoms/button/button';

@Component({
  selector: 'app-voter-registration',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent],
  templateUrl: './voter-registration.html',
  styleUrls: ['./voter-registration.scss'],
})
export class VoterRegistrationComponent {
  private fb = inject(FormBuilder);
  private adminService = inject(AdminService);

  form = this.fb.group({
    username: ['', [Validators.required, Validators.minLength(3)]],
    password: ['', [Validators.required, Validators.minLength(8)]],
  });

  message = signal<{ text: string; type: 'success' | 'error' } | null>(null);
  loading = signal(false);

  onSubmit() {
    if (this.form.invalid) return;
    this.loading.set(true);
    this.message.set(null);

    const command = {
      username: this.form.value.username!,
      password: this.form.value.password!,
      role: 'VOTANTE',
    };

    this.adminService.registerVoter(command).subscribe({
      next: () => {
        this.message.set({ text: 'Votante registrado exitosamente.', type: 'success' });
        this.form.reset();
        this.loading.set(false);
      },
      error: (err) => {
        console.error(err);
        const errorMsg =
          err.error?.detail || 'Error: El nombre de usuario ya existe o datos inválidos.';
        this.message.set({ text: errorMsg, type: 'error' });
        this.loading.set(false);
      },
    });
  }
}
