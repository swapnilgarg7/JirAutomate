using JirAutomate;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class JiraService
{
    private readonly HttpClient _httpClient;
    private readonly string _email;
    private readonly string _apiToken;

    public JiraService(IHttpClientFactory factory)
    {
        DotNetEnv.Env.Load();
        _httpClient = factory.CreateClient();
        _email = Environment.GetEnvironmentVariable("JIRA_EMAIL") ?? throw new Exception("Missing JIRA_EMAIL");
        _apiToken = Environment.GetEnvironmentVariable("JIRA_API") ?? throw new Exception("Missing JIRA_API");
    }

    public async Task<string?> GetAccountIdFromEmail(string domain, string email)
    {
        var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_email}:{_apiToken}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

        var url = $"https://{domain}/rest/api/3/user/search?query={email}";
        var res = await _httpClient.GetAsync(url);
        var json = await res.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.EnumerateArray().FirstOrDefault().GetProperty("accountId").GetString();
    }

    public async Task<string> CreateTicket(TicketRequest request)
    {
        var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_email}:{_apiToken}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

        var accountId = await GetAccountIdFromEmail(request.JiraDomain, request.AssigneeEmail);
        if (string.IsNullOrWhiteSpace(accountId)) throw new Exception("Assignee not found");

        var payload = new
        {
            fields = new
            {
                project = new { key = request.ProjectKey },
                summary = request.Summary,
                description = request.Description,
                issuetype = new { name = request.IssueType },
                assignee = new { id = accountId }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var url = $"https://{request.JiraDomain}/rest/api/2/issue";
        var response = await _httpClient.PostAsync(url, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Failed to create ticket: {responseBody}");

        return responseBody;
    }
}
