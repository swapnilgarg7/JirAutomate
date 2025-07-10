using Microsoft.AspNetCore.Mvc;

namespace JirAutomate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JiraController : ControllerBase
    {
        private readonly JiraService _jiraService;

        public JiraController(JiraService jiraService)
        {
            _jiraService = jiraService;
        }

        [HttpPost("create-ticket")]
        public async Task<IActionResult> CreateTicket([FromBody] TicketRequest request)
        {
            try
            {
                var result = await _jiraService.CreateTicket(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

}
