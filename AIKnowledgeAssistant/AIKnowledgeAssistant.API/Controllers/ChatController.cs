using AIKnowledgeAssistant.API.Interfaces;
using AIKnowledgeAssistant.API.Models;
using AIKnowledgeAssistant.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIKnowledgeAssistant.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        private readonly IAIResponseService _aIResponseService;
        public ChatController(IAIResponseService aIResponseService)
        {
            _aIResponseService = aIResponseService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] QuestionRequest request)
        {
            var answer = await _aIResponseService.AskQuestionAsync(request.Question);
            return Ok(answer);
        }
    }
}
