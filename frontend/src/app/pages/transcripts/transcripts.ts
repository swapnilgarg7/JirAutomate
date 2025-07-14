import { Component } from '@angular/core';
import {TicketRequest} from '../../models/ticket.model';
import { JiraService } from '../../services/jira';

@Component({
  selector: 'app-transcripts',
  imports: [],
  templateUrl: './transcripts.html',
  styleUrl: './transcripts.scss'
})
export class Transcripts {
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
  this.jiraService.createTicket(ticket).subscribe(() => {
    alert('✅ Ticket created!');
  });
}

}
