using Microsoft.AspNetCore.Mvc;
using JirAutomate.Models;
using JirAutomate.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace JirAutomate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JiraController : ControllerBase
    {
        private readonly JiraService _jiraService;
        private readonly UserService _userService;

        public JiraController(JiraService jiraService, UserService userService)
        {
            _jiraService = jiraService;
            _userService = userService;
        }

        [Authorize]
        [HttpPost("create-ticket")]
        public async Task<IActionResult> CreateTicket([FromBody] TicketRequest request)
        {
            // Get user id from JWT
            var userId = User.FindFirstValue("id");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found in token");
            var user = _userService.GetById(userId);
            if (user == null)
                return Unauthorized("User not found");
            if (string.IsNullOrWhiteSpace(user.JiraEmail) || string.IsNullOrWhiteSpace(user.JiraApi))
                return BadRequest("User Jira credentials missing");
            try
            {
                var result = await _jiraService.CreateTicket(request, user.JiraEmail, user.JiraApi);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

}
