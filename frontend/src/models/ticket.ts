export interface TicketRequest {
  summary: string;
  description: string;
  assigneeName?: string;
  assigneeEmail?: string;
  jiraDomain?: string;
  projectKey?: string;
  issueType?: string;
}
