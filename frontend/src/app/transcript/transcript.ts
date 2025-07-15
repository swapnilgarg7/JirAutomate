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
  loading = false;

  constructor(private jira: JiraService) {}

  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) this.file = input.files[0];
  }

  upload() {
    if (!this.file) return;
    this.loading = true;
    this.jira.uploadTranscript(this.file).subscribe({
      next: (res) => {
        this.tickets = res;
        this.loading = false;
         // Reset form
          this.tickets = [];
          this.file = undefined!;
          (document.querySelector('input[type="file"]') as HTMLInputElement).value = '';
      },
      error: () => {
        alert('❌ Failed to extract tickets');
        this.loading = false;
      }
    });
  }

  submitAll() {
    this.loading = true;
    let completed = 0;
    for (const ticket of this.tickets) {
      console.log(ticket);
      ticket.assigneeName = ticket.assigneeName || '';
      this.jira.createTicket(ticket).subscribe({
        next: () => {
          completed++;
          if (completed === this.tickets.length) {
            alert('✅ All tickets created!');
            this.loading = false;
          }
        },
        error: () => {
          alert('❌ Failed to create one or more tickets.');
          this.loading = false;
        }
      });
    }
  }
}