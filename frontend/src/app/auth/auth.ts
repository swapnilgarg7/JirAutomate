import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-auth',
  imports: [CommonModule, FormsModule],
  templateUrl: './auth.html',
  styleUrl: './auth.scss'
})
export class Auth {
  isLogin = true;
  email = '';
  password = '';
  errorMessage = '';
  isLoading = false;

  constructor(private http: HttpClient, private router: Router) {}

  toggleMode() {
    this.isLogin = !this.isLogin;
    this.errorMessage = '';
  }

  submit() {
    if (!this.email || !this.password) {
      this.errorMessage = 'Please fill in all fields';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    
    const endpoint = this.isLogin ? 'login' : 'register';
    const body = { email: this.email, passwordHash: this.password };

    this.http.post<any>(`${environment.apiUrl}/auth/${endpoint}`, body)
      .subscribe({
        next: (res) => {
          this.isLoading = false;
          console.log('Response:', res);
          if (this.isLogin) {
            localStorage.setItem('token', res.token);
            this.router.navigate(['/transcript']);
          } else {
            alert('✅ Registered. You can now login.');
            this.toggleMode();
          }
        },
        error: (error) => {
          this.isLoading = false;
          if (this.isLogin) {
            this.errorMessage = 'Login failed. Please check your credentials.';
          } else {
            // Check if it's a success case (account created)
            if (error.status === 201 || error.status === 200) {
              alert('Account created successfully. You can now login.');
              this.toggleMode();
            } else {
              this.errorMessage = 'Registration failed. Please try again.';
            }
          }
        }
      });
  }
}