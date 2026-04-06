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
        private readonly ILogger<DocumentController> _logger;
        public DocumentController(IEmbeddingService embeddingService, IVectorDatabaseService vectorDatabaseService, IDocumentProcessingService documentProcessingService, ILogger<DocumentController> logger)
        {
            _embeddingService = embeddingService;
            _documentProcessingService = documentProcessingService;
            _vectorDatabaseService = vectorDatabaseService;
            _logger = logger;
        }
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                _logger.LogInformation("Upload request received");

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Upload failed: File is empty");
                    return BadRequest("File is empty");
                }
                _logger.LogInformation($"Uploading file: {file.FileName}");
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                    _logger.LogInformation("Uploads directory created");
                }

                var filePath = Path.Combine(uploadFolder, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                _logger.LogInformation("Document Uploaded successfully at {FilePath}",filePath);
                _logger.LogInformation("Starting document processing pipeline");
                await _documentProcessingService.ProcessDocument(file,filePath);
                _logger.LogInformation("Document processing completed successfully for {FileName}", file.FileName);

                return Ok("Document processed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uploading or processing document {FileName}", file?.FileName);

                return StatusCode(500, "An internal server error occurred while processing the document.");
            }
        }

       
    }
}
