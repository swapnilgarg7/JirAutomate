import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { TicketRequest } from '../models/ticket';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class JiraService {
  constructor(private http: HttpClient) {}

  createTicket(ticket: TicketRequest) {
    return this.http.post(`${environment.apiUrl}/jira/create-ticket`, ticket);
  }

  uploadTranscript(file: File) {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<TicketRequest[]>(`${environment.apiUrl}/transcript/upload`, form);
  }
}
