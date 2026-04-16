using AIKnowledgeAssistant.API.Interfaces;
using AIKnowledgeAssistant.API.Models;
using Microsoft.AspNetCore.Http;
using System.Text;
using UglyToad.PdfPig;

namespace AIKnowledgeAssistant.API.Services
{
    public class DocumentProcessingService : IDocumentProcessingService
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IVectorDatabaseService _vectorDatabaseService;
        private readonly ILogger<DocumentProcessingService> _logger;

        public DocumentProcessingService(IEmbeddingService embeddingService, IVectorDatabaseService vectorDatabaseService, ILogger<DocumentProcessingService> logger)
        {
            _embeddingService = embeddingService;
            _vectorDatabaseService = vectorDatabaseService;
            _logger = logger;
        }

        public async Task ProcessDocument(IFormFile file, string filePath)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Uploaded file is empty or null");
                throw new ArgumentException("Invalid file");
            }
            _logger.LogInformation("Starting document processing: {FileName}", file.FileName);
           
          //  return;
            try
            {
               
                // Step 1 : Extract text from PDF and reterive file information
                var text = ExtractTextFromPdf(filePath);
                if (string.IsNullOrWhiteSpace(text))
                {
                    _logger.LogWarning("No text extracted from PDF: {FileName}", file.FileName);
                    return;
                }
                _logger.LogInformation("Text extraction completed");
                //Step 2: Split text into chunks
                var chunks = SplitIntoChunks(text, 300,50);
                _logger.LogInformation("Created {ChunkCount} chunks from document", chunks.Count);

                //Step 3 : Generate embeddings and store them

                var allVectors = new List<float[]>();
               
                var tasks = chunks.Select(async (chunk,index) =>
                {
                    FileMetaData metaData = new FileMetaData { Name = file.FileName, ChunkIndex = index, DocumentId = $"{file.FileName}_{Guid.NewGuid()}" };
                    _logger.LogDebug("Generating embedding for chunk");

                    var embedding = await _embeddingService.GenerateEmbeddingAsync(chunk);

                    await _vectorDatabaseService.StoreEmbedding(embedding.ToList(), chunk, metaData);
                    _logger.LogDebug("Stored embedding for chunk");
                });



                await Task.WhenAll(tasks);
                _logger.LogInformation("Document processing completed successfully : {FileName}", file.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing document {FileName}", file.FileName);

                throw;
            }

        }
        public string ExtractTextFromPdf(string filePath)
        {
            _logger.LogInformation("Extracting text from PDF: {FilePath}", filePath);

            try
            {
                using (var document = PdfDocument.Open(filePath))
                {
                    var textBuilder = new StringBuilder();
                    foreach (var page in document.GetPages())
                    {
                        textBuilder.Append(page.Text);
                    }
                    _logger.LogInformation("PDF text extraction finished");

                    return textBuilder.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to extract text from PDF: {FilePath}", filePath);

                throw;
            }
               

        }
        private List<string> SplitIntoChunks(string text, int chunkSize, int overlap)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("Cannot split empty text into chunks");
                return new List<string>();
            }
           
            var step = chunkSize - overlap;
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var chunks = new List<string>();
            for(int i =0;i<words.Length;i+= step)
            {
                var chunk = string.Join(" ", words.Skip(i).Take(chunkSize));
                if(!string.IsNullOrWhiteSpace(chunk))
                {
                    chunks.Add(chunk);
                }
                //Stop if we reached the end
                if(i+chunkSize >=words.Length)
                {
                    break;
                }
            }
            //Console.WriteLine($"Total chunks: {chunks.Count}");
            //Console.WriteLine($"First chunk: {chunks[0]}");
            //Console.WriteLine($"Second chunk: {chunks[1]}");
            _logger.LogInformation("Extracted {ChunkCount} chunks", chunks.Count);
            return chunks;

        }
    }
}
