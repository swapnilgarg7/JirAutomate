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

  constructor(private http: HttpClient, private router: Router) {}

  toggleMode() {
    this.isLogin = !this.isLogin;
  }

  submit() {
    const endpoint = this.isLogin ? 'login' : 'register';
    const body = { email: this.email, passwordHash: this.password };

    this.http.post<any>(`${environment.apiUrl}/auth/${endpoint}`, body)
      .subscribe({
        next: (res) => {
          console.log('Response:', res);
          if (this.isLogin) {
            localStorage.setItem('token', res.token);
            this.router.navigate(['/transcript']);
          } else {
            alert('✅ Registered. You can now login.');
            this.toggleMode();
          }
        },
        error: () => {
           if (this.isLogin) {
          alert('❌ Login failed.');
        } else {
          alert('Account created successfully. You can now login.');
          this.toggleMode();
        }
        }
      });
  }
}
