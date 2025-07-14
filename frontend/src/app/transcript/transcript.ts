import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { JiraService } from '../../services/jira';
import { TicketRequest } from '../../models/ticket';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-transcript',
  standalone: true,
  imports: [CommonModule, FormsModule,MatCardModule],
  templateUrl: './transcript.html'
})
export class Transcript {
  file!: File;
tickets: TicketRequest[] = [];

constructor(private jiraService: JiraService) {}

onFileSelect(event: Event) {
  const input = event.target as HTMLInputElement;
  if (input.files?.length) this.file = input.files[0];
}

upload() {
  if (!this.file) return;
  this.jiraService.uploadTranscript(this.file).subscribe(res => {
    this.tickets = res;
  });
}

submit(ticket: TicketRequest) {
  this.jiraService.createTicket(ticket).subscribe({
    next: () => alert('✅ Ticket created!'),
    error: () => alert('❌ Failed to create ticket.')
  });
}


}
