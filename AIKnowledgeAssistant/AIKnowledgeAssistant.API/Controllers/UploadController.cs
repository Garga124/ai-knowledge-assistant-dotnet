using AIKnowledgeAssistant.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIKnowledgeAssistant.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class DocumentController : Controller
    {
        private readonly EmbeddingService _embeddingService;
        private readonly DocumentProcessingService _documentProcessingService;
        private readonly VectorDatabaseService _vectorDatabaseService;
        public DocumentController()
        {
            _embeddingService = new EmbeddingService();
            _documentProcessingService = new DocumentProcessingService();
            _vectorDatabaseService =  new VectorDatabaseService();
        }
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length ==0) 
            {
                return BadRequest("File is empty");
            }
            var filePath = Path.Combine("Uploads",file.FileName);
            using(var stream = new FileStream(filePath,FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            await _documentProcessingService.ProcessDocument(file);
            return Ok("Document processed successfully");
        }

       
    }
}
