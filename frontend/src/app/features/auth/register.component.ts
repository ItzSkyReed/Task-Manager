import { Component, inject } from '@angular/core';

import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
],
  template: `
    <div class="auth-container">
      <mat-card class="auth-card">
        <mat-card-header>
          <mat-card-title>Регистрация в TaskManager</mat-card-title>
        </mat-card-header>

        <mat-card-content>
          <form [formGroup]="registerForm" (ngSubmit)="onSubmit()">

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Имя</mat-label>
              <input matInput type="text" formControlName="name" placeholder="Иван Иванов">
              @if (registerForm.get('name')?.hasError('required') && registerForm.get('name')?.touched) {
                <mat-error>Имя обязательно</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Email</mat-label>
              <input matInput type="email" formControlName="email" placeholder="example@domain.com">
              @if (registerForm.get('email')?.hasError('required') && registerForm.get('email')?.touched) {
                <mat-error>Email обязателен</mat-error>
              }
              @if (registerForm.get('email')?.hasError('email')) {
                <mat-error>Некорректный формат email</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Пароль</mat-label>
              <input matInput type="password" formControlName="password" placeholder="••••••••">
              @if (registerForm.get('password')?.hasError('required') && registerForm.get('password')?.touched) {
                <mat-error>Пароль обязателен</mat-error>
              }
              @if (registerForm.get('password')?.hasError('minlength')) {
                <mat-error>Минимум 8 символов</mat-error>
              }
            </mat-form-field>

            <button mat-flat-button color="primary" class="full-width" [disabled]="registerForm.invalid || isLoading">
              {{ isLoading ? 'Регистрация...' : 'Зарегистрироваться' }}
            </button>
          </form>
        </mat-card-content>

        <mat-card-actions class="auth-actions">
          <p><a routerLink="/login">Войти</a></p>
        </mat-card-actions>
      </mat-card>
    </div>
  `,
  styles: [`
    .auth-container {
      display: flex;
      justify-content: center;
      align-items: center;
      height: 100vh;
      background-color: #f5f5f5;
    }
    .auth-card {
      width: 100%;
      max-width: 400px;
      padding: 20px;
    }
    .full-width {
      width: 100%;
      margin-bottom: 15px;
    }
    .auth-actions {
      display: flex;
      justify-content: center;
      width: 100%;
      margin-top: 15px;
    }
  `]
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  isLoading = false;

  registerForm: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(255)]],
    password: ['', [Validators.required, Validators.minLength(8)]]
  });

  onSubmit(): void {
    if (this.registerForm.invalid) return;

    this.isLoading = true;
    this.authService.register(this.registerForm.value).subscribe({
      next: () => {
        this.router.navigate(['/projects']);
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }
}
