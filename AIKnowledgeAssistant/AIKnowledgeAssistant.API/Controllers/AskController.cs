using Microsoft.AspNetCore.Mvc;

namespace AIKnowledgeAssistant.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        [HttpPost("ask")]
        public IActionResult AskQuestion()
        {
            return Ok("Ask Controller Working");
        }
    }
}
