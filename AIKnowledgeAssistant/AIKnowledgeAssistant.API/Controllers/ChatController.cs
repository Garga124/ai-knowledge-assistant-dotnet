using AIKnowledgeAssistant.API.Interfaces;
using AIKnowledgeAssistant.API.Models.DTO;
using AIKnowledgeAssistant.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIKnowledgeAssistant.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        private readonly IAIResponseService _aIResponseService;
        private readonly ILogger<DocumentController> _logger;
        public ChatController(IAIResponseService aIResponseService, ILogger<DocumentController> logger)
        {
            _aIResponseService = aIResponseService;
            _logger = logger;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] QuestionRequest request)
        {
            _logger.LogInformation("Received Ask Question at {Time}", DateTime.UtcNow);
            //Validate request object
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state");
                return BadRequest(ModelState);
            }
            /*
             if(request == null)
             {
                 _logger.LogWarning("Request body is null");
                 return BadRequest("Request body cannot be null");
             }
             //Validate question
             if(string.IsNullOrWhiteSpace(request.Question))
             {
                 _logger.LogWarning("Question is empty or whitespace");
                 return BadRequest("Question cannot be empty");
             }
             if(request.Question.Length >1000)
             {
                 _logger.LogWarning("Question exceeds max length :{Length}", request.Question.Length);
                 return BadRequest("Question is too long");
             }
             */
            try
            {
                _logger.LogInformation("Processing question :{Question}", request.Question);
                var answer = await _aIResponseService.AskQuestionAsync(request.Question);
                _logger.LogInformation("Successfully generated response");
                return Ok(answer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing question");
                return StatusCode(500, "An error occured while processing your request");
               
            }
        }
    }
}
