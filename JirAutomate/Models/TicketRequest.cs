using System.ComponentModel;
namespace JirAutomate;

public class TicketRequest
{
    public string Summary { get; set; }
    public string Description { get; set; }
    public string AssigneeName { get; set; } // from Gemini
    public string AssigneeEmail { get; set; } // resolved by service

    public string JiraDomain { get; set; }  // Set this in controller/service
    public string ProjectKey { get; set; }
    public string IssueType { get; set; }
}

