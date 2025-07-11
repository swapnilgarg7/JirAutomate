using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes; // Added for JsonNode
using JirAutomate;

namespace JirAutomate.Services;

public class GeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GeminiService(IHttpClientFactory factory)
    {
        DotNetEnv.Env.Load();
        _httpClient = factory.CreateClient();
        _apiKey = Environment.GetEnvironmentVariable("GEMINI_API") ?? throw new Exception("Missing GEMINI_API");
       
    }

    public async Task<List<TicketRequest>> ExtractTicketsFromTranscript(string transcript)
    {
        var prompt = $@"
You are an AI assistant helping extract Jira tasks from meeting transcripts.

From the text below, extract a list of action items. For each item, return:
- summary
- description
- assigneeName (only if mentioned clearly, else leave blank)

Format output as a JSON array of:
{{ ""summary"": ""..."", ""description"": ""..."", ""assigneeName"": ""..."" }}

Transcript:
{transcript}
";

        var payload = new
        {
            tools = new[]
            {
                new
                {
                    functionDeclarations = new[]
                    {
                        new
                        {
                            name = "createJiraTickets",
                            description = "Extract action items from meeting transcript.",
                            parameters = new
                            {
                                type = "object",
                                properties = new
                                {
                                    tickets = new
                                    {
                                        type = "array",
                                        items = new
                                        {
                                            type = "object",
                                            properties = new
                                            {
                                                summary = new { type = "string" },
                                                description = new { type = "string" },
                                                assigneeEmail = new { type = "string" }
                                            },
                                            required = new[] { "summary", "description", "assigneeEmail" }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new[]
                    {
                        new
                        {
                            text = prompt
                        }
                    }
                }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key=" + _apiKey,
            content
        );

        response.EnsureSuccessStatusCode(); // Throws an exception if the HTTP response status is an error code.

        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine("🔍 Gemini Raw Response:\n" + responseBody);

        try
        {
            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            if (!root.TryGetProperty("candidates", out var candidatesElement) || candidatesElement.ValueKind != JsonValueKind.Array || candidatesElement.GetArrayLength() == 0)
            {
                Console.WriteLine("❌ Gemini response missing 'candidates' or 'candidates' is empty.");
                // Check for safety ratings if no candidates
                if (root.TryGetProperty("promptFeedback", out var promptFeedbackElement) &&
                    promptFeedbackElement.TryGetProperty("safetyRatings", out var safetyRatingsElement))
                {
                    Console.WriteLine("Safety Feedback: " + safetyRatingsElement.GetRawText());
                }
                throw new Exception("Gemini did not return any candidates.");
            }

            var firstCandidate = candidatesElement[0];
            if (!firstCandidate.TryGetProperty("content", out var contentElement) || contentElement.ValueKind != JsonValueKind.Object)
            {
                Console.WriteLine("❌ Gemini response candidate missing 'content'.");
                throw new Exception("Gemini candidate content is missing.");
            }

            if (!contentElement.TryGetProperty("parts", out var partsElement) || partsElement.ValueKind != JsonValueKind.Array || partsElement.GetArrayLength() == 0)
            {
                Console.WriteLine("❌ Gemini response content missing 'parts' or 'parts' is empty.");
                throw new Exception("Gemini content parts are missing.");
            }

            var firstPart = partsElement[0];

            if (firstPart.TryGetProperty("functionCall", out var functionCallElement))
            {
                if (!functionCallElement.TryGetProperty("args", out var argsElement))
                {
                    Console.WriteLine("❌ 'functionCall' missing 'args'.");
                    throw new Exception("Gemini function call arguments are missing.");
                }

                if (!argsElement.TryGetProperty("tickets", out var ticketsElement))
                {
                    Console.WriteLine("❌ 'args' missing 'tickets'.");
                    throw new Exception("Gemini function call 'tickets' argument is missing.");
                }

                try
                {
                    var tickets = JsonSerializer.Deserialize<List<TicketRequest>>(ticketsElement.GetRawText(), new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    foreach (var t in tickets)
                    {
                        Console.WriteLine($"🔍 Summary: {t.Summary}");
                        Console.WriteLine($"🔍 Desc: {t.Description}");
                        Console.WriteLine($"🔍 Assignee: {t.AssigneeEmail}");
                    }

                    return tickets ?? new();

                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine($"❌ Error deserializing tickets: {jsonEx.Message}");
                    Console.WriteLine($"Raw tickets JSON: {ticketsElement.GetRawText()}");
                    throw new Exception("Failed to deserialize tickets from Gemini response", jsonEx);
                }
            }
            else if (firstPart.TryGetProperty("text", out var textElement))
            {
                // The model returned a text response instead of a function call.
                // You might want to log this or try to parse it differently if applicable.
                Console.WriteLine("💡 Gemini returned a text response instead of a function call:");
                Console.WriteLine(textElement.GetString());
                return new List<TicketRequest>(); // Or throw an exception if a function call is strictly required.
            }
            else
            {
                Console.WriteLine("❌ Gemini response did not contain 'functionCall' or 'text' in the first part.");
                throw new Exception("Unexpected Gemini response format.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Error parsing Gemini response: " + ex.Message);
            throw new Exception("Failed to parse Gemini response", ex);
        }
    }
}