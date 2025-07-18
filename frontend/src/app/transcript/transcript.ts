import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { JiraService } from '../../services/jira';
import { TicketRequest } from '../../models/ticket';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-transcript',
  standalone: true,
  imports: [CommonModule, FormsModule, MatCardModule],
  templateUrl: './transcript.html',
  styleUrl: './transcript.scss'
})
export class Transcript {
  logout() {
    localStorage.removeItem('token');
    window.location.href = '/';
  }
  file!: File;
  tickets: TicketRequest[] = [];
  loading = false;
  uploadProgress = false;
  submitProgress = false;
  dragOver = false;
  successMessage = '';
  errorMessage = '';

  constructor(private jira: JiraService) {}

  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.file = input.files[0];
      this.validateFile();
    }
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
    this.dragOver = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    this.dragOver = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    this.dragOver = false;
    const files = event.dataTransfer?.files;
    if (files?.length) {
      this.file = files[0];
      this.validateFile();
    }
  }

  validateFile() {
    this.errorMessage = '';
    if (this.file) {
      const allowedTypes = ['text/plain', 'application/pdf', 'text/csv'];
      const maxSize = 10 * 1024 * 1024; // 10MB
      
      if (!allowedTypes.includes(this.file.type)) {
        this.errorMessage = 'Please upload a valid file (TXT, PDF, or CSV)';
        this.file = undefined!;
        return;
      }
      
      if (this.file.size > maxSize) {
        this.errorMessage = 'File size must be less than 10MB';
        this.file = undefined!;
        return;
      }
    }
  }

  upload() {
    if (!this.file) {
      this.errorMessage = 'Please select a file first';
      return;
    }
    
    this.uploadProgress = true;
    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';
    this.tickets = [];
    
    this.jira.uploadTranscript(this.file).subscribe({
      next: (res) => {
        this.tickets = res;
        this.uploadProgress = false;
        this.loading = false;
        this.successMessage = `Successfully extracted ${res.length} tickets from transcript`;
        
        // Clear file input
        const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
        if (fileInput) fileInput.value = '';
        this.file = undefined!;
      },
      error: (error) => {
        this.uploadProgress = false;
        this.loading = false;
        this.errorMessage = 'Failed to extract tickets from transcript. Please try again.';
        console.error('Upload error:', error);
      }
    });
  }

  submitAll() {
    if (!this.tickets.length) return;
    
    this.submitProgress = true;
    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';
    
    let completed = 0;
    let failed = 0;
    
    for (const ticket of this.tickets) {
      ticket.assigneeName = ticket.assigneeName || '';
      
      this.jira.createTicket(ticket).subscribe({
        next: () => {
          completed++;
          if (completed + failed === this.tickets.length) {
            this.handleSubmitComplete(completed, failed);
          }
        },
        error: (error) => {
          failed++;
          console.error('Ticket creation error:', error);
          if (completed + failed === this.tickets.length) {
            this.handleSubmitComplete(completed, failed);
          }
        }
      });
    }
  }

  private handleSubmitComplete(completed: number, failed: number) {
    this.submitProgress = false;
    this.loading = false;
    
    if (failed === 0) {
      this.successMessage = `✅ All ${completed} tickets created successfully!`;
      this.tickets = [];
    } else if (completed > 0) {
      this.errorMessage = `⚠️ ${completed} tickets created, ${failed} failed`;
    } else {
      this.errorMessage = `❌ Failed to create all ${failed} tickets`;
    }
  }

  removeTicket(index: number) {
    this.tickets.splice(index, 1);
    if (this.tickets.length === 0) {
      this.successMessage = '';
    }
  }

  getFileName(): string {
    return this.file?.name || 'No file selected';
  }

  getFileSize(): string {
    if (!this.file) return '';
    const size = this.file.size;
    if (size < 1024) return `${size} B`;
    if (size < 1024 * 1024) return `${(size / 1024).toFixed(1)} KB`;
    return `${(size / (1024 * 1024)).toFixed(1)} MB`;
  }
}