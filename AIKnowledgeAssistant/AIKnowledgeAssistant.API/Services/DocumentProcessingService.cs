using AIKnowledgeAssistant.API.Interfaces;
using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;

namespace AIKnowledgeAssistant.API.Services
{
    public class DocumentProcessingService
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IVectorDatabaseService _vectorDatabaseService;
        public DocumentProcessingService(IEmbeddingService embeddingService, IVectorDatabaseService vectorDatabaseService) 
        {
            _embeddingService = embeddingService;
            _vectorDatabaseService = vectorDatabaseService;
        }

        public async Task ProcessDocument(IFormFile file)
        {
            //Extract Text

            var filePath = Path.Combine("Uploads", file.FileName);
            var text = ExtractTextFromPdf(filePath);    
            var chunks = SplitIntoChunks(text, 10);
            var allVectors = new List<float[]>();
            var tasks = chunks.Select(async chunk =>
            {
                var embedding = await _embeddingService.GenerateEmbeddingAsync(chunk);

                await _vectorDatabaseService.StoreEmbedding(embedding.ToList(), chunk);
            });

            await Task.WhenAll(tasks);

        }
        public string ExtractTextFromPdf(string filePath)
        {
            using (var document = PdfDocument.Open(filePath))
            {
                var text = "";
                foreach(var page in document.GetPages())
                {
                    text += page.Text;
                }
                return text;
            }
               

        }
        public List<string> SplitIntoChunks(string text, int chunkSize)
        {
            var words = text.Split(' ');
            var chunks = new List<string>();
            for(int i =0;i<words.Length;i+= chunkSize)
            {
                var chunk = string.Join(" ", words.Skip(i).Take(chunkSize));
                chunks.Add(chunk);
            }
            Console.WriteLine("Extracted " + chunks.Count + " Chunks");
            foreach(var chunk in chunks)
            {
                Console.WriteLine("Chunks : " + chunk);
            }
            return chunks;

        }
    }
}
