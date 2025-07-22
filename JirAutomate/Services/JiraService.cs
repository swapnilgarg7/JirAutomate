using JirAutomate;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class JiraService
{
    private readonly HttpClient _httpClient;

    public JiraService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    public async Task<string?> GetAccountIdFromEmail(string domain, string email, string jiraEmail, string jiraApi)
    {
        var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{jiraEmail}:{jiraApi}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

        var url = $"https://{domain}/rest/api/3/user/search?query={email}";
        var res = await _httpClient.GetAsync(url);
        var json = await res.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.EnumerateArray().FirstOrDefault().GetProperty("accountId").GetString();
    }

    public async Task<string> CreateTicket(TicketRequest request, string jiraEmail, string jiraApi)
    {
        var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{jiraEmail}:{jiraApi}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

        string? accountId = null;

        if (!string.IsNullOrWhiteSpace(request.AssigneeEmail))
        {
            try
            {
                accountId = await GetAccountIdFromEmail(request.JiraDomain, request.AssigneeEmail, jiraEmail, jiraApi);
            }
            catch
            {
                Console.WriteLine($"⚠️ Failed to get accountId for {request.AssigneeEmail}, skipping assignee.");
            }
        }

        var fields = new Dictionary<string, object>
        {
            ["project"] = new { key = request.ProjectKey },
            ["summary"] = request.Summary,
            ["description"] = request.Description,
            ["issuetype"] = new { name = request.IssueType }
        };

        if (!string.IsNullOrWhiteSpace(accountId))
        {
            fields["assignee"] = new { id = accountId };
        }

        var payload = new { fields };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var url = $"https://{request.JiraDomain}/rest/api/2/issue";
        var response = await _httpClient.PostAsync(url, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Failed to create ticket: {responseBody}");

        return responseBody;
    }
}
