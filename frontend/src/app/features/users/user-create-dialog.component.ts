import { Component, inject, signal } from '@angular/core';

import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { UsersService } from '../../core/services/users.service';

@Component({
  selector: 'app-user-create-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
],
  template: `
    <h2 mat-dialog-title>Добавление пользователя</h2>

    <mat-dialog-content>
      <form [formGroup]="form" class="dialog-form">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Имя</mat-label>
          <input matInput formControlName="name" placeholder="Например: Алексей Смирнов" maxlength="100">
          @if (form.get('name')?.hasError('required') && form.get('name')?.touched) {
            <mat-error>Имя обязательно</mat-error>
          }
          @if (form.get('name')?.hasError('maxlength')) {
            <mat-error>Максимум 100 символов</mat-error>
          }
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Email</mat-label>
          <input matInput type="email" formControlName="email" placeholder="user@example.com" maxlength="255">
          @if (form.get('email')?.hasError('required') && form.get('email')?.touched) {
            <mat-error>Email обязателен</mat-error>
          }
          @if (form.get('email')?.hasError('email')) {
            <mat-error>Некорректный формат email</mat-error>
          }
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Пароль</mat-label>
          <input matInput
                 [type]="hidePassword() ? 'password' : 'text'"
                 formControlName="password"
                 placeholder="Минимум 8 символов">
          <button mat-icon-button
                  matSuffix
                  type="button"
                  (click)="hidePassword.set(!hidePassword())"
                  [attr.aria-label]="'Показать или скрыть пароль'">
            <mat-icon>{{ hidePassword() ? 'visibility_off' : 'visibility' }}</mat-icon>
          </button>
          @if (form.get('password')?.hasError('required') && form.get('password')?.touched) {
            <mat-error>Пароль обязателен</mat-error>
          }
          @if (form.get('password')?.hasError('minlength')) {
            <mat-error>Минимум 8 символов</mat-error>
          }
        </mat-form-field>
      </form>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="onCancel()" [disabled]="isSaving">
        Отмена
      </button>
      <button mat-flat-button
              color="primary"
              type="button"
              (click)="onSave()"
              [disabled]="form.invalid || isSaving">
        {{ isSaving ? 'Добавление...' : 'Добавить' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .dialog-form {
      display: flex;
      flex-direction: column;
      min-width: 400px;
      padding-top: 8px;
    }
    .full-width {
      width: 100%;
      margin-bottom: 12px;
    }
  `]
})
export class UserCreateDialogComponent {
  private fb = inject(FormBuilder);
  private usersService = inject(UsersService);
  private dialogRef = inject(MatDialogRef<UserCreateDialogComponent>);

  public hidePassword = signal<boolean>(true);
  public isSaving = false;

  public form: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(255)]],
    password: ['', [Validators.required, Validators.minLength(8)]]
  });

  // ЗАкрыть без изменений
  public onCancel(): void {
    this.dialogRef.close(false);
  }

  // Запрос на создание
  public onSave(): void {
    if (this.form.invalid) return;

    this.isSaving = true;
    const payload = {
      name: this.form.value.name.trim(),
      email: this.form.value.email.trim(),
      password: this.form.value.password
    };

    this.usersService.createUser(payload).subscribe({
      next: () => {
        this.dialogRef.close(true);
      },
      error: () => {
        this.isSaving = false;
      }
    });
  }
}
