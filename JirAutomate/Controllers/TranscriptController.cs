using JirAutomate.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TranscriptController : ControllerBase
{
    private readonly GeminiService _geminiService;
    private readonly JiraService _jiraService;

    private static readonly Dictionary<string, string> NameToEmail = new(StringComparer.OrdinalIgnoreCase)
    {
        ["swapnil"] = "swapnilgarg810@gmail.com"
    };

    public TranscriptController(GeminiService geminiService, JiraService jiraService)
    {
        _geminiService = geminiService;
        _jiraService = jiraService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadTranscript(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        using var reader = new StreamReader(file.OpenReadStream());
        var transcript = await reader.ReadToEndAsync();

        var tickets = await _geminiService.ExtractTicketsFromTranscript(transcript);

        foreach (var ticket in tickets)
        {
            ticket.JiraDomain ??= "swapnilgarg.atlassian.net";
            ticket.ProjectKey ??= "CRM";
            ticket.IssueType ??= "Task";
            if (!string.IsNullOrWhiteSpace(ticket.AssigneeName) &&
        NameToEmail.TryGetValue(ticket.AssigneeName.Trim().ToLower(), out var email))
            {
                ticket.AssigneeEmail = email;
            }
            string _email = ticket.AssigneeEmail.Trim().ToLower();
            string _name="";
            if (_email.EndsWith("@example.com"))
            {
                _name = _email.Substring(0, _email.Length - "@example.com".Length);
            }
            if (!string.IsNullOrWhiteSpace(_name) &&
        NameToEmail.TryGetValue(_name, out var emailfinal))
            {
                ticket.AssigneeEmail = emailfinal;
            }
            ticket.AssigneeEmail ??= "";
            ticket.Summary ??= "Sample Ticket";
            ticket.Description ??= "Sample Ticket Description";
        }

        return Ok(tickets);
    }
}
