import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { TicketRequest } from '../models/ticket.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class JiraService {
  constructor(private http: HttpClient) {}

  createTicket(ticket: TicketRequest) {
    return this.http.post(`${environment.apiBase}/jira/create`, ticket);
  }

  uploadTranscript(file: File) {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<TicketRequest[]>(`${environment.apiBase}/transcript/upload`, form);
  }
}
