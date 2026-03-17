using AIKnowledgeAssistant.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIKnowledgeAssistant.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class DocumentController : Controller
    {
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
            var processor = new DocumentProcessingService();
            var embeddingService = new EmbeddingService();
            var text = processor.ExtractTextFromPdf(filePath);
            var chunks = processor.SplitIntoChunks(text, 10);
            var chunk = chunks.FirstOrDefault();
            await embeddingService.GenerateEmbedding(chunk);
            return Ok(text); //preview
        }
    }
}
