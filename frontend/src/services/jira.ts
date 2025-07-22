import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { TicketRequest } from '../models/ticket';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class JiraService {
  constructor(private http: HttpClient) {}

  createTicket(ticket: TicketRequest) {
    const token = localStorage.getItem('token');
    const headers = token ? new HttpHeaders({ 'Authorization': `Bearer ${token}` }) : undefined;
    return this.http.post(`${environment.apiUrl}/jira/create-ticket`, ticket, { headers });
  }

  uploadTranscript(file: File) {
    const form = new FormData();
    form.append('file', file);
    const token = localStorage.getItem('token');
    const headers = token ? new HttpHeaders({ 'Authorization': `Bearer ${token}` }) : undefined;
    return this.http.post<TicketRequest[]>(`${environment.apiUrl}/transcript/upload`, form, { headers });
  }
}
