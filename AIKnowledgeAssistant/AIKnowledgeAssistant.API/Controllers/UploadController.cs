using AIKnowledgeAssistant.API.Interfaces;
using AIKnowledgeAssistant.API.Models;
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
        /// <summary>
        /// Upload your documents into the system
        /// </summary>
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                _logger.LogInformation("Upload request received");

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Upload failed: File is empty");
                    return BadRequest(new APIResponse<UploadResponse>
                    {
                        Success = false,
                        Message = "File is empty",
                        Data = null
                    });
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

                return Ok(new APIResponse<UploadResponse>
                {
                    Success = true,
                    Message = "Document uploaded and processed successfully",
                    Data = new UploadResponse
                    {
                        FileName = file.FileName,
                        FilePath = filePath
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uploading or processing document {FileName}", file?.FileName);

                return StatusCode(500, new APIResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while processing the document",
                    Data = null
                });
            }
        }

       
    }
}
