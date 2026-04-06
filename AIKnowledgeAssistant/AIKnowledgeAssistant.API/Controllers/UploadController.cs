using AIKnowledgeAssistant.API.Interfaces;
using AIKnowledgeAssistant.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIKnowledgeAssistant.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class DocumentController : Controller
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IDocumentProcessingService _documentProcessingService;
        private readonly IVectorDatabaseService _vectorDatabaseService;
        public DocumentController(IEmbeddingService embeddingService, IVectorDatabaseService vectorDatabaseService, IDocumentProcessingService documentProcessingService)
        {
            _embeddingService = embeddingService;
            _documentProcessingService = documentProcessingService;
            _vectorDatabaseService = vectorDatabaseService;
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
