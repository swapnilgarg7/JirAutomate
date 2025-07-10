using System.ComponentModel;
namespace JirAutomate;

public class TicketRequest
{
    [DefaultValue("Test Ticket from SaaS")]
    public string Summary { get; set; } = "Test Ticket from SaaS";

    [DefaultValue("This is a default ticket description.")]
    public string Description { get; set; } = "This is a default ticket description.";

    [DefaultValue("swapnilgarg810@gmail.com")]
    public string AssigneeEmail { get; set; } = "swapnilgarg810@gmail.com";

    [DefaultValue("swapnilgarg.atlassian.net")]
    public string JiraDomain { get; set; } = "swapnilgarg.atlassian.net";

    [DefaultValue("CRM")]
    public string ProjectKey { get; set; } = "CRM";

    [DefaultValue("Task")]
    public string IssueType { get; set; } = "Task";
}
