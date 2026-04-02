using AIKnowledgeAssistant.API.Models;
using AIKnowledgeAssistant.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIKnowledgeAssistant.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        private readonly AIResponseService _aIResponseService;
        public ChatController()
        {
            _aIResponseService = new AIResponseService();
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] QuestionRequest request)
        {
            var answer = await _aIResponseService.AskQuestion(request.Question);
            return Ok(answer);
        }
    }
}
